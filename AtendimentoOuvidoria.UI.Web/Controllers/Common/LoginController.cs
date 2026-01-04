using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using AtendimentoOuvidoria.Model;
using AtendimentoOuvidoria.Repository;
using AtendimentoOuvidoria.UI.Web.Models;
using AtendimentoOuvidoria.Data;

namespace AtendimentoOuvidoria.UI.Web.Controllers
{
    public class LoginController : Controller
    {
        #region Repositorios
        private readonly LoginREP _repositorioLogin;
        private readonly UsuarioREP _repositorioUsuario;
        private readonly SistemaREP _repositorioSistema;
        private readonly AcessaDados _acessaDados;
        private readonly IConfiguration _configuration;
        #endregion

        #region Construtor
        public LoginController(LoginREP loginService, UsuarioREP usuarioService, SistemaREP sistemaService, IConfiguration configuration, AcessaDados acessaDados)
        {
            _repositorioLogin = loginService;
            _repositorioUsuario = usuarioService;
            _repositorioSistema = sistemaService;
            _configuration = configuration;
            _acessaDados = acessaDados;
        }
        #endregion

        #region EfeturarLogin
        [AllowAnonymous]
        public async Task<IActionResult> EfetuarLogin()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> EfetuarLogin(LoginViewMOD model, string? returnUrl)
        {

            UsuarioMOD usuario = new UsuarioMOD();

            usuario.TxLogin = model.Login.ToUpper();
            usuario.TxSenha = model.Password;


            //valida usuario
            bool valido = await _repositorioLogin.ValidarLogin(usuario);

            if (valido)
            {
                //busca os dados do usuario
                usuario = await _repositorioLogin.BuscarPorLogin(usuario);
                var id = _configuration.GetSection("CodigoSistema").Value;


                if (!string.IsNullOrEmpty(usuario.NrCpf))
                {
                    //complementa as informações gerais
                    usuario = await _repositorioLogin.BuscarPorCpf(usuario);
                    SistemaMOD sistema = await _repositorioSistema.BuscarPorCodigo(Convert.ToInt32(id));
                    UsuarioSistemaMOD usuarioSistema = await _repositorioUsuario.BuscarAcessoUsuarioSistema(usuario.CdUsuario, sistema.CdSistema);
                    String tpNivelAcesso = _repositorioLogin.ObterTpNivelAcessoUsuario(usuario.CdUsuario);

                    //úsuário válido e possui acesso
                    if (usuario.CdUsuario > 0 && usuarioSistema.Usuario.CdUsuario > 0)
                    {
                        string role = tpNivelAcesso == "A" ? "Admin" : "Comum";

                        //inclui os clains para o usuario autenticado
                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, usuario.TxLogin),
                                new Claim("NrCpf", usuario.NrCpf),
                                new Claim("CdUsuario", usuario.CdUsuario.ToString()),
                                new Claim("NmUsuario", usuario.NmUsuario.ToString()),
                                new Claim("Avatar", usuario.Avatar.TxLocalFoto),
                                new Claim(ClaimTypes.Role, role),
                            };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            IsPersistent = true,
                            IssuedUtc = DateTime.Now,
                        };

                        //seta cookie de autenticação
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        if (!string.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError<LoginViewMOD>(x => x.Login, "Usuário e/ou Senha Inválidos.");
            return View(model);
        }
        #endregion

        #region Sair
        public async Task<ActionResult> Sair(string? returnUrl = null)
        {
            //busca o codigo do usuário no claim do login
            var CdUsuario = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type == "CdUsuario").Value);

            //limpa o cookie de auth
            await HttpContext.SignOutAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/Login/EfetuarLogin" });

            return RedirectToAction("EfetuarLogin");
        }
        #endregion
    }
}

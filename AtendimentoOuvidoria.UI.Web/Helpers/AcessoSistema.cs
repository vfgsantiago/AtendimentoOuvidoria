using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using AtendimentoOuvidoria.Model;
using AtendimentoOuvidoria.Repository;

namespace AtendimentoOuvidoria.UI.Web.Helpers
{
    public class AcessoSistema(SistemaREP _repositorioSistema,
        UsuarioREP _repositorioUsuario,
        ILogger<AcessoSistema> _logger,
        IConfiguration _configuration) : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //filtro de acesso ao sistema 
            //recupera o ip do usuário
            var remoteIp = filterContext.HttpContext.Connection.RemoteIpAddress;
            string referrer = filterContext.HttpContext.Request.Host.Value; // dominio de origewm da reuiquisicao

            var CdUsuario = Convert.ToInt32(filterContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("ID")).Value);
            var url = _configuration.GetSection("Link").Value;
            var id = _configuration.GetSection("CodigoSistema").Value;

            Controller controller = filterContext.Controller as Controller;

            if (CdUsuario > 0)
            {
                SistemaMOD sistema = await _repositorioSistema.BuscarPorCodigo(Convert.ToInt32(id));
                UsuarioSistemaMOD usuarioSistema = await _repositorioUsuario.BuscarAcessoUsuarioSistema(CdUsuario, sistema.CdSistema);

                if (usuarioSistema.Usuario.CdUsuario > 0)
                {
                    if (sistema.TpVisibilidade == "I")
                    {
                        if (!IsIpAddressInternal(remoteIp.ToString()))
                        {
                            filterContext.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                            _logger.LogWarning($"Acesso bloqueado do IP: {remoteIp}");
                            controller.HttpContext.Response.Redirect(url);
                        }
                    }
                    return;
                }
                else
                {
                    filterContext.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                    _logger.LogWarning($"Acesso bloqueado, usuario sem acesso: {remoteIp}");
                    controller.HttpContext.Response.Redirect(url);
                }
            }
            else
            {
                filterContext.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                _logger.LogWarning($"Acesso bloqueado, usuario sem acesso: {remoteIp}");
                controller.HttpContext.Response.Redirect(url);
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary> 
        /// Verifica se o IP é interno ou externo
        /// </summary> 
        /// <param name="ipAddress">String contendo o IP do Usuário</param>
        /// <returns></returns> 
        public static bool IsIpAddressInternal(string ipAddress)
        {
            //Split the users IP address into it's 4 octets (Assumes IPv4) 
            string[] incomingOctets = ipAddress.Trim().Split(new char[] { '.' });

            //Get the valid IP addresses from the web.config 
            string addresses = "::1, 127.0.0.1, 10.0.*.*, 172.*.*.*, 192.*.*.*";

            //Store each valid IP address in a string array 
            string[] validIpAddresses = addresses.Trim().Split(new char[] { ',' });

            //Iterate through each valid IP address 
            foreach (var validIpAddress in validIpAddresses)
            {
                //Return true if valid IP address matches the users 
                if (validIpAddress.Trim() == ipAddress)
                {
                    return true;
                }

                //Split the valid IP address into it's 4 octets 
                string[] validOctets = validIpAddress.Trim().Split(new char[] { '.' });

                bool matches = true;

                //Iterate through each octet 
                for (int index = 0; index < validOctets.Length; index++)
                {
                    //Skip if octet is an asterisk indicating an entire 
                    //subnet range is valid 
                    if (validOctets[index] != "*")
                    {
                        if (validOctets[index] != incomingOctets[index])
                        {
                            matches = false;
                            break; //Break out of loop 
                        }
                    }
                }

                if (matches)
                {
                    return true;
                }
            }

            //Found no matches 
            return false;
        }

    }
}

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.Repository
{
    public class UsuarioREP
    {
        #region Conexoes

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly AcessaDados _acessaDados;

        #endregion

        #region Construtor
        /// <summary>
        /// Instancia um novo REP e injeta os parametros
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpClient"></param>
        public UsuarioREP(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _acessaDados = new AcessaDados(_configuration);
            _httpClient = _acessaDados.conexaoWebApiLogin();
        }
        #endregion

        #region Métodos

        #region BuscarPorCodigo
        /// <summary>
        /// Buscar 1 usuario por código
        /// </summary>
        /// <param name="CdUsuario"></param>
        /// <returns></returns>
        public async Task<UsuarioMOD> BuscarPorCodigo(int CdUsuario)
        {
            UsuarioMOD usuario = new UsuarioMOD();
            String url = $"Usuarios/api/Usuario/BuscarUsuarioPorCodigo?CdUsuario={CdUsuario}";

            using (var response = await _httpClient.GetAsync(url))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                usuario = JsonConvert.DeserializeObject<UsuarioMOD>(apiResponse);
            }

            return usuario;
        }
        #endregion

        #region BuscarPorLogin
        /// <summary>
        /// Buscar 1 usuário por login
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public async Task<UsuarioMOD> BuscarPorLogin(UsuarioMOD usuario)
        {
            String url = $"Usuarios/api/Usuario/BuscarUsuarioPorLogin?TxLogin={usuario.TxLogin}";

            using (var response = await _httpClient.GetAsync(url))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                usuario = JsonConvert.DeserializeObject<UsuarioMOD>(apiResponse);
            }

            return usuario;
        }
        #endregion

        #region BuscarPorCpf
        /// <summary>
        /// Buscar 1 usuario por cpf
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public async Task<UsuarioMOD> BuscarPorCpf(UsuarioMOD usuario)
        {
            String url = $"Usuarios/api/Usuario/BuscarUsuarioPorCpf?NrCpf={usuario.NrCpf}";

            using (var response = await _httpClient.GetAsync(url))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                usuario = JsonConvert.DeserializeObject<UsuarioMOD>(apiResponse);
            }

            return usuario;
        }
        #endregion

        #region Buscar
        /// <summary>
        /// Buscar todos usuarios
        /// </summary>
        /// <returns></returns>
        public async Task<List<UsuarioMOD>> Buscar()
        {
            List<UsuarioMOD> ListaUsuario = new List<UsuarioMOD>();


            using (var response = await _httpClient.GetAsync("Usuarios/api/Usuario/BuscarUsuarios"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                ListaUsuario = JsonConvert.DeserializeObject<List<UsuarioMOD>>(apiResponse);
            }

            return ListaUsuario;
        }
        #endregion

        #region BuscarAcessoUsuarioSistema
        /// <summary>
        /// Buscar o acesso do usuário ao sistema
        /// </summary>
        /// <param name="CdUsuario"></param>
        /// <param name="CdSistema"></param>
        /// <returns></returns>
        public async Task<UsuarioSistemaMOD> BuscarAcessoUsuarioSistema(Int32 CdUsuario, Int32 CdSistema)
        {
            UsuarioSistemaMOD usuarioSistema = new UsuarioSistemaMOD();

            String url = $"Usuarios/api/Usuario/BuscarAcessoPorCodigoUsuarioSistema?cdUsuario={CdUsuario}&cdSistema={CdSistema}";

            using (var response = await _httpClient.GetAsync(url))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                usuarioSistema = JsonConvert.DeserializeObject<UsuarioSistemaMOD>(apiResponse);
            }

            return usuarioSistema;
        }
        #endregion

        #endregion
    }
}

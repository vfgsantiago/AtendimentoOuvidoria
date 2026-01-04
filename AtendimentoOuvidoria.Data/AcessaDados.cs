using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace AtendimentoOuvidoria.Data
{
    public sealed class AcessaDados
    {
        #region Repositorios
        private readonly IConfiguration _configuration;
        #endregion

        #region Construtor
        public AcessaDados(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Conexao Oracle
        /// <summary>
        /// Recupera a string de conexão do arquivo de configuração
        /// e efetua a conexão com o banco de dados em produção
        /// </summary>
        /// <returns>Retorna a string de conexão</returns>
        ///
        public string conexaoOracle()
        {
            return _configuration.GetConnectionString("OracleConnection");
        }
        #endregion

        #region Conexao Oracle Alt
        /// <summary>
        /// Recupera a string de conexão do arquivo de configuração
        /// e efetua a conexão com o banco de dados alternativo
        /// </summary>
        /// <returns>Retorna a string de conexão</returns>
        ///
        public string conexaoAlt()
        {
            return _configuration.GetConnectionString("AltConnection");
        }
        #endregion

        #region Conexao API Login
        /// <summary>
        /// Efetua a conexão as Web APIs Login
        /// </summary>
        /// <returns>Retorna o caminho da Web API</returns>
        ///
        public HttpClient conexaoWebApiLogin()
        {
            HttpClient _servico = new HttpClient();
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            _servico.BaseAddress = new Uri("https://www.ouvidoria.coop.br/Servicos/");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _servico.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _servico.DefaultRequestHeaders.Add("Autenticacao", "webapi:w3b4p1");

            return _servico;
        }
        #endregion
    }
}

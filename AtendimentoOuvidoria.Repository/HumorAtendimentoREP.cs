using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.Repository
{
    public class HumorAtendimentoREP
    {
        #region Conexoes

        private readonly IConfiguration _configuration;
        private readonly AcessaDados _acessaDados;
        private readonly string _conexaoOracle;

        #endregion

        #region Construtor
        public HumorAtendimentoREP(IConfiguration configuration, HttpClient httpClient, AcessaDados acessaDados)
        {
            _configuration = configuration;
            _acessaDados = acessaDados;
            _conexaoOracle = _acessaDados.conexaoOracle();
        }
        #endregion

        #region Métodos

        #region Buscar
        /// <summary>
        /// Busca todos os humores cadastrados
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<HumorAtendimentoMOD> Buscar()
        {
            List<HumorAtendimentoMOD> ListaHumor = new List<HumorAtendimentoMOD>();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                              CD_HUMOR,
                                              TX_HUMOR,
                                              TX_ICONE,
                                              TX_COR
                                          FROM 
                                              OUVIDORIA_ATENDIMENTO_HUMOR
                                          ORDER BY
                                              CD_HUMOR ASC";

                    ListaHumor = con.Query<HumorAtendimentoMOD>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return ListaHumor;
        }
        #endregion

        #region BuscarPorCodigo
        /// <summary>
        /// Busca o humor cadastrado por código
        /// </summary>
        /// <param name="CdHumor"></param>
        /// <returns></returns>
        public HumorAtendimentoMOD BuscarPorCodigo(int CdHumor)
        {
            HumorAtendimentoMOD Humor = new HumorAtendimentoMOD();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                              CD_HUMOR,
                                              TX_HUMOR,
                                              TX_ICONE,
                                              TX_COR
                                          FROM 
                                              OUVIDORIA_ATENDIMENTO_HUMOR
                                         WHERE 
                                              CD_HUMOR = :CdHumor";

                    Humor = con.Query<HumorAtendimentoMOD>(query, new { CdHumor }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return Humor;
        }
        #endregion

        #endregion
    }
}

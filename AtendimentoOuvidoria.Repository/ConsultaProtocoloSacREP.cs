using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.Repository
{
    public class ConsultaProtocoloSacREP
    {
        #region Conexoes

        private readonly IConfiguration _configuration;
        private readonly AcessaDados _acessaDados;
        private readonly string _conexaoOracleAlt;

        #endregion

        #region Construtor
        public ConsultaProtocoloSacREP(IConfiguration configuration, HttpClient httpClient, AcessaDados acessaDados)
        {
            _configuration = configuration;
            _acessaDados = acessaDados;
            _conexaoOracleAlt = _acessaDados.conexaoAlt();
        }
        #endregion

        #region Métodos

        #region ConsultaProtocoloSac
        /// <summary>
        /// Consultas as informações do protocolo do SAC
        /// </summary>
        /// <param name="nrProtocoloSac"></param>
        /// <returns></returns>
        public ConsultaProtocoloSacMOD ConsultaProtocoloSac(string nrProtocoloSac)
        {
            ConsultaProtocoloSacMOD ProtocoloSac = new ConsultaProtocoloSacMOD();

            using (var con = new OracleConnection(_conexaoOracleAlt))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT 
                                              A.CD_ATENDIMENTO,
                                              A.NR_PROTOCOLO,
                                              A.DT_INICIO,
                                              A.DT_TERMINO,
                                              A.DS_DETALHE_ATENDIMENTO,
                                              CASE
                                                  WHEN A.DT_INICIO >= SYSDATE -90
                                                   AND A.DT_TERMINO <= SYSDATE THEN 1
                                                  ELSE 0
                                              END AS PROTOCOLO_VALIDO                         
                                          FROM 
                                              ATENDIMENTO_SAC A
                                         WHERE 
                                              A.NR_PROTOCOLO_ = :NrProtocoloSac 
                                            OR
                                              A.CD_ATENDIMENTO = :NrProtocoloSac";

                    ProtocoloSac = con.Query<ConsultaProtocoloSacMOD>(query, new { NrProtocoloSac = nrProtocoloSac }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return ProtocoloSac;
        }
        #endregion

        #endregion
    }
}

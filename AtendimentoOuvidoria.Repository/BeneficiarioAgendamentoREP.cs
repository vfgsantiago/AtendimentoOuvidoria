using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.Repository
{
    public class BeneficiarioAgendamentoREP
    {
        #region Conexoes

        private readonly IConfiguration _configuration;
        private readonly AcessaDados _acessaDados;
        private readonly string _conexaoOracle;

        #endregion

        #region Construtor
        public BeneficiarioAgendamentoREP(IConfiguration configuration, HttpClient httpClient, AcessaDados acessaDados)
        {
            _configuration = configuration;
            _acessaDados = acessaDados;
            _conexaoOracle = _acessaDados.conexaoOracle();
        }
        #endregion

        #region Métodos

        #region Buscar
        /// <summary>
        /// Busca todos os beneficiário com agendamento cadastrado
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<BeneficiarioAgendamentoMOD> Buscar()
        {
            List<BeneficiarioAgendamentoMOD> ListaBeneficiario = new List<BeneficiarioAgendamentoMOD>();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                              CD_BENEFICIARIO,
                                              NR_PROTOCOLO_SAC,
                                              NO_BENEFICIARIO,
                                              NR_CPF,
                                              TX_EMAIL,
                                              TX_TELEFONE,
                                              TX_MOTIVO_AGENDAMENTO,
                                              SN_INTERCAMBIO
                                          FROM 
                                              OUVIDORIA_BENEFICIARIO";

                    ListaBeneficiario = con.Query<BeneficiarioAgendamentoMOD>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return ListaBeneficiario;
        }
        #endregion

        #region BuscarPorCodigo
        /// <summary>
        /// Busca o beneficiário cadastrado por código
        /// </summary>
        /// <param name="CdBeneficiario"></param>
        /// <returns></returns>
        public BeneficiarioAgendamentoMOD BuscarPorCodigo(int CdBeneficiario)
        {
            BeneficiarioAgendamentoMOD Beneficiario = new BeneficiarioAgendamentoMOD();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                              CD_BENEFICIARIO,
                                              NR_PROTOCOLO_SAC,
                                              NO_BENEFICIARIO,
                                              NR_CPF,
                                              TX_EMAIL,
                                              TX_TELEFONE,
                                              TX_MOTIVO_AGENDAMENTO,
                                              SN_INTERCAMBIO
                                          FROM 
                                              OUVIDORIA_BENEFICIARIO,
                                         WHERE
                                              CD_BENEFICIARIO = :CdBeneficiario";

                    Beneficiario = con.Query<BeneficiarioAgendamentoMOD>(query, new { CdBeneficiario }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return Beneficiario;
        }
        #endregion

        #region BuscarPorCpf
        /// <summary>
        /// Busca o beneficiário cadastrado por CPF
        /// </summary>
        /// <param name="NrCpf"></param>
        /// <returns></returns>
        public BeneficiarioAgendamentoMOD BuscarPorCodigo(string NrCpf)
        {
            BeneficiarioAgendamentoMOD Beneficiario = new BeneficiarioAgendamentoMOD();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                              CD_BENEFICIARIO,
                                              NR_PROTOCOLO_SAC,
                                              NO_BENEFICIARIO,
                                              NR_CPF,
                                              TX_EMAIL,
                                              TX_TELEFONE,
                                              TX_MOTIVO_AGENDAMENTO,
                                              SN_INTERCAMBIO
                                          FROM 
                                              OUVIDORIA_BENEFICIARIO
                                         WHERE
                                              NR_CPF = :NrCpf";

                    Beneficiario = con.Query<BeneficiarioAgendamentoMOD>(query, new { NrCpf }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return Beneficiario;
        }
        #endregion

        #region Cadastrar
        /// <summary>
        /// Registra o beneficiário
        /// </summary>
        /// <param name="beneficiario"></param>
        /// <returns></returns>
        public bool Cadastrar(BeneficiarioAgendamentoMOD beneficiario)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO                                                
                                            OUVIDORIA_BENEFICIARIO
                                            ( 
                                            NO_BENEFICIARIO,
                                            NR_CPF,
                                            TX_EMAIL,  
                                            TX_TELEFONE, 
                                            TX_MOTIVO_AGENDAMENTO,
                                            SN_INTERCAMBIO
                                            )
                                      VALUES
                                            (
                                            :NoBeneficiario, 
                                            :NrCpf, 
                                            :TxEmail,
                                            :TxTelefone,
                                            :TxMotivoAgendamento,
                                            :SnIntercambio
                                            ) 
                                            RETURNING CD_BENEFICIARIO INTO :CdBeneficiario";

                    var parametroCadastrar = new DynamicParameters(beneficiario);

                    parametroCadastrar.Add("NoBeneficiario", beneficiario.NoBeneficiario);
                    parametroCadastrar.Add("NrCpf", beneficiario.NrCpf);
                    parametroCadastrar.Add("TxEmail", beneficiario.TxEmail);
                    parametroCadastrar.Add("TxTelefone", beneficiario.TxTelefone);
                    parametroCadastrar.Add("TxMotivoAgendamento", beneficiario.TxMotivoAgendamento);
                    parametroCadastrar.Add("SnIntercambio", beneficiario.SnIntercambio);
                    parametroCadastrar.Add("CdBeneficiario", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    con.Execute(query, parametroCadastrar, transaction: transacao);

                    beneficiario.CdBeneficiario = parametroCadastrar.Get<int>("CdBeneficiario");

                    transacao.Commit();
                    cadastrou = true;
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                }
            }
            return cadastrou;
        }
        #endregion

        #endregion
    }
}

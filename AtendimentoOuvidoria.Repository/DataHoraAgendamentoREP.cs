using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.Repository
{
    public class DataHoraAgendamentoREP
    {
        #region Conexoes

        private readonly IConfiguration _configuration;
        private readonly AcessaDados _acessaDados;
        private readonly string _conexaoOracle;

        #endregion

        #region Construtor
        public DataHoraAgendamentoREP(IConfiguration configuration, HttpClient httpClient, AcessaDados acessaDados)
        {
            _configuration = configuration;
            _acessaDados = acessaDados;
            _conexaoOracle = _acessaDados.conexaoOracle();
        }
        #endregion

        #region Métodos

        #region Buscar
        /// <summary>
        /// Busca todos os horários de agendamentos cadastrados
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<DataHoraAgendamentoMOD> Buscar()
        {
            List<DataHoraAgendamentoMOD> ListaDataHora = new List<DataHoraAgendamentoMOD>();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                             D.CD_DATA_HORA,
                                             D.DT_DATA_HORA,
                                             D.SN_DISPONIVEL,
                                             D.SN_TEM_AGENDAMENTO,
                                             D.DT_CADASTRO,
                                             D.CD_USUARIO_CADASTROU,
                                             D.DT_ALTERACAO,
                                             D.CD_USUARIO_ALTEROU,
                                             U.NOUSUARIO,
                                             D.TX_MOTIVO_INATIVACAO
                                          FROM 
                                              OUVIDORIA_AGENDAMENTO_DATA_HORA D,
                                              USUARIO U
                                         WHERE 
                                              D.CD_USUARIO_ALTEROU = U.CDUSUARIO(+)";

                    ListaDataHora = con.Query<DataHoraAgendamentoMOD>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return ListaDataHora;
        }
        #endregion

        #region BuscarPorCodigo
        /// <summary>
        /// Busca o horário de agendamento cadastrado por código
        /// </summary>
        /// <param name="CdDataHora"></param>
        /// <returns></returns>
        public DataHoraAgendamentoMOD BuscarPorCodigo(int CdDataHora)
        {
            DataHoraAgendamentoMOD DataHora = new DataHoraAgendamentoMOD();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                             D.CD_DATA_HORA,
                                             D.DT_DATA_HORA,
                                             D.SN_DISPONIVEL,
                                             D.SN_TEM_AGENDAMENTO,
                                             D.DT_CADASTRO,
                                             D.CD_USUARIO_CADASTROU,
                                             D.DT_ALTERACAO,
                                             D.CD_USUARIO_ALTEROU,
                                             U.NOUSUARIO,
                                             D.TX_MOTIVO_INATIVACAO
                                          FROM 
                                              OUVIDORIA_AGENDAMENTO_DATA_HORA D,
                                              USUARIO U
                                         WHERE 
                                              D.CD_USUARIO_ALTEROU = U.CDUSUARIO(+)
                                           AND
                                              CD_DATA_HORA = :CdDataHora";

                    DataHora = con.Query<DataHoraAgendamentoMOD>(query, new { CdDataHora }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return DataHora;
        }
        #endregion

        #region Cadastrar
        /// <summary>
        /// Cadastra o horário de agendamento
        /// </summary>
        /// <param name="dataHora"></param>
        /// <returns></returns>
        public bool Cadastrar(DataHoraAgendamentoMOD dataHora)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO 
                                                OUVIDORIA_AGENDAMENTO_DATA_HORA 
                                                (
                                                DT_DATA_HORA,
                                                SN_DISPONIVEL,
                                                SN_TEM_AGENDAMENTO,
                                                DT_CADASTRO, 
                                                CD_USUARIO_CADASTROU
                                                )
                                          VALUES
                                                (
                                                :DtDataHora, 
                                                :SnDisponivel, 
                                                :SnTemAgendamento, 
                                                :DtCadastro, 
                                                :CdUsuarioCadastrou
                                                )";

                    var parametroCadastrar = new DynamicParameters(dataHora);

                    parametroCadastrar.Add("DtDataHora", dataHora.DtDataHora);
                    parametroCadastrar.Add("SnDisponivel", dataHora.SnDisponivel);
                    parametroCadastrar.Add("SnTemAgendamento", dataHora.SnTemAgendamento);
                    parametroCadastrar.Add("DtCadastro", dataHora.DtCadastro);
                    parametroCadastrar.Add("CdUsuarioCadastrou", dataHora.CdUsuarioCadastrou);

                    con.Execute(query, parametroCadastrar);
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

        #region AlterarStatus
        /// <summary>
        /// Edita o horário de agendamento
        /// </summary>
        /// <param name="dataHora"></param>
        /// <returns></returns>
        public bool AlterarStatus(DataHoraAgendamentoMOD dataHora)
        {
            bool alterouStatus = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE OUVIDORIA_AGENDAMENTO_DATA_HORA
                                         SET 
                                             SN_DISPONIVEL = :SnDisponivel, 
                                             DT_ALTERACAO= :DtAlteracao, 
                                             CD_USUARIO_ALTEROU = :CdUsuarioAlterou, 
                                             TX_MOTIVO_INATIVACAO = :TxMotivoInativacao
                                        WHERE 
                                             CD_DATA_HORA = :CdDataHora";

                    var parametroEditar = new DynamicParameters(dataHora);

                    parametroEditar.Add("SnDisponivel", dataHora.SnDisponivel);
                    parametroEditar.Add("DtAlteracao", dataHora.DtAlteracao);
                    parametroEditar.Add("CdUsuarioAlterou", dataHora.CdUsuarioAlterou);
                    parametroEditar.Add("CdDataHora", dataHora.CdDataHora);
                    parametroEditar.Add("TxMotivoInativacao", dataHora.TxMotivoInativacao);

                    con.Execute(query, parametroEditar);
                    transacao.Commit();
                    alterouStatus = true;
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                }
            }
            return alterouStatus;
        }
        #endregion

        #region Excluir
        /// <summary>
        /// Exlui o horário de agendamento
        /// </summary>
        /// <param name="cdDataHora"></param>
        /// <returns></returns>
        public bool Excluir(Int32 cdDataHora)
        {
            bool excluiu = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"DELETE 
                                           FROM
                                               OUVIDORIA_AGENDAMENTO_DATA_HORA
                                          WHERE 
                                               CD_DATA_HORA = :CdDataHora";

                    var parametroExcluir = con.Execute(query, new { CdDataHora = cdDataHora });
                    transacao.Commit();
                    excluiu = true;
                }
                catch(Exception ex)
                {
                    transacao.Rollback();
                }
                return excluiu;
            }
        }
        #endregion

        #region ValidarHorario
        public bool ValidarHorario(DateTime dataHora)
        {
            bool horarioValido = false;
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();
                    var query = @"SELECT COUNT(*) 
                                  FROM OUVIDORIA_AGENDAMENTO_DATA_HORA 
                                  WHERE DT_DATA_HORA = :DtDataHora";

                    int count = con.ExecuteScalar<int>(query, new { DtDataHora = dataHora });
                    horarioValido = count == 0;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return horarioValido;
        }
        #endregion

        #endregion
    }
}

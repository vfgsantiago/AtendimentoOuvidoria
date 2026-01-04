using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.Repository
{
    public class MotivoAusenciaAtendimentoREP
    {
        #region Conexoes

        private readonly IConfiguration _configuration;
        private readonly AcessaDados _acessaDados;
        private readonly string _conexaoOracle;

        #endregion

        #region Construtor
        public MotivoAusenciaAtendimentoREP(IConfiguration configuration, HttpClient httpClient, AcessaDados acessaDados)
        {
            _configuration = configuration;
            _acessaDados = acessaDados;
            _conexaoOracle = _acessaDados.conexaoOracle();
        }
        #endregion

        #region Métodos

        #region Buscar
        /// <summary>
        /// Busca todos os motivos de ausência cadastrados
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<MotivoAusenciaAtendimentoMOD> Buscar()
        {
            List<MotivoAusenciaAtendimentoMOD> ListaMotivoAusencia = new List<MotivoAusenciaAtendimentoMOD>();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                              CD_MOTIVO_AUSENCIA,
                                              NO_MOTIVO,
                                              TX_DESCRICAO_MOTIVO,
                                              DT_CADASTRO,
                                              CD_USUARIO_CADASTROU,
                                              DT_ALTERACAO,
                                              CD_USUARIO_ALTEROU,
                                              SN_ATIVO
                                          FROM 
                                              OUVIDORIA_ATENDIMENTO_MOTIVO_AUSENCIA";

                    ListaMotivoAusencia = con.Query<MotivoAusenciaAtendimentoMOD>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return ListaMotivoAusencia;
        }
        #endregion

        #region BuscarPorCodigo
        /// <summary>
        /// Busca o motivo de ausência cadastrado por código
        /// </summary>
        /// <param name="CdMotivoAusencia"></param>
        /// <returns></returns>
        public MotivoAusenciaAtendimentoMOD BuscarPorCodigo(int CdMotivoAusencia)
        {
            MotivoAusenciaAtendimentoMOD MotivoAusencia = new MotivoAusenciaAtendimentoMOD();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                              CD_MOTIVO_AUSENCIA,
                                              NO_MOTIVO,
                                              TX_DESCRICAO_MOTIVO,
                                              DT_CADASTRO,
                                              CD_USUARIO_CADASTROU,
                                              DT_ALTERACAO,
                                              CD_USUARIO_ALTEROU,
                                              SN_ATIVO
                                          FROM 
                                              OUVIDORIA_ATENDIMENTO_MOTIVO_AUSENCIA
                                         WHERE
                                              CD_MOTIVO_AUSENCIA = :CdMotivoAusencia";

                    MotivoAusencia = con.Query<MotivoAusenciaAtendimentoMOD>(query, new { CdMotivoAusencia }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return MotivoAusencia;
        }
        #endregion

        #region Cadastrar
        /// <summary>
        /// Cadastra o motivo de ausência no agendamento
        /// </summary>
        /// <param name="motivoAusencia"></param>
        /// <returns></returns>
        public bool Cadastrar(MotivoAusenciaAtendimentoMOD motivoAusencia)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO 
                                                OUVIDORIA_ATENDIMENTO_MOTIVO_AUSENCIA
                                                (
                                                NO_MOTIVO,
                                                TX_DESCRICAO_MOTIVO, 
                                                DT_CADASTRO, 
                                                CD_USUARIO_CADASTROU,
                                                SN_ATIVO
                                                )
                                          VALUES
                                                (
                                                :NoMotivo, 
                                                :TxDescricaoMotivo, 
                                                :DtCadastro, 
                                                :CdUsuarioCadastrou,
                                                :SnAtivo
                                                )";

                    var parametroCadastrar = new DynamicParameters(motivoAusencia);

                    parametroCadastrar.Add("NoMotivo", motivoAusencia.NoMotivo);
                    parametroCadastrar.Add("TxDescricaoMotivo", motivoAusencia.TxDescricaoMotivo);
                    parametroCadastrar.Add("DtCadastro", motivoAusencia.DtCadastro);
                    parametroCadastrar.Add("CdUsuarioCadastrou", motivoAusencia.CdUsuarioCadastrou);
                    parametroCadastrar.Add("SnAtivo", motivoAusencia.SnAtivo);

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

        #region Editar
        /// <summary>
        /// Edita o horário de agendamento
        /// </summary>
        /// <param name="motivoAusencia"></param>
        /// <returns></returns>
        public bool Editar(MotivoAusenciaAtendimentoMOD motivoAusencia)
        {
            bool editou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE OUVIDORIA_ATENDIMENTO_MOTIVO_AUSENCIA
                                         SET 
                                             NO_MOTIVO = :NoMotivo, 
                                             TX_DESCRICAO_MOTIVO = :TxDescricaoMotivo, 
                                             DT_ALTERACAO = :DtAlteracao, 
                                             CD_USUARIO_ALTEROU = :CdUsuarioAlterou, 
                                             SN_ATIVO = :SnAtivo
                                        WHERE 
                                             CD_MOTIVO_AUSENCIA = :CdMotivoAusencia";

                    var parametroEditar = new DynamicParameters(motivoAusencia);

                    parametroEditar.Add("NoMotivo", motivoAusencia.NoMotivo);
                    parametroEditar.Add("TxDescricaoMotivo", motivoAusencia.TxDescricaoMotivo);
                    parametroEditar.Add("DtAlteracao", motivoAusencia.DtAlteracao);
                    parametroEditar.Add("CdUsuarioAlterou", motivoAusencia.CdUsuarioAlterou);
                    parametroEditar.Add("CdMotivoAusencia", motivoAusencia.CdMotivoAusencia);

                    con.Execute(query, parametroEditar);
                    transacao.Commit();
                    editou = true;
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                }
            }
            return editou;
        }
        #endregion

        #region AlterarStatus
        /// <summary>
        /// Edita o horário de agendamento
        /// </summary>
        /// <param name="motivoAusencia"></param>
        /// <returns></returns>
        public bool AlterarStatus(MotivoAusenciaAtendimentoMOD motivoAusencia)
        {
            bool alterouStatus = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE OUVIDORIA_ATENDIMENTO_MOTIVO_AUSENCIA
                                         SET 
                                             DT_ALTERACAO = :DtAlteracao, 
                                             CD_USUARIO_ALTEROU = :CdUsuarioAlterou, 
                                             SN_ATIVO = :SnAtivo
                                        WHERE 
                                             CD_MOTIVO_AUSENCIA = :CdMotivoAusencia";

                    var parametroAlterarStatus = new DynamicParameters(motivoAusencia);

                    parametroAlterarStatus.Add("DtAlteracao", motivoAusencia.DtAlteracao);
                    parametroAlterarStatus.Add("CdUsuarioAlterou", motivoAusencia.CdUsuarioAlterou);
                    parametroAlterarStatus.Add("SnAtivo", motivoAusencia.SnAtivo);
                    parametroAlterarStatus.Add("CdMotivoAusencia", motivoAusencia.CdMotivoAusencia);

                    con.Execute(query, parametroAlterarStatus);
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

        #endregion
    }
}

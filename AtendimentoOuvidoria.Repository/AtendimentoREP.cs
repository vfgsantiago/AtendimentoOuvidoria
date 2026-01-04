using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.Repository
{
    public class AtendimentoREP
    {
        #region Conexoes

        private readonly IConfiguration _configuration;
        private readonly AcessaDados _acessaDados;
        private readonly string _conexaoOracle;

        #endregion

        #region Construtor
        public AtendimentoREP(IConfiguration configuration, HttpClient httpClient, AcessaDados acessaDados)
        {
            _configuration = configuration;
            _acessaDados = acessaDados;
            _conexaoOracle = _acessaDados.conexaoOracle();
        }
        #endregion

        #region Métodos

        #region Buscar
        /// <summary>
        /// Busca todos os atendimentos da Ouvidoria
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<AtendimentoMOD> Buscar()
        {
            List<AtendimentoMOD> ListaAtendimento = new List<AtendimentoMOD>();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                              CD_ATENDIMENTO,
                                              CD_AGENDAMENTO,
                                              SN_PRESENTE,
                                              CD_MOTIVO_AUSENCIA,
                                              CD_HUMOR,
                                              TX_MOTIVO_HUMOR,
                                              TX_DESCRICAO_ATENDIMENTO,
                                              SN_POSSIVEL_PROCESSO,
                                              SN_POSSIVEL_CHURN,
                                              SN_EXISTE_PENDENCIA,
                                              TX_DESCRICAO_PENDENCIA,
                                              SN_REVERSAO_EXPERIENCIA,
                                              SN_PREENCHIDO,
                                              DT_PREENCHIMENTO,
                                              CD_USUARIO_PREENCHEU,
                                              CD_BENEFICIARIO,
                                              CD_TIPO_ATENDIMENTO
                                          FROM
                                              OUVIDORIA_ATENDIMENTO";

                    ListaAtendimento = con.Query<AtendimentoMOD>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return ListaAtendimento;
        }
        #endregion

        #region BuscarPorCodigo
        /// <summary>
        /// Busca o atendimento por código
        /// </summary>
        /// <param name="CdAtendimento"></param>
        /// <returns></returns>
        public AgendamentoMOD BuscarPorCodigo(int CdAtendimento)
        {
            AgendamentoMOD Atendimento = new AgendamentoMOD();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT
                                              CD_ATENDIMENTO,
                                              CD_AGENDAMENTO,
                                              SN_PRESENTE,
                                              CD_MOTIVO_AUSENCIA,
                                              CD_HUMOR,
                                              TX_MOTIVO_HUMOR,
                                              TX_DESCRICAO_ATENDIMENTO,
                                              SN_POSSIVEL_PROCESSO,
                                              SN_POSSIVEL_CHURN,
                                              SN_EXISTE_PENDENCIA,
                                              TX_DESCRICAO_PENDENCIA,
                                              SN_REVERSAO_EXPERIENCIA,
                                              SN_PREENCHIDO,
                                              DT_PREENCHIMENTO,
                                              CD_USUARIO_PREENCHEU
                                              CD_BENEFICIARIO,
                                              CD_TIPO_ATENDIMENTO      
                                          FROM
                                              OUVIDORIA_ATENDIMENTO
                                         WHERE
                                              CD_ATENDIMENTO = :CdAtendimento";

                    Atendimento = con.Query<AgendamentoMOD>(query, new { CdAtendimento }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return Atendimento;
        }
        #endregion

        #region BuscarAtendimentoPendentePaginado
        /// <summary>
        /// Busca os agendamentos realizados de forma paginada, mas com atendimento pendente
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="itensPorPagina"></param>
        /// <returns>Lista paginada de agendamentos</returns>
        public async Task<PaginacaoResposta<AgendamentoMOD>> BuscarAtendimentoPendentePaginado(int pagina, int itensPorPagina, string? filtro, DateTime? dtAgendamento)
        {
            using var con = new OracleConnection(_conexaoOracle);
            try
            {
                await con.OpenAsync();
                int offset = (pagina - 1) * itensPorPagina;

                var parametros = new DynamicParameters();
                parametros.Add("Offset", offset);
                parametros.Add("ItensPorPagina", itensPorPagina);

                string condicaoFiltro = "";
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    filtro = filtro.Trim().ToUpper();
                    parametros.Add("Filtro", $"%{filtro}%");

                    condicaoFiltro += @" AND (
                                                UPPER(B.NO_BENEFICIARIO) LIKE   :Filtro 
                                                OR
                                                  B.NR_CPF LIKE :Filtro 
                                                OR
                                                  A.NR_PROTOCOLO_AGENDAMENTO LIKE :Filtro
                                               )";
                }
                if (dtAgendamento.HasValue)
                {
                    parametros.Add("DtDataHora", dtAgendamento.Value.Date);
                    condicaoFiltro += " AND TRUNC(D.DT_DATA_HORA) = :DtDataHora";
                }

                var query = $@" SELECT 
                                             A.CD_AGENDAMENTO,
                                             A.CD_BENEFICIARIO,
                                             B.NO_BENEFICIARIO,
                                             B.TX_EMAIL,
                                             B.TX_TELEFONE,
                                             B.TX_MOTIVO_AGENDAMENTO,
                                             B.NR_CPF,
                                             A.CD_DATA_HORA,
                                             D.DT_DATA_HORA,
                                             A.DT_CADASTRO,
                                             A.NR_PROTOCOLO_AGENDAMENTO,
                                             T.SN_PREENCHIDO
                                         FROM 
                                             OUVIDORIA_AGENDAMENTO A,
                                             OUVIDORIA_BENEFICIARIO B,
                                             OUVIDORIA_AGENDAMENTO_DATA_HORA D,
                                             OUVIDORIA_ATENDIMENTO T
                                         WHERE
                                             A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                         AND 
                                             A.CD_DATA_HORA = D.CD_DATA_HORA
                                         AND 
                                             A.CD_AGENDAMENTO = T.CD_AGENDAMENTO(+)
                                         AND 
                                             D.DT_DATA_HORA < SYSDATE
                                         AND 
                                             T.CD_AGENDAMENTO IS NULL
                                         {condicaoFiltro}
                                         ORDER BY CD_AGENDAMENTO DESC
                                         OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                var agendamentos = (await con.QueryAsync<AgendamentoMOD>(query, parametros)).ToList();

                var totalQuery = $@"SELECT 
                                                    COUNT(*)
                                               FROM 
                                                    OUVIDORIA_AGENDAMENTO A,
                                                    OUVIDORIA_BENEFICIARIO B,
                                                    OUVIDORIA_AGENDAMENTO_DATA_HORA D,
                                                    OUVIDORIA_ATENDIMENTO T
                                               WHERE
                                                   A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                               AND 
                                                   A.CD_DATA_HORA = D.CD_DATA_HORA
                                               AND 
                                                   A.CD_AGENDAMENTO = T.CD_AGENDAMENTO(+)
                                               AND 
                                                   D.DT_DATA_HORA < SYSDATE
                                               AND 
                                                   T.CD_AGENDAMENTO IS NULL
                                               {condicaoFiltro}";

                int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);

                return new PaginacaoResposta<AgendamentoMOD>
                {
                    Dados = agendamentos,
                    Paginacao = new Paginacao
                    {
                        PaginaAtual = pagina,
                        QuantidadePorPagina = itensPorPagina,
                        TotalPaginas = (int)Math.Ceiling((double)totalItens / itensPorPagina),
                        TotalItens = totalItens
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar atendimentos pendentes paginados.", ex);
            }
        }
        #endregion

        #region BuscarAtendimentosRealizadosPaginado
        public async Task<PaginacaoResposta<AtendimentoMOD>> BuscarAtendimentosRealizadosPaginado(int pagina, int itensPorPagina, string? filtro, DateTime? dtAgendamento)
        {
            using var con = new OracleConnection(_conexaoOracle);
            try
            {
                await con.OpenAsync();
                int offset = (pagina - 1) * itensPorPagina;

                var parametros = new DynamicParameters();
                parametros.Add("Offset", offset);
                parametros.Add("ItensPorPagina", itensPorPagina);

                string condicaoFiltro = "";
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    filtro = filtro.Trim().ToUpper();
                    parametros.Add("Filtro", $"%{filtro}%");

                    condicaoFiltro += @" AND (
                                                UPPER(B.NO_BENEFICIARIO) LIKE   :Filtro 
                                                OR
                                                  B.NR_CPF LIKE :Filtro 
                                                OR
                                                  A.NR_PROTOCOLO_AGENDAMENTO LIKE :Filtro
                                               )";
                }
                if (dtAgendamento.HasValue)
                {
                    parametros.Add("DtDataHora", dtAgendamento.Value.Date);
                    condicaoFiltro += " AND TRUNC(D.DT_DATA_HORA) = :DtDataHora";
                }

                var query = $@"SELECT 
                                            T.CD_ATENDIMENTO,
                                            A.CD_AGENDAMENTO,
                                            A.CD_BENEFICIARIO,
                                            B.NO_BENEFICIARIO,
                                            B.TX_EMAIL,
                                            B.TX_TELEFONE,
                                            B.TX_MOTIVO_AGENDAMENTO,
                                            B.NR_CPF,
                                            A.CD_DATA_HORA,
                                            D.DT_DATA_HORA,
                                            A.DT_CADASTRO,
                                            A.NR_PROTOCOLO_AGENDAMENTO,
                                            T.SN_PREENCHIDO,
                                            T.SN_PRESENTE,
                                            T.CD_HUMOR,
                                            H.TX_HUMOR,
                                            H.TX_ICONE,
                                            H.TX_COR,
                                            T.TX_MOTIVO_HUMOR,
                                            T.TX_DESCRICAO_ATENDIMENTO,
                                            T.SN_POSSIVEL_PROCESSO,
                                            T.SN_EXISTE_PENDENCIA,
                                            T.TX_DESCRICAO_PENDENCIA,
                                            T.DT_PREENCHIMENTO,
                                            T.CD_USUARIO_PREENCHEU,
                                            U.NOUSUARIO,
                                            T.SN_REVERSAO_EXPERIENCIA,
                                            T.SN_POSSIVEL_CHURN,
                                            T.CD_TIPO_ATENDIMENTO,
                                            TA.NO_TIPO_ATENDIMENTO,
                                            T.SN_PRESENTE
                                        FROM 
                                            OUVIDORIA_AGENDAMENTO A,
                                            OUVIDORIA_BENEFICIARIO B,
                                            OUVIDORIA_AGENDAMENTO_DATA_HORA D,
                                            OUVIDORIA_ATENDIMENTO T,
                                            OUVIDORIA_TIPO_ATENDIMENTO TA,
                                            OUVIDORIA_ATENDIMENTO_HUMOR H,
                                            USUARIO U
                                       WHERE 
                                            T.CD_USUARIO_PREENCHEU = U.CDUSUARIO(+)  
                                        AND
                                            A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                        AND 
                                            A.CD_DATA_HORA = D.CD_DATA_HORA
                                        AND 
                                            A.CD_AGENDAMENTO = T.CD_AGENDAMENTO
                                        AND 
                                            T.CD_TIPO_ATENDIMENTO = TA.CD_TIPO_ATENDIMENTO 
                                        AND 
                                            T.CD_HUMOR = H.CD_HUMOR
                                        AND 
                                            T.SN_PREENCHIDO = 'S'
                                        AND 
                                            T.SN_PRESENTE = 'S'
                                        {condicaoFiltro}
                                        ORDER BY CD_AGENDAMENTO DESC
                                        OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                var atendimentos = (await con.QueryAsync<AtendimentoMOD>(query, parametros)).ToList();

                var totalQuery = $@"SELECT 
                                                COUNT(*)
                                              FROM 
                                                  OUVIDORIA_AGENDAMENTO A,
                                                  OUVIDORIA_BENEFICIARIO B,
                                                  OUVIDORIA_AGENDAMENTO_DATA_HORA D,
                                                  OUVIDORIA_ATENDIMENTO T,
                                                  OUVIDORIA_TIPO_ATENDIMENTO TA
                                              WHERE
                                                  A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                                  AND A.CD_DATA_HORA = D.CD_DATA_HORA
                                                  AND A.CD_AGENDAMENTO = T.CD_AGENDAMENTO
                                                  AND T.CD_TIPO_ATENDIMENTO = TA.CD_TIPO_ATENDIMENTO
                                                  AND T.SN_PREENCHIDO = 'S'
                                                  AND T.SN_PRESENTE = 'S'
                                                  {condicaoFiltro}";

                int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);

                return new PaginacaoResposta<AtendimentoMOD>
                {
                    Dados = atendimentos,
                    Paginacao = new Paginacao
                    {
                        PaginaAtual = pagina,
                        QuantidadePorPagina = itensPorPagina,
                        TotalPaginas = (int)Math.Ceiling((double)totalItens / itensPorPagina),
                        TotalItens = totalItens
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar atendimentos realizados com filtro.", ex);
            }
        }
        #endregion

        #region BuscarAtendimentoCanceladoPaginado
        /// <summary>
        /// Busca os agendamentos realizados de forma paginada, porém que foram cancelados
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="itensPorPagina"></param>
        /// <returns>Lista paginada de agendamentos</returns>
        public async Task<PaginacaoResposta<AtendimentoMOD>> BuscarAtendimentoCanceladoPaginado(int pagina, int itensPorPagina, string? filtro, DateTime? dtAgendamento)
        {
            using var con = new OracleConnection(_conexaoOracle);
            try
            {
                await con.OpenAsync();
                int offset = (pagina - 1) * itensPorPagina;

                var parametros = new DynamicParameters();
                parametros.Add("Offset", offset);
                parametros.Add("ItensPorPagina", itensPorPagina);

                string condicaoFiltro = "";
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    filtro = filtro.Trim().ToUpper();
                    parametros.Add("Filtro", $"%{filtro}%");

                    condicaoFiltro += @" AND (
                                                UPPER(B.NO_BENEFICIARIO) LIKE   :Filtro 
                                                OR
                                                  B.NR_CPF LIKE :Filtro 
                                                OR
                                                  A.NR_PROTOCOLO_AGENDAMENTO LIKE :Filtro
                                               )";
                }
                if (dtAgendamento.HasValue)
                {
                    parametros.Add("DtDataHora", dtAgendamento.Value.Date);
                    condicaoFiltro += " AND TRUNC(D.DT_DATA_HORA) = :DtDataHora";
                }

                var query = $@" SELECT 
                                             A.CD_AGENDAMENTO,
                                             A.CD_BENEFICIARIO,
                                             B.NO_BENEFICIARIO,
                                             B.TX_EMAIL,
                                             B.TX_TELEFONE,
                                             B.TX_MOTIVO_AGENDAMENTO,
                                             B.NR_CPF,
                                             A.CD_DATA_HORA,
                                             D.DT_DATA_HORA,
                                             A.DT_CADASTRO,
                                             A.NR_PROTOCOLO_AGENDAMENTO,
                                             T.SN_PREENCHIDO,
                                             T.SN_PRESENTE,
                                             T.CD_MOTIVO_AUSENCIA,
                                             M.NO_MOTIVO,
                                             T.TX_DESCRICAO_ATENDIMENTO,
                                             T.CD_TIPO_ATENDIMENTO,
                                             TA.NO_TIPO_ATENDIMENTO,
                                             T.DT_PREENCHIMENTO,
                                             T.CD_USUARIO_PREENCHEU,
                                             U.NOUSUARIO
                                         FROM 
                                             OUVIDORIA_AGENDAMENTO A,
                                             OUVIDORIA_BENEFICIARIO B,
                                             OUVIDORIA_AGENDAMENTO_DATA_HORA D,
                                             OUVIDORIA_ATENDIMENTO T,
                                             OUVIDORIA_ATENDIMENTO_MOTIVO_AUSENCIA M,
                                             OUVIDORIA_TIPO_ATENDIMENTO TA,
                                             USUARIO U
                                       WHERE 
                                            T.CD_USUARIO_PREENCHEU = U.CDUSUARIO(+) 
                                         AND
                                             A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                         AND 
                                             A.CD_DATA_HORA = D.CD_DATA_HORA
                                         AND 
                                             A.CD_AGENDAMENTO = T.CD_AGENDAMENTO
                                         AND 
                                             T.CD_MOTIVO_AUSENCIA = M.CD_MOTIVO_AUSENCIA
                                         AND 
                                             T.CD_TIPO_ATENDIMENTO = TA.CD_TIPO_ATENDIMENTO
                                         AND 
                                             T.SN_PREENCHIDO = 'S'
                                         AND 
                                             T.SN_PRESENTE = 'N'
                                         {condicaoFiltro}
                                         ORDER BY CD_AGENDAMENTO DESC
                                         OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                var atendimentos = (await con.QueryAsync<AtendimentoMOD>(query, parametros)).ToList();

                var totalQuery = $@"SELECT 
                                                    COUNT(*)
                                               FROM 
                                                   OUVIDORIA_AGENDAMENTO A,
                                                   OUVIDORIA_BENEFICIARIO B,
                                                   OUVIDORIA_AGENDAMENTO_DATA_HORA D,
                                                   OUVIDORIA_ATENDIMENTO T,
                                                   OUVIDORIA_ATENDIMENTO_MOTIVO_AUSENCIA M,
                                                   OUVIDORIA_TIPO_ATENDIMENTO TA
                                               WHERE
                                                     A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                                 AND 
                                                     A.CD_DATA_HORA = D.CD_DATA_HORA
                                                 AND 
                                                     A.CD_AGENDAMENTO = T.CD_AGENDAMENTO
                                                 AND 
                                                     T.CD_MOTIVO_AUSENCIA = M.CD_MOTIVO_AUSENCIA
                                                 AND 
                                                     T.CD_TIPO_ATENDIMENTO = TA.CD_TIPO_ATENDIMENTO
                                                 AND 
                                                     T.SN_PREENCHIDO = 'S'
                                                 AND 
                                                     T.SN_PRESENTE = 'N'
                                                 {condicaoFiltro}";

                int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);

                return new PaginacaoResposta<AtendimentoMOD>
                {
                    Dados = atendimentos,
                    Paginacao = new Paginacao
                    {
                        PaginaAtual = pagina,
                        QuantidadePorPagina = itensPorPagina,
                        TotalPaginas = (int)Math.Ceiling((double)totalItens / itensPorPagina),
                        TotalItens = totalItens
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar atendimentos cancelados com filtro.", ex);
            }
        }
        #endregion

        #region BuscarAtendimentoEspontaneoPaginado
        /// <summary>
        /// Busca os atendimentos espontâneos com filtro por nome, CPF ou protocolo de forma paginada
        /// </summary>
        /// <param name="pagina">Número da página</param>
        /// <param name="itensPorPagina">Quantidade de itens por página</param>
        /// <param name="filtro">Texto de busca</param>
        /// <returns>Lista paginada de agendamentos</returns>
        public async Task<PaginacaoResposta<AtendimentoMOD>> BuscarAtendimentoEspontaneoPaginado(int pagina, int itensPorPagina, string? filtro, DateTime? dtAtendimento)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    await con.OpenAsync();
                    int offset = (pagina - 1) * itensPorPagina;

                    var parametros = new DynamicParameters();
                    parametros.Add("Offset", offset);
                    parametros.Add("ItensPorPagina", itensPorPagina);

                    string condicaoFiltro = "";
                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        filtro = filtro.Trim().ToUpper();
                        parametros.Add("Filtro", $"%{filtro}%");

                        condicaoFiltro += @" AND (
                                                UPPER(B.NO_BENEFICIARIO) LIKE   :Filtro 
                                                OR
                                                  B.NR_CPF LIKE :Filtro
                                               )";
                    }
                    if (dtAtendimento.HasValue)
                    {
                        parametros.Add("DtPreenchimento", dtAtendimento.Value.Date);
                        condicaoFiltro += " AND TRUNC(A.DT_PREENCHIMENTO) = :DtPreenchimento";
                    }

                    var query = $@"SELECT 
                                               A.CD_ATENDIMENTO,
                                               A.CD_BENEFICIARIO,
                                               B.NO_BENEFICIARIO,
                                               B.TX_EMAIL,
                                               B.TX_TELEFONE,
                                               B.TX_MOTIVO_AGENDAMENTO,
                                               B.NR_CPF,
                                               B.SN_INTERCAMBIO AS SnIntercambio,
                                               A.CD_HUMOR,
                                               H.TX_HUMOR,
                                               H.TX_ICONE,
                                               H.TX_COR,
                                               A.TX_MOTIVO_HUMOR,
                                               A.TX_DESCRICAO_ATENDIMENTO,
                                               A.SN_POSSIVEL_PROCESSO,
                                               A.SN_EXISTE_PENDENCIA,
                                               A.TX_DESCRICAO_PENDENCIA,
                                               A.DT_PREENCHIMENTO,
                                               A.CD_USUARIO_PREENCHEU,
                                               U.NOUSUARIO,
                                               A.SN_REVERSAO_EXPERIENCIA,
                                               A.SN_POSSIVEL_CHURN,
                                               A.CD_TIPO_ATENDIMENTO,
                                               T.NO_TIPO_ATENDIMENTO,
                                               A.SN_PRESENTE
                                          FROM 
                                               SU_OUVIDORIA_ATENDIMENTO              A,
                                               SU_OUVIDORIA_BENEFICIARIO             B,
                                               SU_OUVIDORIA_ATENDIMENTO_HUMOR        H,
                                               SU_OUVIDORIA_TIPO_ATENDIMENTO         T,
                                               USUARIO U
                                         WHERE 
                                              A.CD_USUARIO_PREENCHEU = U.CDUSUARIO(+)  
                                          AND
                                               A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                           AND 
                                               A.CD_HUMOR = H.CD_HUMOR
                                           AND 
                                               A.CD_TIPO_ATENDIMENTO = T.CD_TIPO_ATENDIMENTO
                                           AND 
                                               T.CD_TIPO_ATENDIMENTO = 2
                                           {condicaoFiltro}
                                         ORDER BY A.DT_PREENCHIMENTO DESC
                                         OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                    var atendimentos = (await con.QueryAsync<AtendimentoMOD>(query, parametros)).ToList();

                    var totalQuery = $@"SELECT 
                                                    COUNT(*)
                                               FROM 
                                                    OUVIDORIA_ATENDIMENTO              A,
                                                    OUVIDORIA_BENEFICIARIO             B,
                                                    OUVIDORIA_ATENDIMENTO_HUMOR        H,
                                                    OUVIDORIA_TIPO_ATENDIMENTO         T
                                              WHERE 
                                                    A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                                AND 
                                                    A.CD_HUMOR = H.CD_HUMOR
                                                AND 
                                                    A.CD_TIPO_ATENDIMENTO = T.CD_TIPO_ATENDIMENTO
                                                AND 
                                                    T.CD_TIPO_ATENDIMENTO = 2
                                                {condicaoFiltro}";

                    int totalItens = await con.ExecuteScalarAsync<int>(totalQuery, parametros);

                    return new PaginacaoResposta<AtendimentoMOD>
                    {
                        Dados = atendimentos,
                        Paginacao = new Paginacao
                        {
                            PaginaAtual = pagina,
                            QuantidadePorPagina = itensPorPagina,
                            TotalPaginas = (int)Math.Ceiling((double)totalItens / itensPorPagina),
                            TotalItens = totalItens
                        }
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar atendimentos espontâneos com filtro.", ex);
                }
            }
        }
        #endregion

        #region RegistrarAtendimentoAgendado
        /// <summary>
        /// Registra o atendimento agendado realizado pela Ouvidoria
        /// </summary>
        /// <param name="atendimento"></param>
        /// <returns></returns>
        public bool RegistrarAtendimentoAgendado(AtendimentoMOD atendimento)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO 
                                                OUVIDORIA_ATENDIMENTO
                                                (
                                                CD_AGENDAMENTO,
                                                SN_PRESENTE, 
                                                CD_HUMOR, 
                                                TX_MOTIVO_HUMOR, 
                                                TX_DESCRICAO_ATENDIMENTO, 
                                                SN_POSSIVEL_PROCESSO, 
                                                SN_POSSIVEL_CHURN, 
                                                SN_EXISTE_PENDENCIA, 
                                                TX_DESCRICAO_PENDENCIA, 
                                                SN_REVERSAO_EXPERIENCIA,
                                                SN_PREENCHIDO,
                                                DT_PREENCHIMENTO,
                                                CD_USUARIO_PREENCHEU,
                                                CD_TIPO_ATENDIMENTO
                                                )
                                          VALUES
                                                (
                                                :CdAgendamento, 
                                                :SnPresente, 
                                                :CdHumor,
                                                :TxMotivoHumor,
                                                :TxDescricaoAtendimento,
                                                :SnPossivelProcesso,
                                                :SnPossivelChurn,
                                                :SnExistePendencia,
                                                :TxDescricaoPendencia,
                                                :SnReversaoExperiencia,
                                                :SnPreenchido,
                                                :DtPreenchimento,
                                                :CdUsuarioPreencheu,
                                                :CdTipoAtendimento
                                                )";

                    var parametroCadastrar = new DynamicParameters(atendimento);

                    parametroCadastrar.Add("CdAgendamento", atendimento.CdAgendamento);
                    parametroCadastrar.Add("SnPresente", atendimento.SnPresente);
                    parametroCadastrar.Add("CdHumor", atendimento.CdHumor);
                    parametroCadastrar.Add("TxMotivoHumor", atendimento.TxMotivoHumor);
                    parametroCadastrar.Add("TxDescricaoAtendimento", atendimento.TxDescricaoAtendimento);
                    parametroCadastrar.Add("SnPossivelProcesso", atendimento.SnPossivelProcesso);
                    parametroCadastrar.Add("SnPossivelChurn", atendimento.SnPossivelChurn);
                    parametroCadastrar.Add("SnExistePendencia", atendimento.SnExistePendencia);
                    parametroCadastrar.Add("TxDescricaoPendencia", atendimento.TxDescricaoPendencia);
                    parametroCadastrar.Add("SnReversaoExperiencia", atendimento.SnReversaoExperiencia);
                    parametroCadastrar.Add("SnPreenchido", atendimento.SnPreenchido);
                    parametroCadastrar.Add("DtPreenchimento", atendimento.DtPreenchimento);
                    parametroCadastrar.Add("CdUsuarioPreencheu", atendimento.CdUsuarioPreencheu);
                    parametroCadastrar.Add("CdTipoAtendimento", atendimento.CdTipoAtendimento);

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

        #region CancelarAtendimento
        /// <summary>
        /// Cancela o atendimento
        /// </summary>
        /// <param name="atendimento"></param>
        /// <returns></returns>
        public bool CancelarAtendimento(AtendimentoMOD atendimento)
        {
            bool cancelou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO
                                               OUVIDORIA_ATENDIMENTO
                                               (
                                               CD_AGENDAMENTO,
                                               SN_PRESENTE,
                                               TX_DESCRICAO_ATENDIMENTO,
                                               CD_MOTIVO_AUSENCIA,
                                               SN_PREENCHIDO,
                                               DT_PREENCHIMENTO,
                                               CD_USUARIO_PREENCHEU,
                                               CD_TIPO_ATENDIMENTO
                                               )
                                          VALUES
                                               (
                                               :CdAgendamento,
                                               :SnPresente,
                                               :TxDescricaoAtendimento,
                                               :CdMotivoAusencia,
                                               :SnPreenchido,
                                               :DtPreenchimento,
                                               :CdUsuarioPreencheu,
                                               :CdTipoAtendimento
                                               )";

                    var parametroCancelar = new DynamicParameters(atendimento);

                    parametroCancelar.Add("CdAgendamento", atendimento.CdAgendamento);
                    parametroCancelar.Add("SnPresente", atendimento.SnPresente);
                    parametroCancelar.Add("TxDescricaoAtendimento", atendimento.TxDescricaoAtendimento);
                    parametroCancelar.Add("CdMotivoAusencia", atendimento.CdMotivoAusencia);
                    parametroCancelar.Add("SnPreenchido", atendimento.SnPreenchido);
                    parametroCancelar.Add("DtPreenchimento", atendimento.DtPreenchimento);
                    parametroCancelar.Add("CdUsuarioPreencheu", atendimento.CdUsuarioPreencheu);
                    parametroCancelar.Add("CdTipoAtendimento", atendimento.CdTipoAtendimento);


                    con.Execute(query, parametroCancelar);
                    transacao.Commit();
                    cancelou = true;
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                }
            }
            return cancelou;
        }
        #endregion

        #region Alterar
        /// <summary>
        /// Edita o atendimento realizado pela Ouvidoria
        /// </summary>
        /// <param name="atendimento"></param>
        /// <returns></returns>
        public bool Editar(AtendimentoMOD atendimento)
        {
            bool editou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"UPDATE OUVIDORIA_ATENDIMENTO
                                         SET 
                                             CD_AGENDAMENTO = :CdAgendamento,
                                             SN_PRESENTE = :SnPresente, 
                                             CD_MOTIVO_AUSENCIA = :CdMotivoAusencia, 
                                             CD_HUMOR = :CdHumor,
                                             TX_MOTIVO_HUMOR = :TxMotivoHumor,
                                             TX_DESCRICAO_ATENDIMENTO = :TxDescricaoAtendimento,
                                             SN_POSSIVEL_PROCESSO = :SnPossivelProcesso,
                                             SN_POSSIVEL_CHURN = :SnPossivelChurn,
                                             SN_EXISTE_PENDENCIA = :SnExistePendencia,
                                             TX_DESCRICAO_PENDENCIA = :TxDescricaoPendencia,
                                             SN_REVERSAO_EXPERIENCIA = :SnReversaoExperiencia,
                                             SN_PREENCHIDO = :SnPreenchido,
                                             DT_PREENCHIMENTO = :DtPreenchimento,
                                             CD_USUARIO_PREENCHEU = :CdUsuarioPreencheu
                                        WHERE 
                                             CD_ATENDIMENTO = :CdAtendimento";

                    var parametroEditar = new DynamicParameters(atendimento);

                    parametroEditar.Add("CdAgendamento", atendimento.CdAgendamento);
                    parametroEditar.Add("SnPresente", atendimento.SnPresente);
                    parametroEditar.Add("CdMotivoAusencia", atendimento.CdMotivoAusencia);
                    parametroEditar.Add("CdHumor", atendimento.CdHumor);
                    parametroEditar.Add("TxMotivoHumor", atendimento.TxMotivoHumor);
                    parametroEditar.Add("TxDescricaoAtendimento", atendimento.TxDescricaoAtendimento);
                    parametroEditar.Add("SnPossivelProcesso", atendimento.SnPossivelProcesso);
                    parametroEditar.Add("SnPossivelChurn", atendimento.SnPossivelChurn);
                    parametroEditar.Add("SnExistePendencia", atendimento.SnExistePendencia);
                    parametroEditar.Add("TxDescricaoPendencia", atendimento.TxDescricaoPendencia);
                    parametroEditar.Add("SnReversaoExperiencia", atendimento.SnReversaoExperiencia);
                    parametroEditar.Add("SnPreenchido", atendimento.SnPreenchido);
                    parametroEditar.Add("DtPreenchimento", atendimento.DtPreenchimento);
                    parametroEditar.Add("CdUsuarioPreencheu", atendimento.CdUsuarioPreencheu);

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

        #region Contagens

        #region ContarAtendimentosRealizados
        public int ContarAtendimentosRealizados()
        {
            using var con = new OracleConnection(_conexaoOracle);
            return con.ExecuteScalar<int>(@"SELECT COUNT(*) 
                                                    FROM OUVIDORIA_ATENDIMENTO 
                                                    WHERE SN_PREENCHIDO = 'S' AND SN_PRESENTE = 'S' AND CD_TIPO_ATENDIMENTO = 1");
        }
        #endregion

        #region ContarAtendimentosCancelados
        public int ContarAtendimentosCancelados()
        {
            using var con = new OracleConnection(_conexaoOracle);
            return con.ExecuteScalar<int>(@"SELECT COUNT(*) 
                                                    FROM OUVIDORIA_ATENDIMENTO 
                                                    WHERE SN_PREENCHIDO = 'S' AND SN_PRESENTE = 'N'");
        }
        #endregion

        #region ContarAtendimentosPendentes
        public int ContarAtendimentosPendentes()
        {
            using var con = new OracleConnection(_conexaoOracle);
            return con.ExecuteScalar<int>(@"SELECT COUNT(*)
                                           FROM 
                                               OUVIDORIA_AGENDAMENTO              A,
                                               OUVIDORIA_BENEFICIARIO B,
                                               OUVIDORIA_AGENDAMENTO_DATA_HORA    D,
                                               OUVIDORIA_ATENDIMENTO              T
                                           WHERE
                                               A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                           AND
                                               A.CD_DATA_HORA = D.CD_DATA_HORA
                                           AND
                                               A.CD_AGENDAMENTO = T.CD_AGENDAMENTO(+)
                                           AND 
                                               D.DT_DATA_HORA < SYSDATE
                                           AND 
                                               T.CD_AGENDAMENTO IS NULL
                                           AND
                                               1=1");
        }
        #endregion

        #region ContarAtendimentoEspontaneos
        public int ContarAtendimentoEspontaneos()
        {
            using var con = new OracleConnection(_conexaoOracle);
            return con.ExecuteScalar<int>(@"SELECT COUNT(*) 
                                                    FROM OUVIDORIA_ATENDIMENTO 
                                                    WHERE SN_PREENCHIDO = 'S' AND SN_PRESENTE = 'S' AND CD_TIPO_ATENDIMENTO = 2");
        }
        #endregion

        #region RegistarAtendimentoEspontaneo
        /// <summary>
        /// Registra o atendimento espontâneo realizado pela Ouvidoria
        /// </summary>
        /// <param name="atendimento"></param>
        /// <returns></returns>
        public bool RegistarAtendimentoEspontaneo(AtendimentoMOD atendimento)
        {
            bool cadastrou = false;

            using (OracleConnection con = new OracleConnection(_conexaoOracle))
            {
                con.Open();
                OracleTransaction transacao = con.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO 
                                                OUVIDORIA_ATENDIMENTO
                                                (
                                                SN_PRESENTE, 
                                                CD_HUMOR, 
                                                TX_MOTIVO_HUMOR, 
                                                TX_DESCRICAO_ATENDIMENTO, 
                                                SN_POSSIVEL_PROCESSO, 
                                                SN_POSSIVEL_CHURN, 
                                                SN_EXISTE_PENDENCIA, 
                                                TX_DESCRICAO_PENDENCIA, 
                                                SN_REVERSAO_EXPERIENCIA,
                                                SN_PREENCHIDO,
                                                DT_PREENCHIMENTO,
                                                CD_USUARIO_PREENCHEU,
                                                CD_TIPO_ATENDIMENTO,
                                                CD_BENEFICIARIO
                                                )
                                          VALUES
                                                (
                                                :SnPresente, 
                                                :CdHumor,
                                                :TxMotivoHumor,
                                                :TxDescricaoAtendimento,
                                                :SnPossivelProcesso,
                                                :SnPossivelChurn,
                                                :SnExistePendencia,
                                                :TxDescricaoPendencia,
                                                :SnReversaoExperiencia,
                                                :SnPreenchido,
                                                :DtPreenchimento,
                                                :CdUsuarioPreencheu,
                                                :CdTipoAtendimento,
                                                :CdBeneficiario
                                                )";

                    var parametroCadastrar = new DynamicParameters(atendimento);

                    parametroCadastrar.Add("SnPresente", atendimento.SnPresente);
                    parametroCadastrar.Add("CdHumor", atendimento.CdHumor);
                    parametroCadastrar.Add("TxMotivoHumor", atendimento.TxMotivoHumor);
                    parametroCadastrar.Add("TxDescricaoAtendimento", atendimento.TxDescricaoAtendimento);
                    parametroCadastrar.Add("SnPossivelProcesso", atendimento.SnPossivelProcesso);
                    parametroCadastrar.Add("SnPossivelChurn", atendimento.SnPossivelChurn);
                    parametroCadastrar.Add("SnExistePendencia", atendimento.SnExistePendencia);
                    parametroCadastrar.Add("TxDescricaoPendencia", atendimento.TxDescricaoPendencia);
                    parametroCadastrar.Add("SnReversaoExperiencia", atendimento.SnReversaoExperiencia);
                    parametroCadastrar.Add("SnPreenchido", atendimento.SnPreenchido);
                    parametroCadastrar.Add("DtPreenchimento", atendimento.DtPreenchimento);
                    parametroCadastrar.Add("CdUsuarioPreencheu", atendimento.CdUsuarioPreencheu);
                    parametroCadastrar.Add("CdTipoAtendimento", atendimento.CdTipoAtendimento);
                    parametroCadastrar.Add("CdBeneficiario", atendimento.CdBeneficiario);

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

        #endregion

        #endregion
    }
}


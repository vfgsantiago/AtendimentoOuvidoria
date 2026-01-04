using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.Repository
{
    public class AgendamentoREP
    {
        #region Conexoes

        private readonly IConfiguration _configuration;
        private readonly AcessaDados _acessaDados;
        private readonly string _conexaoOracle;

        #endregion

        #region Construtor
        public AgendamentoREP(IConfiguration configuration, HttpClient httpClient, AcessaDados acessaDados)
        {
            _configuration = configuration;
            _acessaDados = acessaDados;
            _conexaoOracle = _acessaDados.conexaoOracle();
        }
        #endregion

        #region Métodos

        #region Buscar
        /// <summary>
        /// Busca todos os agendamentos realizados
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<AgendamentoMOD> Buscar()
        {
            List<AgendamentoMOD> ListaAgendamento = new List<AgendamentoMOD>();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"A.CD_AGENDAMENTO,
                                                A.CD_BENEFICIARIO,
                                                B.NO_BENEFICIARIO,
                                                B.TX_EMAIL,
                                                B.TX_TELEFONE,
                                                B.TX_MOTIVO_AGENDAMENTO,
                                                B.NR_CPF,
                                                A.CD_DATA_HORA,
                                                D.DT_DATA_HORA,
                                                A.DT_CADASTRO,
                                                A.NR_PROTOCOLO_AGENDAMENTO
                                            FROM 
                                                OUVIDORIA_AGENDAMENTO A,
                                                OUVIDORIA_BENEFICIARIO B,
                                                OUVIDORIA_AGENDAMENTO_DATA_HORA D
                                            WHERE 
                                                A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                             AND
                                                A.CD_DATA_HORA = D.CD_DATA_HORA";

                    ListaAgendamento = con.Query<AgendamentoMOD>(query).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return ListaAgendamento;
        }
        #endregion

        #region BuscarPorCodigo
        /// <summary>
        /// Busca o agendamento realizado
        /// </summary>
        /// <param name="CdAgendamento"></param>
        /// <returns></returns>
        public AgendamentoMOD BuscarPorCodigo(int CdAgendamento)
        {
            AgendamentoMOD Agendamento = new AgendamentoMOD();

            using (var con = new OracleConnection(_conexaoOracle))
            {
                try
                {
                    con.Open();

                    var query = @"SELECT 
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
                                                A.NR_PROTOCOLO_AGENDAMENTO
                                            FROM 
                                                OUVIDORIA_AGENDAMENTO A,
                                                OUVIDORIA_BENEFICIARIO B,
                                                OUVIDORIA_AGENDAMENTO_DATA_HORA D
                                            WHERE 
                                                A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                             AND
                                                A.CD_DATA_HORA = D.CD_DATA_HORA
                                           AND
                                              A.CD_AGENDAMENTO = :CdAgendamento";

                    Agendamento = con.Query<AgendamentoMOD>(query, new {CdAgendamento}).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return Agendamento;
        }
        #endregion

        #region BuscarAgendamentosComFiltro
        /// <summary>
        /// Busca os agendamentos com filtro por nome, CPF ou protocolo de forma paginada
        /// </summary>
        /// <param name="pagina">Número da página</param>
        /// <param name="itensPorPagina">Quantidade de itens por página</param>
        /// <param name="filtro">Texto de busca</param>
        /// <returns>Lista paginada de agendamentos</returns>
        public async Task<PaginacaoResposta<AgendamentoMOD>> BuscarAgendamentosComFiltro(int pagina, int itensPorPagina, string? filtro, DateTime? dtAgendamento)
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
                                               A.NR_PROTOCOLO_AGENDAMENTO
                                          FROM 
                                               OUVIDORIA_AGENDAMENTO              A,
                                               OUVIDORIA_BENEFICIARIO             B,
                                               OUVIDORIA_AGENDAMENTO_DATA_HORA    D
                                         WHERE 
                                               A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                           AND 
                                               A.CD_DATA_HORA = D.CD_DATA_HORA
                                           AND 
                                               D.SN_DISPONIVEL = 'N'
                                           AND 
                                               D.SN_TEM_AGENDAMENTO = 'S'
                                           AND 
                                               D.DT_DATA_HORA > SYSDATE
                                           {condicaoFiltro}
                                         ORDER BY CD_AGENDAMENTO DESC
                                         OFFSET :Offset ROWS FETCH NEXT :ItensPorPagina ROWS ONLY";

                    var agendamentos = (await con.QueryAsync<AgendamentoMOD>(query, parametros)).ToList();

                    var totalQuery = $@"SELECT 
                                                    COUNT(*)
                                               FROM 
                                                    OUVIDORIA_AGENDAMENTO A,
                                                    OUVIDORIA_BENEFICIARIO B,
                                                    OUVIDORIA_AGENDAMENTO_DATA_HORA D
                                              WHERE 
                                                    A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                                AND 
                                                    A.CD_DATA_HORA = D.CD_DATA_HORA
                                                AND 
                                                    D.SN_DISPONIVEL = 'N'
                                                AND 
                                                    D.SN_TEM_AGENDAMENTO = 'S'
                                                AND 
                                                    D.DT_DATA_HORA > SYSDATE
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
                    throw new Exception("Erro ao buscar agendamentos com filtro.", ex);
                }
            }
        }
        #endregion

        #region BuscarPorCdDataHora

        public AgendamentoMOD BuscarPorCdDataHora(int cdDataHora)
        {
            using (var con = new OracleConnection(_conexaoOracle))
            {
                string query = @"SELECT 
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
                                                A.NR_PROTOCOLO_AGENDAMENTO
                                            FROM 
                                                OUVIDORIA_AGENDAMENTO A,
                                                OUVIDORIA_BENEFICIARIO B,
                                                OUVIDORIA_AGENDAMENTO_DATA_HORA D
                                            WHERE 
                                                A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                             AND
                                                A.CD_DATA_HORA = D.CD_DATA_HORA
                                           AND
                                              A.CD_DATA_HORA = :CdDataHora";

                return con.QueryFirstOrDefault<AgendamentoMOD>(query, new { CdDataHora = cdDataHora });
            }
        }

        #endregion

        #region ContarAgendamentosTotais
        public int ContarAgendamentosTotais()
        {
            using var con = new OracleConnection(_conexaoOracle);
            return con.ExecuteScalar<int>(@"SELECT COUNT(*)
                                          FROM OUVIDORIA_AGENDAMENTO              A,
                                               OUVIDORIA_BENEFICIARIO B,
                                               OUVIDORIA_AGENDAMENTO_DATA_HORA    D,
                                               OUVIDORIA_ATENDIMENTO              AT
                                         WHERE A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                           AND A.CD_DATA_HORA = D.CD_DATA_HORA
                                           AND A.CD_AGENDAMENTO = AT.CD_ATENDIMENTO(+)
                                           AND D.SN_DISPONIVEL = 'N'
                                           AND D.SN_TEM_AGENDAMENTO = 'S'
                                           AND D.DT_DATA_HORA > SYSDATE
                                           AND 1 = 1");
        }
        #endregion

        #region BuscarAgendamentosDoDia
        public List<AgendamentoMOD> BuscarAgendamentosDoDia(DateTime data)
        {
            using var con = new OracleConnection(_conexaoOracle);
            con.Open();

            var query = @"SELECT 
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
                                       A.NR_PROTOCOLO_AGENDAMENTO
                                   FROM 
                                       OUVIDORIA_AGENDAMENTO A,
                                       OUVIDORIA_BENEFICIARIO B,
                                       OUVIDORIA_AGENDAMENTO_DATA_HORA D 
                                   WHERE 
                                       TRUNC(D.DT_DATA_HORA) = :Data
                                    AND
                                       A.CD_BENEFICIARIO = B.CD_BENEFICIARIO
                                    AND
                                       A.CD_DATA_HORA = D.CD_DATA_HORA
                                   ORDER BY 
                                       D.DT_DATA_HORA";

            return con.Query<AgendamentoMOD>(query, new { Data = data.Date }).ToList();
        }
        #endregion

        #endregion
    }
}

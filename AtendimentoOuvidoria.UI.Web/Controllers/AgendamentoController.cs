using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;
using AtendimentoOuvidoria.Repository;
using AtendimentoOuvidoria.UI.Web.Models;
using X.PagedList;

namespace AtendimentoOuvidoria.UI.Web.Controllers
{
    [Authorize(Roles = "Admin, Comum")]
    public class AgendamentoController : Controller
    {
        #region Repositorios
        private readonly LoginREP _repositorioLogin;
        private readonly UsuarioREP _repositorioUsuario;
        private readonly SistemaREP _repositorioSistema;
        private readonly AgendamentoREP _repositorioAgendamento;
        private readonly DataHoraAgendamentoREP _repositorioDataHoraAgendamento;
        private readonly BeneficiarioAgendamentoREP _repositorioBeneficiarioAgendamento;
        private readonly AcessaDados _acessaDados;
        private readonly IConfiguration _configuration;
        #endregion

        #region Construtor
        public AgendamentoController(LoginREP loginService, UsuarioREP usuarioService, SistemaREP sistemaService, AgendamentoREP repositorioAgendamento, DataHoraAgendamentoREP repositorioDataHoraAgendamento, BeneficiarioAgendamentoREP repositorioBeneficiarioAgendamento, IConfiguration configuration, AcessaDados acessaDados)
        {
            _repositorioLogin = loginService;
            _repositorioUsuario = usuarioService;
            _repositorioSistema = sistemaService;
            _repositorioAgendamento = repositorioAgendamento;
            _repositorioDataHoraAgendamento = repositorioDataHoraAgendamento;
            _repositorioBeneficiarioAgendamento = repositorioBeneficiarioAgendamento;
            _configuration = configuration;
            _acessaDados = acessaDados;
        }
        #endregion

        #region Parametros

        private const int _take = 15;
        private int _numeroPagina = 1;
        private int _pagina;

        #endregion

        #region Métodos

        #region Index
        public async Task<IActionResult> Index(int? pagina, string? filtro, DateTime? dtAgendamento)
        {
            int numeroPagina = pagina ?? 1;
            
            var resultado = await _repositorioAgendamento.BuscarAgendamentosComFiltro(numeroPagina, _take, filtro, dtAgendamento);

            var viewModel = new AgendamentoAtendimentoViewMOD
            {
                ListaAgendamentoPaginada = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            ViewBag.Filtro = filtro;
            ViewBag.DataAgendamento = dtAgendamento;
            ViewBag.Titulo = "Agendamentos Futuros";
            return View("Index", viewModel);
        }
        #endregion

        #endregion
    }
}

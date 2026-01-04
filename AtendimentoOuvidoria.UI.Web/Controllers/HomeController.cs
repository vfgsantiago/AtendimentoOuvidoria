using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Repository;
using AtendimentoOuvidoria.UI.Web.Models;

namespace AtendimentoOuvidoria.UI.Web.Controllers
{
    [Authorize(Roles = "Admin, Comum")]
    public class HomeController : Controller
    {
        #region Repositorios
        private readonly LoginREP _repositorioLogin;
        private readonly UsuarioREP _repositorioUsuario;
        private readonly SistemaREP _repositorioSistema;
        private readonly AgendamentoREP _repositorioAgendamento;
        private readonly DataHoraAgendamentoREP _repositorioDataHoraAgendamento;
        private readonly BeneficiarioAgendamentoREP _repositorioBeneficiarioAgendamento;
        private readonly AtendimentoREP _repositorioAtendimento;
        private readonly HumorAtendimentoREP _repositorioHumorAtendimento;
        private readonly MotivoAusenciaAtendimentoREP _repositorioMotivoAusenciaAtendimento;
        private readonly AcessaDados _acessaDados;
        private readonly IConfiguration _configuration;
        #endregion

        #region Construtor
        public HomeController(LoginREP loginService, UsuarioREP usuarioService, SistemaREP sistemaService, AgendamentoREP repositorioAgendamento, DataHoraAgendamentoREP repositorioDataHoraAgendamento, BeneficiarioAgendamentoREP repositorioBeneficiarioAgendamento, AtendimentoREP repositorioAtendimento, HumorAtendimentoREP repositorioHumorAtendimento, MotivoAusenciaAtendimentoREP repositorioMotivoAusenciaAtendimento,IConfiguration configuration, AcessaDados acessaDados)
        {
            _repositorioLogin = loginService;
            _repositorioUsuario = usuarioService;
            _repositorioSistema = sistemaService;
            _repositorioAgendamento = repositorioAgendamento;
            _repositorioDataHoraAgendamento = repositorioDataHoraAgendamento;
            _repositorioBeneficiarioAgendamento = repositorioBeneficiarioAgendamento;
            _repositorioAtendimento = repositorioAtendimento;
            _repositorioHumorAtendimento = repositorioHumorAtendimento;
            _repositorioMotivoAusenciaAtendimento = repositorioMotivoAusenciaAtendimento;
            _configuration = configuration;
            _acessaDados = acessaDados;
        }
        #endregion

        #region Parametros

        private const int _take = 20;
        private int _numeroPagina = 1;
        private int _pagina;

        #endregion

        #region Index
        public IActionResult Index()
        {
            var viewModel = new AgendamentoAtendimentoViewMOD
            {
                TotalRealizados = _repositorioAtendimento.ContarAtendimentosRealizados(),
                TotalCancelados = _repositorioAtendimento.ContarAtendimentosCancelados(),
                TotalPendentes = _repositorioAtendimento.ContarAtendimentosPendentes(),
                TotalAgendados = _repositorioAgendamento.ContarAgendamentosTotais(),
                TotalEspontaneos = _repositorioAtendimento.ContarAtendimentoEspontaneos(),
                AgendamentosHoje = _repositorioAgendamento.BuscarAgendamentosDoDia(DateTime.Today),
            };

            return View(viewModel);
        }
        #endregion

        #region Recarregar Parciais via AJAX

        #region RecarregarIndicadores
        public IActionResult RecarregarIndicadores()
        {
            var viewModel = new AgendamentoAtendimentoViewMOD
            {
                TotalRealizados = _repositorioAtendimento.ContarAtendimentosRealizados(),
                TotalCancelados = _repositorioAtendimento.ContarAtendimentosCancelados(),
                TotalPendentes = _repositorioAtendimento.ContarAtendimentosPendentes(),
                TotalAgendados = _repositorioAgendamento.ContarAgendamentosTotais(),
                TotalEspontaneos = _repositorioAtendimento.ContarAtendimentoEspontaneos()
            };

            return PartialView("_PartialDadosAtendimentos", viewModel);
        }
        #endregion

        #region RecarregarAgendamentos
        public IActionResult RecarregarAgendamentos()
        {
            var viewModel = new AgendamentoAtendimentoViewMOD
            {
                AgendamentosHoje = _repositorioAgendamento.BuscarAgendamentosDoDia(DateTime.Today)
            };

            return PartialView("_PartialAgendamentosDiarios", viewModel);
        }
        #endregion

        #endregion

        #region Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? errorMessage)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = errorMessage });
        }
        #endregion

    }
}

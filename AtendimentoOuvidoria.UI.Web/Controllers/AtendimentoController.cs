using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Model;
using AtendimentoOuvidoria.Repository;
using AtendimentoOuvidoria.UI.Web.Models;
using X.PagedList;

namespace AtendimentoOuvidoria.UI.Web.Controllers
{
    [Authorize(Roles = "Admin, Comum")]
    public class AtendimentoController : Controller
    {
        #region Repositorios
        private readonly AtendimentoREP _repositorioAtendimento;
        private readonly MotivoAusenciaAtendimentoREP _repositorioMotivoAusenciaAtendimento;
        private readonly HumorAtendimentoREP _repositorioHumorAtendimento;
        private readonly AgendamentoREP _repositorioAgendamento;
        private readonly DataHoraAgendamentoREP _repositorioDataHoraAgendamento;
        private readonly BeneficiarioAgendamentoREP _repositorioBeneficiarioAgendamento;
        private readonly AcessaDados _acessaDados;
        private readonly IConfiguration _configuration;
        #endregion

        #region Construtor
        public AtendimentoController(AtendimentoREP repositorioAtendimento, MotivoAusenciaAtendimentoREP repositorioMotivoAusenciaAtendimento, HumorAtendimentoREP repositorioHumorAtendimento, AgendamentoREP repositorioAgendamento, DataHoraAgendamentoREP repositorioDataHoraAgendamento, BeneficiarioAgendamentoREP repositorioBeneficiarioAgendamento, IConfiguration configuration, AcessaDados acessaDados)
        {
            _repositorioAtendimento = repositorioAtendimento;
            _repositorioMotivoAusenciaAtendimento = repositorioMotivoAusenciaAtendimento;
            _repositorioHumorAtendimento = repositorioHumorAtendimento;
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

        #region Pendente
        public async Task<IActionResult> Pendente(int? pagina, string? filtro, DateTime? dtAgendamento)
        {
            int numeroPagina = pagina ?? 1;
            int take = _take;

            var resultado = await _repositorioAtendimento.BuscarAtendimentoPendentePaginado(numeroPagina, take, filtro, dtAgendamento);

            var atendimento = new AgendamentoAtendimentoViewMOD
            {
                ListaAgendamentoPaginada = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            CarregarMotivoAusenciaAtendimento();
            CarregarHumor();

            ViewBag.Titulo = "Atendimentos Pendentes";
            ViewBag.Filtro = filtro;
            ViewBag.DataAgendamento = dtAgendamento;
            return View("Pendente", atendimento);
        }
        #endregion

        #region RegistrarAtendimentoAgendado
        [HttpPost]
        public IActionResult RegistrarAtendimentoAgendado(AtendimentoMOD dadosTela)
        {
            var atendimento = new AtendimentoMOD
            {
                CdAgendamento = dadosTela.CdAgendamento,
                SnPresente = "S",
                SnPreenchido = "S",
                CdHumor = dadosTela.CdHumor,
                TxMotivoHumor = dadosTela.TxMotivoHumor,
                TxDescricaoAtendimento = dadosTela.TxDescricaoAtendimento,
                SnPossivelProcesso = dadosTela.SnPossivelProcesso,
                SnPossivelChurn = dadosTela.SnPossivelChurn,
                SnExistePendencia = dadosTela.SnExistePendencia,
                TxDescricaoPendencia = dadosTela.TxDescricaoPendencia,
                SnReversaoExperiencia = dadosTela.SnReversaoExperiencia,
                DtPreenchimento = DateTime.Now,
                CdUsuarioPreencheu = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value),
                CdTipoAtendimento = 1 // Tipo de Atendimento: 1 - Agendado
            };

            var sucesso = _repositorioAtendimento.RegistrarAtendimentoAgendado(atendimento);

            TempData[sucesso ? "Modal-Sucesso" : "Modal-Erro"] = sucesso
                ? "Atendimento agendado registrado com sucesso!"
                : "Erro ao registrar atendimento agendado.";

            return RedirectToAction("Realizado");
        }
        #endregion

        #region Realizado
        public async Task<IActionResult> Realizado(int? pagina, string? filtro, DateTime? dtAgendamento)
        {
            int numeroPagina = pagina ?? 1;

            var resultado = await _repositorioAtendimento.BuscarAtendimentosRealizadosPaginado(numeroPagina, _take, filtro, dtAgendamento);

            var atendimento = new AgendamentoAtendimentoViewMOD
            {
                ListaAtendimentoPaginada = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            ViewBag.Filtro = filtro;
            ViewBag.DataAgendamento = dtAgendamento;
            ViewBag.Titulo = "Atendimentos Realizados";
            return View("Realizado", atendimento);
        }
        #endregion

        #region CancelarAtendimento
        [HttpPost]
        public IActionResult CancelarAtendimento(AtendimentoMOD dadosTela)
        {
            var atendimento = new AtendimentoMOD
            {
                CdAgendamento = dadosTela.CdAgendamento,
                SnPresente = "N",
                SnPreenchido = "S",
                CdMotivoAusencia = dadosTela.CdMotivoAusencia,
                TxDescricaoAtendimento = dadosTela.TxDescricaoAtendimento,
                DtPreenchimento = DateTime.Now,
                CdUsuarioPreencheu = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value),
                CdTipoAtendimento = 1 // Tipo de Atendimento: 1 - Agendado
            };

            var cancelou = _repositorioAtendimento.CancelarAtendimento(atendimento);

            TempData[cancelou ? "Modal-Sucesso" : "Modal-Erro"] = cancelou
                ? "Atendimento cancelado com sucesso!"
                : "Ocorreu um erro ao tentar cancelar o atendimento. Tente novamente mais tarde.";

            return RedirectToAction("Pendente", "Atendimento");
        }

        #endregion

        #region Cancelado
        public async Task<IActionResult> Cancelado(int? pagina, string? filtro, DateTime? dtAgendamento)
        {
            int numeroPagina = pagina ?? 1;

            var resultado = await _repositorioAtendimento.BuscarAtendimentoCanceladoPaginado(numeroPagina, _take, filtro, dtAgendamento);

            var atendimento = new AgendamentoAtendimentoViewMOD
            {
                ListaAtendimentoPaginada = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            CarregarMotivoAusenciaAtendimento();

            ViewBag.Titulo = "Atendimentos Cancelados";
            ViewBag.Filtro = filtro;
            ViewBag.DataAgendamento = dtAgendamento;
            return View("Cancelado", atendimento);
        }
        #endregion

        #region RegistrarAtendimentoEspontaneo
        public IActionResult RegistrarAtendimentoEspontaneo()
        {
            CarregarHumor();

            AtendimentoEspontaneoViewMOD atendimento = new AtendimentoEspontaneoViewMOD();
            return View(atendimento);
        }
        [HttpPost]
        public IActionResult RegistrarAtendimentoEspontaneo(AtendimentoEspontaneoViewMOD dadosTela)
        {
            bool cadastrouBeneficiario = false;
            bool cadastrouAtendimento = false;

            var beneficiario = new BeneficiarioAgendamentoMOD
            {
                NoBeneficiario = dadosTela.Beneficiario.NoBeneficiario,
                NrCpf = dadosTela.Beneficiario.NrCpf,
                TxEmail = dadosTela.Beneficiario.TxEmail,
                TxTelefone = dadosTela.Beneficiario.TxTelefone,
                TxMotivoAgendamento = dadosTela.Beneficiario.TxMotivoAgendamento,
                SnIntercambio = dadosTela.Beneficiario.SnIntercambio
            };
            cadastrouBeneficiario = _repositorioBeneficiarioAgendamento.Cadastrar(beneficiario);

            if (cadastrouBeneficiario)
            {
                var atendimento = new AtendimentoMOD
                {
                    SnPresente = "S",
                    SnPreenchido = "S",
                    CdHumor = dadosTela.Atendimento.CdHumor,
                    TxMotivoHumor = dadosTela.Atendimento.TxMotivoHumor,
                    TxDescricaoAtendimento = dadosTela.Atendimento.TxDescricaoAtendimento,
                    SnPossivelProcesso = dadosTela.Atendimento.SnPossivelProcesso,
                    SnPossivelChurn = dadosTela.Atendimento.SnPossivelChurn,
                    SnExistePendencia = dadosTela.Atendimento.SnExistePendencia,
                    TxDescricaoPendencia = dadosTela.Atendimento.TxDescricaoPendencia,
                    SnReversaoExperiencia = dadosTela.Atendimento.SnReversaoExperiencia,
                    DtPreenchimento = DateTime.Now,
                    CdUsuarioPreencheu = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value),
                    CdBeneficiario = beneficiario.CdBeneficiario,
                    CdTipoAtendimento = 2 // Tipo de Atendimento: 2 - Espontâneo
                };

                cadastrouAtendimento = _repositorioAtendimento.RegistarAtendimentoEspontaneo(atendimento);


            }
            TempData[cadastrouAtendimento && cadastrouBeneficiario ? "Modal-Sucesso" : "Modal-Erro"] = cadastrouAtendimento && cadastrouBeneficiario
                ? "Atendimento espontâneo registrado com sucesso!"
                : "Erro ao registrar atendimento espontâneo.";

            return RedirectToAction("Espontaneo");
        }
        #endregion

        #region Espontaneo
        public async Task<IActionResult> Espontaneo(int? pagina, string? filtro, DateTime? dtAtendimento)
        {
            int numeroPagina = pagina ?? 1;

            var resultado = await _repositorioAtendimento.BuscarAtendimentoEspontaneoPaginado(numeroPagina, _take, filtro, dtAtendimento);

            var viewModel = new AgendamentoAtendimentoViewMOD
            {
                ListaAtendimentoPaginada = resultado.Dados,
                QtdTotalDeRegistros = resultado.Paginacao.TotalItens,
                PaginaAtual = resultado.Paginacao.PaginaAtual,
                TotalPaginas = resultado.Paginacao.TotalPaginas
            };

            ViewBag.Filtro = filtro;
            ViewBag.DataAtendimento = dtAtendimento;
            ViewBag.Titulo = "Atendimentos Espontâneos";
            return View("Espontaneo", viewModel);
        }
        #endregion

        #region Util

        #region CarregarMotivoAusenciaAtendimento
        public void CarregarMotivoAusenciaAtendimento()
        {
            var listaMotivoAusencia = _repositorioMotivoAusenciaAtendimento
                .Buscar()
                .Where(x => x.SnAtivo == "S")
                .OrderBy(x => x.NoMotivo)
                .ToList();

            ViewData["listaMotivoAusencia"] = listaMotivoAusencia;
        }

        #endregion

        #region CarregarHumor
        private void CarregarHumor()
        {
            var listaHumor = _repositorioHumorAtendimento.Buscar().ToList();
            ViewData["listaHumor"] = listaHumor;
        }

        #endregion

        #endregion

        #endregion
    }
}

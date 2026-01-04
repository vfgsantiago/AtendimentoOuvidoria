using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using AtendimentoOuvidoria.Data;
using AtendimentoOuvidoria.Helpers;
using AtendimentoOuvidoria.Model;
using AtendimentoOuvidoria.Repository;
using AtendimentoOuvidoria.UI.Web.Models;

namespace AtendimentoOuvidoria.UI.Web.Controllers
{
    public class DataHoraAgendamentoController : Controller
    {
        #region Repositorios
        private readonly DataHoraAgendamentoREP _repositorioDataHoraAgendamento;
        private readonly AgendamentoREP _repositorioAgendamento;
        #endregion

        #region Construtor
        public DataHoraAgendamentoController(DataHoraAgendamentoREP repositorioDataHoraAgendamento, AgendamentoREP repositorioAgendamento)
        {
            _repositorioDataHoraAgendamento = repositorioDataHoraAgendamento;
            _repositorioAgendamento = repositorioAgendamento;
        }
        #endregion

        #region Semana
        [Authorize(Roles = "Admin, Comum")]
        public IActionResult Semana(DateTime? data)
        {
            var dataBase = data ?? DateTime.Today;
            var inicioSemana = dataBase.StartOfWeek(DayOfWeek.Monday);
            var fimSemana = inicioSemana.AddDays(6);

            var listaDataHora = _repositorioDataHoraAgendamento.Buscar()
                .Where(h => h.DtDataHora.Date >= inicioSemana && h.DtDataHora.Date <= fimSemana)
                .ToList();


            foreach (var horario in listaDataHora.Where(h => h.SnTemAgendamento == "S"))
            {
                horario.Agendamento = _repositorioAgendamento.BuscarPorCdDataHora(horario.CdDataHora);
            }

            var semana = new SemanaAgendaViewMOD
            {
                NumeroSemana = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
            inicioSemana, CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                ListaDataHora = listaDataHora
            .GroupBy(h => h.DtDataHora.Date)
            .OrderBy(d => d.Key)
            .ToDictionary(d => d.Key, d => d.OrderBy(h => h.DtDataHora).ToList())
            };

            ViewBag.DataBase = inicioSemana;
            return View("Semana", semana);

        }
        #endregion

        #region CalendarioMensal
        [Authorize(Roles = "Admin, Comum")]
        public IActionResult CalendarioMensal(int? ano, int? mes)
        {
            var dataReferencia = new DateTime(ano ?? DateTime.Today.Year, mes ?? DateTime.Today.Month, 1);
            var primeiroDia = dataReferencia;
            var ultimoDia = primeiroDia.AddMonths(1).AddDays(-1);

            var listaDataHora = _repositorioDataHoraAgendamento.Buscar()
                .Where(h => h.DtDataHora.Date >= primeiroDia && h.DtDataHora.Date <= ultimoDia)
                .ToList();


            foreach (var horario in listaDataHora.Where(h => h.SnTemAgendamento == "S"))
            {
                horario.Agendamento = _repositorioAgendamento.BuscarPorCdDataHora(horario.CdDataHora);
            }

            var diaDoMes = listaDataHora
                .GroupBy(h => h.DtDataHora.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            ViewBag.Mes = dataReferencia;
            ViewBag.Dias = diaDoMes;

            return View("CalendarioMensal", diaDoMes);
        }
        #endregion

        #region CadastrarSemana
        [Authorize(Roles = "Admin")]
        public IActionResult CadastrarSemana()
        {
            SemanaAdministrativaViewMOD semanaAdministrativa = new SemanaAdministrativaViewMOD
            {
                DataInicioSemana = DateTime.Today.StartOfWeek(DayOfWeek.Monday)
            };
            return View(semanaAdministrativa);
        }

        [HttpPost]
        public IActionResult CadastrarSemana(SemanaAdministrativaViewMOD dadosTela)
        {
            bool cadastrou = false;

            int horariosCadastrados = 0;
            int horariosIgnorados = 0;

            if (!ModelState.IsValid)
            {
                TempData["Modal-Erro"] = "Preencha corretamente os campos";
                return View(dadosTela);
            }

            var horariosPorDia = new Dictionary<DayOfWeek, (TimeSpan inicio, TimeSpan fim)>
            {
                {DayOfWeek.Monday, (TimeSpan.Parse("09:00"), TimeSpan.Parse("18:00")) },
                {DayOfWeek.Tuesday, (TimeSpan.Parse("09:00"), TimeSpan.Parse("18:00")) },
                {DayOfWeek.Wednesday, (TimeSpan.Parse("09:00"), TimeSpan.Parse("18:00")) },
                {DayOfWeek.Thursday, (TimeSpan.Parse("09:00"), TimeSpan.Parse("18:00")) },
                {DayOfWeek.Friday, (TimeSpan.Parse("08:00"), TimeSpan.Parse("17:00")) }
            };

            var diasSelecionados = new List<DayOfWeek>();
            if (dadosTela.Segunda) diasSelecionados.Add(DayOfWeek.Monday);
            if (dadosTela.Terca) diasSelecionados.Add(DayOfWeek.Tuesday);
            if (dadosTela.Quarta) diasSelecionados.Add(DayOfWeek.Wednesday);
            if (dadosTela.Quinta) diasSelecionados.Add(DayOfWeek.Thursday);
            if (dadosTela.Sexta) diasSelecionados.Add(DayOfWeek.Friday);

            var horariosParaCadastrar = new List<DataHoraAgendamentoMOD>();
            var usuarioId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "CdUsuario")?.Value);

            foreach (var dia in diasSelecionados)
            {
                var data = dadosTela.DataInicioSemana.StartOfWeek(DayOfWeek.Monday).AddDays((int)dia - 1);
                var (inicio, fim) = horariosPorDia[dia];

                for (var hora = inicio; hora < fim; hora = hora.Add(TimeSpan.FromHours(1)))
                {
                    horariosParaCadastrar.Add(new DataHoraAgendamentoMOD
                    {
                        DtDataHora = data.Date + hora,
                        SnDisponivel = "S",
                        SnTemAgendamento = "N",
                        DtCadastro = DateTime.Now,
                        CdUsuarioCadastrou = usuarioId
                    });
                }
            }
            foreach (var horario in horariosParaCadastrar)
            {
                if (_repositorioDataHoraAgendamento.ValidarHorario(horario.DtDataHora))
                {
                    if (_repositorioDataHoraAgendamento.Cadastrar(horario))
                    {
                        horariosCadastrados++;
                        cadastrou = true;
                    }
                    else
                    {
                        horariosIgnorados++;
                    }
                }

            }
            TempData[cadastrou ? "Modal-Sucesso" : "Modal-Erro"] = cadastrou ? $"Horários cadastrados com sucesso! ({horariosCadastrados} novos, {horariosIgnorados} já existentes)" : "Erro ao cadastrar, horários já existentes. Tente novamente.";

            return RedirectToAction("Semana", "DataHoraAgendamento");
        }
        #endregion

        #region AlterarStatus
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AlterarStatus(Int32 cdDataHora, string motivo)
        {
            var dataHora = _repositorioDataHoraAgendamento.BuscarPorCodigo(cdDataHora);
            if (dataHora == null) return NotFound();

            dataHora.SnDisponivel = dataHora.SnDisponivel == "S" ? "N" : "S";
            dataHora.DtAlteracao = DateTime.Now;
            dataHora.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Contains("CdUsuario"))?.Value);
            dataHora.TxMotivoInativacao = dataHora.SnDisponivel == "N" ? motivo : null;

            var alterou = _repositorioDataHoraAgendamento.AlterarStatus(dataHora);
            return alterou ? Ok() : StatusCode(500);
        }
        #endregion
    }
}

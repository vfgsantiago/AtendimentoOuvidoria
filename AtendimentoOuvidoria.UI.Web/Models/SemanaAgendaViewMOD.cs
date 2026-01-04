using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.UI.Web.Models
{
    public class SemanaAgendaViewMOD
    {
        public int NumeroSemana { get; set; }
        public Dictionary<DateTime, List<DataHoraAgendamentoMOD>> ListaDataHora { get; set; }
        public AgendamentoMOD Agendamento { get; set; } = new AgendamentoMOD();
    }
}

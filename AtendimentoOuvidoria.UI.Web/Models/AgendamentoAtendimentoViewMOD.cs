using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.UI.Web.Models
{
    public class AgendamentoAtendimentoViewMOD
    {
        public AgendamentoMOD Agendamento { get; set; } = new AgendamentoMOD();
        public AtendimentoMOD Atendimento { get; set; } = new AtendimentoMOD();
        public BeneficiarioAgendamentoMOD Beneficiario { get; set; } = new BeneficiarioAgendamentoMOD();
        public DataHoraAgendamentoMOD DataHora { get; set; } = new DataHoraAgendamentoMOD();
        public HumorAtendimentoMOD Humor { get; set; } = new HumorAtendimentoMOD();
        public MotivoAusenciaAtendimentoMOD MotivoAusencia { get; set; } = new MotivoAusenciaAtendimentoMOD();

        public List<AgendamentoMOD> ListaAgendamentoPaginada { get; set; } = new List<AgendamentoMOD>();
        public List<AtendimentoMOD> ListaAtendimentoPaginada { get; set; } = new List<AtendimentoMOD>();

        public int QtdTotalDeRegistros { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }

        public int TotalRealizados { get; set; }
        public int TotalPendentes { get; set; }
        public int TotalCancelados { get; set; }
        public int TotalAgendados { get; set; }
        public int TotalEspontaneos { get; set; }

        public List<AgendamentoMOD> AgendamentosHoje { get; set; } = new();
    }
}

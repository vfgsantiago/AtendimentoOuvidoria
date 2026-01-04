using AtendimentoOuvidoria.Model;

namespace AtendimentoOuvidoria.UI.Web.Models
{
    public class AtendimentoEspontaneoViewMOD 
    {
        public AtendimentoMOD Atendimento { get; set; } = new AtendimentoMOD();
        public BeneficiarioAgendamentoMOD Beneficiario { get; set; } = new BeneficiarioAgendamentoMOD();
    }
}

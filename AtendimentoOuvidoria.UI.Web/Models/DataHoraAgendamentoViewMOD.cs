using System.ComponentModel.DataAnnotations;

namespace AtendimentoOuvidoria.UI.Web.Models
{
    public class DataHoraAgendamentoViewMOD
    {
        [Required(ErrorMessage = "Informe a data e hora")]
        public DateTime DtDataHora { get; set; }
        public int CdDataHora { get; set; }
        public string SnDisponivel { get; set; }
        public string SnTemAgendamento { get; set; }
        public DateTime DtCadastro { get; set; }
        public int CdUsuarioCadastrou { get; set; }
        public DateTime DtAlteracao { get; set; }
        public int CdUsuarioAlterou { get; set; }
    }
}

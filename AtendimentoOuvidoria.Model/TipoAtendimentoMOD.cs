using System.ComponentModel.DataAnnotations;

namespace AtendimentoOuvidoria.Model
{
    public class TipoAtendimentoMOD
    {
        [Display(Name = "Código do Tipo de Atendimento")]
        public Int32 CdTipoAtendimento { get; set; }

        [Display(Name = "Tipo de Atendimento")]
        public String NoTipoAtendimento { get; set; }

        [Display(Name = "Descrição do Tipo de Atendimento")]
        public String TxDescricaoTipo { get; set; }
    }
}

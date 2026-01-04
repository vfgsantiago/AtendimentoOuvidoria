using System.ComponentModel.DataAnnotations;

namespace AtendimentoOuvidoria.Model
{
    public class HumorAtendimentoMOD
    {
        [Display(Name = "Código do Humor")]
        public Int32 CdHumor { get; set; }

        [Display(Name = "Descrição do Humor")]
        public string TxHumor { get; set; }

        [Display(Name = "Icone do Humor")]
        public string TxIcone { get; set; }

        [Display(Name = "Cor do Icone")]
        public string TxCor { get; set; }
    }
}

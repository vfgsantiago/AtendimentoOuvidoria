using System.ComponentModel.DataAnnotations;

namespace AtendimentoOuvidoria.Model
{
    public class BeneficiarioAgendamentoMOD
    {
        [Display(Name = "Código do Beneficiário")]
        public Int32 CdBeneficiario { get; set; }

        [Display(Name = "Nome do Beneficiário")]
        public string NoBeneficiario { get; set; }

        [Display(Name = "Número do Protocolo SAC")]
        public string NrProtocoloSac { get; set; }

        [Display(Name = "CPF do Beneficiário")]
        public string NrCpf { get; set; }

        [Display(Name = "E-mail do Beneficiário")]
        public string TxEmail { get; set; }

        [Display(Name = "Telefone do Beneficiário")]
        public string TxTelefone { get; set; }

        [Display(Name = "Motivo do Agendamento")]
        public string TxMotivoAgendamento { get; set; }

        [Display(Name = "Beneficiário de Intercâmbio")]
        public string SnIntercambio { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace AtendimentoOuvidoria.Model
{
    public class AgendamentoMOD
    {
        [Display(Name = "Código do Agendamento")]
        public Int32 CdAgendamento { get; set; }

        [Display(Name = "Código do Beneficiário")]
        public Int32 CdBeneficiario { get; set; }

        [Display(Name = "Nome do Beneficiário")]
        public string NoBeneficiario { get; set; }

        [Display(Name = "CPF do Beneficiário")]
        public string NrCpf { get; set; }

        [Display(Name = "E-mail do Beneficiário")]
        public string TxEmail { get; set; }

        [Display(Name = "Telefone do Beneficiário")]
        public string TxTelefone { get; set; }

        [Display(Name = "Motivo do Agendamento")]
        public string TxMotivoAgendamento { get; set; }

        [Display(Name = "Data Agendada")]
        public Int32 CdDataHora { get; set; }

        [Display(Name = "Horário do Agendamento")]
        public DateTime DtDataHora { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Protocolo do Agendamento")]
        public string NrProtocoloAgendamento { get; set; }

        [Display(Name = "Atendimento preenchido?")]
        public string SnPreenchido { get; set; }

        [Display(Name = "Motivo de Ausência")]
        public int CdMotivoAusencia { get; set; }

        [Display(Name = "Motivo de Ausência")]
        public string NoMotivo { get; set; }

        [Display(Name = "Tipo do Atendimento")]
        public Int32 CdTipoAtendimento { get; set; }

        public string NoTipoAtendimento { get; set; }

        public int TotalRegistros { get; set; }

    }
}

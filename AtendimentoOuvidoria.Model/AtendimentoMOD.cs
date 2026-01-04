using System.ComponentModel.DataAnnotations;

namespace AtendimentoOuvidoria.Model
{
    public class AtendimentoMOD
    {
        [Display(Name = "Código do Atendimento")]
        public Int32 CdAtendimento { get; set; }

        [Display(Name = "Código do Agendamento")]
        public Int32 CdAgendamento { get; set; }

        [Display(Name = "Beneficiário esteve presente?")]
        public string? SnPresente { get; set; }

        [Display(Name = "Código do Motivo de Ausência")]
        public Int32 CdMotivoAusencia { get; set; }
        public string? NoMotivo { get; set; }

        [Display(Name = "Código do Humor")]
        public Int32 CdHumor { get; set; }

        public string? TxHumor { get; set; }

        public string? TxIcone { get; set; }

        public string? TxCor { get; set; }

        [Display(Name = "Motivo do Humor")]
        public string? TxMotivoHumor { get; set; }

        [Display(Name = "Descrição do Atendimento")]
        public string? TxDescricaoAtendimento { get; set; }

        [Display(Name = "Possível Processo?")]
        public string? SnPossivelProcesso { get; set; }

        [Display(Name = "Possível Churn?")]
        public string? SnPossivelChurn { get; set; }

        [Display(Name = "Existe alguma pendência?")]
        public string? SnExistePendencia { get; set; }

        [Display(Name = "Descrição da Pendência")]
        public string? TxDescricaoPendencia { get; set; }

        [Display(Name = "Ocorreu uma reversão de experiência do beneficiário?")]
        public string? SnReversaoExperiencia { get; set; }

        [Display(Name = "Atendimento foi preenchido?")]
        public string? SnPreenchido { get; set; }

        [Display(Name = "Horário do Preenchimento do Atendimento")]
        public DateTime DtPreenchimento { get; set; }

        [Display(Name = "Usuário que Preencheu o Atendimento")]
        public Int32 CdUsuarioPreencheu { get; set; }

        [Display(Name = "Beneficiário")]
        public Int32 CdBeneficiario { get; set; }

        public string? NoBeneficiario { get; set; }

        public string? NrCpf { get; set; }

        public string? TxEmail { get; set; }
        public string? TxTelefone { get; set; }
        public string? NrProtocoloAgendamento { get; set; }
        public string? TxMotivoAgendamento { get; set; }
        public string? SnIntercambio { get; set; }
        public Int32 CdDataHora { get; set; }
        public DateTime DtDataHora { get; set; }

        [Display(Name = "Tipo do Atendimento")]
        public Int32 CdTipoAtendimento { get; set; }
        public string? NoTipoAtendimento { get; set; }

        [Display(Name = "Usuário que Cadastrou o Horário")]
        public string? NoUsuario { get; set; }

        public AgendamentoMOD Agendamento { get; set; } = new AgendamentoMOD();

        public MotivoAusenciaAtendimentoMOD MotivoAusencia { get; set; } = new MotivoAusenciaAtendimentoMOD();

        public HumorAtendimentoMOD Humor { get; set; } = new HumorAtendimentoMOD();
    }
}

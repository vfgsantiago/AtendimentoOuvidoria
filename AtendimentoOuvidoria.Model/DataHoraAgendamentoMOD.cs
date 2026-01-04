using System.ComponentModel.DataAnnotations;

namespace AtendimentoOuvidoria.Model
{
    public class DataHoraAgendamentoMOD
    {
        [Display(Name = "Código do Horário")]
        public Int32 CdDataHora { get; set; }

        [Display(Name = "Horário do Agendamento")]
        public DateTime DtDataHora { get; set; }

        [Display(Name = "Disponibilidade do Horário")]
        public string SnDisponivel { get; set; }

        [Display(Name = "Existe Agendamento para o Horário")]
        public string SnTemAgendamento { get; set; }

        [Display(Name = "Data e Hora de Cadastro de Horário")]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Usuário que Cadastrou o Horário")]
        public int CdUsuarioCadastrou { get; set; }

        [Display(Name = "Usuário que Cadastrou o Horário")]
        public string NoUsuario { get; set; }

        [Display(Name = "Data e Hora de Alteração de Horário")]
        public DateTime DtAlteracao { get; set; }

        [Display(Name = "Usuário que Alterou o Horário")]
        public int CdUsuarioAlterou { get; set; }

        [Display(Name = "Descrição do Motivo da Inativação")]
        public string TxMotivoInativacao { get; set; }

        public AgendamentoMOD Agendamento { get; set; } = new AgendamentoMOD();
    }
}

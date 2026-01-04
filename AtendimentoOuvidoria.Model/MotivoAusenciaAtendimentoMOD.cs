using System.ComponentModel.DataAnnotations;

namespace AtendimentoOuvidoria.Model
{
    public class MotivoAusenciaAtendimentoMOD
    {
        [Display(Name = "Código do Motivo de Ausência")]
        public Int32 CdMotivoAusencia { get; set; }

        [Display(Name = "Título do Motivo de Ausência")]
        public string NoMotivo { get; set; }

        [Display(Name = "Descrição do Motivo de Ausência")]
        public string TxDescricaoMotivo { get; set; }

        [Display(Name = "Data de Cadastro do Motivo de Ausência")]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Usuário que Cadastrou o Motivo de Ausência")]
        public Int32 CdUsuarioCadastrou { get; set; }

        [Display(Name = "Data de Alteração do Motivo de Ausência")]
        public DateTime DtAlteracao { get; set; }

        [Display(Name = "Usuário que Alterou o Motivo de Ausência")]
        public Int32 CdUsuarioAlterou { get; set; }

        [Display(Name = "Disponibilidade")]
        public string SnAtivo { get; set; }
    }
}

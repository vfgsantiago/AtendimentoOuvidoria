using System.ComponentModel.DataAnnotations;

namespace AtendimentoOuvidoria.UI.Web.Models
{
    public class SemanaAdministrativaViewMOD
    {
        [Display(Name = "Data de início da semana")]
        [Required(ErrorMessage = "Informe a data de início da semana.")]
        [DataType(DataType.Date)]
        public DateTime DataInicioSemana { get; set; }

        public bool Segunda { get; set; } = true;
        public bool Terca { get; set; } = true;
        public bool Quarta { get; set; } = true;
        public bool Quinta { get; set; } = true;
        public bool Sexta { get; set; } = true;
    }
}

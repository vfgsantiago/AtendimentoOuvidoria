namespace AtendimentoOuvidoria.UI.Web.Models
{
    public class UsuarioViewMOD
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Cpf { get; set; }

        public AvatarViewMOD Avatar { get; set; }

        public string PrimeiroNome(string Nome)
        {
            string[] nomeQuebrado = Nome.Split(' ');
            string nomeCurto = string.Format("{0}", nomeQuebrado.First());
            return nomeCurto;
        }
    }
}

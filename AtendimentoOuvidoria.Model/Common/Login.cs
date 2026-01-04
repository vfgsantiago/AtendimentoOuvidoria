namespace AtendimentoOuvidoria.Model
{
    public record Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int Api { get; set; }
    }
}

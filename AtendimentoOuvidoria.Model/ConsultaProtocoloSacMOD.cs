namespace AtendimentoOuvidoria.Model
{
    public class ConsultaProtocoloSacMOD
    {
        public string NrProtocoloAns { get; set; }

        public string? CdAtendCallCenter { get; set; }

        public DateTime DtInicio { get; set; }

        public DateTime? DtTermino { get; set; }

        public string? DsDetalheAtendimento { get; set; }

        public bool ProtocoloValido { get; set; }
    }
}

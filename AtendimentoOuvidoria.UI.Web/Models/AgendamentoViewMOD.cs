using AtendimentoOuvidoria.Model;
using X.PagedList;

namespace AtendimentoOuvidoria.UI.Web.Models
{
    public class AgendamentoViewMOD
    {
        public IPagedList<AgendamentoMOD> ListaAgendamentoPaginada { get; set; }
        public Int32 QtdTotalDeRegistros { get; set; }

        public int CdAgendamento { get; set; }
        public int CdBeneficiario { get; set; }
        public string NoBeneficiario { get; set; }
        public string NrCpf { get; set; }
        public string TxEmail { get; set; }
        public string TxTelefone { get; set; }
        public string TxMotivoAgendamento { get; set; }
        public int CdDataHora { get; set; }
        public DateTime DtCadastro { get; set; }
        public DateTime DtDataHora { get; set; }
        public string NrProtocoloAgendamento { get; set; }
        public string SnPreenchido { get; set; }
        public int TotalRegistros { get; set; }
        public int CdMotivoAusencia { get; set; }
        public string NoMotivo { get; set; }

        public BeneficiarioAgendamentoViewMOD Beneficiario { get; set; }
        public DataHoraAgendamentoViewMOD DataHora { get; set; }

    }
}

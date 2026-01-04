using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AtendimentoOuvidoria.Model;
using AtendimentoOuvidoria.Repository;

namespace AtendimentoOuvidoria.UI.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MotivoAusenciaAtendimentoController : Controller
    {
        #region Repositorios
        private readonly MotivoAusenciaAtendimentoREP _repositorio;
        #endregion

        #region Construtor
        public MotivoAusenciaAtendimentoController(MotivoAusenciaAtendimentoREP repositorio)
        {
            _repositorio = repositorio;
        }
        #endregion

        #region Index
        public IActionResult Index()
        {
            var motivos = _repositorio.Buscar();
            return View(motivos);
        }
        #endregion

        #region Cadastrar
        public IActionResult Cadastrar()
        {
            MotivoAusenciaAtendimentoMOD motivoAusencia = new MotivoAusenciaAtendimentoMOD
            {
                SnAtivo = "S"
            };
            return View(motivoAusencia);
        }

        [HttpPost]
        public IActionResult Cadastrar(MotivoAusenciaAtendimentoMOD motivoAusencia)
        {
            if (!ModelState.IsValid)
                return View(motivoAusencia);

            motivoAusencia.NoMotivo = motivoAusencia.NoMotivo;
            motivoAusencia.TxDescricaoMotivo = motivoAusencia.TxDescricaoMotivo;
            motivoAusencia.DtCadastro = DateTime.Now;
            motivoAusencia.CdUsuarioCadastrou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "CdUsuario")?.Value);

            var cadastrou = _repositorio.Cadastrar(motivoAusencia);

            if (cadastrou)
                TempData["Modal-Sucesso"] = "Motivo de ausência cadastrado com sucesso!";
            else
                TempData["Modal-Erro"] = "Erro ao cadastrar motivo de ausência. Tente novamente.";

            return RedirectToAction("Index", "MotivoAusenciaAtendimento");

        }
        #endregion

        #region Editar
        public IActionResult Editar(Int32 cdMotivoAusencia)
        {
            var motivo = _repositorio.BuscarPorCodigo(cdMotivoAusencia);
            if (motivo == null)
            {
                TempData["Modal-Erro"] = "Motivo de ausência não encontrado.";
                return RedirectToAction("Index", "MotivoAusenciaAtendimento");
            }

            return View(motivo);
        }

        [HttpPost]
        public IActionResult Editar(MotivoAusenciaAtendimentoMOD motivoAusencia)
        {
            if (!ModelState.IsValid)
                TempData["Modal-Erro"] = "Dados inválidos. Verifique os campos e tente novamente.";
            
            motivoAusencia.NoMotivo = motivoAusencia.NoMotivo;
            motivoAusencia.TxDescricaoMotivo = motivoAusencia.TxDescricaoMotivo;
            motivoAusencia.DtAlteracao = DateTime.Now;
            motivoAusencia.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "CdUsuario")?.Value);

            var editou = _repositorio.Editar(motivoAusencia);
            if (editou)
                TempData["Modal-Sucesso"] = "Motivo de ausência editado com sucesso!";
            else
                TempData["Modal-Erro"] = "Erro ao editar motivo de ausência. Tente novamente.";

            return RedirectToAction("Index", "MotivoAusenciaAtendimento");

        }
        #endregion

        #region AlterarStatus
        [HttpPost]
        public IActionResult AlterarStatus(Int32 cdMotivoAusencia)
        {
            var motivo = _repositorio.BuscarPorCodigo(cdMotivoAusencia);
            if (motivo == null) return NotFound();

            motivo.SnAtivo = motivo.SnAtivo == "S" ? "N" : "S";
            motivo.DtAlteracao = DateTime.Now;
            motivo.CdUsuarioAlterou = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "CdUsuario")?.Value);

            var alterouStatus =_repositorio.AlterarStatus(motivo);

            if (alterouStatus)
                TempData["Modal-Sucesso"] = $"Motivo de ausência {(motivo.SnAtivo == "S" ? "ativado" : "desativado")} com sucesso!";
            else
                TempData["Modal-Erro"] = "Erro ao alterar status do motivo de ausência. Tente novamente.";

            return RedirectToAction("Index");
        }
        #endregion
    }
}

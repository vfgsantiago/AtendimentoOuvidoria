using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AtendimentoOuvidoria.Model;
using AtendimentoOuvidoria.Repository;

namespace AtendimentoOuvidoria.UI.Web.Controllers
{
    [Authorize(Roles = "Admin, Comum")]
    public class ConsultaProtocoloSacController : Controller
    {
        #region Repositorios
        private readonly ConsultaProtocoloSacREP _repositorio;
        #endregion

        #region Construtor
        public ConsultaProtocoloSacController(ConsultaProtocoloSacREP repositorio)
        {
            _repositorio = repositorio;
        }
        #endregion

        #region Index
        public IActionResult Index()
        {
            ConsultaProtocoloSacMOD consultaProtocoloSac = new ConsultaProtocoloSacMOD();
            return View(consultaProtocoloSac);
        }
        #endregion

        #region Consultar
        [HttpPost]
        public IActionResult Consultar(string nrProtocoloSac)
        {
            if (string.IsNullOrEmpty(nrProtocoloSac))
            {
                TempData["Modal-Erro"] = "O número do protocolo não pode ser vazio.";
                return RedirectToAction("Index", new ConsultaProtocoloSacMOD());
            }

            var consulta = _repositorio.ConsultaProtocoloSac(nrProtocoloSac);

            if (consulta == null)
            {
                TempData["Modal-Erro"] = "Nenhum protocolo encontrado com o número informado.";
                return RedirectToAction("Index");
            }

            return View("Index", consulta);
        }
        #endregion
    }
}
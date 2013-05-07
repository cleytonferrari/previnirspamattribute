using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrevinirSpam.Models;
using PrevinirSpam.Atributo;

namespace PrevinirSpam.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [PrevinirSpam(DelayRequest = 30)]
        public ActionResult Index(Comentario comentario)
        {
            //Verifica se o model é valido, se for manda uma mensagem de sucesso!
            if (ModelState.IsValid)
            {
                return Content(string.Format("A mensagem: {0}, foi envida com sucesso!", comentario.Mensagem));
            }
            //Se não for valido retorna a mensagens de validação
            return View(comentario);

        }

    }
}

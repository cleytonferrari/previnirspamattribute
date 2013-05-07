using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace PrevinirSpam.Atributo
{
    public class PrevinirSpamAttribute : ActionFilterAttribute
    {
        //Tempo de espera entre as requisições em segundos
        public int DelayRequest = 10;
        //Mensagem que sera exibida em caso de excesso de requisições
        public string ErrorMessage = "Muitas requisições detectadas.";
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Armazena o HttpContext para ser usado
            var request = filterContext.HttpContext.Request;
            //Armazena o HttpContext.Cache para ser referenciado
            var cache = filterContext.HttpContext.Cache;

            //Pega o IP da requisição de origem 
            var originationInfo = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;

            //Adiciona o user agente (navegador)
            originationInfo += request.UserAgent;

            //Agora vamos marcar as requisições da url
            var targetInfo = request.RawUrl + request.QueryString;

            //Gera um Hash da string (não é necessário)
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originationInfo + targetInfo)).Select(s => s.ToString("x2")));

            //Checa se ja não existe em cache
            if (cache[hashValue] != null)
            {
                //Se exister adiciona o erro
                filterContext.Controller.ViewData.ModelState.AddModelError("ExcessiveRequests", ErrorMessage);
            }
            else
            {
                //Adiciona uma nova entrada no cache para verificar se a entrada é valida ou não
                cache.Add(hashValue, new object(), null, DateTime.Now.AddSeconds(DelayRequest), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
using System.Web.Mvc;

namespace Paypal_MVC3_Sample.Controllers
{
    public class PaypalErrorsController : Controller
    {
        /// <summary>
        /// à utiliser en cas d'erreur dans paypal
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string ErrorCode)
        {
            return View("Index",(object)ErrorCode);
        }
    }
}

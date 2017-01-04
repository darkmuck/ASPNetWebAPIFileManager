using Newtonsoft.Json;
using System.Web.Mvc;

namespace WebAPIFileManager.Controllers
{
	public class DefaultController : Controller
    {
        
        public ActionResult Index(string directory)
		{
            return View();
		}
        
    }
}
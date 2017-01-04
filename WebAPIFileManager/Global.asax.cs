using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;

namespace WebAPIFileManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {        
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(App_Start.WebApiCfg.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}

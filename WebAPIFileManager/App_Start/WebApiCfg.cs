using System.Net.Http.Headers;
using System.Web.Http;

namespace WebAPIFileManager.App_Start
{
    public class WebApiCfg
    {
        public static void Register(HttpConfiguration conf)
        {
            conf.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            conf.MapHttpAttributeRoutes();
            conf.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{dir}",
                defaults: new { dir = RouteParameter.Optional }
            );
        }
    }
}
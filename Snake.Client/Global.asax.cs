using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Snake.Client
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Config.DATABASE_CONNECTION_STRING = System.Configuration.ConfigurationManager.ConnectionStrings["UserDb"].ConnectionString;

            GameConnection.Game = new Server.Game();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}

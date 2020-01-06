using System.Web.Mvc;
using System.Web.Routing;

namespace MyJCBApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Organisation", action = "Index", id = UrlParameter.Optional }
            );

           

            //routes.MapRoute(
            //    name: "GetOrganisations",
            //    url: "ManageImport/GetOrganisations/{id}",
            //    defaults: new { controller = "ManageImport", action = "GetOrganisations", id = UrlParameter.Optional });
        }
    }
}

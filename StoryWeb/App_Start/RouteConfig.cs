using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StoryWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{chapterIndex}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, chapterIndex = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Default2",
               url: "{controller}/{action}/{storyId}/{chapterIndex}",
               defaults: new { controller = "Home", action = "Index", storyId = UrlParameter.Optional, chapterIndex = UrlParameter.Optional }
           );
        }
    }
}

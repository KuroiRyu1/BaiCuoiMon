using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StoryWeb.Models.ModelView;

namespace StoryWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var requestPath = app.Context.Request.Url.AbsolutePath.ToLower();

            if (requestPath.Contains("/admin"))
            {
                if (app.Context.Session["user"] == null)
                {
                    app.Context.Session["TempData"] = new TempDataDictionary { ["ErrorMessage"] = "Bạn cần đăng nhập để truy cập trang này." };
                    app.Context.Response.RedirectToRoute(new { controller = "User", action = "Login" });
                }
                else
                {
                    User usr = (User)app.Context.Session["user"];

                    if (usr.Role != 1) 
                    {
                        app.Context.Session["TempData"] = new TempDataDictionary { ["ErrorMessage"] = "Bạn không có quyền truy cập trang này." };
                        app.Context.Response.RedirectToRoute(new { controller = "Home", action = "Index" });
                    }
                }
            }
        }

    }
}

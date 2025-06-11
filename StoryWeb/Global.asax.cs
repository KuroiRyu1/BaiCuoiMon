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
            //var app=(HttpApplication)sender;
            //var uriObj = app.Context.Request.Url.AbsolutePath;
            //if (uriObj.ToLower().Contains("admin") ||uriObj.ToLower().Contains("userinfo"))
            //{
            //    if (app.Context.Session["user"] != null)
            //    {
            //        User user = (User)app.Context.Session["user"];
            //        var user_role = user.Role;
            //        if (user_role != 0&& uriObj.ToLower().Contains("admin"))
            //        {
            //            app.Context.Response.RedirectToRoute(new { controller = "Home", action = "index" });
            //        }
            //    }
            //    else if (uriObj.ToLower().Contains("admin")||uriObj.ToLower().Contains("userinfo"))
            //    {
            //        app.Context.Response.RedirectToRoute(new { controller = "User", action = "Login" });
            //    }
            //}
        }
    }
    
}

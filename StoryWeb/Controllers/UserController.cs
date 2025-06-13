using StoryWeb.Models;
using StoryWeb.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StoryWeb.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserInfo()
        {
            return View();
        }
        public ActionResult Register()
        {
            if (Session["user"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public string Code(string email)
        {
            string result = Function.SendMail(email);
            return result;
        }
        public async Task<ActionResult> RegisterConfirm()
        {
            try
            {
                string Username = Request.Form["username"];
                string Password = Request.Form["password"];
                if (Username != "" && Password != "")
                {
                    var user = await UserRep.Instance.login(Username, Password);
                    if (user != null)
                    {
                        Session["user"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login", "User");
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return RedirectToAction("Index");
        }
        public ActionResult Login()
        {
            if (Session["user"]!=null)
            {
                return RedirectToAction("index","Home");
            }
            return View();
        }
        public async Task<ActionResult> LoginConfirm()
        {
            try
            {
                string Username = Request.Form["username"];
                string Password = Request.Form["password"];
                if (Username != "" && Password != "")
                {
                   var user= await UserRep.Instance.login(Username, Password);
                    if (user != null)
                    {
                        Session["user"] = user;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login", "User");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<ActionResult> FollowPage()
        {
            try
            {

            }
            catch (Exception ex)
            {
            }
            return View();
        }
    }
}
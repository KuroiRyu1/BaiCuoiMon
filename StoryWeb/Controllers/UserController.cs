using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using StoryWeb.Models;
using StoryWeb.Models.ModelView;
using StoryWeb.Models.Repositories;

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

        public ActionResult Login()
        {
            if (Session["user"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            User user = await UserRep.Instance.AuthenticateAsync(username, password);
            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }
            Session["user"] = user;

            return RedirectToAction("Index", "Home");

        }


        public ActionResult Register()
        {
            if (Session["user"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
      
        public async Task<ActionResult> Register(string username, string password, string email, string fullname)
        {
            var newUser = new User
            {
                Username = username,
                Password =password,
                Email = email,
                FullName = fullname,
                Active = 1,
                Role = 0,
            };

            var isSuccess = await UserRep.Instance.Register(newUser);
            if (!isSuccess)
            {
                ViewBag.Error = "Đăng ký không thành công. Tên đăng nhập hoặc email có thể đã tồn tại.";
                return View();
            }

            else
            {
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("Login");
            }

         

        }
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
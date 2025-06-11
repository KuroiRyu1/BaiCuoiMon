using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
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
        //public string Code(string email)
        //{
        //    string result = Function.SendMail(email);
        //    return result;
        //}
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
            string hashedPassword = Function.MD5Hash(password);
            var user = await UserRep.Instance.Login(username, hashedPassword);
            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
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
                Password = Function.MD5Hash(password),
                Email = email,
                FullName = fullname,
                Active = 1,
                Role = 1,
            };

            var isSuccess = await UserRep.Instance.Register(newUser);
            if (!isSuccess)
            {
                ViewBag.Error = "Đăng ký không thành công";
                return View();
            }

            // Tự động đăng nhập sau khi đăng ký
            Session["user"] = newUser;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            // Xóa session user
            Session.Remove("user");
            return RedirectToAction("Index", "Home");
        }
    }
}
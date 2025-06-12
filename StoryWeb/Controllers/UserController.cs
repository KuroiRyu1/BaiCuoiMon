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
            if (Request.Cookies["AuthToken"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            string token = await UserRep.Instance.AuthenticateAsync(username, password);

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            var authTokenCookie = new HttpCookie("AuthToken", token)
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(8)
            };
            Response.Cookies.Add(authTokenCookie);


            return RedirectToAction("Index", "Home");
        }


        public ActionResult Register()
        {
            if (Request.Cookies["AuthToken"] != null)
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
                Role = 1,
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

            if (Request.Cookies["AuthToken"] != null)
            {
                var expiredCookie = new HttpCookie("AuthToken", "")
                {
                    Expires = DateTime.Now.AddDays(-1),

                    HttpOnly = true
                };

                Response.Cookies.Add(expiredCookie);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
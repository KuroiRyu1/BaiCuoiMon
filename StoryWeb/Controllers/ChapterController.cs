using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StoryWeb.Controllers
{
    public class ChapterController : Controller
    {
        // GET: Chapter
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read()
        {
            return View();
        }
    }
}
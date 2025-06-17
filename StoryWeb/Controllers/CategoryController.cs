using StoryWeb.Models.ModelView;
using StoryWeb.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StoryWeb.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public async Task<ActionResult> Index()
        {
            var cate = await CategoryRep.Instance.getCates();
            ViewBag.cate = cate;
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public async Task<ActionResult> CreateConfirm(Category item)
        {
            string a = await CategoryRep.Instance.addCates(item);
            if (item != null)
            {
                if (a != "")
                {
                    Console.WriteLine(a);
                    return RedirectToAction("index");
                }
            }
            return RedirectToAction("create");

        }
        public async Task<ActionResult> CategoryList(string name="")
        {

            var cateList = await CategoryRep.Instance.getCates(); ;
            if (!string.IsNullOrEmpty(name))
            {
                cateList = await CategoryRep.Instance.Search(name);

            }
            ViewBag.cate = cateList;
            return View();
        }
    }
}
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
    public class StoryController : Controller
    {
        // GET: Story
        public async Task<ActionResult> Index()
        {
            var stories = await StoryRep.Instance.GetStories(categoryId: 1, page: 1, pageSize: 10);
            ViewBag.stories = stories;
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public async Task<ActionResult> CreateConfirm(Story item)
        {
            int newId = await StoryRep.Instance.AddStory(item);
            if (newId != 0)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
        }
    }
}
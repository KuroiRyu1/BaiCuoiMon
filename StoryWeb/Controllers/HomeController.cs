﻿using StoryWeb.Models.ModelView;
using StoryWeb.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StoryWeb.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            List<Story> story = new List<Story>();
            List<Story> novel = new List<Story>();
            story = await StoryRep.Instance.GetStories(0, 1, 18, 1);
            novel = await StoryRep.Instance.GetStories(0, 1, 18, 2);
            ViewBag.Story = story;
            ViewBag.novel=novel;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
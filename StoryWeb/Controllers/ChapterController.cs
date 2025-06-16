using StoryWeb.Models.Repositories;
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
        public async Task<ActionResult> Read(int id=0,int chapterIndex=0)
        {
            var chapter = await ChapterRep.Instance.Read(id,chapterIndex);
            var Story = await StoryRep.Instance.GetStoryById(id);
            var chapterImage = await ChapterImageRes.Instance.getChapterImage(chapter.Id);
            ViewBag.chapter = chapter;
            ViewBag.story = Story;
            ViewBag.chapterImage = chapterImage;
            return View();
        }
    }
}
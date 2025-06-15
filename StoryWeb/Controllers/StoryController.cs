using StoryWeb.Models.ModelView;
using StoryWeb.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StoryWeb.Controllers
{
    [RoutePrefix("truyen")]
    public class StoryController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        // GET: Story
        public async Task<ActionResult> Index()
        {
            var stories = await StoryRep.Instance.GetStories(categoryId: 1, page: 1, pageSize: 10);
            ViewBag.stories = stories ?? new List<Story>();
            return View();
        }
        [Route("thongtintruyen/{id}")]
        public async Task<ActionResult> StoryInfo(int id=0)
        {
            var story = await StoryRep.Instance.GetStoryById(id);
            var chapterList = await ChapterRep.Instance.getListOfChapter(id);
            ViewBag.story = story;
            ViewBag.chapterList = chapterList;  
            return View();
        }

        // GET: Story/Create
        public ActionResult Create()
        {
            return View(new Story());
        }
        

        // POST: Story/Create
        [HttpPost]
        public async Task<ActionResult> CreateConfirm(Story item)
        {
            if (ModelState.IsValid)
            {
                int newId = await StoryRep.Instance.AddStory(item);
                if (newId != 0)
                {
                    TempData["Success"] = "Thêm truyện thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Lỗi khi thêm truyện.";
                }
            }
            return View("Create", item);
        }

        // GET: Story/Edit/{id}
        public async Task<ActionResult> Edit(int id)
        {
            var story = await StoryRep.Instance.GetStoryById(id);
            if (story == null)
            {
                return HttpNotFound();
            }
            return View(story);
        }

        // POST: Story/Edit
        [HttpPost]
        public async Task<ActionResult> EditConfirm(Story item)
        {
            if (ModelState.IsValid)
            {
                int result = await StoryRep.Instance.UpdateStory(item);
                if (result == 1)
                {
                    TempData["Success"] = "Cập nhật truyện thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Lỗi khi cập nhật truyện.";
                }
            }
            return View("Edit", item);
        }

        // GET: Story/Delete/{id}
        public async Task<ActionResult> Delete(int id)
        {
            int result = await StoryRep.Instance.DeleteStory(id);
            if (result == 1)
            {
                TempData["Success"] = "Xóa truyện thành công!";
            }
            else
            {
                TempData["Error"] = "Lỗi khi xóa truyện.";
            }
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> StoryList(int page=1)
        {
            var storyList = await StoryRep.Instance.GetStories(null,page,6);
            var allstory = await StoryRep.Instance.GetAllStories();
            ViewBag.storyList = storyList;
            ViewBag.allstory = allstory;
            ViewBag.page = page;
            return View();
        }
    }
}
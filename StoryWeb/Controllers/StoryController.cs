using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using StoryWeb.Models;
using StoryWeb.Models.ModelView;
using StoryWeb.Models.Repositories;

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
        public async Task<ActionResult> StoryInfo(int id)
        {
            var story = await StoryRep.Instance.GetStoryById(id);
            var chapterList = await ChapterRep.Instance.getListOfChapter(id);
            bool isFollowing = false;
            User user = (User)Session["user"];
            if (user != null)
            {
                isFollowing = await StoryRep.Instance.CheckIsFollowingAsync(user.Id, id);
            }
            var commentList = await CommentRep.Instance.GetStoryCommentsAsync(id);

            ViewBag.comments = commentList;
            ViewBag.IsFollowing = isFollowing;
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
            var storyList = await StoryRep.Instance.GetStories(null,page,12);
            ViewBag.storyList = storyList;
            ViewBag.page = page;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Follow(int storyId)
        {
            var user = Session["user"] as User;
            if (user == null)
                return Json(new { success = false, message = "Bạn cần đăng nhập." });

            int result = await StoryRep.Instance.FollowStoryAsync(user.Id, storyId);
            if (result == 1)
                return Json(new { success = true, message = "Bạn đã theo dõi truyện." });

            return Json(new { success = false, message = "Không thể theo dõi truyện." });
        }

        [HttpPost]
        public async Task<JsonResult> Unfollow(int storyId)
        {
            var user = Session["user"] as User;
            if (user == null)
                return Json(new { success = false, message = "Bạn cần đăng nhập." });

            int result = await StoryRep.Instance.UnfollowStoryAsync(user.Id, storyId);
            if (result == 1)
                return Json(new { success = true, message = "Bạn đã hủy theo dõi truyện." });

            return Json(new { success = false, message = "Không thể theo dõi truyện." });
        }

        


        [HttpPost]
        public async Task<JsonResult> PostStoryComment(int storyId, string content)
        {
            var user = Session["user"] as User;
            if (user == null)
                return Json(new { success = false, message = "Bạn cần đăng nhập để bình luận." });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Bình luận không được để trống." });

            var comment = new StoryComment
            {
                StoryId = storyId,
                UserId = user.Id,
                Content = content,
                Active = 1
            };

            int result = await CommentRep.Instance.AddStoryCommentAsync(comment);
            if (result == 1)
                return Json(new { success = true, message = "Bình luận đã được gửi." });

            return Json(new { success = false, message = "Không thể gửi bình luận." });
        }

    }
}
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
            var chapterComment = await CommentRep.Instance.GetChapterCommentsAsync(chapter.Id);
            ViewBag.comments = chapterComment;
            ViewBag.chapter = chapter;
            ViewBag.story = Story;
            ViewBag.chapterImage = chapterImage;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> PostChapterComment(int chapterId, string content)
        {
            var user = Session["user"] as User;
            if (user == null)
                return Json(new { success = false, message = "Bạn cần đăng nhập để bình luận." });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Bình luận không được để trống." });

            var comment = new ChapterComment
            {
                ChapterId = chapterId,
                UserId = user.Id,
                Content = content,
                Active = 1
            };

            int result = await CommentRep.Instance.AddChapterCommentAsync(comment);
            if (result == 1)
                return Json(new { success = true, message = "Bình luận đã được gửi." });

            return Json(new { success = false, message = "Không thể gửi bình luận." });
        }
    }
}
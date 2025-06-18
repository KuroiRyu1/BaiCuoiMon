using StoryWeb.Models.ModelView;
using StoryWeb.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StoryWeb.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> UserList()
        {
            var user = await UserRep.Instance.GetUser();
            ViewBag.user = user ?? new List<User>();
            return View();
        }
        public async Task<ActionResult> StoryList()
        {
            try
            {
                var stories = await StoryRep.Instance.GetAllStories();
                if (stories == null || !stories.Any())
                {
                    TempData["Error"] = "Không thể tải danh sách truyện. Vui lòng kiểm tra kết nối API.";
                }
                return View(stories);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách truyện: {ex.Message}";
                return View(new List<Story>());
            }
        }

        [HttpPost]
        public async Task<ActionResult> StoryDelete(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8078/");
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("id", id.ToString()),
                        new KeyValuePair<string, string>("Title", ""), // Gửi các trường để khớp với Story object
                        new KeyValuePair<string, string>("ChapterNumber", "0"),
                        new KeyValuePair<string, string>("Introduction", ""),
                        new KeyValuePair<string, string>("Image", "default.jpg"),
                        new KeyValuePair<string, string>("LikeNumber", "0"),
                        new KeyValuePair<string, string>("FollowNumber", "0"),
                        new KeyValuePair<string, string>("ViewNumber", "0"),
                        new KeyValuePair<string, string>("AuthorId", "0"),
                        new KeyValuePair<string, string>("StatusId", "0"),
                        new KeyValuePair<string, string>("CategoryId", "0"),
                        new KeyValuePair<string, string>("StoryTypeId", "0"),
                        new KeyValuePair<string, string>("AuthorName", ""),
                        new KeyValuePair<string, string>("CategoryName", "")
                    });
                    var response = await client.PostAsync("story/delete", content);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Truyện đã được xóa mềm thành công.";
                    }
                    else
                    {
                        TempData["Error"] = $"Xóa truyện thất bại. Mã lỗi: {response.StatusCode}, Chi tiết: {responseContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa truyện: {ex.Message}";
            }
            return RedirectToAction("StoryList");
        }
    }
}
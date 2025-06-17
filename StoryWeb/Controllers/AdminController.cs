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
            var stories = await StoryRep.Instance.GetAllStories();
            ViewBag.Stories = stories ?? new System.Collections.Generic.List<Story>();
            return View();
        }

        public ActionResult StoryCreate()
        {
            return View(new Story());
        }

        [HttpPost]
        public async Task<ActionResult> StoryCreateConfirm(Story item)
        {
            return View("StoryCreate", item);
        }

        public async Task<ActionResult> StoryEdit(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> StoryEditConfirm(Story item)
        {
            return View("StoryEdit", item);
        }

        public async Task<ActionResult> StoryDelete(int id)
        {
            return RedirectToAction("StoryList");
        }

        public async Task<ActionResult> StoryDetail(int id)
        {
            var story = await StoryRep.Instance.GetStoryById(id);
            if (story == null)
            {
                return HttpNotFound();
            }
            ViewBag.Story = story;
            var chapters = await ChapterRep.Instance.GetChaptersByStoryId(id);
            ViewBag.Chapters = chapters ?? new System.Collections.Generic.List<Chapter>();
            return View();
        }

        public ActionResult ChapterCreate(int storyId)
        {
            return View(new Chapter { StoryId = storyId });
        }

        [HttpPost]
        public async Task<ActionResult> ChapterCreateConfirm(Chapter chapter, HttpPostedFileBase[] images)
        {
            if (ModelState.IsValid)
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:8078/");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(chapter.Title), "title");
                    content.Add(new StringContent(chapter.Content), "content");
                    content.Add(new StringContent(chapter.StoryId.ToString()), "storyId");

                    if (images != null && images.Length > 0)
                    {
                        foreach (var image in images)
                        {
                            if (image != null && image.ContentLength > 0)
                            {
                                var streamContent = new StreamContent(image.InputStream);
                                streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                                {
                                    Name = "images",
                                    FileName = image.FileName
                                };
                                content.Add(streamContent);
                            }
                        }
                    }

                    var response = await client.PostAsync("api/chapters", content);
                    var uploadResponse = await client.PostAsync($"api/chapter-images/upload?ChapterId={chapter.StoryId}", content);
                    if (response.IsSuccessStatusCode && uploadResponse.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Thêm chương thành công!";
                        return RedirectToAction("StoryDetail", new { id = chapter.StoryId });
                    }
                }
            }
            TempData["Error"] = "Thêm chương thất bại!";
            return View("ChapterCreate", chapter);
        }

        public async Task<ActionResult> ChapterEdit(int storyId, int chapterId)
        {
            var chapters = await ChapterRep.Instance.GetChaptersByStoryId(storyId);
            var chapter = chapters?.FirstOrDefault(c => c.Id == chapterId);
            if (chapter == null) return HttpNotFound();

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8078/");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("pwd", "123");
            client.DefaultRequestHeaders.Add("tk", "12345");
            var response = await client.GetAsync($"api/chapter-images/{chapterId}");
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Images = await response.Content.ReadAsAsync<System.Collections.Generic.List<dynamic>>();
            }
            return View(chapter);
        }

        [HttpPost]
        public async Task<ActionResult> ChapterEditConfirm(Chapter chapter, HttpPostedFileBase[] images)
        {
            if (ModelState.IsValid)
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:8078/");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(chapter.Id.ToString()), "id");
                    content.Add(new StringContent(chapter.Title), "title");
                    content.Add(new StringContent(chapter.Content), "content");

                    if (images != null && images.Length > 0)
                    {
                        foreach (var image in images)
                        {
                            if (image != null && image.ContentLength > 0)
                            {
                                var streamContent = new StreamContent(image.InputStream);
                                streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                                {
                                    Name = "images",
                                    FileName = image.FileName
                                };
                                content.Add(streamContent);
                            }
                        }
                    }

                    var response = await client.PutAsync($"api/chapters/{chapter.Id}", content);
                    var uploadResponse = await client.PostAsync($"api/chapter-images/upload?ChapterId={chapter.Id}", content);
                    if (response.IsSuccessStatusCode && uploadResponse.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Cập nhật chương thành công!";
                        return RedirectToAction("StoryDetail", new { id = chapter.StoryId });
                    }
                }
            }
            TempData["Error"] = "Cập nhật chương thất bại!";
            return View("ChapterEdit", chapter);
        }

        public async Task<ActionResult> ChapterDelete(int storyId, int chapterId)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8078/");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("tk", "12345");
            var response = await client.DeleteAsync($"api/chapters/{chapterId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa chương thành công!";
            }
            else
            {
                TempData["Error"] = "Xóa chương thất bại!";
            }
            return RedirectToAction("StoryDetail", new { id = storyId });
        }

        public async Task<ActionResult> DeleteImage(int chapterId, long imageId)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8078/");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("tk", "12345");
            var response = await client.DeleteAsync($"api/chapter-images/{imageId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa ảnh thành công!";
            }
            else
            {
                TempData["Error"] = "Xóa ảnh thất bại!";
            }
            return RedirectToAction("ChapterEdit", new { storyId = chapterId, chapterId = chapterId });
        }

        public async Task<ActionResult> UpdateImage(int chapterId, long imageId, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:8078/");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("tk", "12345");

                using (var content = new MultipartFormDataContent())
                {
                    var streamContent = new StreamContent(image.InputStream);
                    streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "image",
                        FileName = image.FileName
                    };
                    content.Add(streamContent);

                    var response = await client.PutAsync($"api/chapter-images/{imageId}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Cập nhật ảnh thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Cập nhật ảnh thất bại!";
                    }
                }
            }
            return RedirectToAction("ChapterEdit", new { storyId = chapterId, chapterId = chapterId });
        }

        public async Task<ActionResult> GetChapterImages(int chapterId)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8078/");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("pwd", "123");
            client.DefaultRequestHeaders.Add("tk", "12345");
            var response = await client.GetAsync($"api/chapter-images/{chapterId}");
            if (response.IsSuccessStatusCode)
            {
                return Json(await response.Content.ReadAsAsync<System.Collections.Generic.List<dynamic>>(), JsonRequestBehavior.AllowGet);
            }
            return Json(new System.Collections.Generic.List<dynamic>(), JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> CategoryList(string name="")
        {
            var cateList = new List<Category>();
            if (string.IsNullOrEmpty(name))
            {
               cateList = await CategoryRep.Instance.getCates();
            }
            else
            {
                cateList =await CategoryRep.Instance.Search(name);
            }
            
            ViewBag.cateList = cateList;
            return View();
        }
    }
}
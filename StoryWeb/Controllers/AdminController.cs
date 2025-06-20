using StoryWeb.Models;
using StoryWeb.Models.ModelView;
using StoryWeb.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
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
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8078/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    // Lấy thông tin truyện
                    var storyResponse = await client.GetAsync($"story/get/{id}");
                    if (!storyResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Không thể tải thông tin truyện. Mã lỗi: {storyResponse.StatusCode}";
                        return RedirectToAction("StoryList");
                    }
                    var story = await storyResponse.Content.ReadAsAsync<Story>();
                    if (story == null)
                    {
                        TempData["Error"] = "Không tìm thấy thông tin truyện.";
                        return RedirectToAction("StoryList");
                    }

                    // Lấy danh sách chapter
                    var chapterResponse = await client.GetAsync($"api/chapters/{id}");
                    List<Chapter> chapters = new List<Chapter>();
                    if (chapterResponse.IsSuccessStatusCode)
                    {
                        chapters = await chapterResponse.Content.ReadAsAsync<List<Chapter>>();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Chapter API failed: {chapterResponse.StatusCode} - {await chapterResponse.Content.ReadAsStringAsync()}");
                    }

                    ViewBag.Story = story;
                    ViewBag.Chapters = chapters;
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin truyện: {ex.Message}";
                return RedirectToAction("StoryList");
            }
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


        public async Task<ActionResult> CategoryList(string name = "")
        {
            var cateList = new List<Category>();
            if (string.IsNullOrEmpty(name))
            {
                cateList = await CategoryRep.Instance.getCates();
            }
            else
            {
                cateList = await CategoryRep.Instance.Search(name);
            }

            ViewBag.cateList = cateList;
            return View();
        }
        public ActionResult CateCreate()
        {
            return View();
        }
        public async Task<ActionResult> cateCreateConfirm(Category cate)
        {
            if (cate != null)
            {
                await CategoryRep.Instance.addCates(cate);
            }
            return RedirectToAction("CategoryList", "Admin");
        }
        public async Task<ActionResult> CateEdit(int id)
        {
            var item = await CategoryRep.Instance.getById(id);
            ViewBag.cate = item;
            return View();

        }
        public async Task<ActionResult> CateEditConfirm(Category cate)
        {
            if (cate != null)
            {
                int result = await CategoryRep.Instance.Edit(cate);
                if (result == 1)
                {
                    TempData["Success"] = "Thay đổi thành công";
                }
                else
                {
                    TempData["Error"] = "Thay đổi thất bại";
                }
            }
            return RedirectToAction("CategoryList", "Admin");
        }

        public async Task<ActionResult> AddStory()
        {
            var cate = await CategoryRep.Instance.getCates();
            var status = await StatusRep.Instance.getAllStatus();
            ViewBag.cate = cate;
            ViewBag.status = status;
            return View();
        }
        public async Task<ActionResult> AddStoryConfirm(HttpPostedFileBase Img, Story story)
        {
            if (story != null)
            {
                string name = Function.ConvertToUnsign(story.Title);
                try
                {
                    if (Img != null)
                    {
                        string newFileName = $"{name.Trim()}{Img.FileName}";
                        string fullPathSave = $"{Server.MapPath(Url.Content($"~/content/Image/{name.Trim()}"))}\\{newFileName}";
                        string createFolder = Server.MapPath(Url.Content($"~/content/Image/{name.Trim()}"));
                        if (!Directory.Exists(createFolder))
                        {
                            Directory.CreateDirectory(createFolder);
                        }
                        Img.SaveAs(fullPathSave);
                        story.Image = newFileName;
                        var result = await StoryRep.Instance.AddStory(story);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return RedirectToAction("StoryList", "Admin");
        }
        public async Task<ActionResult> ChangeUserRole(int id=0)
        {
            try
            {
                var userGet = new User();
                if (id != 0)
                {
                    userGet = await UserRep.Instance.GetUserById(id);
                }
                if (userGet != null)
                {
                    ViewBag.user = userGet;
                }
            }
            catch (Exception ex)
            {
            }
            return View();
        }
        public async Task<ActionResult> ChangeUserRoleConfirm(User user)
        {
            try
            {
                if(user != null)
                {
                   int result = await UserRep.Instance.ChangeUserRole(user);
                }
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("UserList", "Admin");
        }
        public async Task<ActionResult> BanOrUnbanUser(User user)
        {
            try
            {
                if (user != null)
                {
                    if (user.Active == 0)
                    {
                        user.Active = 1;
                    }
                    else
                    {
                        user.Active = 0;
                    }
                    int result = await UserRep.Instance.BanOrUnBan(user);
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("UserList", "Admin");
        }
        public async Task<ActionResult> CategoryDelete(int id)
        {
            Category cate = await CategoryRep.Instance.getById(id);
            if (cate != null)
            {
                int a = await CategoryRep.Instance.Delete(cate);
            }
            return RedirectToAction("CategoryList","Admin");
        }
    }
}
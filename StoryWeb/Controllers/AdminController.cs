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
                // Lấy danh sách truyện
                var stories = await StoryRep.Instance.GetAllStories();
                if (stories == null || !stories.Any())
                {
                    TempData["Error"] = "Không thể tải danh sách truyện. Vui lòng kiểm tra kết nối API.";
                }

                // Lấy danh sách trạng thái
                var statuses = await StatusRep.Instance.getAllStatus();
                ViewBag.Statuses = statuses.ToDictionary(s => s.Id, s => s.Title);

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
            if (id <= 0)
            {
                TempData["Error"] = "ID truyện không hợp lệ.";
                return RedirectToAction("StoryList");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8078/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("username", "admin");
                    client.DefaultRequestHeaders.Add("tk", "12345");

                    // Lấy thông tin truyện để lấy Title
                    var storyResponse = await client.GetAsync($"story/get/{id}");
                    if (!storyResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Không thể tải thông tin truyện: {await storyResponse.Content.ReadAsStringAsync()}";
                        return RedirectToAction("StoryList");
                    }
                    var story = await storyResponse.Content.ReadAsAsync<Story>();
                    if (story == null)
                    {
                        TempData["Error"] = "Không tìm thấy truyện.";
                        return RedirectToAction("StoryList");
                    }

                    // Xóa thư mục ảnh truyện
                    string storyFolder = Function.ConvertToUnsign(story.Title).Trim().Replace(" ", "");
                    string storyPath = Server.MapPath($"~/Content/Image/{storyFolder}");
                    if (Directory.Exists(storyPath))
                    {
                        Directory.Delete(storyPath, true);
                    }

                    // Xóa truyện qua API
                    var deleteData = new { Id = id };
                    var deleteContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(deleteData), System.Text.Encoding.UTF8, "application/json");
                    var deleteResponse = await client.PostAsync("story/delete", deleteContent);
                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Xóa truyện thất bại: {await deleteResponse.Content.ReadAsStringAsync()}";
                        return RedirectToAction("StoryList");
                    }

                    TempData["Success"] = "Xóa truyện thành công!";
                    return RedirectToAction("StoryList");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa truyện: {ex.Message}";
                return RedirectToAction("StoryList");
            }
        }

        public async Task<ActionResult> StoryDetail(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8078/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("username", "admin");
                    client.DefaultRequestHeaders.Add("tk", "12345");

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
            ViewBag.StoryId = storyId;
            return View(new Chapter());
        }

        [HttpPost]
        public async Task<ActionResult> ChapterCreateConfirm(int storyId, Chapter chapter, string ChapterType, HttpPostedFileBase[] Images)
        {
            if (ModelState.IsValid && chapter != null)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:8078/");
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("username", "admin");
                        client.DefaultRequestHeaders.Add("tk", "12345");

                        // Lấy danh sách chương để kiểm tra trùng tên và tính C_chapter_number
                        var chaptersResponse = await client.GetAsync($"api/chapters/{storyId}");
                        List<Chapter> chapters = new List<Chapter>();
                        if (chaptersResponse.IsSuccessStatusCode)
                        {
                            chapters = await chaptersResponse.Content.ReadAsAsync<List<Chapter>>();
                            if (chapters.Any(c => c.Title.ToLower() == chapter.Title.ToLower()))
                            {
                                TempData["Error"] = "Tên chương đã tồn tại.";
                                return RedirectToAction("StoryDetail", new { id = storyId });
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Lỗi khi kiểm tra tên chương. Vui lòng thử lại.";
                            return RedirectToAction("StoryDetail", new { id = storyId });
                        }

                        // Lấy thông tin truyện để tạo thư mục
                        var storyResponse = await client.GetAsync($"story/get/{storyId}");
                        if (!storyResponse.IsSuccessStatusCode)
                        {
                            TempData["Error"] = "Không tìm thấy truyện.";
                            return RedirectToAction("StoryDetail", new { id = storyId });
                        }
                        var story = await storyResponse.Content.ReadAsAsync<Story>();
                        string storyFolder = Function.ConvertToUnsign(story.Title).Trim().Replace(" ", "");
                        string chapterFolder = chapter.ChapterIndex.ToString();
                        string chapterPath = Server.MapPath($"~/Content/Image/{storyFolder}/{chapterFolder}");

                        // Tạo chương mới qua API
                        var chapterData = new
                        {
                            C_title = chapter.Title,
                            C_content = ChapterType == "text" ? chapter.Content : null,
                            C_story_id = storyId,
                            C_chapter_index = chapter.ChapterIndex,
                            C_active = 1,
                            C_day_create = DateTime.Now
                        };
                        var chapterContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(chapterData), System.Text.Encoding.UTF8, "application/json");
                        var chapterResponse = await client.PostAsync("api/chapters", chapterContent);
                        if (!chapterResponse.IsSuccessStatusCode)
                        {
                            TempData["Error"] = $"Lỗi khi tạo chương: {await chapterResponse.Content.ReadAsStringAsync()}";
                            return RedirectToAction("StoryDetail", new { id = storyId });
                        }
                        var newChapterId = await chapterResponse.Content.ReadAsAsync<int>();

                        // Nếu là truyện ảnh, lưu ảnh trực tiếp và gửi danh sách đường dẫn qua API
                        if (ChapterType == "image" && Images != null && Images.Any(i => i != null))
                        {
                            Directory.CreateDirectory(chapterPath);
                            var imageList = new List<object>();
                            int index = 1;
                            foreach (var image in Images.Where(i => i != null))
                            {
                                if (image.ContentLength > 0)
                                {
                                    var fileExtension = Path.GetExtension(image.FileName);
                                    var fileName = $"{index}{fileExtension}";
                                    var fullPath = Path.Combine(chapterPath, fileName);
                                    image.SaveAs(fullPath);

                                    // Thêm vào danh sách ảnh để gửi qua API
                                    var imagePath = $"images/chapters/{newChapterId}/{fileName}";
                                    if (imagePath.Length > 50)
                                    {
                                        TempData["Error"] = $"Đường dẫn ảnh {imagePath} vượt quá 50 ký tự.";
                                        return RedirectToAction("StoryDetail", new { id = storyId });
                                    }
                                    imageList.Add(new
                                    {
                                        C_image = imagePath,
                                        C_index = index,
                                        C_chapter_id = newChapterId
                                    });
                                    index++;
                                }
                            }

                            // Gửi danh sách ảnh qua API
                            if (imageList.Any())
                            {
                                var imageContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(imageList), System.Text.Encoding.UTF8, "application/json");
                                var imageResponse = await client.PostAsync("api/chapter-images/add-multiple", imageContent);
                                if (!imageResponse.IsSuccessStatusCode)
                                {
                                    TempData["Error"] = $"Lỗi khi thêm ảnh chương: {await imageResponse.Content.ReadAsStringAsync()}";
                                    return RedirectToAction("StoryDetail", new { id = storyId });
                                }
                            }
                        }

                        // Cập nhật số chương trong bảng tbl_story
                        var storyUpdate = new { Id = storyId, C_chapter_number = chapters.Any() ? chapters.Max(c => c.ChapterIndex) + 1 : 1 };
                        var storyContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(storyUpdate), System.Text.Encoding.UTF8, "application/json");
                        var storyResponseUpdate = await client.PutAsync("story/put", storyContent);
                        if (!storyResponseUpdate.IsSuccessStatusCode)
                        {
                            TempData["Error"] = $"Lỗi khi cập nhật số chương: {await storyResponseUpdate.Content.ReadAsStringAsync()}";
                            return RedirectToAction("StoryDetail", new { id = storyId });
                        }

                        TempData["Success"] = "Thêm chương thành công!";
                        return RedirectToAction("StoryDetail", new { id = storyId });
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi thêm chương: {ex.Message}";
                    return RedirectToAction("StoryDetail", new { id = storyId });
                }
            }
            TempData["Error"] = "Dữ liệu không hợp lệ.";
            return RedirectToAction("StoryDetail", new { id = storyId });
        }

        public async Task<ActionResult> ChapterDelete(int storyId, int chapterId)
        {
            if (storyId <= 0 || chapterId <= 0)
            {
                TempData["Error"] = "ID truyện hoặc chương không hợp lệ.";
                return RedirectToAction("StoryDetail", new { id = storyId });
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8078/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("username", "admin");
                    client.DefaultRequestHeaders.Add("tk", "12345");

                    // Lấy thông tin truyện để tạo thư mục
                    var storyResponse = await client.GetAsync($"story/get/{storyId}");
                    if (!storyResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Không thể tải thông tin truyện: {await storyResponse.Content.ReadAsStringAsync()}";
                        return RedirectToAction("StoryDetail", new { id = storyId });
                    }
                    var story = await storyResponse.Content.ReadAsAsync<Story>();
                    if (story == null)
                    {
                        TempData["Error"] = "Không tìm thấy truyện.";
                        return RedirectToAction("StoryDetail", new { id = storyId });
                    }

                    // Lấy danh sách chương để lấy ChapterIndex
                    var chaptersResponse = await client.GetAsync($"api/chapters/{storyId}");
                    if (!chaptersResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Không thể tải danh sách chương: {await chaptersResponse.Content.ReadAsStringAsync()}";
                        return RedirectToAction("StoryDetail", new { id = storyId });
                    }
                    var chapters = await chaptersResponse.Content.ReadAsAsync<List<Chapter>>();
                    var chapter = chapters.FirstOrDefault(c => c.Id == chapterId);
                    if (chapter == null)
                    {
                        TempData["Error"] = "Không tìm thấy chương.";
                        return RedirectToAction("StoryDetail", new { id = storyId });
                    }

                    // Xóa thư mục ảnh chương
                    string storyFolder = Function.ConvertToUnsign(story.Title).Trim().Replace(" ", "");
                    string chapterFolder = chapter.ChapterIndex.ToString();
                    string chapterPath = Server.MapPath($"~/Content/Image/{storyFolder}/{chapterFolder}");
                    if (Directory.Exists(chapterPath))
                    {
                        Directory.Delete(chapterPath, true);
                    }

                    // Xóa chương qua API
                    var deleteResponse = await client.DeleteAsync($"api/chapters/{chapterId}");
                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Xóa chương thất bại: {await deleteResponse.Content.ReadAsStringAsync()}";
                        return RedirectToAction("StoryDetail", new { id = storyId });
                    }

                    // Cập nhật số chương trong bảng tbl_story
                    var remainingChapters = chapters.Where(c => c.Id != chapterId).ToList();
                    var storyUpdate = new { Id = storyId, C_chapter_number = remainingChapters.Any() ? remainingChapters.Max(c => c.ChapterIndex) : 0 };
                    var storyContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(storyUpdate), System.Text.Encoding.UTF8, "application/json");
                    var storyResponseUpdate = await client.PutAsync("story/put", storyContent);
                    if (!storyResponseUpdate.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Lỗi khi cập nhật số chương: {await storyResponseUpdate.Content.ReadAsStringAsync()}";
                        return RedirectToAction("StoryDetail", new { id = storyId });
                    }

                    TempData["Success"] = "Xóa chương thành công!";
                    return RedirectToAction("StoryDetail", new { id = storyId });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa chương: {ex.Message}";
                return RedirectToAction("StoryDetail", new { id = storyId });
            }
        }

        public async Task<ActionResult> DeleteImage(int chapterId, long imageId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8078/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("username", "admin");
                    client.DefaultRequestHeaders.Add("tk", "12345");
                    var response = await client.DeleteAsync($"api/chapter-images/{imageId}");
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Xóa ảnh thành công!";
                    }
                    else
                    {
                        TempData["Error"] = $"Xóa ảnh thất bại: {await response.Content.ReadAsStringAsync()}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa ảnh: {ex.Message}";
            }
            return RedirectToAction("ChapterEdit", new { storyId = chapterId, chapterId = chapterId });
        }

        public async Task<ActionResult> UpdateImage(int chapterId, long imageId, HttpPostedFileBase image)
        {
            try
            {
                if (image != null && image.ContentLength > 0)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:8078/");
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
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
                                TempData["Error"] = $"Cập nhật ảnh thất bại: {await response.Content.ReadAsStringAsync()}";
                            }
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Không có ảnh được chọn.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật ảnh: {ex.Message}";
            }
            return RedirectToAction("ChapterEdit", new { storyId = chapterId, chapterId = chapterId });
        }

        public async Task<ActionResult> CategoryList(string name = "")
        {
            var cateList = new List<Category>();
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    cateList = await CategoryRep.Instance.getCatesAdmin();
                }
                else
                {
                    cateList = await CategoryRep.Instance.Search(name);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách danh mục: {ex.Message}";
            }
            ViewBag.cateList = cateList;
            return View();
        }

        public ActionResult CateCreate()
        {
            return View();
        }

        public async Task<ActionResult> CateCreateConfirm(Category cate)
        {
            try
            {
                if (cate != null)
                {
                    await CategoryRep.Instance.addCates(cate);
                    TempData["Success"] = "Thêm danh mục thành công!";
                }
                else
                {
                    TempData["Error"] = "Dữ liệu danh mục không hợp lệ.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi thêm danh mục: {ex.Message}";
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
            try
            {
                if (cate != null)
                {
                    int result = await CategoryRep.Instance.Edit(cate);
                    if (result == 1)
                    {
                        TempData["Success"] = "Thay đổi danh mục thành công";
                    }
                    else
                    {
                        TempData["Error"] = "Thay đổi danh mục thất bại";
                    }
                }
                else
                {
                    TempData["Error"] = "Dữ liệu danh mục không hợp lệ.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi chỉnh sửa danh mục: {ex.Message}";
            }
            return RedirectToAction("CategoryList", "Admin");
        }

        public async Task<ActionResult> AddStory()
        {
            try
            {
                var cate = await CategoryRep.Instance.getCates();
                var status = await StatusRep.Instance.getAllStatus();
                var authors = await AuthorRep.Instance.GetAuthors();
                var storyTypes = await StoryTypeRep.Instance.GetStoryTypes();
                ViewBag.cate = cate;
                ViewBag.status = status;
                ViewBag.authors = authors;
                ViewBag.storyTypes = storyTypes;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddStoryConfirm(HttpPostedFileBase Img, Story story)
        {
            try
            {
                // Kiểm tra các trường bắt buộc
                if (story == null ||
                    string.IsNullOrEmpty(story.Title) ||
                    story.AuthorId <= 0 ||
                    story.CategoryId <= 0 ||
                    story.StatusId <= 0 ||
                    story.StoryTypeId <= 0 ||
                    Img == null || Img.ContentLength <= 0)
                {
                    TempData["Error"] = "Vui lòng điền đầy đủ các trường bắt buộc (Tiêu đề, Tác giả, Thể loại, Trạng thái, Loại truyện, Ảnh bìa).";
                    System.Diagnostics.Debug.WriteLine($"Validation failed: Title={story?.Title}, AuthorId={story?.AuthorId}, CategoryId={story?.CategoryId}, StatusId={story?.StatusId}, StoryTypeId={story?.StoryTypeId}, Img={(Img != null ? Img.FileName : "null")}");
                    return RedirectToAction("AddStory");
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8078/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("username", "admin");
                    client.DefaultRequestHeaders.Add("tk", "12345");

                    // Lưu ảnh bìa
                    string storyFolder = Function.ConvertToUnsign(story.Title).Trim().Replace(" ", "");
                    string fileExtension = Path.GetExtension(Img.FileName);
                    string fileName = $"cover{fileExtension}";
                    string createFolder = Server.MapPath($"~/Content/Image/{storyFolder}");
                    string fullPathSave = Path.Combine(createFolder, fileName);
                    if (!Directory.Exists(createFolder))
                    {
                        Directory.CreateDirectory(createFolder);
                    }
                    Img.SaveAs(fullPathSave);
                    story.Image = $"Content/Image/{storyFolder}/{fileName}";

                    // Chuẩn bị dữ liệu truyện để gửi qua API
                    var storyData = new
                    {
                        Title = story.Title,
                        AuthorId = story.AuthorId,
                        CategoryId = story.CategoryId,
                        StatusId = story.StatusId,
                        StoryTypeId = story.StoryTypeId,
                        Introduction = story.Introduction,
                        Image = story.Image,
                        Active = 1
                    };

                    System.Diagnostics.Debug.WriteLine($"Sending story data to API: {Newtonsoft.Json.JsonConvert.SerializeObject(storyData)}");

                    // Gửi request thêm truyện qua API
                    var storyContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(storyData), System.Text.Encoding.UTF8, "application/json");
                    var storyResponse = await client.PostAsync("story/post", storyContent);
                    if (!storyResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await storyResponse.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Lỗi khi thêm truyện: {errorContent}";
                        System.Diagnostics.Debug.WriteLine($"API error: {errorContent}");
                        return RedirectToAction("AddStory");
                    }

                    TempData["Success"] = "Thêm truyện thành công!";
                    return RedirectToAction("StoryList");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi thêm truyện: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Exception in AddStoryConfirm: {ex.Message}");
            }
            return RedirectToAction("AddStory");
        }

        public async Task<ActionResult> ChangeUserRole(int id = 0)
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
                else
                {
                    TempData["Error"] = "Không tìm thấy người dùng.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin người dùng: {ex.Message}";
            }
            return View();
        }

        public async Task<ActionResult> ChangeUserRoleConfirm(User user)
        {
            try
            {
                if (user != null)
                {
                    int result = await UserRep.Instance.ChangeUserRole(user);
                    if (result == 1)
                    {
                        TempData["Success"] = "Thay đổi vai trò thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Thay đổi vai trò thất bại.";
                    }
                }
                else
                {
                    TempData["Error"] = "Dữ liệu người dùng không hợp lệ.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi thay đổi vai trò: {ex.Message}";
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
                    if (result == 1)
                    {
                        TempData["Success"] = "Thay đổi trạng thái người dùng thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Thay đổi trạng thái người dùng thất bại.";
                    }
                }
                else
                {
                    TempData["Error"] = "Dữ liệu người dùng không hợp lệ.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi thay đổi trạng thái người dùng: {ex.Message}";
            }
            return RedirectToAction("UserList", "Admin");
        }

        public async Task<ActionResult> CategoryDelete(int id)
        {
            try
            {
                Category cate = await CategoryRep.Instance.getById(id);
                if (cate != null)
                {
                    int result = await CategoryRep.Instance.Delete(cate);
                    if (result == 1)
                    {
                        TempData["Success"] = "Xóa danh mục thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Xóa danh mục thất bại.";
                    }
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy danh mục.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa danh mục: {ex.Message}";
            }
            return RedirectToAction("CategoryList", "Admin");
        }
    }
}
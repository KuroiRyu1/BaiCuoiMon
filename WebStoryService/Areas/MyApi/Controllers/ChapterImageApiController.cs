using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebStoryService.Models.Entities;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("api/chapter-images")]
    public class ChapterImageApiController : ApiController
    {
        private readonly DbEntities _db = new DbEntities();
        private const string UploadPath = "~/Images/Chapters/";

        [HttpGet]
        [Route("{chapterId}")]
        public IHttpActionResult GetByChapter(int chapterId)
        {
            try
            {
                if (!_db.tbl_chapter.Any(c => c.C_id == chapterId))
                    return NotFound();

                var images = _db.tbl_chapter_image
                    .Where(i => i.C_chapter_id == chapterId)
                    .OrderBy(i => i.C_index)
                    .Select(i => new
                    {
                        i.C_id,
                        i.C_image,
                        i.C_index,
                        FullPath = VirtualPathUtility.ToAbsolute(string.IsNullOrEmpty(i.C_image) ? "Content/images/default.jpg" : i.C_image)
                    })
                    .ToList();

                return Ok(images);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddImage([FromBody] tbl_chapter_image image)
        {
            try
            {
                if (image == null || string.IsNullOrEmpty(image.C_image) || image.C_chapter_id == 0)
                    return BadRequest("Dữ liệu ảnh không hợp lệ.");

                if (image.C_image.Length > 50)
                    return BadRequest($"Đường dẫn ảnh vượt quá 50 ký tự: {image.C_image}");

                if (!_db.tbl_chapter.Any(c => c.C_id == image.C_chapter_id))
                    return NotFound();

                // Kiểm tra unique constraint
                if (_db.tbl_chapter_image.Any(i => i.C_chapter_id == image.C_chapter_id && i.C_index == image.C_index))
                    return BadRequest($"Chỉ số ảnh {image.C_index} đã tồn tại cho chương {image.C_chapter_id}.");

                _db.tbl_chapter_image.Add(image);
                await _db.SaveChangesAsync();

                return Ok(new { Message = "Thêm ảnh thành công", ImageId = image.C_id });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                return InternalServerError(new Exception($"Validation failed: {string.Join("; ", errors)}"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("add-multiple")]
        public async Task<IHttpActionResult> AddMultipleImages([FromBody] List<tbl_chapter_image> images)
        {
            try
            {
                if (images == null || !images.Any())
                    return BadRequest("Danh sách ảnh rỗng.");

                if (images.Any(i => string.IsNullOrEmpty(i.C_image) || !i.C_chapter_id.HasValue))
                    return BadRequest("Dữ liệu ảnh không hợp lệ: Một số ảnh thiếu đường dẫn hoặc chapter_id.");

                if (images.Any(i => i.C_image.Length > 50))
                    return BadRequest($"Một hoặc nhiều đường dẫn ảnh vượt quá 50 ký tự: {string.Join(", ", images.Where(i => i.C_image.Length > 50).Select(i => i.C_image))}");

                int chapterId = images.First().C_chapter_id.Value;
                if (!_db.tbl_chapter.Any(c => c.C_id == chapterId))
                    return NotFound();

                // Kiểm tra unique constraint
                foreach (var image in images)
                {
                    if (_db.tbl_chapter_image.Any(i => i.C_chapter_id == image.C_chapter_id && i.C_index == image.C_index))
                        return BadRequest($"Chỉ số ảnh {image.C_index} đã tồn tại cho chương {image.C_chapter_id}.");
                }

                _db.tbl_chapter_image.AddRange(images);
                await _db.SaveChangesAsync();

                return Ok(new { Message = "Thêm danh sách ảnh thành công", ImageCount = images.Count });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                return InternalServerError(new Exception($"Validation failed: {string.Join("; ", errors)}"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IHttpActionResult> UploadImages()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Yêu cầu không hỗ trợ upload file.");

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                var chapterId = int.Parse(Request.Headers.GetValues("ChapterId").FirstOrDefault() ?? "0");
                if (!_db.tbl_chapter.Any(c => c.C_id == chapterId))
                    return NotFound();

                int? lastIndex = _db.tbl_chapter_image
                    .Where(i => i.C_chapter_id == chapterId)
                    .Max(i => (int?)i.C_index);
                int nextIndex = lastIndex.HasValue ? lastIndex.Value + 1 : 1;

                var uploadFolder = HttpContext.Current.Server.MapPath(UploadPath);
                Directory.CreateDirectory(uploadFolder);

                foreach (var file in provider.Contents)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('\"'));
                    var localFilePath = Path.Combine(uploadFolder, fileName);
                    var stream = await file.ReadAsStreamAsync();
                    using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    var newImage = new tbl_chapter_image
                    {
                        C_image = UploadPath + fileName,
                        C_index = nextIndex++,
                        C_chapter_id = chapterId
                    };
                    _db.tbl_chapter_image.Add(newImage);
                }

                await _db.SaveChangesAsync();
                return Ok(new { Message = "Upload ảnh thành công", LastIndex = nextIndex - 1 });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                return InternalServerError(new Exception($"Validation failed: {string.Join("; ", errors)}"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{imageId}")]
        public IHttpActionResult DeleteImage(long imageId)
        {
            try
            {
                var image = _db.tbl_chapter_image.Find(imageId);
                if (image == null) return NotFound();

                int? chapterId = image.C_chapter_id;
                int? deletedIndex = image.C_index;
                if (!chapterId.HasValue || !deletedIndex.HasValue)
                    return BadRequest("Dữ liệu ảnh không hợp lệ");

                _db.tbl_chapter_image.Remove(image);
                _db.SaveChanges();

                var remainingImages = _db.tbl_chapter_image
                    .Where(i => i.C_chapter_id == chapterId.Value && i.C_index > deletedIndex.Value);
                foreach (var img in remainingImages)
                {
                    img.C_index--;
                }
                _db.SaveChanges();

                return Ok(new { Message = "Đã xóa ảnh thành công" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{imageId}")]
        public async Task<IHttpActionResult> UpdateImage(long imageId)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Yêu cầu không hỗ trợ upload file.");

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                var image = _db.tbl_chapter_image.Find(imageId);
                if (image == null) return NotFound();

                var file = provider.Contents[0];
                var uploadFolder = HttpContext.Current.Server.MapPath(UploadPath);
                Directory.CreateDirectory(uploadFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('\"'));
                var localFilePath = Path.Combine(uploadFolder, fileName);
                var stream = await file.ReadAsStreamAsync();
                using (var fileStream = new FileStream(localFilePath, FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream);
                }

                image.C_image = UploadPath + fileName;
                _db.SaveChanges();

                return Ok(new { Message = "Đã cập nhật ảnh thành công", ImagePath = image.C_image });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("update-paths")]
        public async Task<IHttpActionResult> UpdateImagePaths([FromBody] List<tbl_chapter_image> images)
        {
            try
            {
                if (images == null || !images.Any())
                    return BadRequest("Danh sách ảnh rỗng.");

                if (images.Any(i => i.C_id <= 0 || string.IsNullOrEmpty(i.C_image)))
                    return BadRequest("Dữ liệu ảnh không hợp lệ: Một số ảnh thiếu ID hoặc đường dẫn.");

                foreach (var image in images)
                {
                    var existingImage = _db.tbl_chapter_image.Find(image.C_id);
                    if (existingImage == null)
                        return BadRequest($"Không tìm thấy ảnh với ID {image.C_id}.");

                    existingImage.C_image = image.C_image;
                }

                await _db.SaveChangesAsync();
                return Ok(new { Message = "Cập nhật đường dẫn ảnh thành công", ImageCount = images.Count });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                return InternalServerError(new Exception($"Validation failed: {string.Join("; ", errors)}"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
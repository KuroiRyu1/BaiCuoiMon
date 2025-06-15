using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("api/chapter-images")]
    public class ChapterImageApiController : ApiController
    {
        private readonly DbEntities _db = new DbEntities();

        // GET api/chapter-images/{chapterId}
        [HttpGet]
        [Route("{chapterId}")]
        public IHttpActionResult GetByChapter(int chapterId)
        {
            var images = _db.tbl_chapter_image
                          .Where(i => i.C_chapter_id == chapterId)
                          .OrderBy(i => i.C_index)
                          .Select(i => new
                          {
                              i.C_id,
                              i.C_image,
                              i.C_index
                          })
                          .ToList();
            return Ok(images);
        }
        [HttpGet]
        [Route("images/{chapterId}")]
        public List<ChapterImage> GetImage(int chapterId=0)
        {
            var image = new List<ChapterImage>();
            try
            {
                ChapterImageRes res = new ChapterImageRes();
                var item = res.getImage(chapterId);
                if (item != null)
                {
                    image = item;
                }
            }
            catch (Exception ex)
            {
            }
            return image;
        }
        [HttpPost]
        [Route("batch-append")]
        public IHttpActionResult AddImages([FromBody] AppendImagesModel model)
        {
            try
            {
                // Validate
                if (model == null || model.Images == null || !model.Images.Any())
                    return BadRequest("Danh sách ảnh không được trống");

                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        // Bước 1: Kiểm tra chapter tồn tại
                        if (!_db.tbl_chapter.Any(c => c.C_id == model.ChapterId))
                            return NotFound();

                        // Bước 2: Tìm index cuối cùng
                        int lastIndex = _db.tbl_chapter_image
                            .Where(i => i.C_chapter_id == model.ChapterId)
                            .Max(i => (int?)i.C_index) ?? 0;

                        // Bước 3: Thêm từng ảnh mới
                        var addedImages = new List<object>();
                        int currentIndex = lastIndex + 1;

                        foreach (var img in model.Images)
                        {
                            var newImage = new tbl_chapter_image
                            {
                                C_image = img.ImagePath,
                                C_index = currentIndex++,
                                C_chapter_id = model.ChapterId
                            };

                            _db.tbl_chapter_image.Add(newImage);
                            addedImages.Add(new { Id = newImage.C_id, Index = newImage.C_index });
                        }

                        _db.SaveChanges();
                        transaction.Commit();

                        return Ok(new
                        {
                            Message = $"Đã thêm {model.Images.Count} ảnh vào cuối chapter",
                            AddedImages = addedImages,
                            LastIndex = currentIndex - 1
                        });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return InternalServerError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        // POST api/chapter-images
        [HttpPost]
        [Route("")]
        public IHttpActionResult InsertImage([FromBody] dynamic requestData)
        {
            try
            {
                // Validate input
                if (requestData == null)
                    return BadRequest("Dữ liệu không hợp lệ");

                string imagePath = requestData.ImagePath?.ToString();
                if (string.IsNullOrEmpty(imagePath))
                    return BadRequest("Đường dẫn ảnh không được trống");

                if (!int.TryParse(requestData.TargetIndex?.ToString(), out int targetIndex) || targetIndex < 1)
                    return BadRequest("Vị trí phải ≥ 1");

                if (!int.TryParse(requestData.ChapterId?.ToString(), out int chapterId) || chapterId < 1)
                    return BadRequest("Chapter ID không hợp lệ");

                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var indexService = new ChapterImageRes(_db);

                        // Bước 1: Dời index bằng raw SQL
                        indexService.ShiftIndexesForInsert(chapterId, targetIndex);

                        // Bước 2: Thêm ảnh mới
                        var newImage = new tbl_chapter_image
                        {
                            C_image = imagePath,
                            C_index = targetIndex,
                            C_chapter_id = chapterId
                        };

                        _db.tbl_chapter_image.Add(newImage);
                        _db.SaveChanges();
                        transaction.Commit();

                        return Ok(new { Id = newImage.C_id });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return InternalServerError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/chapter-images/{id}
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteImage(long id)
        {
            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        // Bước 1: Lấy thông tin ảnh (chỉ đọc)
                        var image = _db.tbl_chapter_image
                            .AsNoTracking()
                            .FirstOrDefault(i => i.C_id == id);

                        if (image == null)
                            return NotFound();

                        int chapterId = (int)image.C_chapter_id;
                        int deletedIndex = (int)image.C_index;

                        // Bước 2: Xóa ảnh trước
                        int affectedRows = _db.Database.ExecuteSqlCommand(
                            "DELETE FROM tbl_chapter_image WHERE _id = {0}",
                            id
                        );

                        if (affectedRows == 0)
                            return NotFound();

                        // Bước 3: Dời các ảnh phía sau LÊN 1 đơn vị
                        var indexService = new ChapterImageRes(_db);
                        indexService.ShiftIndexesAfterDelete(chapterId, deletedIndex);

                        transaction.Commit();

                        return Ok(new
                        {
                            DeletedId = id,
                            AdjustedImages = _db.tbl_chapter_image
                                .Where(i => i.C_chapter_id == chapterId && i.C_index >= deletedIndex)
                                .Select(i => new { i.C_id, i.C_index })
                                .ToList()
                        });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return InternalServerError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            //    }
            //    [HttpPut]
            //    [Route("reorder")]
            //    public IHttpActionResult ReorderImage([FromBody] dynamic requestData)
            //    {
            //        try
            //        {
            //            // === Kiểm tra dữ liệu ===
            //            if (requestData == null)
            //                return BadRequest("Dữ liệu không hợp lệ");

            //            if (!long.TryParse(requestData.ImageId?.ToString(), out long imageId) || imageId < 1)
            //                return BadRequest("Image ID không hợp lệ");

            //            if (!int.TryParse(requestData.NewIndex?.ToString(), out int newIndex) || newIndex < 1)
            //                return BadRequest("Vị trí mới không hợp lệ");

            //            // === Kiểm tra tồn tại ===
            //            var image = _db.tbl_chapter_image.Find(imageId);
            //            if (image == null)
            //                return NotFound();

            //            int oldIndex = (int)image.C_index;
            //            int chapterId = (int)image.C_chapter_id;

            //            // Kiểm tra chapter
            //            if (!_db.tbl_chapter.Any(c => c.C_id == chapterId))
            //                return NotFound();

            //            // Kiểm tra vị trí mới
            //            var maxIndex = _db.tbl_chapter_image
            //                            .Where(i => i.C_chapter_id == chapterId)
            //                            .Count();

            //            if (newIndex > maxIndex)
            //                return BadRequest($"Vị trí {newIndex} vượt quá số lượng ảnh hiện có ({maxIndex})");

            //            // === Xử lý chính ===
            //            using (var transaction = _db.Database.BeginTransaction())
            //            {
            //                try
            //                {
            //                    if (newIndex > oldIndex)
            //                    {
            //                        // Dời các ảnh giữa oldIndex và newIndex
            //                        var imagesToUpdate = _db.tbl_chapter_image
            //                                            .Where(i => i.C_chapter_id == chapterId
            //                                                   && i.C_index > oldIndex
            //                                                   && i.C_index <= newIndex)
            //                                            .ToList();

            //                        foreach (var img in imagesToUpdate)
            //                        {
            //                            img.C_index -= 1;
            //                        }
            //                    }
            //                    else if (newIndex < oldIndex)
            //                    {
            //                        // Dời các ảnh giữa newIndex và oldIndex
            //                        var imagesToUpdate = _db.tbl_chapter_image
            //                                            .Where(i => i.C_chapter_id == chapterId
            //                                                   && i.C_index >= newIndex
            //                                                   && i.C_index < oldIndex)
            //                                            .ToList();

            //                        foreach (var img in imagesToUpdate)
            //                        {
            //                            img.C_index += 1;
            //                        }
            //                    }

            //                    // Cập nhật vị trí mới
            //                    image.C_index = newIndex;
            //                    _db.SaveChanges();
            //                    transaction.Commit();

            //                    return Ok();
            //                }
            //                catch (Exception ex)
            //                {
            //                    transaction.Rollback();
            //                    return InternalServerError(ex);
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            return InternalServerError(ex);
            //        }
            //    }
        }

        public class AppendImagesModel
        {
            public int ChapterId { get; set; }
            public List<ChapterImage> Images { get; set; }
        }

    }
}

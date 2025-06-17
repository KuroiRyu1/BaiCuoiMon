using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class ChapterImageRes
    {
        private readonly DbEntities _db;

        public ChapterImageRes(DbEntities db)
        {
            _db = db;
        }
        public ChapterImageRes() { }
        public List<ChapterImage> getImage(int chapterId) 
        {
            DbEntities _db = new DbEntities();
            List<ChapterImage> res = new List<ChapterImage>();
            try
            {
                var item = _db.tbl_chapter_image.Where(d=>d.C_chapter_id==chapterId)
                    .Select(d=> new ChapterImage
                    {
                        Id = d.C_id,
                        ChapterId = chapterId,
                        ImagePath = d.C_image,
                    })
                    .ToList();
                if (item != null)
                {
                    res = item;
                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }

        public void ShiftIndexesForInsert(int chapterId, int targetIndex)
        {
            // Lấy danh sách ID và Index cần cập nhật (chỉ đọc)
            var imagesToUpdate = _db.tbl_chapter_image
                .AsNoTracking()
                .Where(i => i.C_chapter_id == chapterId && i.C_index >= targetIndex)
                .OrderByDescending(i => i.C_index)
                .Select(i => new { i.C_id, i.C_index })
                .ToList();

            // Thực hiện update trực tiếp qua SQL
            foreach (var img in imagesToUpdate)
            {
                _db.Database.ExecuteSqlCommand(
                    "UPDATE tbl_chapter_image SET _index = {0} WHERE _id = {1}",
                    img.C_index + 1,
                    img.C_id
                );
            }
        }

        public void ShiftIndexesAfterDelete(int chapterId, int deletedIndex)
        {
            // Dùng ExecuteSqlCommand để tránh tracking
            _db.Database.ExecuteSqlCommand(
                @"UPDATE tbl_chapter_image 
              SET _index = _index - 1 
              WHERE _chapter_id = {0} AND _index > {1}",
                chapterId,
                deletedIndex
            );
        }
    }
}
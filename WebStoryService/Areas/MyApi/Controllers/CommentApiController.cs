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
    [RoutePrefix("api/comment")]
    public class CommentApiController : ApiController
    {
        private readonly DbEntities _db = new DbEntities();

        // Lấy tất cả bình luận của một chương
        [HttpGet]
        [Route("chapter/{chapterId}")]
        public IHttpActionResult GetByChapter(int chapterId)
        {
            var comments = (from c in _db.tbl_chapter_comment
                            join u in _db.tbl_user on c.C_user_id equals u.C_id
                            where c.C_chapter_id == chapterId && c.C_active == 1
                            select new
                            {
                                c.C_id,
                                c.C_content,
                                c.C_user_id,
                                UserFullname = u.C_fullname
                            }).ToList();

            return Ok(comments);
        }

        // Lấy tất cả bình luận của một truyện
        [HttpGet]
        [Route("story/{storyId}")]
        public IHttpActionResult GetByStory(int storyId)
        {
            var comments = (from c in _db.tbl_story_comment
                            join u in _db.tbl_user on c.C_user_id equals u.C_id
                            where c.C_story_id == storyId && c.C_active == 1
                            select new
                            {
                                c.C_id,
                                c.C_content,
                                c.C_user_id,
                                UserFullname = u.C_fullname
                            }).ToList();

            return Ok(comments);
        }

        // Thêm bình luận mới cho chương
        [HttpPost]
        [Route("chapter")]
        public IHttpActionResult CreateChapterComment([FromBody] tbl_chapter_comment comment)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                comment.C_active = 1; // Mặc định bình luận hoạt động
                _db.tbl_chapter_comment.Add(comment);
                _db.SaveChanges();

                return Ok(comment.C_id);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Thêm bình luận mới cho truyện
        [HttpPost]
        [Route("story")]
        public IHttpActionResult CreateStoryComment([FromBody] tbl_story_comment comment)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                comment.C_active = 1;
                _db.tbl_story_comment.Add(comment);
                _db.SaveChanges();

                return Ok(comment.C_id);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Cập nhật bình luận
        [HttpPut]
        [Route("story/{commentId}")]
        public IHttpActionResult UpdateStoryComment(int commentId, [FromBody] tbl_story_comment data)
        {
            try
            {
                var comment = _db.tbl_story_comment.Find(commentId);
                if (comment == null) return NotFound();

                comment.C_content = data.C_content;
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPut]
        [Route("chapter/{commentId}")]
        public IHttpActionResult UpdateChapterComment(int commentId, [FromBody] tbl_chapter_comment data)
        {
            try
            {
                var comment = _db.tbl_chapter_comment.Find(commentId);
                if (comment == null) return NotFound();

                comment.C_content = data.C_content;
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // Xóa hoặc ẩn bình luận
        [HttpDelete]
        [Route("chapter/{commentId}")]
        public IHttpActionResult DeleteChapterComment(int commentId)
        {
            try
            {
                var comment = _db.tbl_chapter_comment.Find(commentId);
                if (comment == null) return NotFound();

                comment.C_active = 0; // Thay vì xoá, có thể chỉ ẩn bình luận
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("story/{commentId}")]
        public IHttpActionResult DeleteStoryComment(int commentId)
        {
            try
            {
                var comment = _db.tbl_story_comment.Find(commentId);
                if (comment == null) return NotFound();

                comment.C_active = 0; // Thay vì xoá, có thể chỉ ẩn bình luận
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

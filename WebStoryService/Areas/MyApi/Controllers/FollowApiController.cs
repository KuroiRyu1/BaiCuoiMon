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
    [RoutePrefix("api/follow")]
    public class FollowApiController : ApiController
    {
        private readonly DbEntities _db = new DbEntities();

        // 1. Follow truyện
        [Route("story")]
        [HttpPost]
        public IHttpActionResult FollowStory([FromBody] StoryFollowModel model)
        {
            if (model == null || model.UserId <= 0 || model.StoryId <= 0)
                return BadRequest("Thiếu dữ liệu theo dõi");

            var existing = _db.tbl_story_follow
                .FirstOrDefault(f => f.C_user_id == model.UserId && f.C_story_id == model.StoryId);

            if (existing != null)
            {
                existing.C_status = 1;
                existing.day_create = DateTime.Now;
            }
            else
            {
                var newFollow = new tbl_story_follow
                {
                    C_user_id = model.UserId,
                    C_story_id = model.StoryId,
                    day_create = DateTime.Now,
                    C_status = 1
                };
                _db.tbl_story_follow.Add(newFollow);
            }

            _db.SaveChanges();
            return Ok(new { Message = "Đã theo dõi truyện thành công." });
        }

        // 2. Unfollow truyện
        [Route("story/unfollow")]
        [HttpPost]
        public IHttpActionResult UnfollowStory([FromBody] StoryFollowModel model)
        {
            var follow = _db.tbl_story_follow
                .FirstOrDefault(f => f.C_user_id == model.UserId && f.C_story_id == model.StoryId);

            if (follow == null)
                return NotFound();

            follow.C_status = 0;
            _db.SaveChanges();

            return Ok(new { Message = "Đã hủy theo dõi truyện." });
        }

        // 3. Xem danh sách truyện người dùng đang follow
        [Route("story/user/{userId}")]
        [HttpGet]
        public IHttpActionResult GetFollowedStories(int userId)
        {
            var stories = (from f in _db.tbl_story_follow
                           join s in _db.tbl_story on f.C_story_id equals s.C_id
                           where f.C_user_id == userId && f.C_status == 1
                           select new
                           {
                               s.C_id,
                               s.C_title,
                               s.C_image,
                               s.C_introduction
                           }).ToList();

            return Ok(stories);
        }
    }

    // Model để nhận dữ liệu follow/unfollow
    public class StoryFollowModel
    {
        public long UserId { get; set; }
        public int StoryId { get; set; }
    }
}

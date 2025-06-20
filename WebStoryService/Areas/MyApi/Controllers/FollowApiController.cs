using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("follow")]
    public class FollowApiController : ApiController
    {
        private readonly DbEntities _db = new DbEntities();
        FollowRep rep = new FollowRep();

        // 1. Follow truyện
        [Route("story")]
        [HttpPost]
        public int FollowStory([FromBody] Follow model)
        {
            if (model == null || model.UserId <= 0 || model.StoryId <= 0)
            {
                //return BadRequest("Thiếu dữ liệu theo dõi");
                
            }
            else
            {
                int result = rep.FollowStory(model);
                if (result != 0) {
                    return result;
                };
            }

            return 0;
        }
        [Route("check")]
        [HttpPost]
        public bool CheckFollow(Follow follow) {
            return rep.CheckFollow(follow.StoryId,follow.UserId);
        }

        // 2. Unfollow truyện
        //[Route("story/unfollow")]
        //[HttpPost]
        //public IHttpActionResult UnfollowStory([FromBody] Follow model)
        //{
        //    var follow = _db.tbl_story_follow
        //        .FirstOrDefault(f => f.C_user_id == model.UserId && f.C_story_id == model.StoryId);

        //    if (follow == null)
        //        return NotFound();

        //    follow.C_status = 0;
        //    _db.SaveChanges();

        //    return Ok(new { Message = "Đã hủy theo dõi truyện." });
        //}

        // 3. Xem danh sách truyện người dùng đang follow
        [Route("story/user/{userId}")]
        [HttpGet]
        public IHttpActionResult GetFollowedStories(int userId=0)
        {
            List<Story> stories = new List<Story>();
            if (userId != 0)
            {
                
                stories = rep.GetFollowedStories(userId);
            }
            return Ok(stories);
        }
    }
}
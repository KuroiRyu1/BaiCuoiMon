using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("story")]
    public class StoryApiController : ApiController
    {
        private readonly StoryRes _storyRes = new StoryRes();

        [Route("get")]
        [HttpGet]
        public IEnumerable<Story> Get(int? categoryId = null, int page = 1, int pageSize = 10)
        {
            try
            {
                return _storyRes.Gets(categoryId, page, pageSize);
            }
            catch (Exception ex)
            {
            }

            return new List<Story>();
        }
        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            try
            {
                var stories = _storyRes.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, stories);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("getall/cate")]
        public List<Story> getAll(int? cateId=null)
        {
            var story = new List<Story>();
            try
            {
                StoryRes storyRes = new StoryRes();
                var item = storyRes.GetAll(cateId);
                if (item != null)
                {
                    story = item;
                }
            }
            catch (Exception ex)
            {
            }
            return story;
        }
        [Route("get/{id}")]
        [HttpGet]
        public HttpResponseMessage GetById(int id)
        {
            try
            {
                var story = _storyRes.GetById(id);
                if (story != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, story);
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Message = "Truyện không tìm thấy." });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }
        [Route("post")]
        [HttpPost]
        public HttpResponseMessage Post(Story story)
        {
            try
            {
                if (story != null)
                {
                    var result = _storyRes.Create(story);
                    if (result != 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Truyện đã được thêm thành công." });
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "Dữ liệu truyện không hợp lệ." });
        }

        [Route("delete")]
        [HttpPost]
        public HttpResponseMessage Delete([FromBody] Story item)
        {
            try
            {
                if (item == null || item.Id == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "Dữ liệu truyện không hợp lệ." });
                }
                System.Diagnostics.Debug.WriteLine($"Received delete request for Story ID: {item.Id}");
                var result = _storyRes.Delete(item.Id);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Truyện đã được xóa mềm thành công." });
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Message = "Truyện không tìm thấy." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Delete: {ex.Message}");
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }
    }
}

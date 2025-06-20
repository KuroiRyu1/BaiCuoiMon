using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
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
        private StoryRes _storyRes = new StoryRes();

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
            var headerData = Request.Headers;
            string username = string.Empty;
            string password = string.Empty;
            string token = string.Empty;

            if (headerData.Contains("username"))
            {
                StoryRes storyRes = new StoryRes();
                var item = storyRes.GetAll(cateId);
                if (item != null)
                {
                    story = item;
                }
            }
            if (headerData.Contains("pwd"))
            {
                password = headerData.GetValues("pwd").First();
            }
            if (headerData.Contains("tk"))
            {
                token = headerData.GetValues("tk").First();
            }

            if (AccountRep.checkToken(username, password, token) == true)
            {
                return _storyRes.Gets(categoryId, page, pageSize);
            }

            return new List<Story>();
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
                username = headerData.GetValues("username").First();
            }
            if (headerData.Contains("pwd"))
            {
                password = headerData.GetValues("pwd").First();
            }
            if (headerData.Contains("tk"))
            {
                token = headerData.GetValues("tk").First();
            }

            if (AccountRep.checkToken(username, password, token) == true)
            {
                var story = _storyRes.GetById(id);
                if (story == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                return Request.CreateResponse(HttpStatusCode.OK, story);
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [Route("post")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Story value)
        {
            if (value == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var headerData = Request.Headers;
            string username = string.Empty;
            string password = string.Empty;
            string token = string.Empty;

            if (headerData.Contains("username"))
            {
                username = headerData.GetValues("username").First();
            }
            if (headerData.Contains("pwd"))
            {
                password = headerData.GetValues("pwd").First();
            }
            if (headerData.Contains("tk"))
            {
                token = headerData.GetValues("tk").First();
            }

            if (AccountRep.checkToken(username, password, token) == true)
            {
                int result = _storyRes.Post(value, username, token);
                if (result == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, value.Id);
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [Route("put/{id}")]
        [HttpPut]
        public HttpResponseMessage Put(int id, [FromBody] Story value)
        {
            if (value == null || id != value.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var headerData = Request.Headers;
            string username = string.Empty;
            string password = string.Empty;
            string token = string.Empty;

            if (headerData.Contains("username"))
            {
                username = headerData.GetValues("username").First();
            }
            if (headerData.Contains("pwd"))
            {
                password = headerData.GetValues("pwd").First();
            }
            if (headerData.Contains("tk"))
            {
                token = headerData.GetValues("tk").First();
            }

            if (AccountRep.checkToken(username, password, token) == true)
            {
                int result = _storyRes.Put(value, username, token);
                if (result == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [Route("delete/{id}")]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            var headerData = Request.Headers;
            string username = string.Empty;
            string password = string.Empty;
            string token = string.Empty;

            if (headerData.Contains("username"))
            {
                username = headerData.GetValues("username").First();
            }
            if (headerData.Contains("pwd"))
            {
                password = headerData.GetValues("pwd").First();
            }
            if (headerData.Contains("tk"))
            {
                token = headerData.GetValues("tk").First();
            }

            if (AccountRep.checkToken(username, password, token) == true)
            {
                int result = _storyRes.Delete(id, username, token);
                if (result == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [Route("increment-view/{id}")]
        [HttpPost]
        public HttpResponseMessage IncrementView(int id)
        {
            var headerData = Request.Headers;
            string username = string.Empty;
            string password = string.Empty;
            string token = string.Empty;

            if (headerData.Contains("username"))
            {
                username = headerData.GetValues("username").First();
            }
            if (headerData.Contains("pwd"))
            {
                password = headerData.GetValues("pwd").First();
            }
            if (headerData.Contains("tk"))
            {
                token = headerData.GetValues("tk").First();
            }

            if (AccountRep.checkToken(username, password, token) == true)
            {
                int result = _storyRes.IncrementView(id);
                if (result == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [Route("search")]
        [HttpGet]
        public IEnumerable<Story> Search(string keyword = "", int? categoryId = null)
        {
            var headerData = Request.Headers;
            string username = string.Empty;
            string password = string.Empty;
            string token = string.Empty;

            if (headerData.Contains("username"))
            {
                username = headerData.GetValues("username").First();
            }
            if (headerData.Contains("pwd"))
            {
                password = headerData.GetValues("pwd").First();
            }
            if (headerData.Contains("tk"))
            {
                token = headerData.GetValues("tk").First();
            }

            if (AccountRep.checkToken(username, password, token) == true)
            {
                return _storyRes.Search(keyword, categoryId);
            }

            return new List<Story>();
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
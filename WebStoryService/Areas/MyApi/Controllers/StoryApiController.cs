using System;
using System.Collections.Generic;
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
        [HttpGet]
        [Route("getall")]
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
                var headerData = Request.Headers;
                string username = headerData.Contains("username") ? headerData.GetValues("username").First() : "";
                string password = headerData.Contains("pwd") ? headerData.GetValues("pwd").First() : "";
                string token = headerData.Contains("tk") ? headerData.GetValues("tk").First() : "";

                if (AccountRep.CheckToken(username, password, token))
                {
                    var story = _storyRes.GetById(id);
                    if (story == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Story not found");
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, story);
                }
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid credentials");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}

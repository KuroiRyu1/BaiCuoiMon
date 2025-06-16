using System;
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
        private readonly StoryRes _storyRes = new StoryRes();

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            try
            {
                var headerData = Request.Headers;
                string username = headerData.Contains("username") ? headerData.GetValues("username").First() : "";
                string password = headerData.Contains("pwd") ? headerData.GetValues("pwd").First() : "";
                string token = headerData.Contains("tk") ? headerData.GetValues("tk").First() : "";

                if (AccountRep.CheckToken(username, password, token))
                {
                    var stories = _storyRes.GetAll();
                    return Request.CreateResponse(HttpStatusCode.OK, stories);
                }
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid credentials");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
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
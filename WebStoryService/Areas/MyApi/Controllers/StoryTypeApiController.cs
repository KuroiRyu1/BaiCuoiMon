using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("api/story-types")]
    public class StoryTypeApiController : ApiController
    {
        [Route("get")]
        [HttpGet]
        public IEnumerable<StoryType> Get()
        {
            try
            {
                StoryTypeRes res = new StoryTypeRes();
                return res.GetStoryTypes();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetStoryTypes: {ex.Message}");
            }
            return new List<StoryType>();
        }
    }
}
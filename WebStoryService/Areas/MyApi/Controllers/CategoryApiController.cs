using System;
using System.Collections.Generic;
using WebStoryService.Models.ModelData;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("category")]
    public class CategoryApiController : ApiController
    {
        [Route("get")]
        [HttpGet]
        public IEnumerable<Models.ModelData.Category> Get()
        {

        }
    }
}

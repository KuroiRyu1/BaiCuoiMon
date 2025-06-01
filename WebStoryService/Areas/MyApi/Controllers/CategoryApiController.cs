using System;
using System.Collections.Generic;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("category")]
    public class CategoryApiController : ApiController
    {
        [Route("get")]
        [HttpGet]
        public IEnumerable<Category> Get()
        {
            var headerData = Request.Headers;
            string username = string.Empty;
            string password = string.Empty;
            string token = string.Empty;
            if (headerData.Contains("username"))
            {
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
                    CategoryRes res = new CategoryRes();
                    return res.Gets();
                }
            }

            return new List<Category>();
        }
        [Route("post")]
        [HttpPost]
        public int Post([FromBody] Category value)
        {
            if (value != null)
            {
                CategoryRes res = new CategoryRes();

                if (res.Post(value) == 1)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}

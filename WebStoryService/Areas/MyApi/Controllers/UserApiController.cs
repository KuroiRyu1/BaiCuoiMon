using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("user")]
    public class UserApiController : ApiController
    {
        [Route("login")]
        [HttpGet]
    }
}

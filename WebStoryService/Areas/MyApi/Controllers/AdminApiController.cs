using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using WebStoryService.Models.Entities;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("admin")]
    public class AdminApiController : ApiController
    {
        private readonly DbEntities _db = new DbEntities(); // Sử dụng DbEntities từ EDMX

        [Route("dashboard")]
        [HttpGet]
        [AdminAuthorize]
        public IHttpActionResult GetAdminDashboard()
        {
            return Ok(new { Message = "Chào mừng Admin!" });
        }

        
    }
}

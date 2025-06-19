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
    [RoutePrefix("status")]
    public class StatusController : ApiController
    {
        [Route("get")]
        public List<Status> GetAll()
        {
            try
            {
                StatusRep rep = new StatusRep();
                var item = rep.GetAllStatus();
                if (item != null)
                {
                    return item;
                }
            }
            catch (Exception ex)
            {
            }
            return new List<Status>();
        }
    }
}

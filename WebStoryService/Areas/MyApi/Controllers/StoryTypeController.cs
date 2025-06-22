using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("storytype")]
    public class StoryTypeController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public List<StoryType> getStoryType()
        {
            try
            {
                StoryTypeRep rep = new StoryTypeRep();
                return rep.get();
            }
            catch (Exception ex)
            {
            }
            return new List<StoryType>();
        }
        [HttpGet]
        [Route("get/{id}")]
        public StoryType getStoryTypeById(int id)
        {
            try
            {
                StoryTypeRep rep = new StoryTypeRep();
                return rep.getById(id);
            }
            catch (Exception ex)
            {
            }
            return new StoryType();
        }
    }
}

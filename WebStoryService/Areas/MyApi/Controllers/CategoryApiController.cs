using System;
using System.Collections.Generic;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("category")]
    public class CategoryApiController : ApiController
    {
        [Route("get")]
        [HttpGet]
        public IEnumerable<Category> Get()
        {
            try
            {
                //var headerData = Request.Headers;
                //string username = string.Empty;
                //string password = string.Empty;
                //string token = string.Empty;
                //if (headerData.Contains("username"))
                //{
                //    {
                //        username = headerData.GetValues("username").First();
                //    }
                //    if (headerData.Contains("pwd"))
                //    {
                //        password = headerData.GetValues("pwd").First();
                //    }
                //    if (headerData.Contains("tk"))
                //    {
                //        token = headerData.GetValues("tk").First();
                //    }
                //    if (AccountRep.checkToken(username, password, token) == true)
                //{
                CategoryRes res = new CategoryRes();
                return res.Gets();
                //}

                // }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return new List<Category>();
        }
        [Route("post")]
        [HttpPost]
        public int Post([FromBody] Category value)
        {
            try
            {
                //var headerData = Request.Headers;
                //string username = string.Empty;
                //string password = string.Empty;
                //string token = string.Empty;
                //if (headerData.Contains("username"))

                //{
                //    username = headerData.GetValues("username").First();
                //}
                //if (headerData.Contains("pwd"))
                //{
                //    password = headerData.GetValues("pwd").First();
                //}
                //if (headerData.Contains("tk"))
                //{
                //    token = headerData.GetValues("tk").First();
                //}
                //if (AccountRep.checkToken(username, password, token) == true)
                //{
                if (value != null)
                {
                    CategoryRes res = new CategoryRes();

                    if (res.Post(value) == 1)
                    {
                        return 1;
                    }
                }
            
             } 
            catch (Exception ex) { 
            }
            return 0;
        }
        [Route("put")]
        [HttpPut]
        public int Put([FromBody] Category value)
        {
            try
            {
                CategoryRes res =new CategoryRes();
                //var headerData = Request.Headers;
                //string username = string.Empty;
                //string password = string.Empty;
                //string token = string.Empty;
                //if (headerData.Contains("username"))

                //{
                //    username = headerData.GetValues("username").First();
                //}
                //if (headerData.Contains("pwd"))
                //{
                //    password = headerData.GetValues("pwd").First();
                //}
                //if (headerData.Contains("tk"))
                //{
                //    token = headerData.GetValues("tk").First();
                //}
                //if (AccountRep.checkToken(username, password, token) == true)
                //{
                    if (value != null)
                    {

                        if (res.Put(value) == 1)
                        {
                            return 1;
                        }
                    }
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return 0;
        }
        [Route("soft")]
        [HttpPut]
        public int soft([FromBody]Category item)
        {
            try
            {
                CategoryRes res = new CategoryRes();
                //var headerData = Request.Headers;
                //string username = string.Empty;
                //string password = string.Empty;
                //string token = string.Empty;
                //if (headerData.Contains("username"))

                //{
                //    username = headerData.GetValues("username").First();
                //}
                //if (headerData.Contains("pwd"))
                //{
                //    password = headerData.GetValues("pwd").First();
                //}
                //if (headerData.Contains("tk"))
                //{
                //    token = headerData.GetValues("tk").First();
                //}
                //if (AccountRep.checkToken(username, password, token) == true)
                //{
                    if (item != null)
                    {

                        if (res.Soft_Delete(item) == 1)
                        {
                            return 1;
                        }
                   }
                //}

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return 0;
        }
    }
}

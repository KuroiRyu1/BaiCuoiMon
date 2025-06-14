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
        private StoryRes _storyRes = new StoryRes();

        [Route("get")]
        [HttpGet]
        public IEnumerable<Story> Get(int? categoryId = null, int page = 1, int pageSize = 10)
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
                    return _storyRes.Gets(categoryId, page, pageSize);
                //}
            }
            catch (Exception ex)
            {
            }

            return new List<Story>();
        }

        [Route("get/{id}")]
        [HttpGet]
        public HttpResponseMessage GetById(int id)
        {
            try
            {
                var headerData = Request.Headers;
                string username = string.Empty;
                string password = string.Empty;
                string token = string.Empty;

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
                {
                    var story = _storyRes.GetById(id);
                    if (story == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, story);
                }
            }
            catch (Exception ex)
            {
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [Route("post")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Story value)
        {
            if (value == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                //var headerData = Request.Headers;
                string username = string.Empty;
                //string password = string.Empty;
                string token = string.Empty;

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
                    int result = _storyRes.Post(value, username, token);
                    if (result == 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, value.Id);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                //}
            }
            catch (Exception ex)
            {
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [Route("put/{id}")]
        [HttpPut]
        public HttpResponseMessage Put(int id, [FromBody] Story value)
        {
            if (value == null || id != value.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                //var headerData = Request.Headers;
                string username = string.Empty;
                //string password = string.Empty;
                string token = string.Empty;

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
                    int result = _storyRes.Put(value, username, token);
                    if (result == 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                //}
            }
            catch (Exception ex)
            {
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        [Route("delete/{id}")]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                //var headerData = Request.Headers;
                string username = string.Empty;
                //string password = string.Empty;
                string token = string.Empty;

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
                {
                    int result = _storyRes.Delete(id, username, token);
                    if (result == 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                    //}
                }
            }
            catch (Exception e)
            {
            }
                

                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            [Route("increment-view/{id}")]
            [HttpPost]
            public HttpResponseMessage IncrementView(int id)
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
                    int result = _storyRes.IncrementView(id);
                    if (result == 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                    //}
                }
                catch (Exception ex)
                {

                }

                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            [Route("search")]
            [HttpGet]
            public IEnumerable<Story> Search(string keyword = "", int? categoryId = null)
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
                    return _storyRes.Search(keyword, categoryId);
                    //}
                }
                catch (Exception ex)
                {
                }

                return new List<Story>();
            }
        }
    }
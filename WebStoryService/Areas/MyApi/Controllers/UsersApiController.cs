using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebStoryService.Models;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("user")]
    public class UsersApiController : ApiController
    {
        //[Route("login")]
        //[HttpGet]
        //public User Login(string username = "getun", string password = "123")
        //{
        //    User user = new User();
        //    try
        //    {
        //        string a = Function.MD5Hash(password);
        //        UserRes userRes = new UserRes();
        //        user = userRes.Login(username, a);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return user;
        //}
        //[Route("register")]
        //[HttpGet]
        //public int Register(string username = "",string fullname="",string password="")
        //{
        //    int result = 0;
        //    //if (user != null)
        //    {
        //        UserRes userRes = new UserRes();
        //        if (userRes.checkUsername(username) == 0)
        //        {
        //            //if (userRes.Register(user) != 0)
        //            //{
        //            //    result = 1;
        //            //}
        //        }
        //        else
        //        {
        //            result = 0;
        //        }

        //    }
        //    return result;
        //}
        
    }
}

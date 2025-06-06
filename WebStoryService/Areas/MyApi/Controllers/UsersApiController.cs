using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.ModelBinding;
using WebStoryService.Models;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("user")]
    public class UsersApiController : ApiController
    {
        [Route("login")]
        [HttpPost]
        public User Login(User user)
        {
            try
            {
                string a = Function.MD5Hash(user.Password);
                UserRes userRes = new UserRes();
                user = userRes.Login(user.Username, a);
            }
            catch (Exception ex)
            {
            }
            return user;
        }
        [Route("register")]
        [HttpPost]
        public int Register(User user)
        {
            int result = 0;
            if (user != null)
            {
                UserRes userRes = new UserRes();
                if (userRes.checkUsername(user.Username) == 0)
                {
                    user.Password = Function.MD5Hash(user.Password);
                    user.token = Function.GenerateToken();

                        if (userRes.Register(user) != 0)
                        {
                            result = 1;
                        }
                }
                else
                {
                    result = 0;
                }

            }
            return result;
        }
        
    }
}

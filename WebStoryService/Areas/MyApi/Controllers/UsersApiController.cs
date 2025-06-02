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
        [Route("login")]
        [HttpGet]
        public User Login(string username = "getun", string password = "123")
        {
            User user = new User();
            try
            {
                string a = Function.MD5Hash(password);
                UserRes userRes = new UserRes();
                user = userRes.Login(username, a);
            }
            catch (Exception ex)
            {
            }
            return user;
        }
        [Route("register")]
        [HttpGet]
        public int Register(string username = "",string fullname="",string password="")
        {
            int result = 0;
            //if (user != null)
            {
                UserRes userRes = new UserRes();
                if (userRes.checkUsername(username) == 0)
                {
                    //if (userRes.Register(user) != 0)
                    //{
                    //    result = 1;
                    //}
                }
                else
                {
                    result = 0;
                }

            }
            return result;
        }
        [Route("list")]
        [HttpGet]
        public IEnumerable<User> Get()
        {
            UserRes res = new UserRes();
            return res.Gets();
        }
        [Route("delete/{userId}")]
        [HttpPatch]
        public int Delete(int userId)
        {
            try
            {
                using (DbEntities en = new DbEntities())
                {
                    var userToDeactivate = en.tbl_user.FirstOrDefault(d => d.C_id == userId);
                    if (userToDeactivate != null)
                    {
                        userToDeactivate.C_active = 0;
                        en.SaveChanges();
                        return 1; 
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return 0; 
        }
        [Route("detail/{userId}")]
        [HttpGet]
        public User Detail(int userId)
        {
            User user = null;
            try
            {
                using (DbEntities en = new DbEntities())
                {
                    user = en.tbl_user.Where(d => d.C_id == userId)
                        .Select(d => new User
                        {
                            Id = (int)d.C_id,
                            FullName = d.C_fullname,
                            Username = d.C_username,
                            Active = d.C_active ?? 0,
                            Password = d.C_password,
                            token = d.C_token, 
                            role = d.C_role ?? 0
                        }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return user;
        }
    }
}

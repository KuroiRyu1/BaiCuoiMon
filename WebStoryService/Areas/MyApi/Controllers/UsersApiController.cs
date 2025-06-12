using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.ModelBinding;
using WebStoryService.Models;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("user")]
    public class UsersApiController :ApiController
    {
        private readonly UserRes _userRes;

        public UsersApiController()
        {
            _userRes = new UserRes();
        }
        
        [Route("login")]
        [HttpPost]
        public IHttpActionResult Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userFromDb = _userRes.ValidateUser(request.Username, request.Password);

                if (userFromDb == null)
                {
                    return Unauthorized(); 
                }

                var token = Function.GenerateToken();

                var User = new User
                {
                    Id = userFromDb.Id,
                    Username = userFromDb.Username,
                    FullName = userFromDb.FullName,
                    Email = userFromDb.Email,
                    role = userFromDb.role,
                    token = token 
                };

                return Ok(User); 
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("register")]
        [HttpPost]
        public IHttpActionResult Register(User userRequest)
        {
            if (userRequest == null || string.IsNullOrEmpty(userRequest.Username) || string.IsNullOrEmpty(userRequest.Password))
            {
                return BadRequest("Tên đăng nhập và mật khẩu là bắt buộc.");
            }

            if (userRequest.Username.Length > 10)
            {
                return BadRequest("Tên đăng nhập tối đa 10 ký tự.");
            }

            try
            {
                using (var db = new DbEntities())
                {
                    var existingUser = db.tbl_user.FirstOrDefault(u => u.C_username == userRequest.Username);
                    if (existingUser != null)
                    {
                        return Content(HttpStatusCode.Conflict, new
                        {
                            Message = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác."
                        });
                    }
                }

                var createdUser = _userRes.Register(userRequest);

                if (createdUser == null)
                {
                    return Conflict();
                }
                else
                {
                    var response = new
                    {
                        Id = createdUser.Id,
                        Username = createdUser.Username,
                        Message = "Đăng ký thành công."
                    };

                    return Created($"user/detail/{response.Id}", response);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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

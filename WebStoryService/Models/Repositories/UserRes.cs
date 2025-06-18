using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;


namespace WebStoryService.Models.Repositories
{
    public class UserRes
    {
        private readonly DbEntities _db;

        public UserRes()
        {
            _db = new DbEntities();
        }

        public List<User> Gets()
        {
            return _db.tbl_user
                  .Select(d => new User
                  {
                      Id = (int)d.C_id,
                      FullName = d.C_fullname,
                      Username = d.C_username,
                      Email = d.C_email,
                      Active = d.C_active ?? 0,
                      role = d.C_role ?? 1
                  })
                  .ToList();
        }
        public User ValidateUser(string username, string plainTextPassword)
        {
            var userEntity = _db.tbl_user.FirstOrDefault(u => u.C_username == username);

            if (userEntity == null)
            {
                return null; 
            }

            if (BCrypt.Net.BCrypt.Verify(plainTextPassword, userEntity.C_password))
            {
                return new User
                {
                    Id = (int)userEntity.C_id,
                    FullName = userEntity.C_fullname,
                    Username = userEntity.C_username,
                    Email = userEntity.C_email,
                    Active = userEntity.C_active ?? 0,
                    role = userEntity.C_role ?? 1
                };
            }

            return null;
        }
        public User Login(string username, string password)
        {
            User user = new User();
            try
            {
                DbEntities en = new DbEntities();
                user = en.tbl_user.Where(d => d.C_username.Equals(username) && d.C_password.Equals(password))
                    .Select(d => new User {
                        Id = (int)d.C_id,
                        Username = d.C_username,
                        FullName = d.C_fullname,
                        Active = d.C_active ?? 0,
                        Password = d.C_password,
                        token = d.C_token,
                        Email = d.C_email,
                        role = (int)d.C_role
                    }).FirstOrDefault();
            }
            catch(Exception ex)
            {
            }
            return user;
        }
        public User Register(User user) 
        {
            try
            {
                var userEntity = new tbl_user
                {
                    C_username = user.Username,
                    C_password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    C_fullname = user.FullName,
                    C_email = user.Email,
                    C_role = user.role,
                    C_active = 1
                };

                _db.tbl_user.Add(userEntity);
                _db.SaveChanges();

                user.Id = (int)userEntity.C_id;
                user.Password = null;
                return user; 
            }
            catch (Exception ex) when (ex.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                return null;
            }
        }


        public int checkUsername(string username)
        {
            int result = 0;
            try
            {
                DbEntities en = new DbEntities();
                var q = en.tbl_user.Where(d=>username.Equals(d.C_username));
                if (q.Any())
                {
                    result = 1;
                }
            }
            catch (Exception ex) {
            }
            return result;
        }
        public int EditUser(User user)
        {
            try
            {
                DbEntities en = new DbEntities();
                if (user == null)
                {
                    return 0;
                }
                var item = en.tbl_user.Find(user.Id);
                if (item != null)
                {
                    item.C_role = user.role;
                    en.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
        public User GetUserById(int id)
        {
            var userEntity = _db.tbl_user.FirstOrDefault(u => u.C_id == id);

            if (userEntity == null)
            {
                return null;
            }

            return new User
            {
                Id = (int)userEntity.C_id,
                FullName = userEntity.C_fullname,
                Username = userEntity.C_username,
                Email = userEntity.C_email,
                Active = userEntity.C_active ?? 0,
                role = userEntity.C_role ?? 1
            };
        }
    }
}
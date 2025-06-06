using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class UserRes
    {
        public List<User> Gets()
        {
            List<User> user = new List<User>();
            try
            {
                DbEntities en =new DbEntities();
                user = en.tbl_user.Select(d => new User{
                    Id =(int) d.C_id,
                    FullName = d.C_fullname,
                    Username = d.C_username,
                    Active = d.C_active ?? 0,
                    Password = d.C_password,
                    Email =d.C_email,
                    role = (int)d.C_role,
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return user;
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
        public int Register(User user)
        {
            try
            {
                DbEntities en = new DbEntities();
                tbl_user tbl = new tbl_user
                {
                    C_username = user.Username,
                    C_password = user.Password,
                    C_fullname = user.FullName,
                    C_role = 1,
                    C_active = 1,
                    C_token = user.token,
                    C_email = user.Email,
                };
                en.tbl_user.Add(tbl);
                en.SaveChanges();
                user.Id = (int)tbl.C_id;
                return 1;
            }
            catch (Exception ex)
            {
            }
            return 0;
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
        
    }
}
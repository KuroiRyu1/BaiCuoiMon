using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;

namespace WebStoryService.Models.Repositories
{
    public class AccountRep
    {
        public static bool checkToken(string username, string password, string token)
        {
            try
            {
                //check exist data in table usertoken(idtk,userid,cokenCurrent,status)
                if (username.Equals("admin") && password.Equals("123") && token.Equals("12345"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

     public static bool CheckToken(string username, string password, string token)
        {
            try
            {
                using (var db = new DbEntities())
                {
                    var user = db.tbl_user
                        .FirstOrDefault(u => u.C_username == username &&
                                             u.C_password == password &&
                                             u.C_token == token &&
                                             u.C_role == 0); // Only admin
                    return user != null;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
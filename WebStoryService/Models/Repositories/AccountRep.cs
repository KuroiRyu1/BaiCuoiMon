using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}
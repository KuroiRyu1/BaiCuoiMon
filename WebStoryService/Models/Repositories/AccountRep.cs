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
                if (username.Equals("getun") && password.Equals("wuwa") && token.Equals("123"))
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
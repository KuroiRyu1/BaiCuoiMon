using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace StoryWeb.Models.Repositories
{
    public class UserRep
    {
        public UserRep() { }
        public static UserRep _instance = null;
        public static UserRep Instance {  
            get {
                
                if (_instance == null)
                {
                    _instance = new UserRep();
                }
                return _instance; 
            } 
        }
    }
}
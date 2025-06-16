using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebStoryService.Models.ModelData
{
    public class User
    {
        public int Id { get; set; } 
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Active { get; set; }
        public string token { get; set; }
        public int role { get; set; }
        public string Email { get; set; } 
    }
    
}
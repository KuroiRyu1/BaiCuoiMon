using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWeb.Models.ModelView
{
    public class User
    {
        public int Id { get; set; } = 0;
        public string FullName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public int Active { get; set; } = 0;
        public int Role { get; set; } = 0;
        public string Email { get; set; } = "no email";
    }
}
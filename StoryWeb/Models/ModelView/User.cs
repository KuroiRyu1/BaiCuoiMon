using Newtonsoft.Json;
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

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
        public int Active { get; set; } = 0;
        public int Role { get; set; } = 0;
        [JsonProperty("token")]
        public string Token { get; set; } 
        public string Email { get; set; } = "no email";
    }
}
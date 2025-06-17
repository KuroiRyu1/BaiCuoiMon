using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace StoryWeb.Models.ModelView
{
    public class ChapterComment
    {
        [JsonProperty("C_id")]
        public int Id { get; set; }

        [JsonProperty("C_content")]
        public string Content { get; set; }

        [JsonProperty("C_chapter_id")]
        public int ChapterId { get; set; }

        [JsonProperty("C_user_id")]
        public int UserId { get; set; }

        [JsonProperty("UserFullname")]
        public string Fullname { get; set; }

        [JsonProperty("C_active")]
        public int Active { get; set; }
    }
}
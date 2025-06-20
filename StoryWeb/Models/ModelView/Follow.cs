using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWeb.Models.ModelView
{
    public class Follow
    {
        public int Id { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public int StoryId { get; set; } = 0;
        public int Active { get; set; } = 0;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWeb.Models.ModelView
{
    public class StoryType
    {
        public int Id { get; set; } = 0;
        public string Title { get; set; } = string.Empty;
        public int Active { get; set; } = 0;
    }
}
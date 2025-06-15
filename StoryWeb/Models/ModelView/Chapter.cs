using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWeb.Models.ModelView
{
    public class Chapter
    {
        public int Id { get; set; } = 0;
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int StoryId { get; set; } = 0;

        public int ChapterIndex { get; set; } = 0;  

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryWeb.Models.ModelView
{
    public class ChapterImage
    {
        public long Id { get; set; } = 0;
        public string ImagePath { get; set; } = "";
        public int ChapterId { get; set; } = 0;
    }
}
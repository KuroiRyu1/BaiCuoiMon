using System;

namespace StoryWeb.Models.ModelView
{
    public class Chapter
    {
        public int Id { get; set; } = 0;
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime DayCreate { get; set; } = DateTime.Now;
        public int StoryId { get; set; } = 0;
        public int ImageCount { get; set; } = 0; 
    }
}
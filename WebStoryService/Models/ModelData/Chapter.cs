using System;

namespace WebStoryService.Models.ModelData
{
    public class Chapter
    {
        public int Id { get; set; } = 0;
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime DayCreate { get; set; } = DateTime.Now;
        public int StoryId { get; set; } = 0;
        public int ImageCount { get; set; } = 0; // Số lượng ảnh cho truyện hình
        public int ChapterIndex {  get; set; } = 0;

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class ChapterRes
    {
        private readonly DbEntities _db = new DbEntities();

        public List<Chapter> GetByStoryId(int storyId)
        {
            try
            {
                return _db.tbl_chapter
                    .Where(c => c.C_story_id == storyId)
                    .Select(c => new Chapter
                    {
                        Id = c.C_id,
                        Title = c.C_title ?? "",
                        Content = c.C_content ?? "",
                        DayCreate = c.C_day_create ?? DateTime.Now,
                        StoryId = c.C_story_id ?? 0,
                        ImageCount = _db.tbl_chapter_image.Count(i => i.C_chapter_id == c.C_id)
                    })
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Chapter>();
            }
        }
        public Chapter StoryRead(int storyId,int chapterIndex)
        {
            var story = new Chapter(); 
            try
            {
                var item = _db.tbl_chapter.Where(d => d.C_story_id == storyId 
                && d.C_chapter_index == chapterIndex).Select(d => new Chapter
                {
                    Id = d.C_id,
                    Title = d.C_title,
                    Content = d.C_content,
                    StoryId = storyId,
                    ChapterIndex = chapterIndex
                }).FirstOrDefault();
                if (item != null)
                {
                    story = item;
                }
            }
            catch (Exception ex)
            {
            }
            return story;
        }
    }

}
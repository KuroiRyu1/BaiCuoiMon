using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class ChapterRes
    {
        private readonly DbEntities _db = new DbEntities();

        public List<tbl_chapter> GetByStoryId(int storyId)
        {
            return _db.tbl_chapter.Where(c => c.C_story_id == storyId).ToList();
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
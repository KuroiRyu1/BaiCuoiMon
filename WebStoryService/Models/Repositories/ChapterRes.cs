using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;

namespace WebStoryService.Models.Repositories
{
    public class ChapterRes
    {
        private readonly DbEntities _db = new DbEntities();

        public List<tbl_chapter> GetByStoryId(int storyId)
        {
            return _db.tbl_chapter.Where(c => c.C_story_id == storyId).ToList();
        }
    }
}
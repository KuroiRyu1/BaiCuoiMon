using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class StoryTypeRep
    {
        public List<StoryType> get()
        {
            try
            {
                DbEntities en = new DbEntities();
                var item = en.tbl_story_type.Select(d => new StoryType
                {
                    Id = d.C_id,
                    Title = d.C_title,
                    Active = d.C_active ?? 1
                });
                if (item != null)
                {
                    return item.ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return new List<StoryType>();
        }
        public StoryType getById(int id)
        {
            try
            {
                DbEntities en = new DbEntities();
                var item = en.tbl_story_type.Where(d=>d.C_id==id).Select(d => new StoryType
                {
                    Id = d.C_id,
                    Title = d.C_title,
                    Active = d.C_active ?? 1
                }).FirstOrDefault();
                if (item != null)
                {
                    return item;
                }
            }
            catch (Exception ex)
            {
            }
            return new StoryType();
        }
    }
}
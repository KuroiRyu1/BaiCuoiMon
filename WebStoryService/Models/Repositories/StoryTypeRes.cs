using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class StoryTypeRes
    {
        public List<StoryType> GetStoryTypes()
        {
            try
            {
                using (var db = new DbEntities())
                {
                    return db.tbl_story_type
                        .Where(st => st.C_active == 1)
                        .Select(st => new StoryType
                        {
                            Id = st.C_id,
                            Title = st.C_title ?? "",
                            Active = st.C_active ?? 0
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetStoryTypes: {ex.Message}");
            }
            return new List<StoryType>();
        }
    }
}
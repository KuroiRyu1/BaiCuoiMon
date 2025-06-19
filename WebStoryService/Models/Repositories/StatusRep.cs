using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class StatusRep
    {
        public List<Status> GetAllStatus()
        {
            try
            {
                DbEntities en = new DbEntities();
                var item = en.tbl_status.Select(d=> new Status
                {
                    Id = d.C_id,
                    Title = d.C_title,
                    Active = d.C_active??1,
                }).ToList();
                if (item != null)
                {
                    return item;
                }
            }
            catch (Exception ex)
            {
            }
            return new List<Status>();
        }
    }
}
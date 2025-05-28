using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class CategoryRes
    {
        public List<Category> Gets()
        {
            List<Category> list = new List<Category>();
            try
            {
                DbEntities entities = new DbEntities();
                list = entities.tbl_category.Select(d => new Category
                {
                    Id = d.C_id,
                    Name = d.C_name,
                    Active = d.C_active ?? 0,
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return list;
        }
    }
}
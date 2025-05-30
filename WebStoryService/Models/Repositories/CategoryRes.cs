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
                DbEntities en = new DbEntities();
                list = en.tbl_category.Select(d => new Category
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
        public int Post(Category item)
        {
            try
            {
                DbEntities en = new DbEntities();
                var cate = new tbl_category { C_name = item.Name, C_active = item.Active };
                en.tbl_category.Add(cate);
                en.SaveChanges();
                item.Id = cate.C_id;
                return 1;

            }
            catch (Exception ex)
            {
            }
            return 0;
        }
    }
}
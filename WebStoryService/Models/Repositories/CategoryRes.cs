using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int Put(Category item)
        {
            try
            {
                DbEntities en = new DbEntities();
                var q = en.tbl_category.Where(d=>d.C_id == item.Id).FirstOrDefault();
                if (q != null)
                {
                    q.C_name = item.Name;
                    q.C_active = item.Active;
                    en.SaveChanges();
                    return 1;
                }                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return 0;
        }
        public int Soft_Delete(Category item)
        {
            try
            {
                DbEntities en = new DbEntities();
                var q = en.tbl_category.Any(p => p.C_id == item.Id);
                if (q)
                {
                    var entity = en.tbl_category.Where(d => d.C_id == item.Id).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.C_active = 0;
                        en.SaveChanges();
                        return 1;
                    }
                    
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return 0;
        }
        public List<Category> findByName(string name)
        {
            List<Category> category = new List<Category>();
            try
            {
                DbEntities en = new DbEntities();
                category = en.tbl_category.Where(d=> d.C_name.ToLower().
                Contains(name.ToLower())).Select(d=> new Category
                {
                    Id = d.C_id,
                    Name = d.C_name,
                    Active = d.C_active??1,
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return category;
        }
    }
}
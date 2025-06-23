using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class AuthorRes
    {
        public List<Author> GetAuthors()
        {
            try
            {
                using (var db = new DbEntities())
                {
                    return db.tbl_author
                        .Select(a => new Author
                        {
                            Id = a.C_id,
                            Name = a.C_name ?? "",
                            Information = a.C_information ?? "",
                            Image = a.C_image ?? ""
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAuthors: {ex.Message}");
            }
            return new List<Author>();
        }
        public Author getAuthorById(int id=0)
        {
            try
            {
                DbEntities en = new DbEntities();
                if (id != 0)
                {
                    var item = en.tbl_author.Where(d=>d.C_id == id).Select(d =>new Author
                    {
                        Id = d.C_id,
                        Name = d.C_name ,
                        Information = d.C_information ,
                        Image = d.C_image ,
                    }).FirstOrDefault();
                    return item;
                }
            }
            catch (Exception ex)
            {
            }
            return new Author();
        }
        public int AddAuthor(Author author)
        {
            try
            {
                DbEntities db = new DbEntities();
                if (author != null)
                {
                    var tbl = new tbl_author
                    {
                        C_id = author.Id,
                        C_name = author.Name,
                        C_image = author.Image,
                        C_information = author.Information
                    };
                    db.tbl_author.Add(tbl);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
    }
}
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
    }
}
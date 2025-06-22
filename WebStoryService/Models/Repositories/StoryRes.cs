using System;
using System.Collections.Generic;
using System.Linq;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class StoryRes
    {
        public List<Story> Gets(int? categoryId = null, int page = 1, int pageSize = 10)
        {
            List<Story> list = new List<Story>();
            try
            {
                using (var en = new DbEntities())
                {
                    var query = en.tbl_story
                        .Include("tbl_author")
                        .Include("tbl_category")
                        .AsQueryable();
                    if (categoryId.HasValue && categoryId.Value != 0)
                    {
                        query = query.Where(s => (s.C_category_id ?? 0) == categoryId.Value && s.C_active == 1);
                    }
                    else
                    {
                        query = query.Where(s => s.C_active == 1);
                    }
                    list = query
                        .OrderBy(s => s.C_id)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(s => new Story
                        {
                            Id = s.C_id,
                            Title = s.C_title,
                            ChapterNumber = s.C_chapter_number ?? 1,
                            Introduction = s.C_introduction,
                            Image = s.C_image,
                            LikeNumber = s.C_like_number ?? 0,
                            FollowNumber = s.C_follow_number ?? 0,
                            ViewNumber = s.C_view_number ?? 0,
                            AuthorId = s.C_author_id ?? 0,
                            StatusId = s.C_status_id ?? 0,
                            CategoryId = s.C_category_id ?? 0,
                            StoryTypeId = s.C_story_type_id ?? 0,
                            AuthorName = s.tbl_author != null ? s.tbl_author.C_name : "",
                            CategoryName = s.tbl_category != null ? s.tbl_category.C_name : ""
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return list;
        }

        public List<Story> GetAll(int? categoryId = null)
        {
            List<Story> list = new List<Story>();
            try
            {
                var en = new DbEntities();
                var query = en.tbl_story
                       .Include("tbl_author")
                       .Include("tbl_category")
                       .AsQueryable();
                if (categoryId.HasValue && categoryId.Value != 0)
                {
                    query = query.Where(s => (s.C_category_id ?? 0) == categoryId.Value && s.C_active == 1);
                }
                else
                {
                    query = query.Where(s => s.C_active == 1);
                }
                var item = query.Select(s => new Story
                {
                    Id = s.C_id,
                    Title = s.C_title,
                    ChapterNumber = s.C_chapter_number ?? 1,
                    Introduction = s.C_introduction,
                    Image = s.C_image,
                    LikeNumber = s.C_like_number ?? 0,
                    FollowNumber = s.C_follow_number ?? 0,
                    ViewNumber = s.C_view_number ?? 0,
                    AuthorId = s.C_author_id ?? 0,
                    StatusId = s.C_status_id ?? 0,
                    CategoryId = s.C_category_id ?? 0,
                    StoryTypeId = s.C_story_type_id ?? 0,
                    AuthorName = s.tbl_author != null ? s.tbl_author.C_name : "",
                    CategoryName = s.tbl_category != null ? s.tbl_category.C_name : ""
                }).ToList();
                if (item != null)
                {
                    list = item;
                }
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public int checkCategory(int categoryId = 0)
        {
            try
            {
                if (categoryId != 0)
                {
                    DbEntities en = new DbEntities();
                    var q = en.tbl_story.Any(d => d.C_category_id == categoryId);
                    if (q)
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        public Story GetById(int id)
        {
            try
            {
                using (var db = new DbEntities())
                {
                    var story = db.tbl_story
                        .Where(s => s.C_active == 1 && s.C_id == id)
                        .Select(s => new Story
                        {
                            Id = s.C_id,
                            Title = s.C_title ?? "",
                            ChapterNumber = s.C_chapter_number ?? 0,
                            Introduction = s.C_introduction ?? "",
                            Image = s.C_image ?? "default.jpg",
                            LikeNumber = s.C_like_number ?? 0,
                            FollowNumber = s.C_follow_number ?? 0,
                            ViewNumber = s.C_view_number ?? 0,
                            AuthorId = s.C_author_id ?? 0,
                            StatusId = s.C_status_id ?? 0,
                            CategoryId = s.C_category_id ?? 0,
                            StoryTypeId = s.C_story_type_id ?? 0,
                            AuthorName = s.tbl_author != null ? s.tbl_author.C_name : "",
                            CategoryName = s.tbl_category != null ? s.tbl_category.C_name : ""
                        })
                        .FirstOrDefault();

                    System.Diagnostics.Debug.WriteLine($"Retrieved story ID {id}: {(story != null ? "Found" : "Not Found")}");
                    return story;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetById: {ex.Message}");
                return null;
            }
        }

        public int Create(Story story)
        {
            try
            {
                using (var db = new DbEntities())
                {
                    if (story == null || story.AuthorId <= 0 || string.IsNullOrEmpty(story.Title))
                    {
                        System.Diagnostics.Debug.WriteLine("Invalid story data: AuthorId or Title is missing.");
                        return 0;
                    }

                    var tbl_story = new tbl_story
                    {
                        C_title = story.Title,
                        C_status_id = story.StatusId,
                        C_category_id = story.CategoryId,
                        C_story_type_id = story.StoryTypeId,
                        C_author_id = story.AuthorId,
                        C_image = story.Image,
                        C_introduction = story.Introduction,
                        C_active = story.Active,
                        C_like_number = 0, // Thiết lập mặc định là 0
                        C_follow_number = 0, // Thiết lập mặc định là 0
                        C_view_number = 0 // Thiết lập mặc định là 0
                    };
                    db.tbl_story.Add(tbl_story);
                    db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine($"Story created with ID: {tbl_story.C_id}, AuthorId: {tbl_story.C_author_id}");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Create: {ex.Message}");
                return 0;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using (var db = new DbEntities())
                {
                    var story = db.tbl_story.FirstOrDefault(s => s.C_id == id);
                    if (story != null)
                    {
                        story.C_active = 0; // Xóa mềm
                        db.SaveChanges();
                        System.Diagnostics.Debug.WriteLine($"Successfully soft-deleted story ID {id}.");
                        return true;
                    }
                    System.Diagnostics.Debug.WriteLine($"Story ID {id} not found in database.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Delete: {ex.Message}");
                return false;
            }
        }

        public int Update(Story story)
        {
            try
            {
                using (var db = new DbEntities())
                {
                    var entity = db.tbl_story.FirstOrDefault(s => s.C_id == story.Id);
                    if (entity != null)
                    {
                        entity.C_chapter_number = story.ChapterNumber;
                        db.SaveChanges();
                        return 1;
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Update: {ex.Message}");
                return 0;
            }
        }
    }
}
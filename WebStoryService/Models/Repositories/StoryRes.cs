using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls.Expressions;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class StoryRes
    {
        public List<Story> Gets(int? categoryId = null, int page = 1, int pageSize = 10, int storyTypeId = 1)
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
                        query = query.Where(s => s.C_active == 1 && s.C_story_type_id == storyTypeId);
                    }

                    list = query
                        .OrderBy(s => s.C_id)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(s => new Story
                        {
                            Id = s.C_id,
                            Title = s.C_title,
                            ChapterNumber = s.C_chapter_number ?? 0,
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
                System.Diagnostics.Debug.WriteLine($"Error in Gets: {ex.Message}");
            }
            return list;
        }

        public List<Story> GetAll(int? categoryId = null, int storyTypeId = 1)
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
                        query = query.Where(s => (s.C_category_id ?? 0) == categoryId.Value && s.C_active == 1 && s.C_story_type_id == storyTypeId);
                    }
                    else
                    {
                        query = query.Where(s => s.C_active == 1 && s.C_story_type_id == storyTypeId);
                    }

                    list = query.Select(s => new Story
                    {
                        Id = s.C_id,
                        Title = s.C_title,
                        ChapterNumber = s.C_chapter_number ?? 0,
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
                System.Diagnostics.Debug.WriteLine($"Error in GetAll: {ex.Message}");
            }
            return list;
        }

        public int checkCategory(int categoryId = 0)
        {
            try
            {
                if (categoryId != 0)
                {
                    using (var en = new DbEntities())
                    {
                        var q = en.tbl_story.Any(d => d.C_category_id == categoryId);
                        if (q)
                        {
                            return 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in checkCategory: {ex.Message}");
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
                    if (story == null || string.IsNullOrEmpty(story.Title) || story.AuthorId <= 0)
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
                        C_like_number = 0,
                        C_follow_number = 0,
                        C_view_number = 0,
                        C_chapter_number = 0
                    };

                    db.tbl_story.Add(tbl_story);
                    db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine($"Story created with ID: {tbl_story.C_id}, AuthorId: {tbl_story.C_author_id}, ChapterNumber: {tbl_story.C_chapter_number}");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Create: {ex.Message}");
                return 0;
            }
        }

        public int Update(Story story)
        {
            try
            {
                using (var db = new DbEntities())
                {
                    if (story == null || story.Id <= 0 || string.IsNullOrEmpty(story.Title) || story.AuthorId <= 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Invalid story data: Id={story?.Id}, Title={story?.Title}, AuthorId={story?.AuthorId}");
                        return 0;
                    }

                    var entity = db.tbl_story.FirstOrDefault(s => s.C_id == story.Id);
                    if (entity != null)
                    {
                        entity.C_title = story.Title;
                        entity.C_author_id = story.AuthorId;
                        entity.C_category_id = story.CategoryId;
                        entity.C_status_id = story.StatusId;
                        entity.C_story_type_id = story.StoryTypeId;
                        entity.C_introduction = story.Introduction;
                        entity.C_image = story.Image;
                        entity.C_active = story.Active;
                        entity.C_chapter_number = story.ChapterNumber;
                        db.SaveChanges();
                        System.Diagnostics.Debug.WriteLine($"Story updated with ID: {story.Id}, ChapterNumber: {story.ChapterNumber}");
                        return 1;
                    }

                    System.Diagnostics.Debug.WriteLine($"Story ID {story.Id} not found in database.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Update: {ex.Message}");
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
                        story.C_active = 0;
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

        public void IncreaseView(int storyId)
        {
            try
            {
                using (var en = new DbEntities())
                {
                    var item = en.tbl_story.FirstOrDefault(d => d.C_id == storyId);
                    if (item != null)
                    {
                        item.C_view_number = (item.C_view_number ?? 0) + 1;
                        en.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in IncreaseView: {ex.Message}");
            }
        }

        public List<Story> Search(string name)
        {
            try
            {
                using (var en = new DbEntities())
                {
                    var item = en.tbl_story
                        .Where(d => d.C_title.ToLower().Contains(name.ToLower()))
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
                        }).ToList();

                    return item ?? new List<Story>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Search: {ex.Message}");
                return new List<Story>();
            }
        }
    }
}

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
                        query = query.Where(s => (s.C_category_id ?? 0) == categoryId.Value);
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
        public List<Story> GetAll()
        {
            List<Story> list = new List<Story>();
            try
            {
                var en = new DbEntities();
                var item = en.tbl_story.Select(s=>new Story
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
                if(item != null)
                {
                    list = item;
                }
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public Story GetById(int id)
        {
            Story story = new Story();
            try
            {
                using (var en = new DbEntities())
                {
                    story = en.tbl_story
                        .Include("tbl_author")
                        .Include("tbl_category")
                        .Where(s => s.C_id == id)
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
                        }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return story;
        }

        public int Post(Story item, string username, string token)
        {
            try
            {
                using (var en = new DbEntities())
                {
                    // Check if user is admin (C_role == 0)
                    var user = en.tbl_user
                        .FirstOrDefault(u => u.C_username == username && u.C_token == token && (u.C_role ?? 0) == 0);
                    if (user == null)
                    {
                        return 0; // Not admin or invalid token
                    }

                    // Validate foreign keys
                    if (!en.tbl_author.Any(a => a.C_id == item.AuthorId) ||
                        !en.tbl_status.Any(s => s.C_id == item.StatusId) ||
                        !en.tbl_category.Any(c => c.C_id == item.CategoryId) ||
                        !en.tbl_story_type.Any(st => st.C_id == item.StoryTypeId))
                    {
                        return 0; // Invalid foreign key
                    }

                    // Validate required fields
                    if (string.IsNullOrEmpty(item.Title))
                    {
                        return 0; // Title is required
                    }

                    var story = new tbl_story
                    {
                        C_title = item.Title,
                        C_chapter_number = item.ChapterNumber,
                        C_introduction = item.Introduction,
                        C_image = string.IsNullOrEmpty(item.Image) ? "default.jpg" : item.Image,
                        C_like_number = 0,
                        C_follow_number = 0,
                        C_view_number = 0,
                        C_author_id = item.AuthorId,
                        C_status_id = item.StatusId,
                        C_category_id = item.CategoryId,
                        C_story_type_id = item.StoryTypeId
                    };
                    en.tbl_story.Add(story);
                    en.SaveChanges();
                    item.Id = story.C_id;
                    return 1;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                return 0;
            }
        }

        public int Put(Story item, string username, string token)
        {
            try
            {
                using (var en = new DbEntities())
                {
                    // Check if user is admin (C_role == 0)
                    var user = en.tbl_user
                        .FirstOrDefault(u => u.C_username == username && u.C_token == token && (u.C_role ?? 0) == 0);
                    if (user == null)
                    {
                        return 0; // Not admin or invalid token
                    }

                    var story = en.tbl_story.Find(item.Id);
                    if (story == null)
                    {
                        return 0; // Story not found
                    }

                    // Validate foreign keys
                    if (!en.tbl_author.Any(a => a.C_id == item.AuthorId) ||
                        !en.tbl_status.Any(s => s.C_id == item.StatusId) ||
                        !en.tbl_category.Any(c => c.C_id == item.CategoryId) ||
                        !en.tbl_story_type.Any(st => st.C_id == item.StoryTypeId))
                    {
                        return 0; // Invalid foreign key
                    }

                    // Validate required fields
                    if (string.IsNullOrEmpty(item.Title))
                    {
                        return 0; // Title is required
                    }

                    story.C_title = item.Title;
                    story.C_chapter_number = item.ChapterNumber;
                    story.C_introduction = item.Introduction;
                    story.C_image = string.IsNullOrEmpty(item.Image) ? "default.jpg" : item.Image;
                    story.C_author_id = item.AuthorId;
                    story.C_status_id = item.StatusId;
                    story.C_category_id = item.CategoryId;
                    story.C_story_type_id = item.StoryTypeId;
                    // C_like_number, C_follow_number, C_view_number không cập nhật ở đây
                    en.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                return 0;
            }
        }

        public int Delete(int id, string username, string token)
        {
            try
            {
                using (var en = new DbEntities())
                {
                    // Check if user is admin (C_role == 0)
                    var user = en.tbl_user
                        .FirstOrDefault(u => u.C_username == username && u.C_token == token && (u.C_role ?? 0) == 0);
                    if (user == null)
                    {
                        return 0; // Not admin or invalid token
                    }

                    var story = en.tbl_story.Find(id);
                    if (story == null)
                    {
                        return 0; // Story not found
                    }

                    // Check if story has chapters
                    if (en.tbl_chapter.Any(c => c.C_story_id == id))
                    {
                        return 0; // Cannot delete story with chapters
                    }

                    en.tbl_story.Remove(story);
                    en.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                return 0;
            }
        }

        public int IncrementView(int id)
        {
            try
            {
                using (var en = new DbEntities())
                {
                    var story = en.tbl_story.Find(id);
                    if (story == null)
                    {
                        return 0; // Story not found
                    }

                    story.C_view_number = (story.C_view_number ?? 0) + 1;
                    en.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                return 0;
            }
        }

        public List<Story> Search(string keyword, int? categoryId = null)
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
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query = query.Where(s => (s.C_title ?? "").Contains(keyword) || (s.C_introduction ?? "").Contains(keyword));
                    }
                    if (categoryId.HasValue && categoryId.Value != 0)
                    {
                        query = query.Where(s => (s.C_category_id ?? 0) == categoryId.Value);
                    }
                    list = query
                        .OrderBy(s => s.C_id)
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
    }
}
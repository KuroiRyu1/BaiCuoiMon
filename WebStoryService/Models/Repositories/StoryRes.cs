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
                var query = en.tbl_story
                       .Include("tbl_author")
                       .Include("tbl_category")
                       .AsQueryable();
                if (categoryId.HasValue && categoryId.Value != 0)
                {
                    query = query.Where(s => (s.C_category_id ?? 0) == categoryId.Value);
                }
                var item = query.Select(s=>new Story
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
            try
            {
                using (var db = new DbEntities())
                {
                    return db.tbl_story
                        .Where(s => s.C_id == id)
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
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
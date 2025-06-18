using System;
using System.Linq;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class StoryRes
    {
        public System.Collections.Generic.List<Story> GetAll()
        {
            try
            {
                using (var db = new DbEntities())
                {
                    var stories = db.tbl_story
                        .Where(s => s.C_active == 1) // Chỉ lấy truyện active
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
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Retrieved {stories.Count} active stories from database.");
                    return stories;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAll: {ex.Message}");
                return new System.Collections.Generic.List<Story>();
            }
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

        public bool Delete(int id)
        {
            try
            {
                using (var db = new DbEntities())
                {
                    var story = db.tbl_story.FirstOrDefault(s => s.C_id == id);
                    if (story != null)
                    {
                        story.C_active = 0; // Xóa mềm bằng cách đặt _active = 0
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;

namespace WebStoryService.Models.Repositories
{
    public class FollowRep
    {
        DbEntities en = new DbEntities();
        public bool CheckFollowExist(Follow model)
        {
            return en.tbl_story_follow.Any(d => d.C_user_id == model.UserId && d.C_story_id == model.StoryId);
        }
        public int FollowStory(Follow model)
        {
            var tontai = en.tbl_story_follow.Any(d => d.C_story_id == model.StoryId && d.C_user_id == model.UserId);
            var FollowCount = en.tbl_story.Where(d => d.C_id == model.StoryId).FirstOrDefault();
            if (FollowCount.C_follow_number == null)
            {
                FollowCount.C_follow_number = 0;
            }
            if (tontai==true)
            {
                var q = en.tbl_story_follow.Where(d=>d.C_story_id==model.StoryId&&d.C_user_id==model.UserId).FirstOrDefault();
                //bo hoac them follow cũ
                if (q != null)
                {
                    if (q.C_status == 0)
                    {
                        q.C_status = 1;
                        FollowCount.C_follow_number++;
                        en.SaveChanges();
                        return 1;
                    }
                    else if(q.C_status == 1)
                    {
                        FollowCount.C_follow_number--;
                        q.C_status = 0;
                        en.SaveChanges();
                        return 2;
                    }
                    
                }
            }
            else
            {
                //follow truyen
                var newFollow = new tbl_story_follow
                {
                    C_user_id = model.UserId,
                    C_story_id = model.StoryId,
                    day_create = DateTime.Now,
                    C_status = 1,
                };
                en.tbl_story_follow.Add(newFollow);
                FollowCount.C_follow_number++;
                en.SaveChanges();
                return 1;
            }
            return 0;
        }
        public bool CheckFollow(int storyId=0,int userId=0)
        {
            try
            {
                if(storyId != 0&&userId!=0)
                {
                    var item = en.tbl_story_follow.Any(d => d.C_story_id == storyId && d.C_user_id == userId&&d.C_status==1);
                    return item;
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        public List<Story> GetFollowedStories(int userId)
        {
            try
            {
                var stories = (from f in en.tbl_story_follow
                               join s in en.tbl_story on f.C_story_id equals s.C_id
                               where f.C_user_id == userId && f.C_status == 1
                               select new Story
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
                return stories;
            }
            catch (Exception ex)
            {
            }

            return new List<Story>();
        }
    }
}
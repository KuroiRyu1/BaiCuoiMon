using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using WebStoryService.Models.Entities;
using WebStoryService.Models.ModelData;
using WebStoryService.Models.Repositories;

namespace WebStoryService.Areas.MyApi.Controllers
{
    [RoutePrefix("api/chapters")]
    public class ChapterApiController : ApiController
    {
        private readonly DbEntities _db = new DbEntities();

        [HttpGet]
        [Route("{storyId}")]
        public IHttpActionResult GetByStory(int storyId)
        {
            var chapters = _db.tbl_chapter
                            .Where(c => c.C_story_id == storyId)
                            .Select(c => new Chapter
                            {
                                Id = c.C_id,
                                Title = c.C_title,
                                Content = c.C_content,
                                StoryId = storyId,
                                ChapterIndex = (int)c.C_chapter_index,
                            })
                            .ToList();
            return Ok(chapters);
        }
        [HttpGet]
        [Route("{storyId}/{chapterIndex}")]
        public IHttpActionResult ReadStory(int storyId, int chapterIndex)
        {
            var chapters = new Chapter();
            ChapterRes res = new ChapterRes();
            chapters = res.StoryRead(storyId, chapterIndex);
            return Ok(chapters);
        }

        [HttpGet]
        [Route("single/{chapterId}")]
        public IHttpActionResult GetSingle(int chapterId)
        {
            try
            {
                var chapter = _db.tbl_chapter.Find(chapterId);
                if (chapter == null) return NotFound();
                return Ok(chapter);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] tbl_chapter chapter)
        {
            try
            {
                if (!ModelState.IsValid || chapter.C_story_id == 0)
                    return BadRequest(ModelState);

                if (!_db.tbl_story.Any(s => s.C_id == chapter.C_story_id))
                    return NotFound();

                chapter.C_day_create = DateTime.Now;
                _db.tbl_chapter.Add(chapter);
                _db.SaveChanges();

                return Ok(chapter.C_id);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{chapterId}")]
        public IHttpActionResult Update(int chapterId, [FromBody] tbl_chapter data)
        {
            try
            {
                var chapter = _db.tbl_chapter.Find(chapterId);
                if (chapter == null) return NotFound();

                chapter.C_title = data.C_title;
                chapter.C_content = data.C_content;
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{chapterId}")]
        public IHttpActionResult Delete(int chapterId)
        {
            try
            {
                var chapter = _db.tbl_chapter.Find(chapterId);
                if (chapter == null) return NotFound();

                var images = _db.tbl_chapter_image
                               .Where(i => i.C_chapter_id == chapterId)
                               .ToList();
                _db.tbl_chapter_image.RemoveRange(images);

                _db.tbl_chapter.Remove(chapter);
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
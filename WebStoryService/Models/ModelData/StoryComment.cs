using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStoryService.Models.ModelData
{
    public class StoryComment
    {
        public long Id { get; set; } = 0;
        public string Content { get; set; } = "";
        public int StoryId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public int Active { get; set; } = 0;
    }
}
﻿using System;
using System.Collections.Generic;

namespace WebStoryService.Models.ModelData
{
    public class Story
    {
        public int Id { get; set; } = 0;
        public string Title { get; set; } = "";
        public int ChapterNumber { get; set; } = 0;
        public string Introduction { get; set; } = "";
        public string Image { get; set; } = "default.jpg";
        public int LikeNumber { get; set; } = 0;
        public int FollowNumber { get; set; } = 0;
        public decimal ViewNumber { get; set; } = 0;
        public int AuthorId { get; set; } = 0;
        public int StatusId { get; set; } = 0;
        public int CategoryId { get; set; } = 0;
        public int StoryTypeId { get; set; } = 0;
        public string AuthorName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
        public int Active {  get; set; }=1;
    }
}
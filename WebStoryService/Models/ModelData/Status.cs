using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStoryService.Models.ModelData
{
    public class Status
    {
        public int Id { get; set; } = 0;
        public string Title { get; set; } = "";
        public int Active {  get; set; } = 0;
    }
}
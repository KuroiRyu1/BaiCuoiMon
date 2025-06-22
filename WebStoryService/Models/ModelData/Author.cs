using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStoryService.Models.ModelData
{
    public class Author
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Information { get; set; } = "";
        public string Image { get; set; } = "";
    }
}
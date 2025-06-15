using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace StoryWeb.Models.Repositories
{
    public sealed class ChapterImageRes
    {
        private ChapterImageRes() { }
        private static ChapterImageRes _instance = new ChapterImageRes();
        public static ChapterImageRes Instance {  
            get { 
                if (_instance == null)
                {
                    _instance = new ChapterImageRes();
                }
                return _instance; 
            } 
        }
        public async Task<List<ChapterImage>> getChapterImage(int id) {
            var chapterImage = new List<ChapterImage>();
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.GetAsync($"api/chapter-images/images/{id}");
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                    chapterImage = JsonConvert.DeserializeObject<List<ChapterImage>>(dataJson);
                }
                return chapterImage;
            }
            catch (Exception ex)
            {
            }
            return chapterImage;
        }
    }
}
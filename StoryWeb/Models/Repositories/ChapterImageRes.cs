using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace StoryWeb.Models.Repositories
{
    public sealed class ChapterImageRes
    {
        private ChapterImageRes() { }
        private static ChapterImageRes _instance = new ChapterImageRes();
        public static ChapterImageRes Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChapterImageRes();
                }
                return _instance;
            }
        }
        public async Task<List<ChapterImage>> getChapterImage(int chapterId)
        {
            var chapterImage = new List<ChapterImage>();
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.GetAsync($"api/chapter-images/{chapterId}");
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                    chapterImage = JsonConvert.DeserializeObject<List<ChapterImage>>(dataJson);
                }
                return chapterImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in getChapterImage: {ex.Message}");
            }
            return chapterImage;
        }

        public async Task<bool> UpdateImagePaths(List<ChapterImage> images)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = new StringContent(JsonConvert.SerializeObject(images), Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PutAsync("api/chapter-images/update-paths", content);
                if (res.IsSuccessStatusCode)
                {
                    return true;
                }
                System.Diagnostics.Debug.WriteLine($"UpdateImagePaths failed: {await res.Content.ReadAsStringAsync()}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateImagePaths: {ex.Message}");
                return false;
            }
        }
    }
}
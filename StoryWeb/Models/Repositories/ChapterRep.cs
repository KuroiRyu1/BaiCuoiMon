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
    public sealed class ChapterRep
    {
        private ChapterRep() { }
        private static ChapterRep _instance = null;
        public static ChapterRep Instance {  
            get {
                if (_instance == null)
                {
                    _instance = new ChapterRep();
                }
                return _instance; 
            } 
        }
        public async Task<Chapter> GetOneChapter(int id=0)
        {
            var chapter = new Chapter();
            try
            {
                if (id != 0)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(base_address.Address);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    HttpResponseMessage res = await client.GetAsync($"chapter/single/{id}");
                }
            }
            catch
            (Exception ex){
            }
            return chapter;
        }
        public async Task<Chapter> GetChapterDetail(string name)
        {
            Chapter chapter = new Chapter();
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.GetAsync("chapter/get");
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                    chapter = JsonConvert.DeserializeObject<Chapter>(dataJson);
                }
                return chapter;
            }
            catch (Exception ex)
            {
            }
            return chapter;
        }
        public async Task<List<Chapter>> getListOfChapter(int storyId)
        {
            List<Chapter> chapters = new List<Chapter>();
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.GetAsync($"api/chapters/{storyId}");
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                    chapters = JsonConvert.DeserializeObject<List<Chapter>>(dataJson);
                }
                return chapters;
            }
            catch (Exception ex)
            {
            }
            return chapters;
        }
        public async Task<Chapter> Read(int storyId,int chapterIndex)
        {
           var chapters = new Chapter();
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.GetAsync($"api/chapters/{storyId}/{chapterIndex}");
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                    chapters = JsonConvert.DeserializeObject<Chapter>(dataJson);
                }
                return chapters;
            }
            catch (Exception ex)
            {
            }
            return chapters;
        }

    }
}
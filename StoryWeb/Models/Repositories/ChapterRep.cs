using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StoryWeb.Models.Repositories
{
    public class ChapterRep
    {
        private static ChapterRep _instance;
        private ChapterRep() { }

        public static ChapterRep Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChapterRep();
                }
                return _instance;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8078/")
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("pwd", "123");
            client.DefaultRequestHeaders.Add("tk", "12345");
            return client;
        }

        public async Task<List<Chapter>> GetChaptersByStoryId(int storyId)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var response = await client.GetAsync($"api/chapters/{storyId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var dataJson = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<Chapter>>(dataJson);
                    }
                }
            }
            catch (Exception)
            {
                return new List<Chapter>();
            }
            return new List<Chapter>();
        }
    }
}
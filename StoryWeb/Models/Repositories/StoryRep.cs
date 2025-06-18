using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StoryWeb.Models.Repositories
{
    public class StoryRep
    {
        private static StoryRep _instance;
        private StoryRep() { }

        public static StoryRep Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StoryRep();
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
            return client;
        }

        public async Task<List<Story>> GetAllStories()
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var response = await client.GetAsync("story/getall");
                    if (response.IsSuccessStatusCode)
                    {
                        var dataJson = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<Story>>(dataJson);
                    }
                }
            }
            catch (Exception)
            {
                return new List<Story>();
            }
            return new List<Story>();
        }
    }
}
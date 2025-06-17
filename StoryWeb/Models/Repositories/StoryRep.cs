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

        public async Task<List<Story>> GetAllStories(int? categoryId = null)
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
        public async Task<List<Story>> GetStories(int? categoryId = null, int page = 1, int pageSize = 10)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                string url = $"story/get?page={page}&pageSize={pageSize}";
                if (categoryId.HasValue && categoryId.Value != 0)
                {
                    url += $"&categoryId={categoryId.Value}";
                }
                HttpResponseMessage res = await client.GetAsync(url);
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = await res.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Story>>(dataJson);
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return new List<Story>();
        }


        public async Task<Story> GetStoryById(int id)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var response = await client.GetAsync($"story/get/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var dataJson = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<Story>(dataJson);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
    }
}
using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

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
        public async Task<List<Story>> GetAllStories(int? cateId=null)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.GetAsync($"story/getall/{cateId}");
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<Story>>(dataJson);
                }
            }
            catch (Exception ex)
            {
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
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.GetAsync($"story/get/{id}");
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = await res.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Story>(dataJson);
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return null;
        }

        public async Task<int> AddStory(Story item)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpContent content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync("story/post", content);
                if (res.IsSuccessStatusCode)
                {
                    var responseJson = await res.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<dynamic>(responseJson);
                    return response.Id ?? 0;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return 0;
        }

        public async Task<int> UpdateStory(Story item)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpContent content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PutAsync($"story/put/{item.Id}", content);
                if (res.IsSuccessStatusCode)
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return 0;
        }

        public async Task<int> DeleteStory(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.DeleteAsync($"story/delete/{id}");
                if (res.IsSuccessStatusCode)
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return 0;
        }

        public async Task<int> IncrementView(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.PostAsync($"story/increment-view/{id}", null);
                if (res.IsSuccessStatusCode)
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return 0;
        }

        public async Task<List<Story>> SearchStories(string keyword = "", int? categoryId = null)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                string url = $"story/search?keyword={Uri.EscapeDataString(keyword)}";
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
    }
}
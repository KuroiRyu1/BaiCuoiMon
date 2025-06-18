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

        public async Task<List<Story>> GetStories(int? categoryId = null, int page = 1, int pageSize = 10)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");
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
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");
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
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");
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
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");
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
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");
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

        public async Task<int> FollowStoryAsync(long userId, int storyId)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");

                var body = new { UserId = userId, StoryId = storyId };
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                HttpResponseMessage res = await client.PostAsync("api/follow/story", content);
                if (res.IsSuccessStatusCode)
                {
                    return 1; // Theo dõi thành công
                }
            }
            catch (Exception ex)
            {
                // Log nếu cần: ex.Message
            }
            return 0; // Thất bại
        }

        public async Task<int> UnfollowStoryAsync(long userId, int storyId)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");

                var body = new { UserId = userId, StoryId = storyId };
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                HttpResponseMessage res = await client.PostAsync("api/follow/story/unfollow", content);
                if (res.IsSuccessStatusCode)
                {
                    return 1; // Theo dõi thành công
                }
            }
            catch (Exception ex)
            {
                // Log nếu cần: ex.Message
            }
            return 0; // Thất bại
        }

        public async Task<bool> CheckIsFollowingAsync(long userId, int storyId)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");

                string url = $"api/follow/story/user/{userId}";
                HttpResponseMessage res = await client.GetAsync(url);

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var stories = JsonConvert.DeserializeObject<List<dynamic>>(json);

                    // Kiểm tra xem có tồn tại truyện đang xem trong danh sách đã theo dõi
                    return stories.Any(s => (int)s.C_id == storyId);
                }
            }
            catch (Exception ex)
            {
                // Log nếu cần: ex.Message
            }

            return false;
        }

    }
}
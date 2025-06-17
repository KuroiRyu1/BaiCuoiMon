using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using StoryWeb.Models.ModelView;

namespace StoryWeb.Models.Repositories
{
    public class CommentRep
    {
        private CommentRep() { }
        private static CommentRep _instance = null;
        public static CommentRep Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommentRep();
                }
                return _instance;
            }
        }
        public async Task<int> AddStoryCommentAsync(StoryComment comment)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");

                var content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");

                HttpResponseMessage res = await client.PostAsync("api/comment/story", content);
                if (res.IsSuccessStatusCode)
                {
                    return 1; // Gửi thành công
                }
            }
            catch (Exception ex)
            {
                // Log nếu cần
            }
            return 0;
        }

        public async Task<List<StoryComment>> GetStoryCommentsAsync(int storyId)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");

                HttpResponseMessage res = await client.GetAsync($"api/comment/story/{storyId}");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<StoryComment>>(json);
                }
            }
            catch (Exception ex)
            {
                // Log nếu cần
            }

            return new List<StoryComment>();
        }
    }
}
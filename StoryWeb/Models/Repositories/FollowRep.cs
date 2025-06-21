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
    public sealed class FollowRep
    {
        private static FollowRep _instance=null;
        private FollowRep() { }
        public static FollowRep Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FollowRep();
                }
                return _instance;
            }
        }
        public async Task<int> FollowOrUnfollow(int storyId,int userId=0)

        {
            try
            {
                Follow follow = new Follow
                {
                    StoryId = storyId,
                    UserId = userId
                };
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpContent content = new StringContent(JsonConvert.SerializeObject(follow), Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync("follow/story",content);
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                   var result = JsonConvert.DeserializeObject<int>(dataJson);
                    return result;
                }
            }
            catch (Exception e)
            {
            }

            return 0;
        }
        public async Task<bool> checkFollow(int storyId=0,int userId=0)

        {
            try
            {
                Follow follow = new Follow
                {
                    StoryId = storyId,
                    UserId = userId
                };
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpContent content = new StringContent(JsonConvert.SerializeObject(follow), Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync("follow/check", content);
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<bool>(dataJson);
                    return result;
                }
            }
            catch (Exception e)
            {

            }

            return false;
        }
        public async Task<List<Story>> getFollowStory(int userId=0)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpContent content = new StringContent(JsonConvert.SerializeObject(userId), Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.GetAsync($"follow/story/user/{userId}");
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<List<Story>>(dataJson);
                }
            }
            catch (Exception e)
            {
            }
            return new List<Story>();
        }

    }
}
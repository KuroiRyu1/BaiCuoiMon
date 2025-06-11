using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace StoryWeb.Models.Repositories
{
    public class UserRep
    {
        public UserRep() { }
        public static UserRep _instance = null;
        public static UserRep Instance {  
            get {
                
                if (_instance == null)
                {
                    _instance = new UserRep();
                }
                return _instance; 
            } 
        }
        public async Task<User> login(string username, string password)
        {
            User user = new User();
            user.Username = username;
            user.Password = password;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8078");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("pwd", "123");
            client.DefaultRequestHeaders.Add("tk", "12345");
            HttpContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage res = await client.PostAsync("user/login",content);
            if (res.IsSuccessStatusCode)
            {
                return user;
            }
            return user;
        }
        public async Task<int> Register(User user)
        {
            int result = 0;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8078");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("pwd", "123");
            client.DefaultRequestHeaders.Add("tk", "12345");
            HttpContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage res = await client.PostAsync("user/login", content);
            if (res.IsSuccessStatusCode)
            {
                var dataJson = res.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<int>(dataJson);
            }
            return result;
        }
        public async Task<List<User>> GetUser()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:8078");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("username", "admin");
                client.DefaultRequestHeaders.Add("pwd", "123");
                client.DefaultRequestHeaders.Add("tk", "12345");
                string url = $"user/list";
              
                HttpResponseMessage res = await client.GetAsync(url);
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = await res.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<User>>(dataJson);
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            return new List<User>();
        }

    }
}
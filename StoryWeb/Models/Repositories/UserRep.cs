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
        private static UserRep _instance;
        private UserRep() { }

        public static UserRep Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserRep();
                }
                return _instance;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8078");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("pwd", "123");
            client.DefaultRequestHeaders.Add("tk", "12345");
            return client;
        }

        public async Task<User> Login(string username, string password)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var loginData = new { Username = username, Password = password };
                    var content = new StringContent(
                        JsonConvert.SerializeObject(loginData),
                        Encoding.UTF8,
                        "application/json");

                    var response = await client.PostAsync("user/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var dataJson = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<User>(dataJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> Register(User user)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var content = new StringContent(
                        JsonConvert.SerializeObject(user),
                        Encoding.UTF8,
                        "application/json");

                    var response = await client.PostAsync("user/register", content);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex.Message}");
                return false;
            }
        }

        public async Task<User> GetUserById(int id)
        {
            try
            {
                using (var client = CreateHttpClient())
                {
                    var response = await client.GetAsync($"user/get/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var dataJson = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<User>(dataJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get user error: {ex.Message}");
            }
            return null;
        }
    }
}
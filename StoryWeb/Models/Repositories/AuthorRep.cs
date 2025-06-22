using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StoryWeb.Models.Repositories
{
    public class AuthorRep
    {
        private static AuthorRep _instance;
        private AuthorRep() { }

        public static AuthorRep Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthorRep();
                }
                return _instance;
            }
        }

        public async Task<List<Author>> GetAuthors()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8078/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    var response = await client.GetAsync("api/authors/get");
                    if (response.IsSuccessStatusCode)
                    {
                        var dataJson = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<Author>>(dataJson);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAuthors: {ex.Message}");
            }
            return new List<Author>();
        }
    }
}
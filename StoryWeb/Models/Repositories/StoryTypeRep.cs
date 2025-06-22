using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StoryWeb.Models.Repositories
{
    public class StoryTypeRep
    {
        private static StoryTypeRep _instance;
        private StoryTypeRep() { }

        public static StoryTypeRep Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StoryTypeRep();
                }
                return _instance;
            }
        }

        public async Task<List<StoryType>> GetStoryTypes()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8078/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    var response = await client.GetAsync("api/story-types/get");
                    if (response.IsSuccessStatusCode)
                    {
                        var dataJson = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<StoryType>>(dataJson);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetStoryTypes: {ex.Message}");
            }
            return new List<StoryType>();
        }
    }
}
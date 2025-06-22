using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace StoryWeb.Models.Repositories
{
    public class StoryTypeRep
    {
        private static StoryTypeRep _instance = null;
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
        public async Task<List<StoryType>> Get()
        {
            var storytype = new List<StoryType>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(base_address.Address);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage res = await client.GetAsync("storytype/get");
            if (res.IsSuccessStatusCode)
            {
                var dataJson = res.Content.ReadAsStringAsync().Result;
                storytype = JsonConvert.DeserializeObject<List<StoryType>>(dataJson);
            }
            return storytype;
        }
        public async Task<StoryType> GetById(int id)
        {
            var storytype = new StoryType();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(base_address.Address);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage res = await client.GetAsync($"storytype/get/{id}");
            if (res.IsSuccessStatusCode)
            {
                var dataJson = res.Content.ReadAsStringAsync().Result;
                storytype = JsonConvert.DeserializeObject<StoryType>(dataJson);
            }
            return storytype;
        }
    }
}
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
    public class StatusRep
    {
        private static StatusRep _instance=null;
        private StatusRep() { }
        public static StatusRep Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StatusRep();
                }
                return _instance;
            }
        }
        public async Task<List<Status>> getAllStatus()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(base_address.Address);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage res = await client.GetAsync("status/get");
                var status = new List<Status>();
                if (res.IsSuccessStatusCode)
                {
                    var dataJson = res.Content.ReadAsStringAsync().Result;
                    status = JsonConvert.DeserializeObject<List<Status>>(dataJson);
                }
                return status;
            }
            catch (Exception e)
            {
            }
            return new List<Status>();
        }
    }
}
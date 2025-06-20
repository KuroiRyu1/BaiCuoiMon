using Newtonsoft.Json;
using StoryWeb.Models.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Xml.Linq;

namespace StoryWeb.Models.Repositories
{
    public class CategoryRep
    {
        private static CategoryRep _instance;
        private CategoryRep() { }
        public static CategoryRep Instance
        {
            get{
                if (_instance == null)
                {
                    _instance = new CategoryRep();
                }
                return _instance;
            } 
        }
        public async Task<List<Category>> getCates()
        {
            var cates = new List<Category>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(base_address.Address);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("pwd", "123");
            client.DefaultRequestHeaders.Add("tk", "12345");
            HttpResponseMessage res = await client.GetAsync("category/get");
            if (res.IsSuccessStatusCode)
            {
                var dataJson = res.Content.ReadAsStringAsync().Result;
                cates = JsonConvert.DeserializeObject<List<Category>>(dataJson);
            }
            return cates;
        }
        public async Task<List<Category>> getCatesAdmin()
        {
            var cates = new List<Category>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(base_address.Address);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage res = await client.GetAsync("category/getadmin");
            if (res.IsSuccessStatusCode)
            {
                var dataJson = res.Content.ReadAsStringAsync().Result;
                cates = JsonConvert.DeserializeObject<List<Category>>(dataJson);
            }
            return cates;
        }
        public async Task<string> addCates(Category item)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(base_address.Address);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("username", "admin");
            client.DefaultRequestHeaders.Add("pwd", "123");
            client.DefaultRequestHeaders.Add("tk", "12345");
            HttpContent content = new StringContent(JsonConvert.SerializeObject(item),Encoding.UTF8,"application/json");
            HttpResponseMessage res = await client.PostAsync("category/post",content);
            string a = JsonConvert.SerializeObject(item);
            if (res.IsSuccessStatusCode)
            {
                return content.ToString();
            }
            return a;
        }
        public async Task<int> Edit(Category item)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(base_address.Address);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpContent content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            HttpResponseMessage res = await client.PutAsync("category/put", content);
            string a = JsonConvert.SerializeObject(item);
            if (res.IsSuccessStatusCode)
            {
                return 1;
            }
            return 0;
        }
        public async Task<int> Delete(Category item)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(base_address.Address);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpContent content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            HttpResponseMessage res = await client.PutAsync("category/soft", content);
            string a = JsonConvert.SerializeObject(item);
            if (res.IsSuccessStatusCode)
            {
                return 1;
            }
            return 0;
        }
        public async Task<List<Category>> Search(string name)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(base_address.Address);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpContent content = new StringContent(JsonConvert.SerializeObject(name), Encoding.UTF8, "application/json");
            HttpResponseMessage res = await client.PostAsync($"category/search/{name}", content);
            string a = JsonConvert.SerializeObject(name);
            if (res.IsSuccessStatusCode)
            {
                var dataJson = res.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<List<Category>>(dataJson);
            }
            return new List<Category>();
        }
        public async Task<Category> getById(int id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(base_address.Address);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage res = await client.GetAsync($"category/{id}");
            if (res.IsSuccessStatusCode)
            {
                var dataJson = res.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Category>(dataJson);
            }
            return new Category();
        }
    }
}
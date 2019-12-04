using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace HackerNews_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController : ControllerBase
    {
        string basePath = "https://hacker-news.firebaseio.com/v0/";
        List<HackerNew> lstHackerNews;
        List<string> lstIds;

        public HackerNewsController()
        {       
            lstHackerNews = new List<HackerNew>();     
            lstIds = new List<string>();
            var url = Path.Combine(basePath, "topstories.json");
            using(WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(Path.Combine(basePath, "topstories.json"));                 
                string[] split = json.Split(',');
                split[0] = split[0].Remove(0, 1);
                lstIds = split.ToList();
            }            
        }

        [HttpGet]
        public async Task<List<HackerNew>> Get()
        {
            HttpClient httpClient = new HttpClient();
            for(int i = 0; i < 100; i++)
            {
                var random = new Random();
                using(HttpResponseMessage itemResponse =  await httpClient.GetAsync(Path.Combine(basePath, "item", lstIds[i] + ".json")))
                using(HttpContent itemContent = itemResponse.Content)
                {
                    string item = await itemContent.ReadAsStringAsync();
                    var hnNew = JsonConvert.DeserializeObject<HackerNew>(item);
                    lstHackerNews.Add(hnNew);
                }
            }
            return lstHackerNews;
        }

        [HttpGet("{searchWord}")]
        public async Task<List<HackerNew>> Search(string searchWord)
        {
            if(!String.IsNullOrEmpty(searchWord) && searchWord != "undefined")
            {
                HttpClient httpClient  = new HttpClient();
                var random = new Random();
                using(HttpResponseMessage itemResponse =  await httpClient.GetAsync("http://hn.algolia.com/api/v1/search?query=" + searchWord + "&tags=story"))
                using(HttpContent itemContent = itemResponse.Content)
                {
                    string item = await itemContent.ReadAsStringAsync();
                    JObject temp = JObject.Parse(item);
                    int count1 = temp.Children().Children().Children().Count();
                    for (int i = 0; i < count1; i++)
                    {
                        HackerNew newHN = new HackerNew();
                        string tokenAuthor = "hits["+i+"].author";
                        string tokenTitle = "hits["+i+"].title";
                        newHN.By = (string)temp.SelectToken(tokenAuthor);
                        newHN.Title = (string)temp.SelectToken(tokenTitle);
                        lstHackerNews.Add(newHN);
                    }
                }
            }
            else
            {
                await Get();
            }
           return lstHackerNews;
        }
    }

    public class HackerNew
    {
        public string By { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
    }
}

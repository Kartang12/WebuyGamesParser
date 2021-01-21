using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebuyParser
{
    class GamesProcesser
    {
        //public async Task<List<Game>> GetGames(object locker, string country, string category, int firstRecord, ProxyProcesser proxyProcesser)
        //{
        //    return await Task.Run(() =>
        //    {
        //        string uri = $"https://wss2.cex.{country}.webuy.io/v3/boxes?categoryIds=[{category}]&firstRecord={firstRecord}&count=50&sortBy=boxname&sortOrder=asc";
        //        string unparsedList = string.Empty;

        //        try
        //        {
        //            Console.WriteLine($"Proxy request {country} {category} {firstRecord}");
        //            unparsedList = proxyProcesser.RequestViaProxy(uri, locker);
        //        }
        //        catch (WebException e)
        //        {
        //            throw e;
        //        }
                  
        //        try
        //        {
        //            var parsed = JObject.Parse(unparsedList);
        //            var a = parsed["response"]["data"]["boxes"];
        //            return JsonConvert.DeserializeObject<List<Game>>(a.ToString());
        //        }
        //        catch (InvalidOperationException e )
        //        {
        //            throw e;
        //        }
        //    });
        //}


        public async Task<int> GetGamesCount(string country, string category, object webLocker)
        {
            return await Task.Run(() =>
            {
                string uri = $"https://wss2.cex.{country}.webuy.io/v3/boxes?categoryIds=[{category}]&firstRecord=1&count=2&sortBy=boxname&sortOrder=asc";

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                if (webRequest == null)
                {
                    return 0;
                }

                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Nothing";
                StreamReader sr;
                lock (webLocker)
                {
                    var s = webRequest.GetResponse().GetResponseStream();
                    sr = new StreamReader(s);
                }
                var unparsedList = sr.ReadToEnd();
                var parsed = JObject.Parse(unparsedList);
                var a = parsed["response"]["data"]["totalRecords"];
                return int.Parse(a.ToString());
            });
        }
    }
}

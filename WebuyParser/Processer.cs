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
    class Processer
    {
        public async Task<List<Game>> GetGames(object locker, string country, string category, int firstRecord)
        {
            return await Task.Run(() =>
            {
                string uri = $"https://wss2.cex.{country}.webuy.io/v3/boxes?categoryIds=[{category}]&firstRecord={firstRecord}&count=50&sortBy=boxname&sortOrder=asc";

                var webRequest = WebRequest.Create(uri) as HttpWebRequest;
                if (webRequest == null)
                {
                    return null;
                }

                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Nothing";

                string unparsedList;
                while (true)
                {
                    try
                    {
                        lock (locker)
                        {
                            Thread.Sleep(100);
                            var s = webRequest.GetResponse().GetResponseStream();
                            var sr = new StreamReader(s);
                            unparsedList = sr.ReadToEnd();
                        }
                        break;
                    }
                    catch (WebException)
                    {
                        webRequest = WebRequest.Create(uri) as HttpWebRequest;
                        webRequest.ContentType = "application/json";
                        webRequest.UserAgent = "Nothing";
                    }
                }

                try
                {
                    var parsed = JObject.Parse(unparsedList);
                    var a = parsed["response"]["data"]["boxes"];
                    return JsonConvert.DeserializeObject<List<Game>>(a.ToString());
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
                   
                //Thread.Sleep(100);
                //using (var s = webRequest.GetResponse().GetResponseStream())
                //{
                //    using (var sr = new StreamReader(s))
                //    {
                //        var unparsedList = sr.ReadToEnd();
                //        var parsed = JObject.Parse(unparsedList);
                //        var a = parsed["response"]["data"]["boxes"];
                //        return JsonConvert.DeserializeObject<List<Game>>(a.ToString());
                //    }
                //}
            });
        }


        public async Task<int> GetGamesCount(string country, string category)
        {
            return await Task.Run(() =>
            {
                string uri = $"https://wss2.cex.{country}.webuy.io/v3/boxes?categoryIds=[{category}]&firstRecord=1&count=2&sortBy=boxname&sortOrder=asc";

                var webRequest = WebRequest.Create(uri) as HttpWebRequest;
                if (webRequest == null)
                {
                    return 0;
                }

                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Nothing";

                using (var s = webRequest.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        var unparsedList = sr.ReadToEnd();
                        var parsed = JObject.Parse(unparsedList);
                        var a = parsed["response"]["data"]["totalRecords"];
                        return int.Parse(a.ToString());
                    }
                }
            });
        }
    }
}

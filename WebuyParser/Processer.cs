using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebuyParser
{
    static class Processer
    {
        public static List<Game> GetGames(string country, string category, int firstRecord)
        {
            string uri = $"https://wss2.cex.{country}.webuy.io/v3/boxes?categoryIds=[{category}]&firstRecord={firstRecord}&count=50&sortBy=boxname&sortOrder=asc";

            var webRequest = WebRequest.Create(uri) as HttpWebRequest;
            if (webRequest == null)
            {
                return null;
            }

            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Nothing";

            //Seems like API can't process all requests in a row, so I added a delay
            Thread.Sleep(100); 
            using (var s = webRequest.GetResponse().GetResponseStream())
            {
                using (var sr = new StreamReader(s))
                {
                    try
                    {
                        var unparsedList = sr.ReadToEnd();
                        var parsed = JObject.Parse(unparsedList);
                        var a = parsed["response"]["data"]["boxes"];
                        return JsonConvert.DeserializeObject<List<Game>>(a.ToString());
                    }
                    catch (InvalidOperationException ex)
                    {
                        //Console.WriteLine("InvalidOperationException" + firstRecord);
                        try
                        {
                            //Thread.Sleep(100);
                            var unparsedList = sr.ReadToEnd();
                            var parsed = JObject.Parse(unparsedList);
                            var a = parsed["response"]["data"]["boxes"];
                            return JsonConvert.DeserializeObject<List<Game>>(a.ToString());
                        }
                        catch (JsonReaderException)
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static int GetGamesCount(string country, string category)
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
        }
    }
}

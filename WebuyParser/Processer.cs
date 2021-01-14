using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebuyParser
{
    class Processer
    {
        public List<Game> GetGames(string country, string category, int firstRecord)
        {
            string uri = $"https://wss2.cex.{country}.webuy.io/v3/boxes?categoryIds=[{category}]&firstRecord={firstRecord}&count=50&sortBy=boxname&sortOrder=asc";

            var webRequest = WebRequest.Create(uri) as HttpWebRequest;
            if (webRequest == null)
            {
                return null;
            }

            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Nothing";

            using (var s = webRequest.GetResponse().GetResponseStream())
            {
                using (var sr = new StreamReader(s))
                {
                    var unparsedList = sr.ReadToEnd();
                    var parsed = JObject.Parse(unparsedList);
                    var a = parsed["response"]["data"]["boxes"];
                    return JsonConvert.DeserializeObject<List<Game>>(a.ToString());

                }
            }
        }

        //public List<Game> GetPLGames(string category, int firstRecord)
        //{
        //    string uri = $"https://wss2.cex.pl.webuy.io/v3/boxes?categoryIds=[{category}]&firstRecord={firstRecord}&count=50&sortBy=boxname&sortOrder=asc";

        //    var webRequest = WebRequest.Create(uri) as HttpWebRequest;
        //    if (webRequest == null)
        //    {
        //        return null;
        //    }

        //    webRequest.ContentType = "application/json";
        //    webRequest.UserAgent = "Nothing";

        //    using (var s = webRequest.GetResponse().GetResponseStream())
        //    {
        //        using (var sr = new StreamReader(s))
        //        {
        //            var unparsedList = sr.ReadToEnd();
        //            var parsed = JObject.Parse(unparsedList);
        //            var a = parsed["response"]["data"]["boxes"];
        //            return JsonConvert.DeserializeObject<List<Game>>(a.ToString());
        //        }
        //    }
        //}
    }
}

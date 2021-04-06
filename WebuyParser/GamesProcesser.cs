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
                    catch (WebException e)
                    {
                        if (e.Message.Contains("403"))
                        {
                            Console.WriteLine("403 error server has temporarily");
                            Console.WriteLine("Connect to VPN and restart");
                            Console.ReadKey();
                        }
                        else
                        {
                            webRequest = WebRequest.Create(uri) as HttpWebRequest;
                            webRequest.ContentType = "application/json";
                            webRequest.UserAgent = "Nothing";
                        }
                    }
                }

                try
                {
                    var parsed = JObject.Parse(unparsedList);
                    var a = parsed["response"]["data"]["boxes"];
                    return JsonConvert.DeserializeObject<List<Game>>(a.ToString());
                }
                catch (InvalidOperationException e)
                {
                    string path = "\\reports\\LOG.txt";
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine("");
                        sw.WriteLine("InvalidOperationException");
                        sw.WriteLine(e.Message);
                    }
                    throw new Exception("Unable to parse");
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
                try
                {
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
                catch (Exception e)
                {
                    if (e.Message.Contains("403"))
                    {
                        Console.WriteLine("403 error server has temporarily banned your IP address");
                        Console.WriteLine("Connect to VPN and restart");
                        Console.ReadKey();
                        Environment.Exit(403);
                    }
                    throw;
                }

            });
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace WebuyParser
{
    internal class PlatformProcesser
    {
        public void GetGamesByPlatform( object webLocker, object fileLocker,  string platform,  string sellCountry, IEnumerable countries)
        {
            //string url1 = $"https://wss2.cex.{sellCountry}.webuy.io/v3/boxes?categoryIds=[{PlatformKeys.CountiesKeys[sellCountry][platform]}]&firstRecord=1&count=50&sortBy=boxname&sortOrder=asc";

            List<Game> GamesList = new List<Game>();
            GamesProcesser processer = new GamesProcesser();
            //ProxyProcesser proxyProcesser = new ProxyProcesser(webLocker);

            //loop to add price in sellCountry
            //try
            {
                Console.WriteLine($"Parsing  {sellCountry} - {platform}") ;
                int k = 1;
                int totlaRecords = processer.GetGamesCount(sellCountry, PlatformKeys.CountiesKeys[sellCountry][platform], webLocker).Result;

                while (k < totlaRecords)
                {
                    try
                    {
                        using (var client = WebConnector.CreateClient(webLocker))
                        {
                            while (k < totlaRecords)
                            {
                                Console.WriteLine($"{platform} {sellCountry} - {k}");
                                string url = $"https://wss2.cex.{sellCountry}.webuy.io/v3/boxes?categoryIds=[{PlatformKeys.CountiesKeys[sellCountry][platform]}]&firstRecord={k}&count=50&sortBy=boxname&sortOrder=asc";
                                string result;

                                var response = client.GetAsync(url).Result;
                                result = response.Content.ReadAsStringAsync().Result;
                            
                                if (result.Contains("429 Too Many Requests"))
                                {
                                    throw new WebException("429");
                                }

                                var parsed = JObject.Parse(result);
                                var a = parsed["response"]["data"]["boxes"];
                                GamesList.AddRange(JsonConvert.DeserializeObject<List<Game>>(a.ToString()));

                                k += 50;
                            }
                        }
                    }
                    catch (WebException e)
                    {
                        if (e.Message.Contains("429"))
                        {
                            Console.WriteLine("Error 429, retrieving new proxy");
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Console.WriteLine("Error in proxy request");
                            Console.WriteLine("you have 2 minutes to connect to another VPN");
                            Console.WriteLine("program will continue in 2 minutes");
                            Thread.Sleep(120000);
                        }
                    }
                }
            }

            GamesList.ForEach(x => x.SetSellPrice(sellCountry));

            //loop to get all games from other websites
            foreach (string country in countries)
            {
                if(country.StartsWith('#'))
                {
                    Console.WriteLine($"Skip country  {country}");
                    continue;
                }
                
                try
                {
                    Console.WriteLine($"Parsing {country} - {platform}");
                    int k = 1;
                    int totlaRecords = processer.GetGamesCount(country, PlatformKeys.CountiesKeys[country][platform], webLocker).Result;
                    while (k < totlaRecords)
                    {

                        try
                        {
                            using (var client = WebConnector.CreateClient(webLocker))
                            {
                                while (k < totlaRecords)
                                {
                                    Console.WriteLine($"{platform} {country} - {k}");
                                    string url = $"https://wss2.cex.{country}.webuy.io/v3/boxes?categoryIds=[{PlatformKeys.CountiesKeys[country][platform]}]&firstRecord={k}&count=50&sortBy=boxname&sortOrder=asc";

                                    HttpResponseMessage response;
                                    //lock (webLocker)
                                    //{
                                    Thread.Sleep(100);
                                    response = client.GetAsync(url).Result;

                                    //}
                                    var result = response.Content.ReadAsStringAsync().Result;

                                    if (result.Contains("429 Too Many Requests"))
                                    {
                                        throw new WebException("429");
                                    }

                                    var parsed = JObject.Parse(result);
                                    var a = parsed["response"]["data"]["boxes"];
                                    var temp = JsonConvert.DeserializeObject<List<Game>>(a.ToString());

                                    foreach (Game game in temp)
                                    {
                                        var t = GamesList.FindIndex(x => x.Name.Trim(' ').ToLower() == game.Name.Trim(' ').ToLower());
                                        if (t > -1)
                                            GamesList[t].SetPrice(game.SellPrice, country);
                                    }

                                    k += 50;
                                }
                            }
                        }
                        catch (WebException e)
                        {
                            if (e.Message.Contains("429"))
                            {
                                Console.WriteLine("Error 429, retrieving new proxy");
                            }
                            else
                            {
                                Console.WriteLine("Error in proxy request");
                                Console.WriteLine("you have 2 minutes to connect to another VPN");
                                Console.WriteLine("program will continue in 2 minutes");
                                Thread.Sleep(120000);
                            }
                        }
                    }
                }
                catch (InvalidOperationException)
                { }
            }

            GamesList.ForEach(game => game.CalculateProfit());
            GamesList = GamesList.OrderByDescending(x => x.Profit).ToList();

            lock(fileLocker)
            {
                ExcelWriter mapper = ExcelWriter.GetInstance();
                mapper.SaveFile(sellCountry.ToUpper() +"_report.xlsx", GamesList, platform);
                Console.WriteLine(platform + " results saved");
            }
        }

    }
}
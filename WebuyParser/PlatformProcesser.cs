using Ganss.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WebuyParser
{
    internal class PlatformProcesser
    {
        public void GetGamesByPlatform( object webLocker, object fileLocker,  string platform,  string sellCountry, IEnumerable countries)
        {
            List<Game> GamesList = new List<Game>();
            GamesProcesser processer = new GamesProcesser();
            ProxyProcesser proxyProcesser = new ProxyProcesser(webLocker);

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
                        List<Game> temp = processer.GetGames(webLocker, sellCountry, PlatformKeys.CountiesKeys[sellCountry][platform], k, proxyProcesser).Result;
                        GamesList.AddRange(temp);
                        k += 50;
                    }
                    catch (WebException e)
                    {
                        if (e.Message.Contains("429"))
                        {
                            Console.WriteLine($"changing proxy on {sellCountry} {PlatformKeys.CountiesKeys[sellCountry][platform]} {k}");
                            proxyProcesser = new ProxyProcesser(webLocker);
                        }
                        else
                        {
                            Console.WriteLine("Error in proxy request");
                            //throw;
                        }
                        proxyProcesser = new ProxyProcesser(webLocker);
                    }
                }
            }
            //catch (InvalidOperationException)
            //{ }


            //loop to get all games from UK website
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
                    int totlaRecords = processer.GetGamesCount(sellCountry, PlatformKeys.CountiesKeys[country][platform], webLocker).Result;
                    while (k < totlaRecords)
                    {
                        List<Game> temp = new List<Game>();
                        try
                        {
                            temp = processer.GetGames(webLocker, country, PlatformKeys.CountiesKeys[country][platform], k, proxyProcesser).Result;
                       
                            foreach (Game game in temp)
                            {
                                var t = GamesList.FindIndex(x => x.Name.Trim(' ').ToLower() == game.Name.Trim(' ').ToLower());
                                if (t > -1)
                                    GamesList[t].SetPrice(game.SellPrice, country);
                            }
                            k += 50;
                        }
                        catch (WebException e)
                        {
                            if (e.Message.Contains("429"))
                            {
                                Console.WriteLine($"changing proxy on {sellCountry} {PlatformKeys.CountiesKeys[sellCountry][platform]} {k}");
                                proxyProcesser = new ProxyProcesser(webLocker);
                            }
                            else
                            {
                                Console.WriteLine("Error in proxy request");
                            }
                            throw;
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
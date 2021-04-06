using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace WebuyParser
{
    internal class PlatformProcesser
    {
        public void GetGamesByPlatform(object webLocker, object fileLocker, string platform, string sellCountry, IEnumerable countries)
        {
            List<Game> GamesList = new List<Game>();
            GamesProcesser processer = new GamesProcesser();

            //loop to add price in PL
            try
            {
                int gamesAmount = processer.GetGamesCount(sellCountry, PlatformKeys.CountiesKeys[sellCountry][platform]).Result;
                Console.WriteLine($"Parsing  {sellCountry} - {platform}");
                int k = 1;
                while (k < gamesAmount)
                {
                    Console.WriteLine($"Parsing  {sellCountry} - {platform} - {k}");
                    List<Game> temp = processer.GetGames(webLocker, sellCountry, PlatformKeys.CountiesKeys[sellCountry][platform], k).Result;
                    if (temp == null)
                        break;
                    GamesList.AddRange(temp);
                    k += 50;
                }
            }
            catch (Exception e)
            {
                string path = @"\reports\LOG.txt";
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine($"{sellCountry} - {platform}");
                    sw.WriteLine($"GamesListSize = {GamesList.Count}");
                    sw.WriteLine(e.Message);
                }
            }

            GamesList.ForEach(x => x.SetBuyPrice(sellCountry));

            //loop to get all games from UK website
            foreach (string country in countries)
            {
                if (country.StartsWith('#'))
                {
                    Console.WriteLine("Skip country " + country);
                    continue;
                }

                try
                {
                    Console.WriteLine("Parsing " + country + " - " + platform);
                    int gamesAmount = processer.GetGamesCount(country, PlatformKeys.CountiesKeys[country][platform]).Result;

                    int i = 1;
                    while (i < gamesAmount)
                    {
                        Console.WriteLine($"Parsing  {country} - {platform} - {i}");
                        List<Game> temp = processer.GetGames(webLocker, country, PlatformKeys.CountiesKeys[country][platform], i).Result;
                        if (temp == null)
                            break;
                        foreach (Game game in temp)
                        {
                            var t = GamesList.FindIndex(x => x.Name.Trim(' ').ToLower() == game.Name.Trim(' ').ToLower());

                            if (t > -1)
                            {
                                GamesList[t].SetPrice(game.SellPrice, country);
                            }
                        }

                        i += 50;
                    }
                }
                catch (Exception e)
                {
                    string path = @"\reports\LOG.txt";
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine($"{sellCountry} - {platform}");
                        sw.WriteLine(e.Message);
                    }
                }
            }

            GamesList.ForEach(game => game.CalculateProfit());
            GamesList = GamesList.OrderByDescending(x => x.Profit).ToList();

            lock (fileLocker)
            {
                var reportsDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\reports";
                if(!Directory.Exists(reportsDirectory))
                    Directory.CreateDirectory(reportsDirectory);
                ExcelWriter mapper = ExcelWriter.GetInstance();
                mapper.SaveFile(reportsDirectory + "\\" +sellCountry.ToUpper() + "_report.xlsx", GamesList, platform);
                Console.WriteLine(platform + " results saved");
            }
        }

    }
}
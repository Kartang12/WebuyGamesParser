using Ganss.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebuyParser
{
    internal class PlatformProcesser
    {
        public void GetGamesByPlatform( object webLocker, object fileLocker,  string platform,  IEnumerable countries)
        {
            List<Game> GamesList = new List<Game>();
            Processer processer = new Processer();

            //loop to add price in PL
            try
            {
                Console.WriteLine("Parsing PL - " + platform);
                int k = 1;
                while (true)
                {
                    List<Game> temp = processer.GetGames(webLocker, "pl", PlatformKeys.CountiesKeys["pl"][platform], k).Result;
                    if (temp == null)
                        break;
                    GamesList.AddRange(temp);
                    k += 50;

                    #region
                    //foreach (Game game in temp)
                    //{
                    //    var t = GamesList.FirstOrDefault(x => x.Name == game.Name);
                    //    if (t != null)
                    //    {
                    //        t.UKSellPrice *= CurrencyConverter.rate;
                    //        t.PLBuyPrice = game.PLBuyPrice;
                    //    }
                    //}
                    #endregion
                }
            }
            catch (InvalidOperationException)
            { }
            
            //loop to get all games from UK website
            foreach(string country in countries)
            {
                if(country.StartsWith('#'))
                {
                    Console.WriteLine("Skip country " + country);
                    continue;
                }
                    try
                {
                    Console.WriteLine("Parsing "+ country + " - " + platform);
                    int i = 1;
                        while (true)
                        {
                            List<Game> temp = processer.GetGames(webLocker, country, PlatformKeys.CountiesKeys[country][platform], i).Result;
                            if (temp == null)
                                break;
                            foreach (Game game in temp)
                            {
                                var t = GamesList.FindIndex(x => x.Name == game.Name);

                                if (t > -1)
                                {
                                    GamesList[t].SetPrice(game.SellPrice, country);
                                    #region
                                    //if (country == "uk")
                                    //    t.UKSellPrice *= CurrencyConverter.poundRate;
                                    //else
                                    ////t.
                                    ////t.UKSellPrice *= CurrencyConverter.rate;
                                    //t.PLBuyPrice = game.PLBuyPrice;
                                    #endregion
                                }
                            }

                            i += 50;
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
                mapper.SaveFile("report.xlsx", GamesList, platform);
                Console.WriteLine(platform + " results saved");
            }
        }
    }
}
using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebuyParser
{
    class Program
    {
        public static Dictionary<string, string> platforms = new Dictionary<string, string>()
        {
            {"PS3", "808" },
            {"PS4", "1003" },
            {"XBox360", "782" },
            {"XBoxOne", "1000" }
        };
          
        static void Main(string[] args)
        {
            CurrencyConverter.GetIndex();

            List<Game> PS3Games = new List<Game>();
            List<Game> PS4Games = new List<Game>();
            List<Game> XBox360Games = new List<Game>();
            List<Game> XBoxOneGames = new List<Game>();

            Console.WriteLine("Starting");

            GetGamesByPlatform(ref PS3Games, platforms["PS3"]);
            //GetGamesByPlatform(ref PS4Games, platforms["PS4"]);
            //GetGamesByPlatform(ref XBox360Games, platforms["XBox360"]);
            //GetGamesByPlatform(ref XBoxOneGames, platforms["XBoxOne"]);

            ExcelMapper mapper = new ExcelMapper();
            mapper.Save("report.xlsx",  PS3Games, "PS 3", true);
            //mapper.Save("report.xlsx", PS4Games, "PS 4", true);
            //mapper.Save("report.xlsx", XBox360Games, "XBox 360", true);
            //mapper.Save("report.xlsx", XBoxOneGames, "XBox One", true);

            Console.WriteLine("Push any button");
            Console.ReadKey();
        }

        static void GetGamesByPlatform(ref List<Game> GamesList, string platform)
        {
            //loop to get all games from UK website
            Console.WriteLine("Parsing " + platform + "platform");
            try
            {
                int i = 1;
                int range = Processer.GetGamesCount("uk", platform);
                while (i < range)
                {
                    
                    List<Game> temp = Processer.GetGames("uk", platform, i);
                    if (temp == null)
                    {
                        Console.WriteLine("Parsing " + platform + "\t" + i);
                        continue;
                    }
                    GamesList.AddRange(temp);
                    
                    i += 50;
                }
            }
            catch (InvalidOperationException ex)
            { }

            //loop to add price in PL and calculate profit
            try
            {
                while (true)
                {
                    int k = 1;
                    List<Game> temp = Processer.GetGames("pl", platform, k);
                    
                    if (temp == null)
                    {
                        Console.WriteLine(platform + "\t" + k);
                        continue;
                    }

                    foreach (Game game in temp)
                    {
                        var t = GamesList.FirstOrDefault(x => x.Name == game.Name);

                        if (t != null)
                        {
                            t.UKSellPrice = Math.Round((t.UKSellPrice * CurrencyConverter.rate), 2);
                            t.PLBuyPrice = Math.Round(game.PLBuyPrice, 2);
                        }
                    }

                    k += 50;
                }
            }
            catch (InvalidOperationException ex)
            { }

            GamesList.ForEach(game => game.CalculateProfit());
            GamesList = GamesList.OrderByDescending(x => x.Profit).ToList();
        }
    }
}

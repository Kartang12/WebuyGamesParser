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


            GetGamesByPlatform(ref PS3Games, platforms["PS3"]);
            GetGamesByPlatform(ref PS4Games, platforms["PS4"]);
            GetGamesByPlatform(ref XBox360Games, platforms["XBox360"]);
            GetGamesByPlatform(ref XBoxOneGames, platforms["XBoxOne"]);


            Console.WriteLine("Saving results...");
            
            ExcelMapper mapper = new ExcelMapper();
            mapper.Save("report.xlsx",  PS3Games, "PS 3", true);
            mapper.Save("report.xlsx", PS4Games, "PS 4", true);
            mapper.Save("report.xlsx", XBox360Games, "XBox 360", true);
            mapper.Save("report.xlsx", XBoxOneGames, "XBox One", true);
           
            Console.WriteLine("Complete!");
        }

        static void GetGamesByPlatform(ref List<Game> GamesList, string platform)
        {
            Console.WriteLine("Parsing "+platform);
            //loop to get all games from UK website
            try
            {
                int i = 1;
                while (true)
                {
                    List<Game> temp = Processer.GetGames("uk", platform, i);
                    temp.ForEach(x => x.PLBuyPrice = -10000);
                    GamesList.AddRange(temp);
                    i += 50;
                }
            }
            catch (InvalidOperationException ex)
            { }

            //loop to add price in PL and calculate profit
            Console.WriteLine("Compaaring prices " + platform);
            try
            {
                int k = 1;
                while (true)
                {
                    List<Game> temp = Processer.GetGames("pl", platform, k);

                    foreach (Game game in temp)
                    {
                        var t = GamesList.FirstOrDefault(x => x.Name == game.Name);

                        if (t != null)
                        {
                            t.UKSellPrice *= CurrencyConverter.rate;
                            t.PLBuyPrice = game.PLBuyPrice;
                        }
                    }

                    k += 50;
                }
            }
            catch (InvalidOperationException ex)
            { }

            GamesList.ForEach(game =>game.CalculateProfit());

            GamesList = GamesList.OrderByDescending(x => x.Profit).ToList();
        }
    }
}

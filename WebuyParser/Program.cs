using Ganss.Excel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebuyParser
{
    //public static class Platforms
    //{
    //    public static string PS3 = "808";
    //    public static string PS4 = "1003";
    //    public static string XBox360 = "782";
    //    public static string XBoxOne = "1000";
    //}
    class Program
    {
        static Mutex mutexObj = new Mutex();
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


            //Parallel.Invoke(
            //    () => GetGamesByPlatform(ref PS3Games, platforms["PS3"]),
            //    () => GetGamesByPlatform(ref PS4Games, platforms["PS4"]),
            //    () => GetGamesByPlatform(ref XBox360Games, platforms["XBox360"]),
            //    () => GetGamesByPlatform(ref XBoxOneGames, platforms["XBoxOne"])
            //    );

            GetGamesByPlatform(PS3Games, platforms["PS3"]);
            GetGamesByPlatform(PS4Games, platforms["PS4"]);
            GetGamesByPlatform(XBox360Games, platforms["XBox360"]);
            GetGamesByPlatform(XBoxOneGames, platforms["XBoxOne"]);

            //GetGamesByPlatform(ref PS3Games, platforms["PS3"]);

            ExcelMapper mapper = new ExcelMapper();

            mapper.Save("report.xlsx",  PS3Games, "PS 3", true);
            mapper.Save("report.xlsx", PS4Games, "PS 4", true);
            mapper.Save("report.xlsx", XBox360Games, "XBox 360", true);
            mapper.Save("report.xlsx", XBoxOneGames, "XBox One", true);

            //ExcelWriter.WriteCSV<Game>(PS3Games);

            //GamesList = GamesList.OrderByDescending(x => x.Profit).ToList();
        }


        void GetGamesByPlatform(List<Game> GamesList, string platform)
        {
            Processer processer = new Processer();
            //loop to get all games from UK website
            #region
            //try
            //{
            //    int i = 1;
            //    while (true)
            //    {
            //        List<Game> temp = processer.GetGames("uk", platform, i).Result;
            //        GamesList.AddRange(temp);
            //        ConcurrentBag<Game> a = new ConcurrentBag<Game>();

            //        i += 50;
            //    }
            //}
            //catch (InvalidOperationException ex)
            //{ }
            #endregion
            try
            {
                int i = 1;
                while (true)
                {
                    List<Game> temp = processer.GetGames("uk", platform, i).Result;
                    GamesList.AddRange(temp);
                    ConcurrentBag<Game> a = new ConcurrentBag<Game>();
                   
                    i += 50;
                }
            }
            catch (InvalidOperationException ex)
            { }

            //loop to add price in PL and calculate profit
            try
            {
                int k = 1;
                while (true)
                {
                    List<Game> temp = processer.GetGames("pl", platform, k).Result;

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

            GamesList.ForEach(f  =>  f.CalculateProfit());

            GamesList = GamesList.OrderByDescending(x => x.Profit).ToList();
        }
    }
}

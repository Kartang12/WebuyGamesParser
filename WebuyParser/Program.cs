using System;
using System.Collections.Generic;
using System.Linq;
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

            Processer processer = new Processer();

            Parallel.Invoke(
                () => GetGamesByPlatform(ref PS3Games, platforms["PS3"]),
                () => GetGamesByPlatform(ref PS4Games, platforms["PS4"]),
                () => GetGamesByPlatform(ref XBox360Games, platforms["XBox360"]),
                () => GetGamesByPlatform(ref XBoxOneGames, platforms["XBoxOne"])
                );
 

            ExcelWriter.WriteCSV<Game>(PS3Games);

        }

        static void GetGamesByPlatform(ref List<Game> GamesList, string platform)
        {
            Processer processer = new Processer();

            try
            {
                int i = 1;
                while (true)
                {
                    List<Game> temp = processer.GetGames("uk", platform, i);
                    GamesList.AddRange(temp);

                    i += 50;
                }
            }
            catch (InvalidOperationException ex)
            { }

            try
            {
                int i = 1;
                while (true)
                {
                    List<Game> temp = processer.GetGames("pl", platform, i);

                    foreach (Game game in temp)
                    {
                        GamesList.First(x => x.Name == game.Name).PLBuyPrice = game.PLBuyPrice;
                        game.GetProfit();
                    }
                    break;

                    i += 50;
                }
            }
            catch (InvalidOperationException ex)
            { }

        }
    }
}

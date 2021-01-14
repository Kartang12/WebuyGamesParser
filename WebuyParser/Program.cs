using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public static string[] platforms = {"808", "1003", "782", "1000"};
        static void Main(string[] args)
        {
            List<Game> PS3Games = new List<Game>();
            List<Game> PS4Games = new List<Game>();
            List<Game> XBox360Games = new List<Game>();
            List<Game> XBoxOneGames = new List<Game>();

            Processer processer = new Processer();

            GetGames(ref PS3Games,ref PS4Games, ref XBox360Games, ref XBoxOneGames);
            ExcelWriter.WriteCSV<Game>(PS3Games);

        }

        static void GetGames(
            ref List<Game> PS3List, 
            ref List<Game> PS4List, 
            ref List<Game> Xbox360List, 
            ref List<Game> XBoxOneList )
        {
            Processer processer = new Processer();
            
            foreach (string platform in platforms)
            {
                try
                {
                    int i = 1;
                    while (true)
                    {
                        List<Game> temp = processer.GetGames("uk", platform, i);
                        switch (platform)
                        {
                            case "808":
                                PS3List.AddRange(temp);
                                break;
                            case "1003":
                                PS4List.AddRange(temp);
                                break;
                            case "782":
                                Xbox360List.AddRange(temp);
                                break;
                            case "1000":
                                XBoxOneList.AddRange(temp);
                                break;
                        }
                        i += 50;
                    }
                }
                catch (InvalidOperationException ex)
                {}
                
            }


            foreach (string platform in platforms)
            {
                try
                {
                    int i = 1;
                    while (true)
                    {
                        List<Game> temp = processer.GetGames("pl", platform, i);
                        switch (platform)
                        {
                            case "808":
                                foreach (Game game in temp)
                                    PS3List.First(x => x.Name == game.Name).PLBuyPrice = game.PLBuyPrice;
                                break;
                            case "1003":
                                foreach (Game game in temp)
                                    PS4List.First(x => x.Name == game.Name).PLBuyPrice = game.PLBuyPrice;
                                break;
                            case "782":
                                foreach (Game game in temp)
                                    Xbox360List.First(x => x.Name == game.Name).PLBuyPrice = game.PLBuyPrice;
                                break;
                            case "1000":
                                foreach (Game game in temp)
                                    XBoxOneList.First(x => x.Name == game.Name).PLBuyPrice = game.PLBuyPrice;
                                break;
                        }
                        i += 50;
                    }
                }
                catch (InvalidOperationException ex)
                { }



            }
        }

    }
}

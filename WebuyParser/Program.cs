using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace WebuyParser
{

    class Program
    {
        static object webLocker = new object();
        static object fileLocker = new object();

        static void Main(string[] args)
        {
            while (true)
            {
                CurrencyConverter.GetIndexes();
                if(CurrencyConverter.euroRate == 0 || CurrencyConverter.poundRate == 0)
                {
                    Console.WriteLine("Press y to try again. Any other button to cancel...");
                    var c = Console.ReadKey();
                    if (c.KeyChar != 'y')
                        Environment.Exit(1);
                    else
                        continue;
                }
                break;
            }
            Console.WriteLine(CurrencyConverter.euroRate);
            Console.WriteLine(CurrencyConverter.poundRate);

            ConcurrentBag<string> countries = new ConcurrentBag<string>(File.ReadAllLines("settings/countries.txt").ToList());
            ConcurrentBag<string> platforms = new ConcurrentBag<string>(File.ReadAllLines("settings/platforms.txt").ToList());

            int allowedThreads = Environment.ProcessorCount;
            int threadCounter = 0;
            Console.WriteLine($"Number Of allowed threads: {allowedThreads}");

            foreach (string platform in platforms)
            {
                    if (platform.StartsWith('#'))
                    {
                        Console.WriteLine("Skip platform " + platform);
                        continue;
                    }

                while (true)
                {

                    if (threadCounter < allowedThreads)
                    {
                        Interlocked.Add(ref threadCounter, 1);
                        new Thread(() =>
                            {
                                new PlatformProcesser().GetGamesByPlatform(
                                webLocker,
                                fileLocker,
                                platform,
                                platformsTable[platform],
                                countries);
                                Interlocked.Add(ref threadCounter, -1);
                            }).Start();
                        Console.WriteLine($"Threads running {threadCounter}  of {allowedThreads}");
                        break;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}

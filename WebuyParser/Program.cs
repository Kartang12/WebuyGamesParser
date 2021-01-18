using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                //Connect to currency API and get exchange data
                CurrencyConverter.GetIndexes();
                //Let user retry if samething went wrong
                if(CurrencyConverter.euroRate == 0 || CurrencyConverter.poundRate == 0)
                {
                    Console.WriteLine("Press 'y' to try again. Any other button to cancel...");
                    var c = Console.ReadKey();
                    if (c.KeyChar != 'y')
                        Environment.Exit(1);
                    continue;
                }
                break;
            }
            Console.WriteLine("EUR excahnge rate " + CurrencyConverter.euroRate);
            Console.WriteLine("GBP excahnge rate " + CurrencyConverter.poundRate);
            Console.WriteLine();

            //get all strings from sellCountry.txt and find selected string (in must NOT start with # and must be only one string)
            List<string> temp = File.ReadAllLines("settings/sellCountry.txt").Where(x=> !x.StartsWith('#')).ToList<string>();
            //Message if 0 or more than one country selected
            if(temp.Count != 1)
            {
                Console.WriteLine("====================ERROR====================");
                Console.WriteLine("Only 1 country must be selected in <settings/sellCountry.txt> ");
                Console.WriteLine("leave only one line without # in the beginning");
                Console.WriteLine("Edit <settings/sellCountry.txt> and restart the program");
                Console.WriteLine("Push any button to cancel...");
                Console.ReadKey();
                Environment.Exit(1);
            }
            string sellCountry = temp[0];

            ConcurrentBag<string> countries = new ConcurrentBag<string>(File.ReadAllLines("settings/countries.txt").ToList());
            ConcurrentBag<string> platforms = new ConcurrentBag<string>(File.ReadAllLines("settings/platforms.txt").ToList());

            if (countries.Contains(sellCountry))
            {
                Console.WriteLine("====================ERROR====================");
                Console.WriteLine("You can't specify \"sell country\" in the list of \"seaarch countries\"");
                Console.WriteLine($"{sellCountry} is also selected in <settings/countries.txt>");
                Console.WriteLine("Please check files <settings/sellCountry.txt> and <settings/countries.txt> for conflicts and restart the program");
                Console.WriteLine("Push any button to cancel...");
                Console.ReadKey();
                Environment.Exit(1);
            }

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
                                sellCountry,
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

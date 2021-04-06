using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace WebuyParser
{
    //get actual exchange rate via this API and store to rate variable
    static class CurrencyConverter
    {
        public static double poundRate;
        public static double euroRate;

        public static void GetIndexes()
        {
            SetRates();
            //GetIndex("GBP");
            //GetIndex("EUR");
        }
        public static void GetIndex(string currency)
        {

            var webRequest = WebRequest.Create($"https://api.exchangeratesapi.io/latest?base={currency}&symbols=PLN") as HttpWebRequest;
            if (webRequest == null)
            {
                return;
            }

            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Nothing";
            try
            {
                using (var s = webRequest.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        var unparsed = sr.ReadToEnd();
                        var parsed = JObject.Parse(unparsed);
                        var a = parsed["rates"]["PLN"];
                        switch (currency)
                        {
                            case "GBP":
                                poundRate = double.Parse(a.ToString());
                                break;
                            case "EUR":
                                euroRate = double.Parse(a.ToString());
                                break;
                        }
                    }
                }
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Unable to ge currency exchange data");
                poundRate = 0;
                euroRate = 0;
            }
        }

        private static void SetRates()
        {
            List<string> temp = File.ReadAllLines("settings/rates.txt").Where(x => !x.StartsWith('#')).ToList<string>();
            poundRate = double.Parse(temp[0]);
            euroRate = double.Parse(temp[1]);
        }
    }
}

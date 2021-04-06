using Ganss.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebuyParser
{
    class Game
    {
        [JsonProperty("boxName")]
        public string Name { get; set; }

        [JsonProperty("categoryFriendlyName")]
        public string Platform { get; set; }

        [Ignore]
        [JsonProperty("sellPrice")]
        public double SellPrice { get; set; }
        [Column("UK")]
        public double UKSellPrice { get; set; }
        [Column("PT")]
        public double PTSellPrice { get; set; }
        [Column("IE")]
        public double IESellPrice { get; set; }
        [Column("IT")]
        public double ITSellPrice { get; set; }
        [Column("ES")]
        public double ESSellPrice { get; set; }
        [Column("NL")]
        public double NLSellPrice { get; set; }
        [Column("IC")]
        public double ICSellPrice { get; set; }
        [Column("PL")]
        public double PLSellPrice { get; set; }

        [JsonProperty("exchangePrice")]
        public double BuyPrice { get; set; }

        public double Profit { get; set; }

        public void CalculateProfit()
        {
            SellPrice = 0;
            List<double> prices = new List<double>();

            if (UKSellPrice > 0)
                prices.Add(UKSellPrice);
            if (PTSellPrice > 0)
                prices.Add(PTSellPrice);
            if (IESellPrice > 0)
                prices.Add(IESellPrice);
            if (ITSellPrice > 0)
                prices.Add(ITSellPrice);
            if (ESSellPrice > 0)
                prices.Add(ESSellPrice);
            if (NLSellPrice > 0)
                prices.Add(NLSellPrice);
            if (ICSellPrice > 0)
                prices.Add(ICSellPrice);
            if (PLSellPrice > 0)
                prices.Add(PLSellPrice);

            if (BuyPrice == -10000)
                Profit = BuyPrice;
            else
            {
                double min = 10000;
                foreach (var price in prices)
                {
                    if (price < min && price > 0)
                    {
                        min = price;
                    }
                }
                if (min > 0)
                    Profit = Math.Round(BuyPrice - min, 2);
            }
        }

        public void SetPrice(double price, string country)
        {
            switch (country)
            {
                case "uk":
                    UKSellPrice = Math.Round(price * CurrencyConverter.poundRate, 2);
                    break;
                case "pt":
                    PTSellPrice = Math.Round(price * CurrencyConverter.euroRate, 2);
                    break;
                case "ie":
                    IESellPrice = Math.Round(price * CurrencyConverter.euroRate, 2);
                    break;
                case "it":
                    ITSellPrice = Math.Round(price * CurrencyConverter.euroRate, 2);
                    break;
                case "es":
                    ESSellPrice = Math.Round(price * CurrencyConverter.euroRate, 2);
                    break;
                case "nl":
                    NLSellPrice = Math.Round(price * CurrencyConverter.euroRate, 2);
                    break;
                case "ic":
                    ICSellPrice = Math.Round(price * CurrencyConverter.euroRate, 2);
                    break;
                case "pl":
                    PLSellPrice = Math.Round(price, 2);
                    break;
            }
        }

        public void SetBuyPrice(string country)
        {
            switch (country)
            {
                case "uk":
                    BuyPrice = Math.Round(BuyPrice * CurrencyConverter.poundRate, 2);
                    break;
                case "pl":
                    BuyPrice = Math.Round(BuyPrice, 2);
                    break;
                default:
                    BuyPrice = Math.Round(BuyPrice * CurrencyConverter.euroRate, 2);
                    break;
            }
        }
    }
}
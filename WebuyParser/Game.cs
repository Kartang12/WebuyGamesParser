using Newtonsoft.Json;
using System;

namespace WebuyParser
{
    class Game
    {
        [JsonProperty("boxName")]
        public string Name { get; set; }

        [JsonProperty("categoryFriendlyName")]
        public string Platform { get; set; }

        [JsonProperty("sellPrice")]
        public double UKSellPrice { get; set; }

        [JsonProperty("exchangePrice")]
        public double PLBuyPrice { get; set; }

        public double Profit { get; set; }

        public void CalculateProfit()
        {
            if (PLBuyPrice == -10000)
                Profit = PLBuyPrice;
            else
                Profit = Math.Round(PLBuyPrice - UKSellPrice, 2);
        }
    }
}

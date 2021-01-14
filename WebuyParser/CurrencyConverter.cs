using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace WebuyParser
{
    //get actual exchange rate via this API and store to rate variable
    static class CurrencyConverter
    {
        static string apiString = "https://api.exchangeratesapi.io/latest?base=PLN&symbols=USD";

        public static double rate;

        public static void GetIndex()
        {
            var webRequest = WebRequest.Create(apiString) as HttpWebRequest;
            if (webRequest == null)
            {
                return;
            }

            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Nothing";

            using (var s = webRequest.GetResponse().GetResponseStream())
            {
                using (var sr = new StreamReader(s))
                {
                    var unparsed = sr.ReadToEnd();
                    var parsed = JObject.Parse(unparsed);
                    var a = parsed["rates"]["USD"];
                    rate = double.Parse(a.ToString());
                }
            }
        }
    }
}

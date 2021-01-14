using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebuyParser
{
    internal class Processer
    {
        public async Task<List<Game>> GetGames(string country, string category, int firstRecord)
        {
            
            return await Task.Run(() => 
            {
                string uri = $"https://wss2.cex.{country}.webuy.io/v3/boxes?categoryIds=[{category}]&firstRecord={firstRecord}&count=50&sortBy=boxname&sortOrder=asc";

                var webRequest = WebRequest.Create(uri) as HttpWebRequest;
                if (webRequest == null)
                {
                    return null;
                }

                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Nothing";

                using (var s = webRequest.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        var unparsedList = sr.ReadToEndAsync();
                        var parsed = JObject.Parse(unparsedList.Result);
                        var a = parsed["response"]["data"]["boxes"];
                        return JsonConvert.DeserializeObject<List<Game>>(a.ToString());

                    }
                }
            });

        }
    }
}

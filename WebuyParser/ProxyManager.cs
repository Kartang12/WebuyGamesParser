using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace WebuyParser
{
    internal static class ProxyManager
    {
        internal static ProxyObject GetProxy(object locker)
        {
            var getProxyString = "http://pubproxy.com/api/proxy?type=http";
            //Stream s;
            //lock (locker)
            //{
                var webRequest = (HttpWebRequest)WebRequest.Create(getProxyString);
                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Nothing";
                var s = webRequest.GetResponse().GetResponseStream();
                Thread.Sleep(500);
            //}
            using var sr1 = new StreamReader(s);
            var result = JsonConvert.DeserializeObject<ProxyObject>(sr1.ReadToEnd());
            Console.WriteLine($"Proxy " +
            $"{result.data[0].ip} : " +
            $"{result.data[0].port}");
            return result;
        }

    }
}

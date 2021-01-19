using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace WebuyParser
{
    class ProxyProcesser
    {
        //this api gives a new proxy on every request
        private string getProxyString = "http://pubproxy.com/api/proxy";

        private string proxyAddress;
        private int proxyPort;

        private WebProxy proxy;

        public ProxyProcesser(object webLocker)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(getProxyString);
            if (webRequest == null)
                return;

            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Nothing";
            string unparsedJson = string.Empty;
            while(true)
            {
                lock (webLocker)
                {
                    Thread.Sleep(1000);
                    var s = webRequest.GetResponse().GetResponseStream();
                    var sr = new StreamReader(s);
                    unparsedJson = sr.ReadToEnd();
                }

                var parsed = JObject.Parse(unparsedJson);
                var type = parsed["data"][0]["type"].ToString();
                Console.WriteLine($"Proxy type {type}");
                if (type != "http")
                    continue;
                proxyAddress = parsed["data"][0]["ip"].ToString(); ;
                proxyPort = int.Parse(parsed["data"][0]["port"].ToString());
                break;
            }

            proxy = new WebProxy(proxyAddress, proxyPort);
            proxy.BypassProxyOnLocal = false;
        }

        public string RequestViaProxy(string requestString, object webLocker)
        {
            var request = (HttpWebRequest)WebRequest.Create(requestString);
            request.ContentType = "application/json";
            request.UserAgent = "Nothing";
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
            //WebProxy proxy = new WebProxy(proxyAddress, proxyPort);
            //proxy.BypassProxyOnLocal = false;
            request.Method = "GET";
            request.Proxy = proxy;
            StreamReader sr;
            lock (webLocker)
            {
                Thread.Sleep(100);
                var s = request.GetResponse().GetResponseStream();
                sr = new StreamReader(s);
            }
            return sr.ReadToEnd();
        }
        
    }
}

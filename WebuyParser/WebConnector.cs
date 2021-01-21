using System;
using System.Net;
using System.Net.Http;

namespace WebuyParser
{
    public class WebConnector
    {
        static int Timeout = 10;
        public static HttpClient CreateClient(object locker, ProxyObject proxyObject = null)
        {
            var httpClientHandler = new HttpClientHandler();
            ProxyObject proxyObjext;
            lock (locker)
            {
                proxyObjext = ProxyManager.GetProxy(locker);
            }
            if (proxyObject != null)
            {
                var proxy = new WebProxy();
                proxy.Address = new Uri($"http://{proxyObjext.data[0].ip}:{proxyObjext.data[0].port}");
                httpClientHandler.Proxy = proxy;
            }

            return new HttpClient(handler: httpClientHandler, disposeHandler: true)
            {
                Timeout = TimeSpan.FromMinutes(Timeout)
            };
        }
    }
}

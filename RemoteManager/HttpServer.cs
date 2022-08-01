using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RemoteManager
{
    public class HttpServer
    {
        public HttpListener httpServer = new HttpListener();
        public int Port { get; private set; }
        public string Host { get; private set; } = "127.0.0.1";
        public bool IsStart { get; private set; }
        public string Prefixe => $"http://{Host}:{Port}/";

        private Dictionary<string, Action<HttpListenerRequest,HttpListenerResponse>> Paths = new Dictionary<string, Action<HttpListenerRequest,HttpListenerResponse>>();

        public HttpServer()
        {
            
        }

        public bool Start(int port)
        {
            if (IsStart) return true;
            try
            {
                Port = port;
                httpServer.Prefixes.Add(Prefixe);
                httpServer.Start();
                httpServer.BeginGetContext(GetContext, null);
                IsStart = true;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void SetHost(string host)
        {
            if (IsStart) return;
            host = host?.Trim();
            if (string.IsNullOrEmpty(host)) return;
            foreach (char c in host)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '.')
                {
                    continue;
                }
                else
                {
                    return;
                }
            }
            Host = "127.0.0.1";
        }

        private void GetContext(IAsyncResult ar)
        {
            httpServer.BeginGetContext(GetContext, null);

            HttpListenerContext context = httpServer.EndGetContext(ar);
            string path = context.Request.Url.AbsolutePath;
            foreach (var item in Paths)
            {
                if (string.Equals(item.Key, path, StringComparison.InvariantCultureIgnoreCase))
                {
                    item.Value?.Invoke(context.Request, context.Response);
                    context.Response.Close();
                    return;
                }
            }

            context.Response.StatusCode = 404;
            context.Response.Close();
        }

        public void AddPath(string path, Action<HttpListenerRequest,HttpListenerResponse> context)
        {
            var split = path.Split('/', '\\').Where(v => v != "");
            Paths.Add("/" + string.Join("/", split), context);
        }

    }

}

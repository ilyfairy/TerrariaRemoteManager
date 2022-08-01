using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace RemoteManager
{
    public class WebSocketManager : IDisposable
    {
        private WatsonWsServer server;
        public Dictionary<string, HttpListenerRequest> Clients = new Dictionary<string, HttpListenerRequest>();
        public event EventHandler<JObject> MessageReceived;

        public async Task Start(string host, int port)
        {
            if (server == null)
            {
                server = new WatsonWsServer(host, port);
                server.ClientConnected += WsServer_ClientConnected;
                server.MessageReceived += WsServer_MessageReceived;
                server.ClientDisconnected += WsServer_ClientDisconnected;
                server.ServerStopped += WsServer_ServerStopped;
                await server.StartAsync();
            }
        }

        public async Task SendAll(string json)
        {
            foreach (var client in Clients)
            {
                await server.SendAsync(client.Key, json);
            }
        }
        public void Dispose()
        {
            server.Stop();
            server.Dispose();
        }

        #region WebSocketServerEvent
        //客户端已连接
        private void WsServer_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Clients.Add(e.IpPort, e.HttpRequest);
        }
        //客户端发送信息
        private void WsServer_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
            {
                string text = Encoding.UTF8.GetString(e.Data);
                try
                {
                    var json = JObject.Parse(text);
                    MessageReceived?.Invoke(this, json);
                }
                catch (Exception)
                {
                    Util.WriteLine($"WebSocket {e.IpPort} 不是一个Json消息: {text}", ConsoleColor.Red);
                }
            }
            else
            {
                Util.WriteLine($"WebSocket {e.IpPort} 未知二进制数据", ConsoleColor.Red);
            }
        }
        //客户端断开连接
        private void WsServer_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Clients.Remove(e.IpPort);
        }
        //服务端停止
        private void WsServer_ServerStopped(object sender, EventArgs e)
        {

        }


        #endregion
    }

    
}

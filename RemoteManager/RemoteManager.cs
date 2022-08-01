using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using RemoteManager.Events;
using RemoteManager.Models.Shared;
using System.Web;

namespace RemoteManager
{
    [ApiVersion(2, 1)]
    public class RemoteManager : TerrariaPlugin
    {
        public override string Name => "RemoteManager";
        public override string Author => "ilyfairy";
        public override string Description => "ws连接";

        public WebSocketManager websocketManager = new WebSocketManager();
        public HttpServer httpServer = new HttpServer();
        
        public RemoteManager(Main game) : base(game)
        {
            websocketManager.MessageReceived += Manager_MessageReceived;
        }

        public override async void Initialize()
        {
            PrintHelp();
            try
            {
                if (!int.TryParse(Util.GetCommandArgs("-ws-port"), out int wsPort)) wsPort = 8001;
                await websocketManager.Start("127.0.0.1", wsPort);
                Util.WriteLine($"WebSocket启动成功  ws://127.0.0.1:{wsPort}/", ConsoleColor.Green);
            }
            catch (Exception)
            {
                Util.WriteLine("WebSocket启动失败", ConsoleColor.Red);
                websocketManager?.Dispose();
            }

            httpServer.SetHost(Util.GetCommandArgs("-http-host"));
            if (!int.TryParse(Util.GetCommandArgs("-http-port"), out int httpPort)) httpPort = 8002;
            if (httpServer.Start(httpPort))
            {
                Util.WriteLine($"HttpServer启动成功  {httpServer.Prefixe}", ConsoleColor.Green);
            }
            else
            {
                Util.WriteLine("HttpServer启动失败", ConsoleColor.Red);
            }

            ServerApi.Hooks.ServerJoin.Register(this, OnServerJoin);
            ServerApi.Hooks.ServerConnect.Register(this, OnServerConnect);
            ServerApi.Hooks.ServerChat.Register(this, OnServerChat);
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            ServerApi.Hooks.ServerCommand.Register(this, OnServerCommand);
            ServerApi.Hooks.ServerBroadcast.Register(this, OnServerBroadcast);
            //ServerApi.Hooks.ServerSocketReset.Register(this, OnServerSocketReset);

            httpServer.AddPath("/api/GetPlayers", (request, response) =>
            {
                var players = Util.GetOnlinePlayers().Select(v => new PlayerInfo(v));
                JArray arr = JArray.FromObject(players);
                response.ContentType = "application/json";
                response.Close(Encoding.UTF8.GetBytes(arr.ToString()), false);
            });
            httpServer.AddPath("/api/SendCommand", (request, response) =>
            {
                var query = HttpUtility.ParseQueryString(request.Url.Query);
                string command = query.Get("Command");

                if(command == null)
                {
                    response.StatusCode = 400;
                    response.Close(Encoding.UTF8.GetBytes("Invalid parameter"), false);
                    return;
                }
                Util.SendCommand(command);
                response.Close(Encoding.UTF8.GetBytes("Success"), false);
            });
            Console.WriteLine();
        }

        public void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine("命令行参数:");
            Console.WriteLine("-http-port  指定HttpServer的端口");
            Console.WriteLine("-http-host  指定HttpServer的Host");
            Console.WriteLine("-ws-port  指定WebSocket的端口");
        }
 

        public void Test()
        {
            TSPlayer[] allPlayers = TShock.Players; //获取所有玩家
            List<Item> items = TShock.Utils.GetItemByIdOrName("123"); //获取物品
            List<TSPlayer> player = TSPlayer.FindByNameOrID("test"); //搜索玩家

            Item item = new Item();

            //player.TPlayer.inventory[0] = TShock.Utils.GetItemById(5); //Set the first slot of the inventory to a mushroom.


            //for (int i = 0; i < player.TPlayer.inventory.Length; i++)
            //{
            //    ref Item item = ref player.TPlayer.inventory[i];

            //    item = TShock.Utils.GetItemById(5); //Set the first slot of the inventory to a mushroom.

            //    //Send the packet to update the player's inventory on the server
            //    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, new NetworkText(item.Name, NetworkText.Mode.Literal), player.Index, 0, item.prefix);
            //    NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, new NetworkText(item.Name, NetworkText.Mode.Literal), player.Index, 0, item.prefix);
            //}
            ////NetMessage.SendData(PacketTypes.PlayerSlot,0,0,)

            //Console.WriteLine("test");


            //TSPlayer player = args.Player; //The player.
            int slot = 50; //The slot index (range: 0 - NetItem.MaxInventory)
            int index; //A variable that will be used to find the index for the needed array (inventory[], armor[], dye[], etc.)
            //Item item = TShock.Utils.GetItemById(5); //The item (mushroom) we will set the slot to.

            ////Inventory slots
            //if (slot < NetItem.InventorySlots)
            //{
            //    index = slot;
            //    player.TPlayer.inventory[slot] = item;

            //    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, new NetworkText(player.TPlayer.inventory[index].Name, NetworkText.Mode.Literal), player.Index, slot, player.TPlayer.inventory[index].prefix);
            //    NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, new NetworkText(player.TPlayer.inventory[index].Name, NetworkText.Mode.Literal), player.Index, slot, player.TPlayer.inventory[index].prefix);
            //}

            //for (int i = 0; i < 50; i++)
            //{
            //    item = player.TPlayer.inventory[i];
            //    // Loops through the player's inventory
            //    if (item.netID == keyID)
            //    {
            //        // Found the item, checking for available slots
            //        if (!player.InventorySlotAvailable)
            //        {
            //            player.SendErrorMessage("背包已满,无法交换!");
            //            return;A
            //        }
            //        if (item.stack >= 1)
            //        {
            //            Console.WriteLine("此物品有" + player.TPlayer.inventory[i].stack + "个");
            //            player.TPlayer.inventory[i].stack--;
            //            Console.WriteLine("此物品有" + player.TPlayer.inventory[i].stack + "个");
            //            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.Empty, player.Index, i);

            //            Console.WriteLine("此物品有" + player.TPlayer.inventory[i].stack + "个");
            //            player.GiveItem(giveItemID, 1, 0);

            //        }
            //    }
            //    // return;
            //}
        }

        #region Hooks
        //玩家连接
        private void OnServerConnect(ConnectEventArgs args)
        {
            ServerConnectEvent connect = new ServerConnectEvent();
            connect.Who = args.Who;
            connect.Build();
            websocketManager.SendAll(connect.ToString());
        }
        //玩家加入
        private void OnServerJoin(JoinEventArgs args)
        {
            //有玩家加入游戏
            TSPlayer player = TShock.Players[args.Who];
            ServerJoinEvent join = new ServerJoinEvent();
            join.Player = new PlayerInfo(player);
            join.Build();
            websocketManager.SendAll(join.ToString());
        }
        //玩家退出
        private void OnServerLeave(LeaveEventArgs args)
        {
            TSPlayer player = TShock.Players[args.Who];
            ServerLeaveEvent leave = new ServerLeaveEvent();
            leave.Player = new PlayerInfo(player);
            leave.Build();
            websocketManager.SendAll(leave.ToString());
        }
        //玩家发送消息
        private void OnServerChat(ServerChatEventArgs args)
        {
            TSPlayer player = TShock.Players[args.Who];
            ServerChatEvent chat = new ServerChatEvent();
            chat.Text = args.Text;
            chat.CommandId = args.CommandId._name;
            chat.Player = new PlayerInfo(player);
            chat.Build();
            websocketManager.SendAll(chat.ToString());
        }
        //服务器输入了命令
        private void OnServerCommand(CommandEventArgs args)
        {
            ServerCommandEvent command = new ServerCommandEvent();
            command.Command = args.Command;
            command.Build();
            websocketManager.SendAll(command.ToString());
        }
        //服务器广播消息
        private void OnServerBroadcast(ServerBroadcastEventArgs args)
        {
            ServerBroadcastEvent broadcast = new ServerBroadcastEvent();
            broadcast.Color = $"{args.Color.R},{args.Color.B},{args.Color.A}";
            broadcast.Mode = args.Message._mode.ToString();
            broadcast.Text = args.Message._text;
            broadcast.Build();
            websocketManager.SendAll(broadcast.ToString());
        }
        //连接重置
        private void OnServerSocketReset(SocketResetEventArgs args)
        {
            //args.Socket.ClientUUID = null;
            
        }
        #endregion


        private void Manager_MessageReceived(object sender, JObject e)
        {
            string type = e["type"]?.ToString() ?? e["Type"]?.ToString();
            switch (type?.ToLower())
            {
                case "sendcommand":
                    try
                    {
                        string command = e["command"]?.ToString() ?? e["Command"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(command))
                        {
                            Util.SendCommand(command);
                        }
                        else
                        {
                            Util.WriteLine("空指令", ConsoleColor.Yellow);
                        }
                    }
                    catch (Exception)
                    {

                    }
                    break;
                case "test":
                    Test();
                    break;
                default:
                    break;
            }
        }

    }

}

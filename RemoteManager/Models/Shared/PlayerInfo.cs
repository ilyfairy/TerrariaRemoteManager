using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RemoteManager.Models.Shared
{
    public class PlayerInfo
    {
        public string Name { get; set; }
        public string UUID { get; set; }
        public string IP { get; set; }
        public int Who { get; set; }
        public GroupInfo Group { get; set; }
        public PlayerInfo()
        {

        }
        public PlayerInfo(TSPlayer player)
        {
            Group = new GroupInfo(player.Group);
            Name = player.Name;
            UUID = player.UUID;
            IP = player.IP;
            Who = Array.IndexOf(TShock.Players, player);
        }
    }
}

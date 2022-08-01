using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RemoteManager.Models.Shared
{
    public class GroupInfo
    {
        public string Name { get; set; }
        public string ChatColor { get; set; }
        public string[] TotalPermissions { get; set; }
        public GroupInfo()
        {

        }
        public GroupInfo(Group group)
        {
            TotalPermissions = group.TotalPermissions.ToArray();
            Name = group.Name;
            ChatColor = group.ChatColor;
        }
    }
}

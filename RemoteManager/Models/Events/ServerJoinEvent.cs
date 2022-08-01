using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RemoteManager.Models.Shared;

namespace RemoteManager.Events
{
    public class ServerJoinEvent : EventBase
    {
        public override EventType Type => EventType.ServerJoin;
        public PlayerInfo Player { get; set; }
        public override EventBase Build()
        {
            Json[nameof(Player)] = JObject.FromObject(Player);
            return base.Build();
        }
    }
}

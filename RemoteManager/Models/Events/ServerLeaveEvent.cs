using Newtonsoft.Json.Linq;
using RemoteManager.Models.Shared;

namespace RemoteManager.Events
{
    public class ServerLeaveEvent : EventBase
    {
        public override EventType Type => EventType.ServerLeave;
        public PlayerInfo Player { get; set; }
        public override EventBase Build()
        {
            Json[nameof(Player)] = JObject.FromObject(Player);
            return base.Build();
        }
    }
}

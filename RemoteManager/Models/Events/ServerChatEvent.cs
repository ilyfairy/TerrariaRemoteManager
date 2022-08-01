using Newtonsoft.Json.Linq;
using RemoteManager.Models.Shared;

namespace RemoteManager.Events
{
    public class ServerChatEvent : EventBase
    {
        public override EventType Type => EventType.ServerChat;
        public string Text { get; set; }
        public string CommandId { get; set; }
        public PlayerInfo Player { get; set; }
        public override EventBase Build()
        {
            Json[nameof(Text)] = Text;
            Json[nameof(CommandId)] = CommandId;
            Json[nameof(Player)] = JObject.FromObject(Player);
            return base.Build();
        }
    }
}

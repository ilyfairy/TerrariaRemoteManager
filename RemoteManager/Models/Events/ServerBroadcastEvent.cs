using Newtonsoft.Json.Linq;
using Terraria.Localization;
using RemoteManager.Models.Shared;

namespace RemoteManager.Events
{
    public class ServerBroadcastEvent : EventBase
    {
        public override EventType Type => EventType.ServerBroadcast;
        public string Mode { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        //public NetworkText[] Substitutions { get; set; }
        public override EventBase Build()
        {
            Json[nameof(Text)] = Text;
            return base.Build();
        }
    }
}

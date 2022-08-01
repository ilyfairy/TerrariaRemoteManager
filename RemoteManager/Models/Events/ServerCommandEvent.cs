using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RemoteManager.Models.Shared;

namespace RemoteManager.Events
{
    public class ServerCommandEvent : EventBase
    {
        public override EventType Type => EventType.ServerCommand;
        public string Command { get; set; }
        public override EventBase Build()
        {
            Json[nameof(Command)] = Command;
            return base.Build();
        }
    }
}

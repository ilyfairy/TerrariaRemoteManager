using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace RemoteManager.Events
{
    public abstract class EventBase
    {
        public JObject Json { get; set; }
        public abstract EventType Type { get; }
        public long Timestamp { get; set; }
        public EventBase()
        {
            Json = new JObject();
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
        public virtual EventBase Build()
        {
            Json[nameof(Type)] = Type.ToString();
            Json[nameof(Timestamp)] = Timestamp;
            return this;
        }
        public static EventBase Parse(string json)
        {
            JObject j = JObject.Parse(json);
            if (!Enum.TryParse((j["Type"] ?? j["type"] ?? j["TYPE"] ?? "").ToString(), true, out EventType type)) return null;

            switch (type)
            {
                case EventType.None:
                    return null;
                case EventType.ServerJoin:
                    return JsonConvert.DeserializeObject<ServerChatEvent>(json);
                case EventType.ServerConnect:
                    return JsonConvert.DeserializeObject<ServerConnectEvent>(json);
                case EventType.ServerChat:
                    return JsonConvert.DeserializeObject<ServerChatEvent>(json);
                default:
                    return null;
            }
        }
        public override string ToString()
        {
            return Json?.ToString();
        }
    }
}

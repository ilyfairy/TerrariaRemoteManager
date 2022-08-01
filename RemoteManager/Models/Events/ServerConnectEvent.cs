namespace RemoteManager.Events
{
    public class ServerConnectEvent : EventBase
    {
        public override EventType Type => EventType.ServerConnect;
        public int Who { get; set; }
        public override EventBase Build()
        {
            Json[nameof(Who)] = Who;
            return base.Build();
        }
    }
}

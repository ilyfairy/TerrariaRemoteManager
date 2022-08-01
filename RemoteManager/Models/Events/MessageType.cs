namespace RemoteManager
{
    /// <summary>
    /// 泰拉瑞亚事件类型
    /// </summary>
    public enum EventType
    {
        None, 
        /// <summary>
        /// 玩家连接
        /// </summary>
        ServerConnect, 
        /// <summary>
        /// 玩家加入
        /// </summary>
        ServerJoin,
        /// <summary>
        /// 玩家退出
        /// </summary>
        ServerLeave,
        /// <summary>
        /// 玩家发送消息
        /// </summary>
        ServerChat, 
        /// <summary>
        /// 服务器输入命令
        /// </summary>
        ServerCommand,
        /// <summary>
        /// 服务器广播消息
        /// </summary>
        ServerBroadcast,
    }
}

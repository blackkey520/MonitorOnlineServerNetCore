using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Sockets
{
    public enum SocketCloseReason
    {
        /// <summary>
        /// 
        /// </summary>
        ServerShutdown,

        /// <summary>
        /// 客户端关闭
        /// </summary>
        ClientClosing,

        /// <summary>
        /// 服务关闭
        /// </summary>
        ServerClosing,

        /// <summary>
        /// Socket 错误
        /// </summary>
        SocketError,

        /// <summary>
        /// 超时
        /// </summary>
        TimeOut,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown,
    }
    /// <summary>
    /// Socket
    /// </summary>
    public class SocketClientClosedEventArgs : EventArgs
    {
        public SocketClientClosedEventArgs(AsynSocketConnection connection, SocketCloseReason reason)
        {
            CloseReason = reason;
            SocketConnection = connection;
        }

        public AsynSocketConnection SocketConnection { get; private set; }

        public SocketCloseReason CloseReason { get; private set; }
    }
}

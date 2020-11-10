using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Server.Core.Sockets
{
    /// <summary>
    /// 回话Session接口
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// SessionID
        /// </summary>
        string SessionID { get; }

        /// <summary>
        /// 远程连接地址
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }
    }
}

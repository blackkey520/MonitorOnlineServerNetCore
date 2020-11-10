using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Sockets
{
    public class SocketDataEventArgs : EventArgs
    {
        public AsynSocketSession Session { get; set; }
        public Byte[] Data { get; set; }
        public Int32 Offset { get; set; }
        public Int32 Length { get; set; }
    }
}

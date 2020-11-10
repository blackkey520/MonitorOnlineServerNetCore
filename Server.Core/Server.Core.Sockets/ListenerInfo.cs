using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Server.Core.Sockets
{
    class ListenerInfo
    {
        public IPEndPoint IPEndPoint { get; set; }

        public int Backlog { get; set; }
    }
}

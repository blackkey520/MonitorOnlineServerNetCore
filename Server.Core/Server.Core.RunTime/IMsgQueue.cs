using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.RunTime
{
    public interface IMsgQueue:IMonitorOnlineModule
    {
        void SendMsg<T>(string queName, T msg) where T : class;
        void Receive(string queName, Action<object> received);

    }
}

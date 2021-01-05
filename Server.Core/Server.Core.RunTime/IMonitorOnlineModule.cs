using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.RunTime
{
    public interface IMonitorOnlineModule
    {
        string ModuleName { get; }
        bool IsRun { get; }
       
        bool Start();
        bool Stop(); 
    }
}

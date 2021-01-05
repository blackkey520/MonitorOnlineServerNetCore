using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Sockets
{
    public interface IAnalytice
    {
        string AnalyticeName { get; }
        string AnalyticeChName { get; }
        PackageInfo Read(byte[] buffer, out int rest);

    }
}

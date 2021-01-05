using Server.Core.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataAnalyticesServices.Command
{
    public class StandardAgreementCommand: IAnalytice
    {
        public string AnalyticeName => "StandardAgreement";
        public string AnalyticeChName => "标准212协议";

        public PackageInfo Read(byte[] buffer, out int rest)
        {
            PackageInfo package = new PackageInfo();
            package.DeviceCode = "111";
            package.PackageStr = "122";
            rest = 0;
            return package;
        }
    }
}

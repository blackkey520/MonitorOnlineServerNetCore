using Microsoft.Extensions.Logging;
using Server.Core.RunTime;
using Server.Core.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.DataAnalyticesServices
{
    public class Module_DataAnalytices : IMonitorOnlineModule
    {
        private readonly ILogger<Module_DataAnalytices> m_logger;
        public Module_DataAnalytices()
        {
            string ip = "127.0.0.1";
            int port = 8078;
            
            listenThread = new Thread(new ThreadStart(() =>
            {

                var server = new AsynTcpServer(ip, port, "StandardAgreement");

                try
                {
                    server.Start();
                    m_logger.LogInformation("信息输出-启动监听{0}成功", ip + "-" + port);
                }
                catch (Exception ex)
                {
                    m_logger.LogInformation("信息输出-启动监听{0}失败，失败原因{1}", ip + "-" + port, ex.ToString());
                }
            }))
            { Name = "Listen" };
            listenThread.Start();
        }
        public string ModuleName => "数据解析模块";

        public bool IsRun => true;


        private static Thread listenThread = null;


        public bool Start()
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool Stop()
        {
            try
            {
                listenThread.Abort();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}

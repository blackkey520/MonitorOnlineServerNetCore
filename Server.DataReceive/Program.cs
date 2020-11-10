using Server.Core.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.DataReceive
{
    class Program
    {

        private static Thread listenThread = null;
        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 8078;

            listenThread = new Thread(new ThreadStart(() =>
            {

                var server = new AsynTcpServer(ip,port);
                try
                {
                    server.Start();
                    Console.WriteLine("启动监听{0}成功", ip+"-"+port);
                }
                catch (Exception ex)
                {
                    //异常输出
                }
            }))
            { Name = "Listen" };
            Console.ReadLine();
        }

         

    }
}

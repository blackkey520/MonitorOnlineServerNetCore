using Microsoft.Extensions.Logging;
using Server.Core.RunTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static Server.Core.RunTime.GlobalENUM;

namespace Server.Core.Sockets
{
    public class AsynTcpServer
    {
        private IEnumerable<IAnalytice> m_Analytices { get; set; }
        public IMsgQueue m_msgQueue { get; set; }
        private readonly ILogger<AsynTcpServer> m_logger;
        private readonly object SyncRoot = new object();

        int m_port = 8080;//端口号
        string m_ip = "127.0.0.1";
        string m_anayticename = "";
        // 用于监听Socket
        private TcpAsynSocketListener listenSocket;
        //可复用的字节缓存用于Socket
        SocketAsyncEventBufferManager bufferManager;
        //接收数据池
        SocketAsyncEventArgsPool poolOfRecSendEventArgs;
        //客户端连接列表
        private List<AsynSocketConnection> m_connectionList = new List<AsynSocketConnection>();

         
        //是否转换配置
        public string Serverdescriptionkey = "";
       
        List<string> List_New = new List<string>();
        List<string> List_Old = new List<string>();
        
        /// <summary>
        /// 连接数量
        /// </summary>
        public int ConnectionCount
        {
            get { return m_connectionList.Count; }
        }
        public List<AsynSocketConnection> ConnectionList
        {
            get { return m_connectionList; }
        }
         
        

         
        public AsynTcpServer(string ip,int port,string anayticename)
        {
            m_port = port;
            m_ip = ip;
            m_anayticename = anayticename;
            Init();
        }
        private Thread listenThread = null;
        private void Init()
        {
            try
            {

                //并发最大连接数
                poolOfRecSendEventArgs = new SocketAsyncEventArgsPool(100);
                //最大连接数*buffer大小等于 并发最大连接buffer大小
                bufferManager = new SocketAsyncEventBufferManager(4096* 100, 4096);

                for (int i = 0; i < 100; i++)
                {
                    SocketAsyncEventArgs eventArgObjectForPool = new SocketAsyncEventArgs();
                    this.poolOfRecSendEventArgs.Push(eventArgObjectForPool);
                }

                listenSocket = new TcpAsynSocketListener(CreateListenerInfo());
                listenSocket.SocketConnected += listenSocket_SocketConnected;

                
                 
            }
            catch (Exception ee)
            {
                m_logger.LogError("TCP链路异常-连接初始化异常,解析模块{0}",m_anayticename);
            }

        }
        private void stopthread()
        {
            try
            {
                lock (m_connectionList)
                {
                    for (int i = m_connectionList.Count - 1; i >= 0; i--)
                    {
                        AsynSocketConnection connection = m_connectionList[i];
                        TimeSpan ts = DateTime.Now - connection.LastReceivedTime;
                        if (ts.TotalMinutes > 15)
                        {
                            connection.Connect = false;
                            connection.Close(SocketCloseReason.TimeOut);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_logger.LogError("TCP链路异常-连接关闭发生异常,解析模块{0}", m_anayticename);
            }
        }
   

        public void Start()
        {
            listenSocket.Start();
            m_connectionList = new List<AsynSocketConnection>();
        }

        public void Send(AsynSocketSession recvDataClient, byte[] datagram)
        {
            try
            {
                recvDataClient.Client.BeginSend(datagram, 0, datagram.Length, SocketFlags.None,
            new AsyncCallback(SendDataEnd), recvDataClient.Client);
            }
            catch (Exception ee)
            {
                m_logger.LogError("TCP链路异常-数据发送异常,解析模块{0}", m_anayticename);
            }
        }
        /// <summary> 
        /// 发送数据完成处理函数 
        /// </summary> 
        /// <param name="iar">目标客户端Socket</param> 
        protected virtual void SendDataEnd(IAsyncResult iar)
        {
            try
            {
                Socket client = null;
                client = (Socket)iar.AsyncState;

                int sent = client.EndSend(iar);
            }
            catch
            {
                m_logger.LogError("TCP链路异常-数据发送结束事件异常,解析模块{0}", m_anayticename);
            }
        }

        public void Stop()
        {
            lock (this)
            {
                listenSocket.Stop();

                while (m_connectionList.Count > 0)
                {
                    m_connectionList.First().Close(SocketCloseReason.ServerClosing);
                }
            }
        }

        void listenSocket_SocketConnected(object sender, SocketEventArgs e)
        {
            try
            {
                SocketAsyncEventArgs args = poolOfRecSendEventArgs.Pop();
                bufferManager.SetBuffer(args);
                AsynSocketConnection connection = new AsynSocketConnection(e.Socket, args);
               
                connection.OnDataReceived += connection_OnDataReceived;
               
                connection.OnSocketDisconnected += connection_OnSocketDisconnected;

                lock (SyncRoot)
                {
                    
                    m_connectionList.Add(connection);
                }
            }
            catch (Exception ee)
            {
                m_logger.LogError("TCP链路异常-连接建立发生异常,解析模块{0}", m_anayticename);
            }

        }

        void connection_OnSocketDisconnected(object sender, SocketClientClosedEventArgs e)
        {
            try
            {
                bufferManager.FreeBuffer(e.SocketConnection.SocketAsyncEventArgs);
                poolOfRecSendEventArgs.Push(e.SocketConnection.SocketAsyncEventArgs);

                lock (SyncRoot)
                {
                    
                    e.SocketConnection.OnDataReceived -= connection_OnDataReceived;
                   
                    e.SocketConnection.OnSocketDisconnected -= connection_OnSocketDisconnected;
                    m_connectionList.Remove(e.SocketConnection);
                    // e.SocketConnection.SocketAsyncEventArgs.UserToken = null;
                    //连接断开时，更新内存中排口的状态

                  
                    if (e.SocketConnection.Client.Connected)
                    {
                        e.SocketConnection.Client.Shutdown(SocketShutdown.Both);
                        System.Threading.Thread.Sleep(10);

                        //关闭客户端Socket,清理资源
                        e.SocketConnection.Client.Close();
                    }
                    
                        if (e.SocketConnection != null)
                        {
                            if (e.SocketConnection.Client != null)
                            {
                            ///添加连接属性
                            //connectionInfo conInfo = new connectionInfo();
                            //conInfo.DEGIMN = e.SocketConnection.DeviceCode;

                            //if (e.SocketConnection.Client.Connected)
                            //{
                            //    string ipport = e.SocketConnection.Client.RemoteEndPoint.ToString();
                            //    string[] ipportArray = ipport.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            //    conInfo.connectionIP = ipportArray[0];
                            //    conInfo.connectionPort = ipportArray[1];
                            //}

                            //conInfo.latestDateTime = e.SocketConnection.LastReceivedTime;
                            //conInfo.IsDataRemove = true;
                            //InterfaceCollection.ConnectionInfoCollection.Add(conInfo);


                            //有连接上来日志输出
                            m_logger.LogInformation("信息输出-新的连接建立");
                            }
                        }

                    
                }
            }
            catch (Exception ee)
            {
                m_logger.LogError("TCP链路异常-连接断开后续处置发生异常,解析模块{0}", m_anayticename);
            }
        }

        void connection_OnDataReceived(object sender, SocketDataEventArgs e)
        {
            try
            {
                byte[] buffer = e.Session.DataHolder.JoinAllBuffer().Concat(e.Data).ToArray();
                 
                AsynSocketConnection socketCon = (AsynSocketConnection)sender;
                string packageString = string.Join(" ", ((byte[])e.Data).Select(o => string.Format("0x{0:X},", o).PadLeft(2, '0')));
                int rest = 0;
                PackageInfo packageInfo = null;
                while (buffer.Length != 0 && buffer.Length - rest != 0)
                {
                     
                        IAnalytice analytice = m_Analytices.FirstOrDefault(a => a.AnalyticeName == m_anayticename);
                        packageInfo = analytice.Read(buffer, out rest);
                        if (packageInfo != null)
                        {
                            if (packageInfo.DeviceCode != null)
                            {
                                socketCon.DeviceCode = packageInfo.DeviceCode.ToString();
                                
                            }
                        }
                    
                    if (rest >= 0 && packageInfo != null)
                    {
                        //输出包信息
                        m_msgQueue.SendMsg<PackageInfo>(MQTYPE.ORIGANLPACKAGE.ToString(), packageInfo);
                        //XXX成功解析

                        buffer = buffer.Skip(rest).ToArray();
                        rest = 0;
                    }
                    else
                    {
                        //解析有问题 
                        buffer = buffer.Skip(rest).ToArray();
                        rest = 0;
                        break;
                    }
                }
                //保存没有解析成功的数据
                e.Session.DataHolder.Enqueue(buffer);
            }
            catch (Exception ex)
            {
                m_logger.LogError("TCP链路异常-数据接收初步解析发生异常,解析模块{0}", m_anayticename);
            }
        }

        

        

        
        
         
        private ListenerInfo CreateListenerInfo()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(m_ip);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, m_port);
                return new ListenerInfo()
                {
                    IPEndPoint = ipEndPoint,
                    Backlog = 511//最大并发连接数
                };
            }
            catch (Exception ee)
            {
                m_logger.LogError("TCP链路异常-创建监听异常,解析模块{0}", m_anayticename);
                return new ListenerInfo();
            }

        }

        
    }

     
}

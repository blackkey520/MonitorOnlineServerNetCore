using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server.Core.Sockets
{
    public class SocketSessionClosedEventArgs : EventArgs
    {
        //public 
    }

    public class AsynSocketConnection
    {
        /// <summary>
        /// 数据接收
        /// </summary>
        public event EventHandler<SocketDataEventArgs> OnDataReceived;

        /// <summary>
        /// 客户端连接断开
        /// </summary>
        public event EventHandler<SocketClientClosedEventArgs> OnSocketDisconnected;

        /// <summary>
        /// SocketAsyncEventArgs 实体
        /// </summary>
        public SocketAsyncEventArgs SocketAsyncEventArgs { get; private set; }

        //public AsynTcpServer Server { get; private set; }

        private object SyncRoot = new object();
        public DateTime LastReceivedTime = DateTime.Now;
        /// <summary>
        /// 客户端Socket
        /// </summary>
        public Socket Client { get; private set; }
        /// <summary>
        /// 连接站点编号
        /// </summary>
        public string DeviceCode { get; set; }
        public bool Connect { get; set; }
        public int Number { get; set; }
        public AsynSocketConnection(Socket socket, SocketAsyncEventArgs args)
        {
            //Server = server;
            Client = socket;
            AsynSocketSession session = new AsynSocketSession(this);
            SocketAsyncEventArgs = args;
            SocketAsyncEventArgs.Completed += ReceivedCompleted;
            SocketAsyncEventArgs.UserToken = session;
            Connect = true;
            Number = 0;
            ListenForData(SocketAsyncEventArgs);
        }

        private void ListenForData(SocketAsyncEventArgs args)
        {
            Socket socket = (args.UserToken as AsynSocketSession).Client;

            if (socket.Connected)
            {
                socket.InvokeAsyncMethod(socket.ReceiveAsync, ReceivedCompleted, args);
            }
        }

        private void ReceivedCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (!Connect)
                {
                    // CloseConnection(SocketCloseReason.SocketError);
                    return;
                }
                if (e.BytesTransferred == 0)
                {
                    CloseConnection(SocketCloseReason.SocketError);
                    return;
                }

                if (e.SocketError != SocketError.Success)
                {
                    CloseConnection(SocketCloseReason.SocketError);
                    return;
                }

                AsynSocketSession session = e.UserToken as AsynSocketSession;

                Byte[] data = new Byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, data.Length);

                if (OnDataReceived != null)
                {
                    LastReceivedTime = DateTime.Now;
                    OnDataReceived(this, new SocketDataEventArgs()
                    {
                        Session = e.UserToken as AsynSocketSession,
                        Data = data
                    });

                }

                ListenForData(e);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void CloseConnection(SocketCloseReason closeReason)
        {
            try
            {

                AsynSocketSession session = SocketAsyncEventArgs.UserToken as AsynSocketSession;
                if (session != null)
                {
                    Socket socket = session.Client;
                    socket.SafeCloseSocket();
                    session.SocketConection.SocketAsyncEventArgs.AcceptSocket.SafeCloseSocket();
                    session.SocketConection.SocketAsyncEventArgs.ConnectSocket.SafeCloseSocket();
                }
                SocketAsyncEventArgs.Completed -= ReceivedCompleted;
                if (OnSocketDisconnected != null)
                {
                    OnSocketDisconnected(this, new SocketClientClosedEventArgs(this, closeReason));
                }

                Thread.Sleep(2000);
                SocketAsyncEventArgs.UserToken = null;
                SocketAsyncEventArgs.SetBuffer(null, 0, 0);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Close(SocketCloseReason closeReason)
        {
            CloseConnection(closeReason);
        }
    }
}

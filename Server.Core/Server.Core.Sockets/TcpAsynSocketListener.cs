using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Core.Sockets
{
    public class SocketEventArgs : EventArgs
    {
        public Socket Socket { get; set; }
    }

    class TcpAsynSocketListener : IDisposable
    {
        private Socket listenerSocket;
        private bool m_IsRunning = false;
        private SocketAsyncEventArgs asynEventArg;

        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsRunning
        {
            get { return m_IsRunning; }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        public IPEndPoint IPEndPoint { get; private set; }

        /// <summary>
        /// ConnectionBacklog 
        /// </summary>
        public int ConnectionBacklog { get; private set; }

        public TcpAsynSocketListener(ListenerInfo listenerInfo)
        {
            IPEndPoint = listenerInfo.IPEndPoint;
            ConnectionBacklog = listenerInfo.Backlog;

            asynEventArg = new SocketAsyncEventArgs();
            asynEventArg.Completed += SocketAccepted;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            if (IsRunning)
                //日志输出

            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(this.IPEndPoint);
            listenerSocket.Listen(this.ConnectionBacklog);

            ListenForConnection(asynEventArg);
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (listenerSocket == null)
                return;

            try
            {
                listenerSocket.Shutdown(SocketShutdown.Both);
            }
            catch { }

            listenerSocket.Close();
            listenerSocket = null;
        }

        private void ListenForConnection(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            listenerSocket.InvokeAsyncMethod(listenerSocket.AcceptAsync, SocketAccepted, args);
        }

        private void SocketAccepted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.OperationAborted)
                return; //Server was stopped

            if (e.SocketError == SocketError.Success)
            {
                Socket handler = e.AcceptSocket;
                OnSocketConnected(handler);
            }

            lock (this)
            {
                ListenForConnection(e);
            }
        }

        public event EventHandler<SocketEventArgs> SocketConnected;

        private void OnSocketConnected(Socket client)
        {
            if (SocketConnected != null)
                SocketConnected(this, new SocketEventArgs() { Socket = client });
        }

        #region IDisposable Members
        private Boolean disposed = false;

        ~TcpAsynSocketListener()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Stop();
                    if (asynEventArg != null)
                        asynEventArg.Dispose();
                }

                disposed = true;
            }
        }
        #endregion
    }
}

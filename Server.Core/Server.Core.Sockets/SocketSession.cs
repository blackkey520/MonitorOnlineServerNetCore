using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Core.Sockets
{
    public interface ISocketSession : ISession
    {
        /// <summary>
        /// 本地地址
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// 客户端Socket
        /// </summary>
        Socket Client { get; }

        void Send(byte[] buffer);
        void Send(string packet);
        void Send(EncodingType type, string packet);
    }

    /// <summary>
    /// 客户端连接回话
    /// </summary>
    public class AsynSocketSession : ISocketSession
    {
        public AsynSocketSession(AsynSocketConnection connection)
        {
            SocketConection = connection;
            RemoteEndPoint = (IPEndPoint)connection.Client.RemoteEndPoint;
            LocalEndPoint = (IPEndPoint)connection.Client.LocalEndPoint;
            SessionID = Guid.NewGuid().ToString();
            DataHolder = new DataHolder();
        }

        /// <summary>
        /// Socket连接
        /// </summary>
        public AsynSocketConnection SocketConection { get; private set; }

        /// <summary>
        /// SessionID
        /// </summary>
        public string SessionID { get; private set; }

        /// <summary>
        /// 远程地址
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// 本地连接地址
        /// </summary>
        public IPEndPoint LocalEndPoint { get; private set; }


        /// <summary>
        /// 客户端地址
        /// </summary>
        public Socket Client
        {
            get { return SocketConection.Client; }
        }

        public DataHolder DataHolder { get; private set; }

        public void Send(byte[] buffer)
        {
            try
            {
                Client.Send(buffer, buffer.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void Send(string packet)
        {
            try
            {
                byte[] bugger = new EncodingConverter().StringToBytes(EncodingType.Default, packet);
                Send(bugger);
            }
            //连接异常
            catch (SocketException)
            {

            }
            catch (Exception)
            {

            }
        }
        public void Send(EncodingType type, string packet)
        {
            try
            {
                byte[] bugger = new EncodingConverter().StringToBytes(type, packet);
                Send(bugger);
            }
            catch (SocketException)
            {

            }
            catch (Exception)
            {


            }
        }
    }
}

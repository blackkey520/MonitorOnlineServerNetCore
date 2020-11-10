using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server.Core.Sockets
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Socket 异步通用调用方法
        /// </summary>
        /// <param name="socket">要执行的Socket</param>
        /// <param name="method">执行的异步方法</param>
        /// <param name="callback">callback方法</param>
        /// <param name="args">callback中 SocketAsyncEventArgs参数</param>
        public static void InvokeAsyncMethod(this Socket socket, Func<SocketAsyncEventArgs, bool> method, EventHandler<SocketAsyncEventArgs> callback, SocketAsyncEventArgs args)
        {
            try
            {
                if (!method(args))
                    callback(socket, args);
            }
            catch (Exception e)
            {
                //异常输出
            }

        }

        /// <summary>
        /// 安全关闭Socket
        /// </summary>
        /// <param name="socket"></param>
        public static void SafeCloseSocket(this Socket socket)
        {
            if (socket == null)
                return;

            if (!socket.Connected)
                return;

            try
            {
                socket.Shutdown(SocketShutdown.Receive);
            }
            catch (Exception e)
            {
                //异常输出
            }

            try
            {
                socket.Close();
            }
            catch (Exception e)
            {
                //异常输出
            }
        }
    }
}

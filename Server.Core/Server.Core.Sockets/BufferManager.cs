using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server.Core.Sockets
{
    class SocketAsyncEventBufferManager : IDisposable
    {
        Int32 m_numBytes;
        byte[] m_buffer;
        Stack<int> m_freeIndexPool;
        int currentIndex;
        int m_bufferSize;

        public SocketAsyncEventBufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            this.currentIndex = 0;
            this.m_bufferSize = bufferSize;
            this.m_freeIndexPool = new Stack<int>();
            InitBuffer();
        }

        internal void InitBuffer()
        {
            this.m_buffer = new byte[m_numBytes];
        }

        internal bool SetBuffer(SocketAsyncEventArgs args)
        {

            //if (this.m_freeIndexPool.Count > 0)
            //{
            //    args.SetBuffer(this.m_buffer, this.m_freeIndexPool.Pop(), this.m_bufferSize);
            //}
            //else
            //{
            //    if ((m_numBytes - this.m_bufferSize) < this.currentIndex)
            //    {
            //        return false;
            //    }
            //    args.SetBuffer(this.m_buffer, this.currentIndex, this.m_bufferSize);
            //    this.currentIndex += this.m_bufferSize;
            //}
            byte[] buffer = new byte[m_bufferSize];
            args.SetBuffer(buffer, 0, buffer.Length);
            return true;

        }

        internal void FreeBuffer(SocketAsyncEventArgs args)
        {
            //this.m_freeIndexPool.Push(args.Offset);
            //args.SetBuffer(null, 0, 0);
        }

        public void Dispose()
        {
            m_freeIndexPool.Clear();
        }
    }
}

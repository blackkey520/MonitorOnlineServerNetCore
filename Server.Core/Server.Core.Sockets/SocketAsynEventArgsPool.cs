using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server.Core.Sockets
{
    internal class SocketAsyncEventArgsPool
    {
        private Int32 nextTokenId = 0;

        Stack<SocketAsyncEventArgs> pool;

        internal SocketAsyncEventArgsPool(Int32 capacity)
        {
            this.pool = new Stack<SocketAsyncEventArgs>();
        }

        internal int Count { get { return this.pool.Count; } }

        internal void Push(SocketAsyncEventArgs item)
        {
            //Guard.ArgumentNotNull(() => item);

            lock (this.pool) {
                this.pool.Push(item);
            }
        }

        internal SocketAsyncEventArgs Pop()
        {
            lock (this.pool) {
                if (this.pool.Count != 0)
                    return this.pool.Pop();
                else
                    return new SocketAsyncEventArgs();
            }
           //return new SocketAsyncEventArgs();
        }


        internal Int32 NewTokenId()
        {
            return Interlocked.Increment(ref nextTokenId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Core.Sockets
{
    public class DataHolder
    {
        private readonly object syncRoot = new object();

        private readonly Queue<ArraySegment<byte>> packageQueue;

        public DataHolder()
        {
            packageQueue = new Queue<ArraySegment<byte>>();
        }

        /// <summary>
        /// 是否存在数据
        /// </summary>
        public bool HasData { get { return packageQueue.Count > 0; } }

        public byte[] JoinAllBuffer()
        {
            lock (syncRoot)
            {
                if (packageQueue.Count == 1) return packageQueue.Dequeue().Array;

                byte[] buffer = new byte[packageQueue.Sum(q => q.Count)];

                int offset = 0;
                while (packageQueue.Count != 0)
                {
                    byte[] package = packageQueue.Dequeue().Array;
                    Buffer.BlockCopy(package, 0, buffer, offset, package.Length);
                    offset += package.Length;
                }
                return buffer;
            }
        }

        public void Enqueue(byte[] buffer)
        {
             

            lock (syncRoot)
            {
                this.packageQueue.Enqueue(new ArraySegment<byte>(buffer));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Sockets
{
    /// <summary>
    /// 字符串编码枚举类
    /// </summary>
    public enum EncodingType
    {
        Default,
        Unicode,
        UTF8,
        ASCII
    }

    /// <summary>
    /// 编码格式转换工具类
    /// </summary>
    public class EncodingConverter
    {
        public static Dictionary<EncodingType, Encoding> EncodingDic = new Dictionary<EncodingType, Encoding>{
           {EncodingType.ASCII,     Encoding.ASCII},
           {EncodingType.Default,   Encoding.Default},
           {EncodingType.UTF8,      Encoding.UTF8},
           {EncodingType.Unicode,   Encoding.Unicode}
        };

        /// <summary>
        /// 根据编码 二进制数据转换成字符串
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bytes"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string BytesToString(EncodingType type, byte[] bytes, int size = -1)
        {
            

            if (size < 0) size = bytes.Length;
            return EncodingDic[type].GetString(bytes, 0, size);
        }

        /// <summary>
        /// 根据编码 字符串转换成二进制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] StringToBytes(EncodingType type, string str)
        {
            
            return EncodingDic[type].GetBytes(str);
        }
    }
}

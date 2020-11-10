using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Utils
{
    public static class ListSearchExtensions
    {
        /// <summary>
        /// 查询IList中指定索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchList">数据集</param>
        /// <param name="target">要搜索的目标</param>
        /// <param name="pos">指定位置</param>
        /// <param name="length">要搜索的长度</param>
        /// <returns></returns>
        public static int IndexOf<T>(this IList<T> searchList, T target, int pos, int length)
            where T : IEquatable<T>
        {
            for (int i = pos; i < pos + length; i++)
            {
                if (searchList[i].Equals(target))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 查找指定数组索引（不存在返回-1）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchList"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static int SearchMark<T>(this IList<T> searchList, T[] mark) where T : IEquatable<T>
        {
            return searchList.SearchMark(0, searchList.Count, mark);
        }

        /// <summary>
        /// 查找指定数组索引（不存在返回-1）
        /// </summary>
        /// <remarks>
        /// KPM 算法
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchList"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static int SearchMark<T>(this IList<T> searchList, int offset, int length, T[] mark) where T : IEquatable<T>
        {
            int pos = offset;
            int endpos = offset + length - 1;
            int matchCount = 0;

            while (true)
            {
                pos = searchList.IndexOf(mark[matchCount], pos, length - pos + offset);

                if (pos < 0) return -1;

                for (int i = matchCount; i < mark.Length; i++)
                {
                    int checkPos = pos + i;

                    if (checkPos > endpos)
                    {
                        return -1;
                    }

                    if (!searchList[checkPos].Equals(mark[i]))
                        break;

                    matchCount++;
                }

                if (matchCount == mark.Length)
                {
                    return pos;
                }

                pos++;
                matchCount = 0;
            }
        }
    }
}

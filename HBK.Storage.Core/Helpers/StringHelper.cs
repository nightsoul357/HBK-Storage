using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Helpers
{
    /// <summary>
    /// 字串輔助類別
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 取得隨機英數字字串
        /// </summary>
        /// <param name="length">字串長度</param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

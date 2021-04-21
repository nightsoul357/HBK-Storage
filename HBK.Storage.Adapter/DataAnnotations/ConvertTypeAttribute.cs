using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.DataAnnotations
{
    /// <summary>
    /// 描述列舉對應應轉換之目標
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ConvertTypeAttribute : Attribute
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="targetType">目標型別</param>
        public ConvertTypeAttribute(Type targetType)
        {
            this.TargetType = targetType;
        }

        /// <summary>
        /// 取得轉換目標型別
        /// </summary>
        public Type TargetType { get; private set; }
    }
}

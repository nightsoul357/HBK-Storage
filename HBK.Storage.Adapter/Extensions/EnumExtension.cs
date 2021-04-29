using HBK.Storage.Adapter.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HBK.Storage.Adapter.Enums
{
    /// <summary>
    /// 列舉的擴充方法
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 轉換旗標形式陣列為複合的旗標列舉值
        /// </summary>
        /// <typeparam name="TEnum">列舉類型</typeparam>
        /// <param name="values">旗標列舉值陣列</param>
        /// <returns>複合的旗標列舉值</returns>
        public static TEnum UnflattenFlags<TEnum>(this IEnumerable<TEnum> values)
            where TEnum : Enum
        {
            long value = 0;
            foreach (var item in values)
            {
                value |= Convert.ToInt64(item);
            }
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        /// <summary>
        /// 轉換旗標形式複合的旗標列舉值為陣列
        /// </summary>
        /// <typeparam name="TEnum">列舉類型</typeparam>
        /// <param name="value">複合的旗標列舉值</param>
        /// <returns>單一旗標列舉的陣列</returns>
        public static TEnum[] FlattenFlags<TEnum>(this TEnum value)
            where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                    .Cast<TEnum>()
                    .Where(x =>
                    {
                        long val = Convert.ToInt64(x);
                        return val != 0 && ((val & (val - 1)) == 0);
                    })
                    .Where(x => value.HasFlag(x))
                    .ToArray();
        }

        /// <summary>
        /// 設定或取消設定指定旗標
        /// </summary>
        /// <typeparam name="TEnum">列舉類型</typeparam>
        /// <param name="value">複合的旗標列舉值</param>
        /// <param name="flag">要設定的旗標</param>
        /// <param name="set">是否設定的旗標</param>
        /// <returns></returns>
        public static TEnum SetFlag<TEnum>(this TEnum value, TEnum flag, bool set)
            where TEnum : Enum
        {
            long val;
            if (set)
            {
                val = Convert.ToInt64(value) | Convert.ToInt64(flag);
            }
            else
            {
                val = Convert.ToInt64(value) & ~Convert.ToInt64(flag);
            }
            return (TEnum)Enum.ToObject(typeof(TEnum), val);
        }
        /// <summary>
        /// 取得 <see cref="ConvertTypeAttribute"/> 所指定的目標型別
        /// </summary>
        /// <typeparam name="TEnum">列舉類型</typeparam>
        /// <param name="value">目標列舉</param>
        /// <returns></returns>
        public static Type GetConvertType<TEnum>(this TEnum value)
            where TEnum : Enum
        {
            var type = typeof(TEnum);
            var attribute = type.GetMember(value.ToString())
                .First(m => m.DeclaringType == type)
                .GetCustomAttributes(typeof(ConvertTypeAttribute), false)
                .Cast<ConvertTypeAttribute>()
                .FirstOrDefault();

            return attribute?.TargetType ?? default;
        }
    }
}

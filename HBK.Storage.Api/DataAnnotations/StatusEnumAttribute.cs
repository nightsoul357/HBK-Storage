using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace HBK.Storage.Api.DataAnnotations
{
    /// <summary>
    /// 判斷欄位 ID 是否在資料表中
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class StatusEnumAttribute : ValidationAttribute
    {
        /// <summary>
        /// 取得或設定列舉類型
        /// </summary>
        public Type EnumType { get; set; }

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="enumType">列舉類型</param>
        public StatusEnumAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumType)} must be a Enum type.", nameof(enumType));
            }
            this.EnumType = enumType;
        }

        /// <summary>
        /// 判斷是否有效
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // 檢查列舉類型
            Type valueType = value.GetType();
            if (value is IEnumerable)
            {
                valueType = valueType.GetInterfaces()
                    .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .First()
                    .GetGenericArguments()[0];
            }
            if (valueType != this.EnumType)
            {
                return new ValidationResult($"Value should be type of {this.EnumType.Name}");
            }

            // 取得可設定的旗標
            var memberList = this.EnumType.GetMembers()
                .Where(m => m.DeclaringType == this.EnumType)
                .ToList();

            string[] valuesToCheck;
            if (value is IEnumerable enumerable)
            {
                valuesToCheck = enumerable.OfType<object>().Select(m => m.ToString()).ToArray();
            }
            else
            {
                valuesToCheck = new[] { value.ToString() };
            }

            foreach (string member in valuesToCheck)
            {
                if (!memberList.Any(x => x.Name == member))
                {
                    return new ValidationResult($"The flag \"{member}\" is auto set and can not be explicitly set.");
                }
            }

            return ValidationResult.Success;
        }
    }
}

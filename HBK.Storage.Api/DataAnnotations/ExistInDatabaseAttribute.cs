using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HBK.Storage.Adapter.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HBK.Storage.Api.DataAnnotations
{
    /// <summary>
    /// 判斷欄位 ID 是否在資料表中
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public sealed class ExistInDatabaseAttribute : ValidationAttribute
    {
        /// <summary>
        /// 取得或設定模型類型
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// 取得或設定欄位名稱
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="modelType">模型類型</param>
        /// <param name="column">欄位名稱</param>
        public ExistInDatabaseAttribute(Type modelType, string column = null)
        {
            this.ModelType = modelType;
            this.Column = column;
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

            var dbContext = validationContext.GetRequiredService<HBKStorageContext>();
            var entitySchema = dbContext.Model.FindEntityType(this.ModelType);
            string table = entitySchema.GetTableName();

            this.Column ??= entitySchema.FindPrimaryKey().Properties
                .Select(p => p.Name).Single();

            int expectedCount;
            int count = 0;
            if (value is IEnumerable enumerable)
            {
                object[] values = enumerable.OfType<object>().ToArray();
                string parameters = string.Join(",", values.Select((v, i) => $"{{{i}}}"));
                expectedCount = values.Length;
                if (expectedCount > 0)
                {
                    count = dbContext.Database.ExecuteSqlRawCount($"SELECT COUNT(*) FROM {table} WHERE {this.Column} IN ({parameters})", values);
                }
            }
            else
            {
                expectedCount = 1;
                count = dbContext.Database.ExecuteSqlRawCount($"SELECT COUNT(*) FROM {table} WHERE {this.Column} = {{0}}", value);
            }

            if (expectedCount != count)
            {
                return new ValidationResult($"{this.ModelType.Name} does not exist.");
            }

            return ValidationResult.Success;
        }
    }
}

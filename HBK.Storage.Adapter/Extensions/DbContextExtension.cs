using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HBK.Storage.Adapter.Storages
{
    /// <summary>
    /// <see cref="DbContext"/> 的擴充方法
    /// </summary>
    public static class DbContextExtension
    {
        /// <summary>
        /// 同步關聯表中項目
        /// </summary>
        /// <typeparam name="T">模型類型</typeparam>
        /// <param name="collection">資料庫關聯集合</param>
        /// <param name="data">新的資料集合</param>
        /// <param name="compareSelector">比較選擇函數</param>
        internal static void Sync<T>(this ICollection<T> collection, IEnumerable<T> data, Func<T, object> compareSelector) where T : class
        {
            var removed = collection.Where(r => !data.Any(d => compareSelector(d).Equals(compareSelector(r)))).ToList();
            var added = data.Where(d => !collection.Any(r => compareSelector(r).Equals(compareSelector(d)))).ToList();

            foreach (var item in removed)
            {
                collection.Remove(item);
            }
            foreach (var item in added)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// 取得指定類型的資料集
        /// </summary>
        /// <param name="context"><see cref="DbContext"/> 實體</param>
        /// <param name="type">資料集類型</param>
        /// <returns></returns>
        public static IQueryable Set(this DbContext context, Type type)
        {
            // Get the generic type definition
            MethodInfo method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);

            // Build a method with the specific type argument you're interested in
            method = method.MakeGenericMethod(type);

            return method.Invoke(context, null) as IQueryable;
        }

        /// <summary>
        /// 執行 SQL 語法並回傳單一整數結果
        /// </summary>
        /// <param name="database"><see cref="DatabaseFacade"/> 實體</param>
        /// <param name="sql">SQL 語法</param>
        /// <param name="parameters">SQL 參數</param>
        /// <returns></returns>
        public static int ExecuteSqlRawCount(this DatabaseFacade database, string sql, params object[] parameters)
        {
            int result = -1;

            var connection = database.GetDbConnection();
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = sql;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.CommandText = command.CommandText.Replace($"{{{i}}}", $"@p{i}");

                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@p{i}";
                parameter.Value = parameters[i];
                command.Parameters.Add(parameter);
            }

            using var dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    result = dataReader.GetInt32(0);
                }
            }
            connection.Close();

            return result;
        }
    }
}

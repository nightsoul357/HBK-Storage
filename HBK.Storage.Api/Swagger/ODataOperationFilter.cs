using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HBK.Storage.Adapter.DataAnnotations;
using HBK.Storage.Adapter.Enums;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HBK.Storage.Api.Swagger
{
    /// <summary>
    /// 提供 Swagger 支援 OData 相關參數
    /// </summary>
    public class ODataOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 套用過濾器
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // 移除多餘的 Request Type
            if (operation.RequestBody != null)
            {
                foreach (string contentType in operation.RequestBody.Content.Keys)
                {
                    if (contentType.Contains("odata"))
                    {
                        operation.RequestBody.Content.Remove(contentType);
                    }
                }
            }
            // 移除多餘的 Response Type
            foreach (var response in operation.Responses.Values)
            {
                foreach (string contentType in response.Content.Keys)
                {
                    if (contentType.Contains("odata"))
                    {
                        response.Content.Remove(contentType);
                    }
                }
            }

            // 加入 URL 參數說明
            var queryAttribute = context.MethodInfo.GetCustomAttributes(true)
                .Union(context.MethodInfo.DeclaringType.GetCustomAttributes(true))
                .OfType<EnableQueryAttribute>().FirstOrDefault();

            if (queryAttribute != null)
            {
                SnakeCaseNamingStrategy snakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

                var modelType = context.MethodInfo.GetParameters()
                    .Single(param => typeof(ODataQueryOptions).IsAssignableFrom(param.ParameterType))
                    .ParameterType.GetGenericArguments()[0];
                var allowedFilterProperty = modelType.GetProperties()
                    .Where(prop => prop.GetCustomAttributes<FilterableAttribute>(false).Any())
                    .Select(prop => snakeCaseNamingStrategy.GetPropertyName(prop.Name, false))
                    .ToList();
                var allowedOrderByProperty = modelType.GetProperties()
                    .Where(prop => prop.GetCustomAttributes<SortableAttribute>(false).Any())
                    .Select(prop => snakeCaseNamingStrategy.GetPropertyName(prop.Name, false))
                    .ToList();

                operation.Summary = "🔧" + operation.Summary;

                // Additional OData query options are available for collections of entities only
                if (this.IsEnumerableType(context.MethodInfo.ReturnType))
                {
                    if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Filter))
                    {
                        string link = "http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$filter_System";
                        var allowedFunctions = queryAttribute.AllowedFunctions.FlattenFlags();
                        operation.Parameters.Add(new OpenApiParameter()
                        {
                            Name = "$filter",
                            Description = $"[OData v4]({link}) 篩選指定欄位\n" +
                                $"* 允許的欄位：{string.Join(", ", allowedFilterProperty.Select(x => $"`{x}`"))}\n\n\n" +
                                $"* 允許的函數：{string.Join(", ", allowedFunctions.Select(x => $"`{x.ToString().ToLower()}()`"))}",
                            In = ParameterLocation.Query,
                            Schema = new OpenApiSchema()
                            {
                                Type = "string"
                            }
                        });
                    }
                    if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.OrderBy))
                    {
                        string link = "http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$select_System_1";
                        operation.Parameters.Add(new OpenApiParameter()
                        {
                            Name = "$orderby",
                            Description = $"[OData v4]({link}) 指定結果依照指定欄位排序\n* 允許的欄位：" + string.Join(", ", allowedOrderByProperty.Select(x => $"`{x}`")),
                            In = ParameterLocation.Query,
                            Schema = new OpenApiSchema()
                            {
                                Type = "string"
                            }
                        });
                    }
                    if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Skip))
                    {
                        string link = "http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$skip_System";
                        int? maxSkip = queryAttribute.MaxSkip == 0 ? null : (int?)queryAttribute.MaxSkip;
                        operation.Parameters.Add(new OpenApiParameter()
                        {
                            Name = "$skip",
                            Description = $"[OData v4]({link}) 指定要跳過的資料數量" + (maxSkip == null ? null : "，上限 " + maxSkip),
                            In = ParameterLocation.Query,
                            Schema = new OpenApiSchema()
                            {
                                Type = "integer",
                                Minimum = 0,
                                Maximum = queryAttribute.MaxSkip == 0 ? null : (int?)queryAttribute.MaxSkip
                            }
                        });
                    }
                    if (queryAttribute.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Top))
                    {
                        string link = "http://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part1-protocol/odata-v4.0-errata03-os-part1-protocol-complete.html#_The_$top_System_1";
                        int? maxTop = queryAttribute.MaxTop == 0 ? null : (int?)queryAttribute.MaxTop;
                        operation.Parameters.Add(new OpenApiParameter()
                        {
                            Name = "$top",
                            Description = $"[OData v4]({link}) 指定要取得的資料數量" + (maxTop == null ? null : "，上限 " + maxTop),
                            In = ParameterLocation.Query,
                            Schema = new OpenApiSchema()
                            {
                                Type = "integer",
                                Minimum = 1,
                                Maximum = queryAttribute.MaxTop == 0 ? null : (int?)queryAttribute.MaxTop
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 檢查回傳類型是否為集合
        /// </summary>
        /// <param name="type">要檢查的類型</param>
        /// <returns></returns>
        private bool IsEnumerableType(Type type)
        {
            Type typeToCheck = type;
            if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == typeof(Task<>))
            {
                typeToCheck = typeToCheck.GetGenericArguments()[0];
            }
            if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == typeof(ActionResult<>))
            {
                typeToCheck = typeToCheck.GetGenericArguments()[0];
            }

            return typeof(IEnumerable).IsAssignableFrom(typeToCheck);
        }
    }
}

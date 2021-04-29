using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.Filters;
using HBK.Storage.Api.Models;
using HBK.Storage.Core.Services;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// HBK Storage 使用的基底控制器
    /// </summary>
    [ApiController]
    [TypeFilter(typeof(HBKAuthorizeFilter))]
    public abstract class HBKControllerBase : ControllerBase
    {
        /// <summary>
        /// 取得驗證用的金鑰
        /// </summary>
        protected Lazy<AuthorizeKey> AuthorizeKey { get; }

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>

        public HBKControllerBase()
        {
            this.AuthorizeKey = new Lazy<AuthorizeKey>(() => 
            {
                var keyService = base.HttpContext.RequestServices.GetService<AuthorizeKeyService>();
                var key = base.HttpContext.Request.Headers["HBKey"].First();
                return keyService.FindByKeyValueAsync(key).Result;
            });
        }
        /// <summary>
        /// 回傳 <see cref="PagedResponse{T}"/> 結果
        /// </summary>
        /// <typeparam name="TModel">模型型別</typeparam>
        /// <typeparam name="TReponse">回應型別</typeparam>
        /// <param name="queryOptions">OData 查詢參數</param>
        /// <param name="query">查詢式</param>
        /// <param name="selector">選擇器</param>
        /// <param name="defaultTake">預設回傳資料數量</param>
        /// <returns></returns>
        protected async Task<PagedResponse<TReponse>> PagedResultAsync<TModel, TReponse>(ODataQueryOptions<TModel> queryOptions,
            IQueryable<TModel> query, Func<IEnumerable<TModel>, IEnumerable<TReponse>> selector, int? defaultTake = null)
        {
            var countQuery = query;
            if (queryOptions.Filter != null)
            {
                countQuery = queryOptions.Filter.ApplyTo(query, new ODataQuerySettings()).Cast<TModel>();
            }
            var newQuery = queryOptions.ApplyTo(query).Cast<TModel>();
            if (defaultTake != null)
            {
                newQuery = newQuery.Take(defaultTake.Value);
            }

            return new PagedResponse<TReponse>()
            {
                TotalCount = await countQuery.CountAsync(),
                Value = selector(await newQuery.ToListAsync()).ToArray(),
            };
        }
    }
}
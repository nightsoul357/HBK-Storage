using System;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OData;


namespace HBK.Storage.Api.OData
{
    /// <summary>
    /// 啟用 OData 參數查詢
    /// </summary>
    public class EnableODataQueryAttribute : EnableQueryAttribute
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public EnableODataQueryAttribute()
        {
            // 預設屬性
            this.AllowedFunctions = AllowedFunctions.Contains | AllowedFunctions.Cast | AllowedFunctions.Any | AllowedFunctions.All;
            this.MaxTop = 100;
        }

        /// <inheritdoc/>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ODataQueryOptions queryOptions = (ODataQueryOptions)context.ActionArguments.Values
                .SingleOrDefault(arg => arg is ODataQueryOptions);
            if (queryOptions == null)
            {
                throw new ArgumentException("No ODataQueryOptions argument in action arguments.", nameof(context));
            }

            try
            {
                base.ValidateQuery(context.HttpContext.Request, queryOptions);
            }
            catch (ODataException ex)
            {
                var dic = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
                dic.AddModelError("OData", ex.Message);
                context.Result = new BadRequestObjectResult(dic);
            }
        }

        /// <inheritdoc/>
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            // Do nothing
        }
    }
}

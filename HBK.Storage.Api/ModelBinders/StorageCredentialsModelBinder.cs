using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HBK.Storage.Api.ModelBinders
{
    /// <summary>
    /// 包含存取儲存個體的驗證資訊的資料綁定
    /// </summary>
    public class StorageCredentialsModelBinder : IModelBinder
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="jsonOptions"></param>
        public StorageCredentialsModelBinder(IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
        {
            _jsonSerializerSettings = jsonOptions.Value.SerializerSettings;

        }
        /// <inheritdoc/>
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var streamReader = new StreamReader(bindingContext.HttpContext.Request.Body);
            var content = await streamReader.ReadToEndAsync();
            var model = JsonConvert.DeserializeObject(content, bindingContext.ModelType, _jsonSerializerSettings);
            bindingContext.Result = ModelBindingResult.Success(model);
            return;
        }
    }
}

using HBK.Storage.VideoConvertM3U8Plugin.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoConvertM3U8Plugin.ConvertHandler
{
    public abstract class ConvertHandlerBase
    {
        protected ILogger<ConvertHandlerBase> _logger;
        public ConvertHandlerBase(ILogger<ConvertHandlerBase> logger, ConvertHandlerBase internalConvertHandler = null)
        {
            _logger = logger;
            this.InternalCovertHandler = internalConvertHandler;
        }
        public ConvertHandlerExecuteResult Execute(ConvertHandlerTaskModel taskModel)
        {
            var result = this.ExecuteInternal(taskModel);
            if (result.Success)
            {
                _logger.LogInformation("[{0}] 轉換成功", taskModel.Identity);
                return result;
            }
            _logger.LogInformation("[{0}] 轉換失敗", taskModel.Identity);

            if (this.InternalCovertHandler != null)
            {
                return this.InternalCovertHandler.Execute(taskModel);
            }

            return new ConvertHandlerExecuteResult()
            {
                Success = false
            };
        }

        protected abstract ConvertHandlerExecuteResult ExecuteInternal(ConvertHandlerTaskModel taskModel);
        public ConvertHandlerBase InternalCovertHandler { get; private set; }
    }
}

using HBK.Storage.VideoConvertM3U8Plugin.Models;
using MediaToolkit;
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
        public ConvertHandlerExecuteResult Execute(ConvertHandlerTaskModel taskModel, Action<object, ConvertProgressEventArgs> convertProgress, Action<object, ConversionCompleteEventArgs> conversionComplete)
        {
            var result = this.ExecuteInternal(taskModel, convertProgress, conversionComplete);
            if (result.Success)
            {
                return result;
            }

            if (this.InternalCovertHandler != null)
            {
                return this.InternalCovertHandler.Execute(taskModel, convertProgress, conversionComplete);
            }

            return new ConvertHandlerExecuteResult()
            {
                Success = false
            };
        }

        protected abstract ConvertHandlerExecuteResult ExecuteInternal(ConvertHandlerTaskModel taskModel, Action<object, ConvertProgressEventArgs> convertProgress, Action<object, ConversionCompleteEventArgs> conversionComplete);
        public ConvertHandlerBase InternalCovertHandler { get; private set; }
    }
}

using HBK.Storage.VideoSubTitleCombinePlugin.Models;
using MediaToolkit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSubTitleCombinePlugin.CombineHandler
{
    public abstract class CombineHandlerBase
    {
        protected ILogger<CombineHandlerBase> _logger;
        public CombineHandlerBase(ILogger<CombineHandlerBase> logger, CombineHandlerBase internalCombineHandler = null)
        {
            _logger = logger;
            this.InternalCombineHandler = internalCombineHandler;
        }
        public CombineHandlerExecuteResult Execute(CombineHandlerTaskModel taskModel, Action<object, ConvertProgressEventArgs> convertProgress, Action<object, ConversionCompleteEventArgs> conversionComplete)
        {
            var result = this.ExecuteInternal(taskModel, convertProgress, conversionComplete);
            if (result.Success)
            {
                return result;
            }

            if (this.InternalCombineHandler != null)
            {
                return this.InternalCombineHandler.Execute(taskModel, convertProgress, conversionComplete);
            }

            return new CombineHandlerExecuteResult()
            {
                Success = false
            };
        }

        protected abstract CombineHandlerExecuteResult ExecuteInternal(CombineHandlerTaskModel taskModel, Action<object, ConvertProgressEventArgs> convertProgress, Action<object, ConversionCompleteEventArgs> conversionComplete);
        public CombineHandlerBase InternalCombineHandler { get; private set; }

    }
}

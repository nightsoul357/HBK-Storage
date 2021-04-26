using HBK.Storage.VideoSubTitleCombinePlugin.Models;
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
        public CombineHandlerExecuteResult Execute(CombineHandlerTaskModel taskModel)
        {
            var result = this.ExecuteInternal(taskModel);
            if (result.Success)
            {
                _logger.LogInformation("[{0}] 合併成功", taskModel.Identity);
                return result;
            }
            _logger.LogInformation("[{0}] 合併失敗", taskModel.Identity);

            if (this.InternalCombineHandler != null)
            {
                return this.InternalCombineHandler.Execute(taskModel);
            }

            return new CombineHandlerExecuteResult()
            {
                Success = false
            };
        }

        protected abstract CombineHandlerExecuteResult ExecuteInternal(CombineHandlerTaskModel taskModel);
        public CombineHandlerBase InternalCombineHandler { get; private set; }

    }
}

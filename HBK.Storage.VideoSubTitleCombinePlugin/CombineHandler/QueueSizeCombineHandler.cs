using HBK.Storage.VideoSubTitleCombinePlugin.Models;
using MediaToolkit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSubTitleCombinePlugin.CombineHandler
{
    public class QueueSizeCombineHandler : CombineHandlerBase
    {
        private CombineHandlerTaskModel _currentTaskModel;
        public QueueSizeCombineHandler(ILogger<QueueSizeCombineHandler> logger, CombineHandlerBase internalCombineHandler = null)
            : base(logger, internalCombineHandler)
        {
        }

        protected override CombineHandlerExecuteResult ExecuteInternal(CombineHandlerTaskModel taskModel, Action<object, ConvertProgressEventArgs> convertProgress, Action<object, ConversionCompleteEventArgs> conversionComplete)
        {
            _currentTaskModel = taskModel;
            using (var engine = new Engine(taskModel.FFmpegLocation))
            {
                engine.ConversionCompleteEvent += new EventHandler<ConversionCompleteEventArgs>(conversionComplete);
                engine.ConvertProgressEvent += new EventHandler<ConvertProgressEventArgs>(convertProgress);
                engine.CustomCommand($@"-i { taskModel.VideoFileName } -max_muxing_queue_size 8196 -vf ""{Path.GetExtension(taskModel.SubTitleFileName).Remove(0, 1)}={ taskModel.SubTitleFileName }"" {taskModel.OutputFileName}");
                var fileInfo = new FileInfo(taskModel.OutputFileName);
                return new CombineHandlerExecuteResult()
                {
                    Success = fileInfo.Exists && fileInfo.Length != 0
                };
            }
        }
    }
}

using HBK.Storage.VideoConvertM3U8Plugin.Models;
using MediaToolkit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoConvertM3U8Plugin.ConvertHandler
{
    public class ClearConvertHandler : ConvertHandlerBase
    {
        private ConvertHandlerTaskModel _currentTaskModel;

        public ClearConvertHandler(ILogger<ClearConvertHandler> logger, ConvertHandlerBase internalConvertHandler = null)
            : base(logger, internalConvertHandler)
        {
        }

        protected override ConvertHandlerExecuteResult ExecuteInternal(ConvertHandlerTaskModel taskModel, Action<object, ConvertProgressEventArgs> convertProgress, Action<object, ConversionCompleteEventArgs> conversionComplete)
        {
            _currentTaskModel = taskModel;
            using (var engine = new Engine(taskModel.FFmpegLocation))
            {
                engine.ConversionCompleteEvent += new EventHandler<ConversionCompleteEventArgs>(conversionComplete);
                engine.ConvertProgressEvent += new EventHandler<ConvertProgressEventArgs>(convertProgress);
                engine.CustomCommand($"-i { taskModel.VideoFileName } -max_muxing_queue_size 8196 -f hls -hls_list_size 0 -hls_time { taskModel.TSInterval } { taskModel.OutputFileName }");
                var fileInfo = new FileInfo(taskModel.OutputFileName);
                return new ConvertHandlerExecuteResult()
                {
                    Success = fileInfo.Exists && fileInfo.Length != 0
                };
            }
        }
    }
}

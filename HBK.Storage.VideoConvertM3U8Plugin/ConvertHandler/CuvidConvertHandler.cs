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
    public class CuvidConvertHandler : ConvertHandlerBase
    {
        private ConvertHandlerTaskModel _currentTaskModel;

        public CuvidConvertHandler(ILogger<ConvertHandlerBase> logger, ConvertHandlerBase internalConvertHandler = null)
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
                engine.CustomCommand($@"-hwaccel cuvid -c:v h264_cuvid -i { taskModel.VideoFileName } -c:v h264_nvenc -c:a aac -strict -2 -f hls -hls_list_size 0  -hls_time { taskModel.TSInterval } { taskModel.OutputFileName }");
                var fileInfo = new FileInfo(taskModel.OutputFileName);
                return new ConvertHandlerExecuteResult()
                {
                    Success = fileInfo.Exists && fileInfo.Length != 0
                };
            }
        }
    }
}

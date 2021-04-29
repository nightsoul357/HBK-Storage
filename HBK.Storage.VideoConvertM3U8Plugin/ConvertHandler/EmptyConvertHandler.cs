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
    public class EmptyConvertHandler : ConvertHandlerBase
    {
        private ConvertHandlerTaskModel _currentTaskModel;

        public EmptyConvertHandler(ILogger<EmptyConvertHandler> logger, ConvertHandlerBase internalConvertHandler = null)
            : base(logger, internalConvertHandler)
        {
        }

        protected override ConvertHandlerExecuteResult ExecuteInternal(ConvertHandlerTaskModel taskModel)
        {
            _currentTaskModel = taskModel;
            using (var engine = new Engine(taskModel.FFmpegLocation))
            {
                engine.ConversionCompleteEvent += this.Engine_ConversionCompleteEvent;
                engine.ConvertProgressEvent += this.Engine_ConvertProgressEvent;
                engine.CustomCommand($"-i { taskModel.VideoFileName } -max_muxing_queue_size 8196 -f hls -hls_list_size 0 -hls_time { taskModel.TSInterval } { taskModel.OutputFileName }");
                var fileInfo = new FileInfo(taskModel.OutputFileName);
                return new ConvertHandlerExecuteResult()
                {
                    Success = fileInfo.Exists && fileInfo.Length != 0
                };
            }
        }

        private void Engine_ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[{ _currentTaskModel.Identity }]");
            sb.AppendLine("------------Converting------------");
            sb.AppendLine($"Bitrate: {e.Bitrate}");
            sb.AppendLine($"Fps: {e.Fps}");
            sb.AppendLine($"Frame: {e.Frame}");
            sb.AppendLine($"ProcessedDuration: {e.ProcessedDuration}");
            sb.AppendLine($"SizeKb: {e.Bitrate}");
            sb.AppendLine($"TotalDuration: {e.TotalDuration}");
            sb.AppendLine($"Speed: {e.Speed}");

            base._logger.LogInformation(sb.ToString());
        }

        private void Engine_ConversionCompleteEvent(object sender, ConversionCompleteEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[{ _currentTaskModel.Identity }]");
            sb.AppendLine("------------Convert Complete------------");
            sb.AppendLine($"Bitrate: {e.Bitrate}");
            sb.AppendLine($"Fps: {e.Fps}");
            sb.AppendLine($"Frame: {e.Frame}");
            sb.AppendLine($"ProcessedDuration: {e.ProcessedDuration}");
            sb.AppendLine($"SizeKb: {e.Bitrate}");
            sb.AppendLine($"TotalDuration: {e.TotalDuration}");
            base._logger.LogInformation(sb.ToString());
        }
    }
}

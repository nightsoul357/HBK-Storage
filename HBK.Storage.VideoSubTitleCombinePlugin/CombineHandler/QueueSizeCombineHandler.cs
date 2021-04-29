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

        protected override CombineHandlerExecuteResult ExecuteInternal(CombineHandlerTaskModel taskModel)
        {
            _currentTaskModel = taskModel;
            using (var engine = new Engine(taskModel.FFmpegLocation))
            {
                engine.ConversionCompleteEvent += this.Engine_ConversionCompleteEvent;
                engine.ConvertProgressEvent += this.Engine_ConvertProgressEvent;
                engine.CustomCommand($@"-i { taskModel.VideoFileName } -max_muxing_queue_size 8196 -vf ""{Path.GetExtension(taskModel.SubTitleFileName).Remove(0, 1)}={ taskModel.SubTitleFileName }"" {taskModel.OutputFileName}");
                var fileInfo = new FileInfo(taskModel.OutputFileName);
                return new CombineHandlerExecuteResult()
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

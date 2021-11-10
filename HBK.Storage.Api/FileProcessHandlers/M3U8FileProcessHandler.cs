using HBK.Storage.Core.FileSystem.Memory;
using HBK.Storage.Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileProcessHandlers
{
    /// <summary>
    /// 處理使用 M3U8 影片轉換插件轉換完成的 M3U8 檔案實體
    /// </summary>
    public class M3U8FileProcessHandler : FileProcessHandlerBase
    {
        private readonly ILogger<M3U8FileProcessHandler> _logger;
        private readonly FileEntityService _fileEntityService;
        private readonly FileAccessTokenService _fileAccessTokenService;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="fileEntityService"></param>
        /// <param name="fileAccessTokenService"></param>
        public M3U8FileProcessHandler(ILogger<M3U8FileProcessHandler> logger, FileEntityService fileEntityService, FileAccessTokenService fileAccessTokenService)
        {
            _logger = logger;
            _fileEntityService = fileEntityService;
            _fileAccessTokenService = fileAccessTokenService;
        }

        /// <inheritdoc/>
        public override async Task<FileProcessTaskModel> ProcessAsync(FileProcessTaskModel taskModel)
        {
            if (taskModel.FileEntity.MimeType != "application/x-mpegURL")
            {
                return taskModel;
            }

            Guid taskId = Guid.NewGuid();

            var tsFiles = (await _fileEntityService.GetChildFileEntitiesAsync(taskModel.FileEntity.FileEntityId))
                .Where(x => x.FileEntityTag.Any(t => t.Value.Contains("ts-")) && x.MimeType == "video/MP2T" && !x.Status.HasFlag(Adapter.Enums.FileEntityStatusEnum.Processing));

            var token = _fileAccessTokenService.GenerateAllowTagNoLimitFileAccessToken(taskModel.StorageProviderId, null, "ts-", taskModel.Token.ValidTo, string.Empty);

            using var sr = new StreamReader(taskModel.FileInfo.CreateReadStream());
            var m3u8Content = sr.ReadToEnd();

            tsFiles.ToList().ForEach(ts =>
            {
                string newAccessTarget = "/docs/{0}/filename/{1}?esic={2}";
                newAccessTarget = string.Format(newAccessTarget, ts.FileEntityId, ts.Name, token);
                m3u8Content = m3u8Content.Replace(ts.Name, newAccessTarget);
            });

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(m3u8Content));
            taskModel.FileInfo = new MemoryFileInfo(ms);

            return taskModel;
        }

        /// <inheritdoc/>
        public override string Name => "m3u8";
    }
}
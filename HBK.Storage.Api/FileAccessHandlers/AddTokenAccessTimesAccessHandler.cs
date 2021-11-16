using HBK.Storage.Api.FileAccessHandlers.Models;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 新增 Token 存取次數檔案存取處理器
    /// </summary>
    public class AddTokenAccessTimesAccessHandler : IFileAccessHandler
    {
        private readonly FileAccessTokenService _fileAccessTokenService;
        /// <summary>
        /// 初始化新增存取次數檔案存取處理器
        /// </summary>
        /// <param name="fileAccessTokenService"></param>
        public AddTokenAccessTimesAccessHandler(FileAccessTokenService fileAccessTokenService)
        {
            _fileAccessTokenService = fileAccessTokenService;
        }
        /// <inheritdoc/>
        public async Task<FileAccessTaskModel> ProcessAsync(FileAccessTaskModel taskModel)
        {
            FileAccessTokenTypeEnum tokenType = Enum.Parse<FileAccessTokenTypeEnum>(taskModel.MiddleData.JwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tokenType").Value);
            if (tokenType == FileAccessTokenTypeEnum.Normal ||
                tokenType == FileAccessTokenTypeEnum.AllowTag)
            {
                await _fileAccessTokenService.TryAddAccessTimesAsync(taskModel.MiddleData.FileAccessToken.FileAccessTokenId);
            }
            return taskModel;
        }
    }
}

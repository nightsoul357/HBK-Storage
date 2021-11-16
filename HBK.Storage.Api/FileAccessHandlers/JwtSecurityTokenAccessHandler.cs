using HBK.Storage.Api.FileAccessHandlers.Models;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 處理 JWT 的檔案存取處理器
    /// </summary>
    public class JwtSecurityTokenAccessHandler : IFileAccessHandler
    {
        private FileAccessTokenTypeEnum[] _fileAccessTokenTypes;
        private FileAccessTokenService _fileAccessTokenService;
        /// <summary>
        /// 初始化處理 JWT 的檔案存取處理器
        /// </summary>
        /// <param name="fileAccessTokenService"></param>
        /// <param name="fileAccessTokenTypes"></param>
        public JwtSecurityTokenAccessHandler(FileAccessTokenService fileAccessTokenService, params FileAccessTokenTypeEnum[] fileAccessTokenTypes)
        {
            _fileAccessTokenTypes = fileAccessTokenTypes;
            _fileAccessTokenService = fileAccessTokenService;
        }

        /// <inheritdoc/>
        public async Task<FileAccessTaskModel> ProcessAsync(FileAccessTaskModel taskModel)
        {
            JwtSecurityToken jwtSecurityToken = await _fileAccessTokenService.ValidateFileAccessTokenAsync(taskModel.Request.Esic);
            taskModel.MiddleData.JwtSecurityToken = jwtSecurityToken;
            FileAccessTokenTypeEnum tokenType = Enum.Parse<FileAccessTokenTypeEnum>(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "tokenType").Value);
            taskModel.MiddleData.FileAccessToken = _fileAccessTokenService.BuildFileAccessToken(jwtSecurityToken);

            if (_fileAccessTokenTypes.All(x => tokenType != x))
            {
                taskModel.ErrorObject = "使用的權杖類型無法透過此路徑存取檔案";
            }

            return taskModel;
        }
    }
}

using HBK.Storage.Api.FileAccessHandlers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileAccessHandlers
{
    /// <summary>
    /// 驗證 Tag 格式的檔案存取處理器
    /// </summary>
    public class ValidateTagPatternAccessHandler : IFileAccessHandler
    {
        /// <inheritdoc/>
        public Task<FileAccessTaskModel> ProcessAsync(FileAccessTaskModel taskModel)
        {
            var allowTagPattern = taskModel.MiddleData.JwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "allowTagPattern").Value;
            if (taskModel.MiddleData.FileEntity.FileEntityTag.All(x => !Regex.IsMatch(x.Value, allowTagPattern)))
            {
                taskModel.ErrorObject = "沒有檔案存取權限";
            }
            return Task.FromResult(taskModel);
        }
    }
}

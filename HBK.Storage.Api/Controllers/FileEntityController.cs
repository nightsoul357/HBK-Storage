using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.Models.FileEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 檔案實體控制器
    /// </summary>
    public class FileEntityController : HBKControllerBase
    {
        /// <summary>
        /// 產生檔案實體回應內容
        /// </summary>
        /// <param name="fileEntity">檔案實體</param>
        /// <returns></returns>
        internal static FileEntityResponse BuildFileEntityResponse(FileEntity fileEntity)
        {
            return new FileEntityResponse()
            {
                CreateDateTime = fileEntity.CreateDateTime.LocalDateTime,
                ExtendProperty = fileEntity.ExtendProperty,
                FileEntityId = fileEntity.FileEntityId,
                MimeType = fileEntity.MimeType,
                Name = fileEntity.Name,
                Size = fileEntity.Size,
                Status = fileEntity.Status.FlattenFlags(),
                Tags = fileEntity.Tags,
                UpdateDateTime = fileEntity.UpdateDateTime?.LocalDateTime
            };
        }
    }
}

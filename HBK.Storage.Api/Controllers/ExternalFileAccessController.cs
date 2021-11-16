using HBK.Storage.Adapter.Storages;
using HBK.Storage.Api.DataAnnotations;
using HBK.Storage.Api.FileAccessHandlers;
using HBK.Storage.Api.FileAccessHandlers.Models;
using HBK.Storage.Api.FileProcessHandlers;
using HBK.Storage.Api.Models;
using HBK.Storage.Core.Cryptography;
using HBK.Storage.Core.Enums;
using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// 外部檔案存取控制器
    /// </summary>
    [AllowAnonymous]
    [Route("/docs")]
    public class ExternalFileAccessController : HBKControllerBase
    {
        private readonly FileAccessHandlerProxy _fileAccessHandlerProxy;
        /// <summary>
        /// 建構一個新的執行個體
        /// </summary>
        /// <param name="fileAccessHandlerProxy"></param>
        public ExternalFileAccessController(FileAccessHandlerProxy fileAccessHandlerProxy)
        {
            _fileAccessHandlerProxy = fileAccessHandlerProxy;
        }
        /// <summary>
        /// 下載檔案(未帶入 esic 為下載公開檔案)
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="esic">存取權杖(該參數可於 Query 或 Header 內則一處帶入)</param>
        /// <returns></returns>
        [HttpGet("{fileEntityId}")]
        [HeaderParameter(Name = "esic", Description = "存取權杖(該參數可於 Query 或 Header 內則一處帶入)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [FileStreamResultResponse]
        public async Task<ActionResult> DirectDownload(
            [ExistInDatabase(typeof(FileEntity))]
            [ExampleParameter("cfa83790-007c-4ba2-91b2-5b18dfe08735")]Guid fileEntityId,
            [FromQuery] string esic)
        {
            if (string.IsNullOrEmpty(esic) && base.Request.Headers.ContainsKey("esic"))
            {
                esic = base.Request.Headers["esic"];
            }

            FileAccessTaskModel result;
            if (string.IsNullOrEmpty(esic))
            {
                result = await _fileAccessHandlerProxy.DoDirectForPublicFileEntityDownloadAsync(fileEntityId);
            }
            else
            {
                result = await _fileAccessHandlerProxy.DoDirectForFileEntityIdDownloadAsync(fileEntityId, esic);
            }

            if (result.ErrorObject != null)
            {
                return base.BadRequest(result.ErrorObject);
            }
            return new MapHeaderFileStreamResult(result.FileInfo.CreateReadStream(), result.MiddleData.FileEntity.MimeType)
            {
                FileDownloadName = result.MiddleData.FileEntity.Name,
            };
        }
        /// <summary>
        /// 下載檔案(未帶入 esic 為下載公開檔案)
        /// </summary>
        /// <param name="fileEntityId">檔案實體 ID</param>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="esic">存取權杖(該參數可於 Query 或 Header 內則一處帶入)</param>
        /// <returns></returns>
        [HttpGet("{fileEntityId}/filename/{fileName}")]
        [HeaderParameter(Name = "esic", Description = "存取權杖(該參數可於 Query 或 Header 內則一處帶入)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [FileStreamResultResponse]
        public async Task<ActionResult> DirectDownload(
            [ExistInDatabase(typeof(FileEntity))]
            [ExampleParameter("cfa83790-007c-4ba2-91b2-5b18dfe08735")]Guid fileEntityId,
            [ExampleParameter("test.mp4")] string fileName,
            [FromQuery] string esic)
        {
            if (string.IsNullOrEmpty(esic) && base.Request.Headers.ContainsKey("esic"))
            {
                esic = base.Request.Headers["esic"];
            }

            FileAccessTaskModel result;
            if (string.IsNullOrEmpty(esic))
            {
                result = await _fileAccessHandlerProxy.DoDirectForPublicFileEntityDownloadAsync(fileEntityId);
            }
            else
            {
                result = await _fileAccessHandlerProxy.DoDirectForFileEntityIdDownloadAsync(fileEntityId, esic);
            }

            if (result.ErrorObject != null)
            {
                return base.BadRequest(result.ErrorObject);
            }
            return new MapHeaderFileStreamResult(result.FileInfo.CreateReadStream(), result.MiddleData.FileEntity.MimeType, fileName)
            {
                FileDownloadName = result.MiddleData.FileEntity.Name
            };
        }
        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="esic">存取權杖(該參數可於 Query 或 Header 內則一處帶入)</param>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HeaderParameter(Name = "esic", Description = "存取權杖(該參數可於 Query 或 Header 內則一處帶入)")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [FileStreamResultResponse]
        public async Task<ActionResult> DirectDownload([FromQuery] string esic)
        {
            if (string.IsNullOrEmpty(esic) && base.Request.Headers.ContainsKey("esic"))
            {
                esic = base.Request.Headers["esic"];
            }

            var result = await _fileAccessHandlerProxy.DoDirectDownloadAsync(esic);
            if (result.ErrorObject != null)
            {
                return base.BadRequest(result.ErrorObject);
            }

            return new MapHeaderFileStreamResult(result.FileInfo.CreateReadStream(), result.MiddleData.FileEntity.MimeType)
            {
                FileDownloadName = result.MiddleData.FileEntity.Name
            };
        }
        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="esic">存取權杖(該參數可於 Query 或 Header 內則一處帶入)</param>
        /// <param name="fileName">檔案名稱</param>
        /// <returns></returns>
        [HttpGet("filename/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HeaderParameter(Name = "esic", Description = "存取權杖(該參數可於 Query 或 Header 內則一處帶入)")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [FileStreamResultResponse]
        public async Task<ActionResult> DirectDownload(
            [ExampleParameter("test.mp4")] string fileName,
            [FromQuery] string esic)
        {
            if (string.IsNullOrEmpty(esic) && base.Request.Headers.ContainsKey("esic"))
            {
                esic = base.Request.Headers["esic"];
            }

            var result = await _fileAccessHandlerProxy.DoDirectDownloadAsync(esic);
            if (result.ErrorObject != null)
            {
                return base.BadRequest(result.ErrorObject);
            }
            return new MapHeaderFileStreamResult(result.FileInfo.CreateReadStream(), result.MiddleData.FileEntity.MimeType, fileName)
            {
                FileDownloadName = result.MiddleData.FileEntity.Name
            };
        }
    }
}
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.FileSystem.AmazonS3
{
    /// <summary>
    /// AWS S3 儲存服務提供者
    /// </summary>
    public class AwsS3FileProvider : AsyncFileProvider
    {
        private readonly AmazonS3Client _client;

        /// <summary>
        /// 取得 Bucket 名稱
        /// </summary>
        public string BucketName { get; }

        /// <summary>
        /// 取得預設的網址過期時間
        /// </summary>
        public TimeSpan DefaultExipreTime => TimeSpan.FromMinutes(10);

        /// <summary>
        /// 取得緩衝區大小
        /// </summary>
        public int BufferSize { get; } = 5 * 1024 * 1024;

        /// <summary>
        /// 取得緩衝區數量
        /// </summary>
        public int BufferCount { get; } = 100;

        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="name">提供者名稱</param>
        /// <param name="accessKey">存取金鑰</param>
        /// <param name="secretKey">存取密鑰</param>
        /// <param name="bucketName">Bucket 名稱</param>
        /// <param name="config">設定值</param>
        public AwsS3FileProvider(string name, string accessKey, string secretKey, string bucketName, AmazonS3Config config) : base(name)
        {
            _client = new AmazonS3Client(accessKey, secretKey, config);
            this.BucketName = bucketName;
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        public override async Task DeleteAsync(string subpath)
        {
            _ = await _client.DeleteObjectAsync(new DeleteObjectRequest()
            {
                BucketName = this.BucketName,
                Key = subpath
            });
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="subpathes">檔案相對路徑集合</param>
        public override async Task DeleteAsync(string[] subpathes)
        {
            var response = await _client.DeleteObjectsAsync(new DeleteObjectsRequest()
            {
                BucketName = this.BucketName,
                Objects = subpathes.Select(subpath => new KeyVersion()
                {
                    Key = subpath
                }).ToList()
            });
        }

        /// <summary>
        /// 取得目錄下的檔案
        /// </summary>
        /// <param name="subpath">相對路徑</param>
        /// <returns></returns>
        public override async Task<IDirectoryContents> GetDirectoryContentsAsync(string subpath)
        {
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = this.BucketName,
                Prefix = subpath.EndsWith("/") ? subpath : subpath + "/"
            };
            List<AwsS3FileInfo> fileInfos = new List<AwsS3FileInfo>();
            ListObjectsV2Response response;
            do
            {
                response = await _client.ListObjectsV2Async(request);

                foreach (S3Object entry in response.S3Objects)
                {
                    fileInfos.Add(new AwsS3FileInfo(_client, entry.Key, this.BucketName, this.GetPreSignedUrl(subpath, this.DefaultExipreTime).AbsoluteUri, entry));
                }

                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return new AwsS3DirectoryContents(fileInfos);
        }

        /// <summary>
        /// 取得檔案資訊
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <returns></returns>
        public override async Task<IAsyncFileInfo> GetFileInfoAsync(string subpath)
        {
            var response = await _client.GetObjectMetadataAsync(new GetObjectMetadataRequest()
            {
                BucketName = this.BucketName,
                Key = subpath
            });

            return new AwsS3FileInfo(_client, subpath, this.BucketName, this.GetPreSignedUrl(subpath, this.DefaultExipreTime).AbsoluteUri, response);
        }

        /// <summary>
        /// 新增檔案
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <param name="fileStream">檔案流</param>
        /// <returns></returns>
        public override async Task<IAsyncFileInfo> PutAsync(string subpath, Stream fileStream)
        {
            InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest
            {
                BucketName = this.BucketName,
                Key = subpath
            };

            InitiateMultipartUploadResponse initResponse = await _client.InitiateMultipartUploadAsync(initiateRequest);
            List<UploadPartResponse> uploadResponses = new List<UploadPartResponse>();

            BlockingCollection<byte[]> commonBuffer = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>(), this.BufferCount);
            Task readTask = new Task(() =>
            {
                byte[] buffer = new byte[this.BufferSize];
                int read;
                int position = 0;
                while ((read = fileStream.ReadAsync(buffer, position, buffer.Length - position).Result) != 0)
                {
                    position += read;
                    if (position == buffer.Length)
                    {
                        commonBuffer.Add(buffer);
                        buffer = new byte[this.BufferSize];
                        position = 0;
                    }
                }

                if (position != 0)
                {
                    commonBuffer.Add(buffer.Take(position).ToArray());
                }
                commonBuffer.CompleteAdding();
            });

            Task writeTask = new Task(() =>
            {
                int partNum = 1;
                while (!commonBuffer.IsAddingCompleted || commonBuffer.Count != 0)
                {
                    var takeResult = commonBuffer.TryTake(out byte[] buffer);
                    if (!takeResult)
                    {
                        continue;
                    }

                    using MemoryStream memoryStream = new MemoryStream(buffer);
                    UploadPartRequest uploadRequest = new UploadPartRequest
                    {
                        BucketName = this.BucketName,
                        Key = subpath,
                        UploadId = initResponse.UploadId,
                        PartNumber = partNum,
                        PartSize = memoryStream.Length,
                        InputStream = memoryStream
                    };
                    uploadResponses.Add(_client.UploadPartAsync(uploadRequest).Result);
                    partNum++;
                }
            });

            readTask.Start();
            writeTask.Start();
            Task.WaitAll(readTask, writeTask);

            CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest
            {
                BucketName = this.BucketName,
                Key = subpath,
                UploadId = initResponse.UploadId
            };
            completeRequest.AddPartETags(uploadResponses);
            CompleteMultipartUploadResponse completeUploadResponse =
                    await _client.CompleteMultipartUploadAsync(completeRequest);

            return await this.GetFileInfoAsync(subpath);
        }
        /// <summary>
        /// 指定的檔案是否存在
        /// </summary>
        /// <param name="subpath">檔案的相對路徑</param>
        /// <returns></returns>
        public override async Task<bool> IsFileExistAsync(string subpath)
        {
            try
            {
                _ = await this.GetFileInfoAsync(subpath);

                return true;
            }

            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
        /// <summary>
        /// 取得檔案的 URL
        /// </summary>
        /// <param name="subpath">檔案相對路徑</param>
        /// <param name="expire">過期時間</param>
        /// <returns></returns>
        public Uri GetPreSignedUrl(string subpath, TimeSpan expire)
        {
            string url = _client.GetPreSignedURL(new GetPreSignedUrlRequest()
            {
                Protocol = _client.Config.UseHttp ? Protocol.HTTP : Protocol.HTTPS,
                BucketName = this.BucketName,
                Key = subpath,
                Expires = DateTime.Now.Add(expire)
            });

            return new Uri(url, UriKind.Absolute);
        }
        /// <summary>
        /// 監視檔案變更
        /// </summary>
        /// <param name="filter">篩選器</param>
        /// <returns></returns>
        public override IChangeToken Watch(string filter)
        {
            throw new NotSupportedException();
        }
    }
}

using HBK.Storage.Adapter.StorageCredentials;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.ValueConversions
{
    /// <summary>
    /// 存取儲存個體的驗證資訊轉換器
    /// </summary>
    internal class StorageCredentialsConverter : ValueConverter<StorageCredentialsBase, string>
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="mappingHints"></param>
        public StorageCredentialsConverter(ConverterMappingHints mappingHints = null)
            : base(
                  target => JsonConvert.SerializeObject(target),
                  str => StorageCredentialsConverter.ConvertFromString(str),
                  mappingHints)
        {
        }
        private static StorageCredentialsBase ConvertFromString(string source)
        {
            StorageCredentialsBase obj = JsonConvert.DeserializeObject<FTPStorageCredentials>(source);
            switch (obj.StorageType)
            {
                case Enums.StorageTypeEnum.FTP:
                    obj = JsonConvert.DeserializeObject<FTPStorageCredentials>(source);
                    break;
                case Enums.StorageTypeEnum.AmazonS3:
                    obj = JsonConvert.DeserializeObject<AmazonS3StorageCredentials>(source);
                    break;
                default:
                    throw new NotImplementedException($"尚未實作轉換至 { obj.StorageType } 的方式");
            }
            return obj;
        }
    }
}

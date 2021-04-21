using HBK.Storage.Adapter.Enums;
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
    public class StorageCredentialsConverter : ValueConverter<StorageCredentialsBase, string>
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="mappingHints"></param>
        public StorageCredentialsConverter(ConverterMappingHints mappingHints = null)
            : base(
                  target => StorageCredentialsConverter.ConvertFromStorageCredentialsBase(target),
                  str => StorageCredentialsConverter.ConvertFromString(str),
                  mappingHints)
        {
        }
        private static string ConvertFromStorageCredentialsBase(StorageCredentialsBase storageCredentialsBase)
        {
            return JsonConvert.SerializeObject(storageCredentialsBase);
        }
        private static StorageCredentialsBase ConvertFromString(string source)
        {
            StorageCredentialsBase obj = JsonConvert.DeserializeObject<FTPStorageCredentials>(source);
            return (StorageCredentialsBase)JsonConvert.DeserializeObject(source, obj.StorageType.GetConvertType());
        }
    }
}
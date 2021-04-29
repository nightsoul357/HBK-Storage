using HBK.Storage.Adapter.Enums;
using HBK.Storage.Adapter.StorageCredentials;
using HBK.Storage.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.JsonConverters
{
    /// <summary>
    /// 存取儲存個體的驗證資訊轉換器
    /// </summary>
    public class StorageCredentialsJsonConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StorageCredentialsBase);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return string.Empty;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return serializer.Deserialize(reader, objectType);
            }
            else
            {
                JObject obj = JObject.Load(reader);
                string source = obj.ToString();
                var settings = serializer.CopySettings();
                StorageCredentialsBase result = JsonConvert.DeserializeObject<FTPStorageCredentials>(source, settings);
                return (StorageCredentialsBase)JsonConvert.DeserializeObject(source, result.StorageType.GetConvertType());
            }
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}

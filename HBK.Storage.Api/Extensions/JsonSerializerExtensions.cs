using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Extensions
{
    /// <summary>
    /// Json Serializer 輔助類別
    /// </summary>
    public static class JsonSerializerExtensions
    {
        /// <summary>
        /// 取得 Serializer 的設定檔副本
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static JsonSerializerSettings CopySettings(this JsonSerializer serializer)
        {
            var copiedSerializerSettings = new JsonSerializerSettings
            {
                Context = serializer.Context,
                Culture = serializer.Culture,
                ContractResolver = serializer.ContractResolver,
                ConstructorHandling = serializer.ConstructorHandling,
                CheckAdditionalContent = serializer.CheckAdditionalContent,
                DateFormatHandling = serializer.DateFormatHandling,
                DateFormatString = serializer.DateFormatString,
                DateParseHandling = serializer.DateParseHandling,
                DateTimeZoneHandling = serializer.DateTimeZoneHandling,
                DefaultValueHandling = serializer.DefaultValueHandling,
                EqualityComparer = serializer.EqualityComparer,
                FloatFormatHandling = serializer.FloatFormatHandling,
                Formatting = serializer.Formatting,
                FloatParseHandling = serializer.FloatParseHandling,
                MaxDepth = serializer.MaxDepth,
                MetadataPropertyHandling = serializer.MetadataPropertyHandling,
                MissingMemberHandling = serializer.MissingMemberHandling,
                NullValueHandling = serializer.NullValueHandling,
                ObjectCreationHandling = serializer.ObjectCreationHandling,
                PreserveReferencesHandling = serializer.PreserveReferencesHandling,
                ReferenceResolver = serializer.ReferenceResolver,
                ReferenceLoopHandling = serializer.ReferenceLoopHandling,
                StringEscapeHandling = serializer.StringEscapeHandling,
                TraceWriter = serializer.TraceWriter,
                TypeNameHandling = serializer.TypeNameHandling,
                SerializationBinder = serializer.SerializationBinder,
                TypeNameAssemblyFormatHandling = serializer.TypeNameAssemblyFormatHandling
            };
            foreach (var converter in serializer.Converters)
            {
                copiedSerializerSettings.Converters.Add(converter);
            }
            return copiedSerializerSettings;
        }
    }
}

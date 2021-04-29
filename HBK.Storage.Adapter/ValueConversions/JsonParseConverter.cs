using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace HBK.Storage.Adapter.ValueConversions
{
    /// <summary>
    /// Json 字串與強行別的轉換器
    /// </summary>
    /// <typeparam name="TTarget">指定的強型別</typeparam>
    internal class JsonParseConverter<TTarget> : ValueConverter<TTarget, string>
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="mappingHints"></param>
        public JsonParseConverter(ConverterMappingHints mappingHints = null)
            : base(
                  target => JsonConvert.SerializeObject(target),
                  str => JsonConvert.DeserializeObject<TTarget>(str),
                  mappingHints)
        {
        }
    }
}
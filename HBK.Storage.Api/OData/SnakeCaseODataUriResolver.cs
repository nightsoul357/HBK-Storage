using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace HBK.Storage.Api.OData
{
    /// <summary>
    /// 小寫底線命名的 URL 剖析器
    /// </summary>
    public class SnakeCaseODataUriResolver : ODataUriResolver
    {
        /// <summary>
        /// 建立一個 新的執行個體
        /// </summary>
        public SnakeCaseODataUriResolver()
        {
            base.EnableNoDollarQueryOptions = true;
            base.EnableCaseInsensitive = true;
        }

        /// <inheritdoc/>
        public override IEdmProperty ResolveProperty(IEdmStructuredType type, string propertyName)
        {
            // 同時允許使用原生的屬性名稱與小寫屬型名稱存取
            if (type is EdmEntityType entityType && entityType.FindProperty(propertyName) != null)
            {
                return base.ResolveProperty(type, propertyName);
            }
            return base.ResolveProperty(type, this.ConvertToPascalCase(propertyName));
        }

        /// <summary>
        /// 轉換 Snake case 為 Pascal case
        /// </summary>
        /// <param name="snakeCase">要轉換的字串</param>
        /// <returns></returns>
        private string ConvertToPascalCase(string snakeCase)
        {
            return string.Join("", snakeCase.Split('_').Select(str => str.First().ToString().ToUpper() + str.Substring(1).ToLower()));
        }
    }
}

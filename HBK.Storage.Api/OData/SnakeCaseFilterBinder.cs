using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace HBK.Storage.Api.OData
{
    /// <summary>
    /// 小寫命名的 <see cref="Microsoft.AspNet.OData.Query.Expressions.FilterBinder"/>
    /// </summary>
    public class SnakeCaseFilterBinder : FilterBinder
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="requestContainer"></param>
        public SnakeCaseFilterBinder(IServiceProvider requestContainer) : base(requestContainer)
        {
        }

        /// <inheritdoc/>
        public override Expression BindCollectionConstantNode(CollectionConstantNode node)
        {
            if (node.ItemType.Definition.TypeKind == EdmTypeKind.Enum)
            {
                PropertyInfo propertyInfo = typeof(ODataEnumValue).GetProperty(nameof(ODataEnumValue.Value));
                foreach (var item in node.Collection)
                {
                    ODataEnumValue odataEnumValue = (ODataEnumValue)item.Value;
                    propertyInfo.SetValue(odataEnumValue, this.ConvertToPascalCase(odataEnumValue.Value));
                }
            }
            return base.BindCollectionConstantNode(node);
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

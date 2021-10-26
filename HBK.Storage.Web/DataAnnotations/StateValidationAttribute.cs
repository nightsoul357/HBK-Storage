using HBK.Storage.Web.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.DataAnnotations
{
    /// <summary>
    /// 狀態驗證器
    /// </summary>
    public class StateValidationAttribute : Attribute
    {
        public StateValidationAttribute()
        {

        }
        public StateValidationAttribute(AuthorizeKeyType authorizeKeyType)
        {
            this.AuthorizeKeyType = authorizeKeyType;
        }
        public bool IsStorageProviderValid { get; set; } = false;
        public bool IsStorageGroupValid { get; set; } = false;
        public AuthorizeKeyType? AuthorizeKeyType { get; set; } = null;
    }
}

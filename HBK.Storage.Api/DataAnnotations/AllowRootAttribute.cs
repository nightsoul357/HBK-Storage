using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.DataAnnotations
{
    /// <summary>
    /// 僅允許 Root 通過
    /// </summary>
    public class AllowRootAttribute : Attribute
    {
    }
}

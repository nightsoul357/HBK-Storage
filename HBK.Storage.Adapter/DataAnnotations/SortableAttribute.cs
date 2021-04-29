using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.DataAnnotations
{
    /// <summary>
    /// 允許欄位可被排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SortableAttribute : Attribute
    {
    }
}

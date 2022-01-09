using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Models
{
    /// <summary>
    /// 物件回傳結果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ObjectResponseResult<T>
    {
        public ObjectResponseResult(T responseObject, string responseText)
        {
            this.Object = responseObject;
            this.Text = responseText;
        }

        public T Object { get; }

        public string Text { get; }
    }
}

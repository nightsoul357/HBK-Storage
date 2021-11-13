using HBK.Storage.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Exceptions
{
    /// <summary>
    /// 操作失敗時擲出的例外
    /// </summary>
    public class OperationFailedException : Exception
    {
        /// <summary>
        /// 初始化操作失敗時擲出的例外
        /// </summary>
        /// <param name="resultCode"></param>
        public OperationFailedException(ResultCodeEnum resultCode)
        {
            this.ResultCode = resultCode;
        }
        /// <summary>
        /// 初始化操作失敗時擲出的例外
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="message"></param>
        public OperationFailedException(ResultCodeEnum resultCode, string message)
            : base(message)
        {
            this.ResultCode = resultCode;
        }
        /// <summary>
        /// 取得操作結果列舉
        /// </summary>
        public ResultCodeEnum ResultCode { get; private set; }
    }
}

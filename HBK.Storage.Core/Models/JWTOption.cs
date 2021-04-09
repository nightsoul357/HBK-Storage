using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Core.Models
{
    /// <summary>
    /// JWT 設定檔
    /// </summary>
    public class JWTOption
    {
        /// <summary>
        /// 設定或取得發行者
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 設定或取得 X509Certificate2 檔案位置
        /// </summary>
        public string X509Certificate2Location { get; set; }
        /// <summary>
        /// 設定或取得 X509Certificate2 使用的密碼
        /// </summary>
        public string X509Certificate2Password { get; set; }
    }
}

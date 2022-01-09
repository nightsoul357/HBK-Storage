using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.SDK.Enums
{
    /// <summary>
    /// 儲存個體類型列舉
    /// <br/>
    /// <br/>* **ftp** - FTP 協議所定義的儲存個體
    /// <br/>* **amazon_s3** - Amazon S3 的儲存個體
    /// <br/>* **local** - 本地儲存空間的儲存個體
    /// <br/>* **google_drive** - Google Drive 的儲存個體
    /// <br/>* **mega** - Mega 的儲存個體
    /// <br/>* **web_dav** - Web DAV 的儲存個體
    /// <br/>
    /// </summary>
    public enum StorageType
    {
        [System.Runtime.Serialization.EnumMember(Value = @"ftp")]
        Ftp = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"amazon_s3")]
        AmazonS3 = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"local")]
        Local = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"google_drive")]
        GoogleDrive = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"mega")]
        Mega = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"web_dav")]
        WebDav = 5,

    }
}

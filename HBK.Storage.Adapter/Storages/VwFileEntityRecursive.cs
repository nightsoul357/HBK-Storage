using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace HBK.Storage.Adapter.Storages
{
    /// <summary>
    /// 檔案實體遞迴解析結果
    /// </summary>
    [Keyless]
    public partial class VwFileEntityRecursive
    {
        /// <summary>
        /// 取得或設定根檔案 ID
        /// </summary>
        [Column("RootFileEntityID")]
        public Guid? RootFileEntityId { get; set; }
        /// <summary>
        /// 取得或設定檔案 ID
        /// </summary>
        [Column("FileEntityID")]
        public Guid? FileEntityId { get; set; }
        /// <summary>
        /// 取得或設定於根檔案相距層數
        /// </summary>
        public int? ChildLevel { get; set; }
    }
}

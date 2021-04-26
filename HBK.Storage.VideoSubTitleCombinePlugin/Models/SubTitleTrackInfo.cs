using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.VideoSubTitleCombinePlugin.Models
{
    public class SubTitleTrackInfo
    {
        /// <summary>
        /// 取得或設定軌道名稱
        /// </summary>
        public string TrackName { get; set; }
        /// <summary>
        /// 取得或設定字幕檔案實體 ID
        /// </summary>
        public Guid SubTitleFileEntityId { get; set; }
        /// <summary>
        /// 取得或設定組合完成之檔案需標記的標籤清單
        /// </summary>
        public List<string> CompleteTags { get; set; }
    }
}

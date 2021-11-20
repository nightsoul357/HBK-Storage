namespace HBK.Storage.Dashboard.Models
{
    public class DialogParsingResult<T>
    {
        /// <summary>
        /// 取得或設定是否取消
        /// </summary>
        public bool Cancelled { get; set; }
        /// <summary>
        /// 取得或設定資料
        /// </summary>
        public T Data { get; set; }
    }
}

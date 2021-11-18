using HBK.Storage.Dashboard.DataSource;

namespace HBK.Storage.Dashboard.Pages.Storages.Credentials
{
    /// <summary>
    /// 提供編輯 Storage Credential 資訊的元件介面
    /// </summary>
    public interface ICredentialsComponent
    {
        /// <summary>
        /// 取得或設定驗證資訊
        /// </summary>
        public StorageCredentialsBase Credential { get; set; }
    }
}

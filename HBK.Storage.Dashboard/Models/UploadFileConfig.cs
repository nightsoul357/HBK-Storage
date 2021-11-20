using HBK.Storage.Dashboard.DataSource;

namespace HBK.Storage.Dashboard.Models
{
    public class UploadFileConfig
    {
        public List<string> Tags { get; set; } = new List<string>();
        public CryptoMode CryptoMode { get; set; } = CryptoMode.No_crypto;
    }
}

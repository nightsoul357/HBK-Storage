using HBK.Storage.Dashboard.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace HBK.Storage.Dashboard.Models
{
    /// <summary>
    /// 上傳任務
    /// </summary>
    public class UploadFileTask
    {
        public IBrowserFile File { get; set; }
        public DateTime? CompleteDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public double Progress { get; set; }
        public UploadFileTaskStatusEnum Status { get; set; }
        public Exception? Exception { get; set; }
    }
}

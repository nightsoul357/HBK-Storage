using HBK.Storage.Dashboard.Models;
using System.Collections.Concurrent;

namespace HBK.Storage.Dashboard.Managers
{
    /// <summary>
    /// 上傳任務管理器
    /// </summary>
    public class UploadFileTaskManager : IDisposable
    {
        private ConcurrentQueue<UploadFileTask> _pendingQueue;

        public void Add(UploadFileTask uploadFileTask)
        {
            _pendingQueue.Enqueue(uploadFileTask);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileProcessHandlers
{
    /// <summary>
    /// 處理檔案處理器代理執行者
    /// </summary>
    public class FileProcessHandlerProxy
    {
        private IEnumerable<FileProcessHandlerBase> _fileAccessHandlers;
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="fileAccessHandlers"></param>
        public FileProcessHandlerProxy(IEnumerable<FileProcessHandlerBase> fileAccessHandlers)
        {
            _fileAccessHandlers = fileAccessHandlers;
        }
        /// <summary>
        /// 根據對應的處理器指示字串執行存取檔案處理器
        /// </summary>
        /// <param name="taskModel">存取檔案任務模型</param>
        /// <param name="handlerIndicate">處理器指示字串</param>
        /// <returns></returns>
        public async Task<FileProcessTaskModel> ProcessAsync(FileProcessTaskModel taskModel, string handlerIndicate)
        {
            if (String.IsNullOrEmpty(handlerIndicate))
            {
                return taskModel;
            }

            List<FileProcessHandlerBase> currentHandlers = new List<FileProcessHandlerBase>();
            handlerIndicate.Split(',').ToList().ForEach(x =>
            {
                var handler = _fileAccessHandlers.FirstOrDefault(f => String.Compare(f.Name, x.Trim(), true) == 0);
                if (handler != null)
                {
                    currentHandlers.Add(handler);
                }
            });

            foreach (var handler in currentHandlers)
            {
                taskModel = await handler.ProcessAsync(taskModel);
            }

            return taskModel;
        }
    }
}

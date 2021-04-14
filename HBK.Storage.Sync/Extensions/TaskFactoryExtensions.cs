using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Sync.Extensions
{
    public static class TaskFactoryExtensions
    {
        /// <summary>
        /// 具有例外攔截的啟動方式
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="action"></param>
        /// <param name="creationOptions"></param>
        /// <param name="errorHandler"></param>
        /// <returns></returns>
        public static Task StartNewSafety(this TaskFactory taskFactory, Action action, TaskCreationOptions creationOptions, Action<Exception> errorHandler = null)
        {
            return taskFactory.StartNew(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    errorHandler?.Invoke(ex);
                }
            }, creationOptions);
        }
    }
}

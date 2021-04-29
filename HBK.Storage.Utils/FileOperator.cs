using System;
using System.IO;
using System.Threading;

namespace HBK.Storage.Utils
{
    public static class FileOperator
    {
        public static bool DeleteSaftey(string path, int tryTimes = 10, int interval = 3000)
        {
            for (int i = 0; i < tryTimes; i++)
            {
                try
                {
                    File.Delete(path);
                    return true;
                }
                catch { }
                SpinWait.SpinUntil(() => false, interval);
            }
            return false;
        }
    }
}

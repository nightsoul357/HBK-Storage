using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBK.Storage.Utils
{
    public static class DirectoryOperator
    {
        public static bool DeleteSaftey(string path, bool recursive, int tryTimes = 10, int interval = 3000)
        {
            for (int i = 0; i < tryTimes; i++)
            {
                try
                {
                    Directory.Delete(path, recursive);
                    return true;
                }
                catch { }
                SpinWait.SpinUntil(() => false, interval);
            }
            return false;
        }
    }
}

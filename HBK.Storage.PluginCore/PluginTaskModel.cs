using HBK.Storage.Adapter.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.PluginCore
{
    public class PluginTaskModel
    {
        public Guid TaskId { get; set; }
        public FileEntity FileEntity { get; set; }
        public Guid StorageProviderId { get; set; }
    }
}

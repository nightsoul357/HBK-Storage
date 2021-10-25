using HBK.Storage.Web.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage.Credential
{
    public interface ICredentialsComponent
    {
        public StorageCredentialsBase Credential { get; set; }
    }
}

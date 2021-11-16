using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.DataSource.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage.Credential
{
    public partial class LocalCredentialComponent : ICredentialsComponent
    {
        public string Directory { get; set; }
        [Parameter]
        public StorageCredentialsBase Credential
        {
            get
            {
                return new LocalStorageCredentials()
                {
                    Directory = this.Directory,
                    Storage_type = StorageType.Local
                };
            }
            set
            {
                if (value != null)
                {
                    this.Directory = ((LocalStorageCredentials)value).Directory;
                }
            }
        }
    }
}

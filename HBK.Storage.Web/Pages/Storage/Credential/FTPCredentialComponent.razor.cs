using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.DataSource.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage.Credential
{
    public partial class FTPCredentialComponent : ICredentialsComponent
    {
        /// <summary>
        /// 取得或設定 Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 取得或設定密碼
        /// </summary>
        public string Password { get; set; }
        [Parameter]
        public StorageCredentialsBase Credential
        {
            get
            {
                return new FTPStorageCredentials()
                {
                    Password = this.Password,
                    Storage_type = StorageType.Ftp,
                    Url = this.Url,
                    Username = this.Username
                };
            }
            set
            {
                if (value != null)
                {
                    this.Password = ((FTPStorageCredentials)value).Password;
                    this.Url = ((FTPStorageCredentials)value).Url;
                    this.Username = ((FTPStorageCredentials)value).Username;
                }
            }
        }
    }
}

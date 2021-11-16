using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.DataSource.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage.Credential
{
    public partial class MegaCredentialComponent : ICredentialsComponent
    {
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 取得或設定密碼
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 取得或設定父系資料夾 ID
        /// </summary>
        public string ParentId { get; set; }
        [Parameter]
        public StorageCredentialsBase Credential
        {
            get
            {
                return new MegaStorageCredentials()
                {
                    ParentId = this.ParentId,
                    Password = this.Password,
                    Storage_type = StorageType.Mega,
                    Username = this.Username
                };
            }
            set
            {
                if (value != null)
                {
                    this.ParentId = ((MegaStorageCredentials)value).ParentId;
                    this.Password = ((MegaStorageCredentials)value).Password;
                    this.Username = ((MegaStorageCredentials)value).Username;
                }
            }
        }
    }
}

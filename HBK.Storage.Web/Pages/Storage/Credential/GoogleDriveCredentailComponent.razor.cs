using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.DataSource.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage.Credential
{
    public partial class GoogleDriveCredentailComponent : ICredentialsComponent
    {
        /// <summary>
        /// 取得或設定父資料夾 ID
        /// </summary>
        public string Parent { get; set; }
        /// <summary>
        /// 取得或設定存取金鑰
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 取得或設定存取密鑰
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// 取得或設定使用者名稱
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 取得或設定 Token 資訊
        /// </summary>
        public Dictionary<string, string> Tokens { get; set; }
        [Parameter]
        public StorageCredentialsBase Credential
        {
            get
            {
                return new GoogleDriveCredentials()
                {
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    Parent = this.Parent,
                    Storage_type = StorageType.Google_drive,
                    Tokens = this.Tokens,
                    User = this.User
                };
            }
            set
            {
                if(value != null)
                {
                    this.ClientId = ((GoogleDriveCredentials)value).ClientId;
                    this.ClientSecret = ((GoogleDriveCredentials)value).ClientSecret;
                    this.Parent = ((GoogleDriveCredentials)value).Parent;
                    this.Tokens = ((GoogleDriveCredentials)value).Tokens;
                    this.User = ((GoogleDriveCredentials)value).User;
                }
            }
        }
    }
}

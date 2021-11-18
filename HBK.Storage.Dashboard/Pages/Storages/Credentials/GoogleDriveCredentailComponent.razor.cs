using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.DataSource.Models;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;

namespace HBK.Storage.Dashboard.Pages.Storages.Credentials
{
    public partial class GoogleDriveCredentailComponent : CredentialComponentBase<GoogleDriveCredentials>
    {
        public string TokenUpdateMessage { get; set; } = string.Empty;

        public void UpdateToken()
        {

        }
    }
}

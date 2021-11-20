using HBK.Storage.Dashboard.Models;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;

namespace HBK.Storage.Dashboard.Pages.FileEntities
{
    public partial class EditUploadFileConfigDialog : HBKDialogBase
    {
        [Parameter]
        public UploadFileConfig? UploadFileConfig { get; set; }

        public Converter<List<string>> TagConverter = new Converter<List<string>>
        {
            SetFunc = value =>
            {
                return value.Aggregate("", (s1, s2) => s1 + s2 + ", ").Trim().Trim(',');
            },
            GetFunc = text =>
            {
                var tags = text.Split(',');
                return tags.Select(x => x.Trim()).ToList();
            },
        };

        /// <summary>
        /// 儲存
        /// </summary>
        public void Save()
        {
            base.MudDialog.Close(this.UploadFileConfig);
        }
    }
}

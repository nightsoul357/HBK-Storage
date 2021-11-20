using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HBK.Storage.Dashboard.Shared
{
    public abstract class HBKDialogBase : HBKLayoutComponentBase
    {
        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }

        /// <summary>
        /// 關閉 Dialog
        /// </summary>
        public void Cancel()
        {
            this.MudDialog.Cancel();
        }
    }
}

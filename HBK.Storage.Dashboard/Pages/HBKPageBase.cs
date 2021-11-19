using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Models;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;

namespace HBK.Storage.Dashboard.Pages
{
    /// <summary>
    /// HBK Storage Dashboard 頁面使用的基底型別
    /// </summary>
    public abstract class HBKPageBase : HBKLayoutComponentBase
    {
        protected override void OnParametersSet()
        {
            if (this.StateContainer.GetSignInKey() == null)
            {
                base.NavigationManager.NavigateTo("/authentication/login");
            }
            base.OnParametersSet();
        }
    }
}

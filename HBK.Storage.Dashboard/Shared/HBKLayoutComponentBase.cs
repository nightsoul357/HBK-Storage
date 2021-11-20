using BlazorDownloadFile;
using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Models;
using HBK.Storage.Dashboard.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HBK.Storage.Dashboard.Shared
{
    /// <summary>
    /// HBK Dashboard 使用的 Layout Component 基底型別
    /// </summary>
    public abstract class HBKLayoutComponentBase : LayoutComponentBase, IDisposable
    {
        /// <summary>
        /// 取得或設定狀態容器
        /// </summary>
        [Inject]
        public StateContainer StateContainer { get; set; }
        /// <summary>
        /// 取得或設定 Snack Bar
        /// </summary>
        [Inject]
        protected ISnackbar Snackbar { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public HBKStorageApi HBKStorageApi { get; set; }
        [Inject]
        public HBKDialogService DialogService { get; set; }
        [Inject]
        public NavigationService NavigationService { get; set; }
        [Inject]
        public HBKLayoutComponentService HBKLayoutComponentService { get; set; }
        [Inject]
        public IBlazorDownloadFileService DownloadFileService { get; set; }
        [Inject]
        public ClipboardService ClipboardService { get; set; }

        /// <summary>
        /// 具有例外攔截方法的啟動方式
        /// </summary>
        /// <param name="action"></param>
        public void InvokeSafety(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                this.OnException(ex);
            }
        }
        /// <summary>
        /// 具有例外攔截方法的啟動方式
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Task InvokeSafetyAsync(Func<Task> func)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    await func();
                }
                catch (Exception ex)
                {
                    this.OnException(ex);
                }
            });
        }

        public string ConvertDateTimeToString(DateTimeOffset? dateTimeOffset)
        {
            if (!dateTimeOffset.HasValue)
            {
                return String.Empty;
            }
            return dateTimeOffset.Value.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
        }

        public void Refresh()
        {
            base.StateHasChanged();
        }

        protected void OnException(Exception ex)
        {
            this.Snackbar?.Add("發生未預期的例外", Severity.Error);
        }

        protected override void OnInitialized()
        {
            this.HBKLayoutComponentService.RegisterComponent(this);
            base.OnInitialized();
        }

        public virtual void Dispose()
        {
            this.HBKLayoutComponentService.UnregisterComponent(this);
        }
    }
}

using Microsoft.JSInterop;

namespace HBK.Storage.Dashboard.Services
{
    public class NavigationService
    {
        private readonly IJSRuntime _jsRuntime;

        public NavigationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask NavigateToAsync(string url)
        {
            return _jsRuntime.InvokeVoidAsync("window.open", url, "_blank");
        }

        public ValueTask CloseWindows()
        {
            return _jsRuntime.InvokeVoidAsync("window.close");
        }
    }
}

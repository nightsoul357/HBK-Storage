using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Features
{
    public class NavigationSerivce
    {
        private readonly IJSRuntime _jsRuntime;

        public NavigationSerivce(IJSRuntime jsRuntime)
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

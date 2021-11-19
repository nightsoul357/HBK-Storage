using HBK.Storage.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Dashboard.DataSource
{
    public partial class HBKStorageApi
    {
        private StateContainer _stateContainer;
        public HBKStorageApi(string baseUrl, System.Net.Http.HttpClient httpClient, StateContainer stateContainer)
            : this(baseUrl, httpClient)
        {
            _stateContainer = stateContainer;
        }
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            var key = _stateContainer.GetSignInKey();
            if (key != null && !request.Headers.Contains("HBKey"))
            {
                request.Headers.Add("HBKey", key.Key_value);
            }
        }
    }
}

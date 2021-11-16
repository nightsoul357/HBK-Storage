using HBK.Storage.Web.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.DataSource
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
            if (_stateContainer.AuthorizeKey != null && !client.DefaultRequestHeaders.Contains("HBKey"))
            {
                client.DefaultRequestHeaders.Add("HBKey", _stateContainer.AuthorizeKey.Key_value);
            }
        }
    }
}

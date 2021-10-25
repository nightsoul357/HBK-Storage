using HBK.Storage.Web.DataSource;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.FileEntity
{
    public partial class FileEntityInforamtionDialog
    {
        private MudTable<ChildFileEntityResponse> _table;
        private string _searchString = null;
        /// <summary>
        /// 取得或設定 Dialog 中使用的字定義屬性(解決 Dialog 無法反白選取的問題)
        /// <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/1186"/>
        /// </summary>
        public Dictionary<string, object> DialogAttributes { get; set; } =
        new()
        {
            { "tabindex", "10" },
        };
        [CascadingParameter]
        public MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public FileEntityResponse FileEntity { get; set; }
        [Inject]
        public HBKStorageApi HBKStorageApi { get; set; }

        public Converter<ICollection<string>> TagConverter = new Converter<ICollection<string>>
        {
            SetFunc = value => JsonConvert.SerializeObject(value),
            GetFunc = text => JsonConvert.DeserializeObject<ICollection<string>>(text),
        };

        /// <summary>
        /// Here we simulate getting the paged, filtered and ordered data from the server
        /// </summary>
        public async Task<TableData<ChildFileEntityResponse>> ServerReloadAsync(TableState state)
        {
            string filter = null;
            string order = null;

            if (!string.IsNullOrEmpty(_searchString))
            {
                filter += $" contains(file_entity/name,'{_searchString}')";
            }
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                order = $"{state.SortLabel} {(state.SortDirection == SortDirection.Ascending ? "asc" : "desc")}";
            }

            var response = await this.HBKStorageApi.ChildsAsync(this.FileEntity.File_entity_id, filter, order, state.Page, state.PageSize);

            return new TableData<ChildFileEntityResponse>()
            {
                Items = response.Value,
                TotalItems = response.OdataCount
            };
        }
        public async Task OnSearchAsync(string text)
        {
            _searchString = text;
            await _table.ReloadServerData();
        }
    }
}

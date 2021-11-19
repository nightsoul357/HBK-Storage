using HBK.Storage.Dashboard.DataSource;
using HBK.Storage.Dashboard.Helpers;
using HBK.Storage.Dashboard.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;

namespace HBK.Storage.Dashboard.Pages.FileEntities
{
    public partial class FileEntityDetailDialog : HBKDialogBase
    {
        private MudTable<ChildFileEntityResponse> _table;
        private string? _searchString = null;
        /// <summary>
        /// 取得或設定 Dialog 中使用的字定義屬性(解決 Dialog 無法反白選取的問題)
        /// <seealso cref="https://github.com/MudBlazor/MudBlazor/issues/1186"/>
        /// </summary>
        public Dictionary<string, object> DialogAttributes { get; set; } =
        new()
        {
            { "tabindex", "10" },
        };

        [Parameter]
        public FileEntityResponse? FileEntity { get; set; }

        public Converter<ICollection<string>> TagConverter = new Converter<ICollection<string>>
        {
            SetFunc = value => JsonConvert.SerializeObject(value),
            GetFunc = text => JsonConvertHelper.DeserializeObjectWithDefault<ICollection<string>>(text, new List<string>()),
        };

        /// <summary>
        /// Here we simulate getting the paged, filtered and ordered data from the server
        /// </summary>
        public async Task<TableData<ChildFileEntityResponse>> ServerReloadAsync(TableState state)
        {
            string? filter = null;
            string? order = null;

            if (!string.IsNullOrEmpty(_searchString))
            {
                filter += $"(contains(file_entity/name,'{_searchString}') or contains(cast(file_entity/file_entity_id, 'Edm.String'),'{_searchString}') or file_entity/file_entity_tag/any(t:contains(t/value,'{_searchString}')) or contains(file_entity/mime_type,'{_searchString}'))";
            }
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                order = $"{state.SortLabel} {(state.SortDirection == SortDirection.Ascending ? "asc" : "desc")}";
            }

            var response = await this.HBKStorageApi.ChildsAsync(this.FileEntity.File_entity_id, filter, order, state.Page * state.PageSize, state.PageSize);

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

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
    }
}

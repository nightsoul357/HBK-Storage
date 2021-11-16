using HBK.Storage.Web.Containers;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Shared
{
    public partial class NavMenu
    {
        [Inject]
        public StateContainer StateContainer { get; set; }
    }
}

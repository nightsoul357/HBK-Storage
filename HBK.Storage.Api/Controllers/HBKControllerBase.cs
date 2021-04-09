using HBK.Storage.Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.Controllers
{
    /// <summary>
    /// HBK Storage 使用的基底控制器
    /// </summary>
    [ApiController]
    [TypeFilter(typeof(HBKAuthorizeFilter))]
    public class HBKControllerBase : ControllerBase
    {

    }
}

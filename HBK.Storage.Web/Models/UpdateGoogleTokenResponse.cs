﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Models
{
    public class UpdateGoogleTokenResponse
    {
        public string State { get; set; }
        public string Code { get; set; }
        public string Scope { get; set; }
    }
}
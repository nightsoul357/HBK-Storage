﻿using HBK.Storage.Adapter.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBK.Storage.Adapter.Models
{
    /// <summary>
    /// 同步策略資料
    /// </summary>
    public class SyncPolicy
    {
        /// <summary>
        /// 取得或設定規則
        /// </summary>
        public string Rule { get; set; }
        /// <summary>
        /// 取得或設定標籤驗證規則
        /// </summary>
        public string TagRule { get; set; }
        /// <summary>
        /// 取得或設定標籤驗證模式
        /// </summary>
        public TagMatchModeEnum TagMatchMode { get; set; }
    }
}
# Policy Rule

## Sync Policy

### 規則撰寫範例

1. 同步所有大於 100MB 之檔案
```
Size > 104857600
```

2. 同步所有影片檔案
```
MimeType.StartsWith("video/")
```

3. 同步所有 Tag 內包含 Special File 且大於 100 MB 之檔案

```
FileEntityTag.Any(x => x.Value == "Special File") && Size > 104857600
```

### 欄位資訊

| 欄位 | 說明 | 類型 | 範例 |
| -------- | -------- | -------- | -------- |
| Name | 檔案名稱 | string | `test.mp4` |
| Size | 檔案大小 | string | `215486325` |
| ExtendProperty | 擴充資訊 | string | `video/mp4` |
| MimeType | 網際網路媒體型式 | string  | `video/mp4` |
| CreateDateTime | 建立時間 | DateTimeOffset | `2019-11-12 22:01:33.8134699 +00:00` |
| FileEntityTag | 標籤清單 | `List<FileEntityTag>`  |  |

#### FileEntityTag

| 欄位 | 說明 | 類型 | 範例 |
| -------- | -------- | -------- | -------- |
| Value | 值 | string | `Special File` |

## Clear Policy

### 規則撰寫範例

1. 清除已有其他副本之檔案
```
ValidFileEntityStorageCount >= 2
```

2. 清除所有影片檔案
```
FileEntity.MimeType.StartsWith("video/")
```

3. 清除已有其他副本且大於 100 MB 之檔案
```
ValidFileEntityStorageCount >= 2 &&  FileEntity.Size > 104857600
```

### 欄位資訊

| 欄位 | 說明 | 類型 | 範例 |
| -------- | -------- | -------- | -------- |
| FileEntity | 檔案資訊 | FileEntity |  |
| FileEntityStorage | 此 Storage Group 所儲存的副本資訊 | FileEntityStorage |  |
| ValidFileEntityStorageCount | 有效的副本數量 | int | `2` |

## 注意

盡量避免 Sync 以及 Clear 同時啟動的狀況，若策略撰寫錯誤可能導致不斷同步及清除。
如下所示為錯誤之設置內容:

| 功能 | 設定 |
| -------- | -------- |
| Sync Mode | Always |
| Clear Policy | `ValidFileEntityStorageCount >= 2` |

此設定會導致清除檔案後，同步服務會嘗試同步該檔案，隨後再被清除，不斷循環。
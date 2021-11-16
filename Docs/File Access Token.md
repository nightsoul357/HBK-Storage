# File Access Token

## 摘要

File Access Token 為 JWT 型式。

若 Token 類型後綴為 `NoLimit` 表示該 Token 不會被記錄在資料庫內，也不允許使用撤銷。

若希望撤銷所有權杖，可以綁定新的 JWT 憑證。 

## Token 類型

### Normal

| 欄位 | 說明 | 類型 | 是否為必填 | 範例 |
| -------- | -------- | -------- | -------- |
| storage_group_id | 強制指定來源 Storage Group | Guid | 否 | `59b50410-e86a-4341-8973-ae325e354210` |
| file_entity_id | 檔案 ID | Guid | 是 | `59b50410-e86a-4341-8973-ae325e354210` |
| expire_after_minutes | 權杖過期時間(分鐘後) | int | 是 | 60 |
| access_times_limit | 使用次數限制 | int | 是 | 60 |
| handler_indicate | 下載指示器字串 | string | 否 | `watermark-default` |

若未指定 `storage_group_id` 則會使用下載優先度排序後自動選擇。

### NormalNoLimit

| 欄位 | 說明 | 類型 | 是否為必填 | 範例 |
| -------- | -------- | -------- | -------- |
| storage_group_id | 強制指定來源 Storage Group | Guid | 否 | `59b50410-e86a-4341-8973-ae325e354210` |
| file_entity_id | 檔案 ID | Guid | 是 | `59b50410-e86a-4341-8973-ae325e354210` |
| expire_after_minutes | 權杖過期時間(分鐘後) | int | 是 | 60 |
| handler_indicate | 下載指示器字串 | string | 否 | `watermark-default` |

若未指定 `storage_group_id` 則會使用下載優先度排序後自動選擇。

### AllowTag

| 欄位 | 說明 | 類型 | 是否為必填 | 範例 |
| -------- | -------- | -------- | -------- |
| storage_group_id | 強制指定來源 Storage Group | Guid | 否 | `59b50410-e86a-4341-8973-ae325e354210` |
| allow_tag_pattern | 允許的 Tag 範本(使用正規表達式) | string | 是 | `special-file` |
| expire_after_minutes | 權杖過期時間(分鐘後) | int | 是 | 60 |
| access_times_limit | 使用次數限制 | int | 是 | 60 |
| handler_indicate | 下載指示器字串 | string | 否 | `watermark-default` |

若未指定 `storage_group_id` 則會使用下載優先度排序後自動選擇。

### AllowTagNoLimit

| 欄位 | 說明 | 類型 | 是否為必填 | 範例 |
| -------- | -------- | -------- | -------- |
| storage_group_id | 強制指定來源 Storage Group | Guid | 否 | `59b50410-e86a-4341-8973-ae325e354210` |
| allow_tag_pattern | 允許的 Tag 範本(使用正規表達式) | string | 是 | `special-file` |
| expire_after_minutes | 權杖過期時間(分鐘後) | int | 是 | 60 |
| handler_indicate | 下載指示器字串 | string | 否 | `watermark-default` |

若未指定 `storage_group_id` 則會使用下載優先度排序後自動選擇。
# API Key

## 摘要

用以存取 API 時的認證金鑰，用帶入 Header: `hbkey` 內，詳細請參照 API Document。

## 類型

### Root

Root 類型的金鑰為最高權限，可以存存取所有 StorageProvider 的所有功能，並且具有管理所有 API Key 的權限。

### Normal

Normal 類型的金鑰無法管理 API Key，僅能指定在某個 StorageProvider 僅擁有查詢、新增、修改、刪除之特定功能。

其中四個功能分別對應至存取 API 時的 HTTP 方法類型。

| 方法 | 對應 HTTP 方法 |
| -------- | -------- |
| Read | GET |
| Insert | POST |
| Update | PUT |
| Delete | DELETE |

詳細能使用的方法請參照 API Document。
# Storage Credential

## Local

### 說明

使用存取本地檔案的方式存取該 Storage 內的資料。

若 Sync Service 或 Plugin 位於不同的機器上，此值設定應包含電腦名稱。

### 驗證資訊

| 欄位 | 說明 | 範例 |
| -------- | -------- | -------- |
| Directory | 伺服器上的實體資料夾路徑 | `\\Win-N2\LocalStorage` or `/app/default_storage` |

### 功能支援

| 功能名稱 | 狀態 |
| -------- | -------- |
| 下載 | 支援 |
| 上傳 | 支援 |
| 分段下載 | 支援 |
| 分段上傳 | 不支援 |

## FTP Storage

使用 FTP 協議存取該 Storage 內的資料。

### 驗證資訊

| 欄位 | 說明 | 範例 |
| -------- | -------- | -------- |
| Url | FTP 存取路徑 | `ftp://sample.hbk/FTPStorage` |
| Username | 使用者名稱 | `username` |
| Password | 密碼 | `password` |

### 功能支援

| 功能名稱 | 狀態 |
| -------- | -------- |
| 下載 | 支援 |
| 上傳 | 支援 |
| 分段下載 | 不支援 |
| 分段上傳 | 不支援 |

## AWS S3

### 說明

使用 AWS S3 SDK(HTTP/HTPPS) 存取該 Storage 內的資料。

### 驗證資訊

| 欄位 | 說明 | 範例 |
| -------- | -------- | -------- |
| AccessKey | 存取金鑰 |  |
| SecretKey | 存取密鑰 |  |
| BucketName | Bucket 名稱 | `myBucket` |
| Region | 位置 | `us-east-2` |
| ServiceURL | [服務位置](https://docs.aws.amazon.com/general/latest/gr/s3.html) | `http://s3.us-east-2.amazonaws.com` |
| ForcePathStyle | 若使用 minio 此值應為 true | `false` |

### 功能支援

| 功能名稱 | 狀態 |
| -------- | -------- |
| 下載 | 支援 |
| 上傳 | 支援 |
| 分段下載 | 支援 |
| 分段上傳 | 不支援 |

## Google Drive

### 說明

使用 Google Drive SDK(HTTP/HTPPS) 存取該 Storage 內的資料。

透過 OAuth 取得使用者身分權杖進行存取。

OAuth 中的 Redirect Url 需加入 `{baseurl}/signin-google`。

### 驗證資訊

| 欄位 | 說明 | 範例 |
| -------- | -------- | -------- |
| ClientId | 存取金鑰 |  |
| ClientSecret | 存取密鑰 |  |
| User | 使用者名稱 | `HBKStorage` |
| Parent | 父資料夾 ID | `1h-niepnwTGKA4x_eR8PEcAON3f34QFKI` |
| Token | 權杖資訊(此欄位請使用 Web 自動刷新) | |

### 注意

**一組使用者使用者請使用一組 ClientId/ClientSecret**，若同一組發行 ClientId/ClientSecret 針對同一個使用者發行 Token 時，只有首次發行能取得 Refresh Token，後續如需要重新取得 Refresh Token 則須先於[這裡](https://myaccount.google.com/u/1/permissions)撤銷存取權。

若無 Refresh Token 時，該 Storage 會在 Access Token 過期(60分鐘)後進入 Disable 狀態。
### 功能支援

| 功能名稱 | 狀態 |
| -------- | -------- |
| 下載 | 支援 |
| 上傳 | 支援 |
| 分段下載 | 支援 |
| 分段上傳 | 不支援 |

## Mega

使用 HTTP/HTPPS 存取該 Storage 內的資料。

### 驗證資訊

| 欄位 | 說明 | 範例 |
| -------- | -------- | -------- |
| Username | 帳號 |  |
| Password | 密碼 |  |
| ParentId | 父資料夾 ID | `zQ3W5QLY` |

### 功能支援

| 功能名稱 | 狀態 |
| -------- | -------- |
| 下載 | 支援 |
| 上傳 | 支援 |
| 分段下載 | 支援 |
| 分段上傳 | 不支援 |

## WebDAV

使用 HTTP/HTPPS 存取該 Storage 內的資料。

### 驗證資訊

| 欄位 | 說明 | 範例 |
| -------- | -------- | -------- |
| Url | 伺服器位置 | `https://hbkstorage.com/Storage1` |
| Username | 帳號 |  |
| Password | 密碼 |  |
| IsSupportPartialUpload | 是否支援分段上傳 | `true` |

部分 WebDAV Server 並不支援分段上傳，例如 Synology NAS 的 WebDAV Server 套件，此時須將 `IsSupportPartialUpload` 設置為 `false`。

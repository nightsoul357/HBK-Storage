# Install Document(Windows)

## 環境

* Windows 10 or Windows Server 2019

## 前置步驟

1. 安裝 .Net 5.0 Hosting Bundle(https://dotnet.microsoft.com/download/dotnet/5.0)
2. 安裝 Microsoft SQL Server(https://www.microsoft.com/zh-tw/sql-server/sql-server-downloads)

## 安裝 API

### 產生 p12 憑證 For JWT

1. `mkdir C:\certificate`
2. `openssl genrsa -out key.pem 2048`
3. `openssl req -new -sha256 -key key.pem -out csr.csr`
4. `openssl req -x509 -sha256 -days 365 -key key.pem -in csr.csr -out certificate.pem`
5. `openssl pkcs12 -export -out certificate.p12 -inkey key.pem -in certificate.pem`

### 解壓縮 API 站台檔案

1. 建立 `C:\HBKStorage\HBKStorage.Api` 目錄
2. 解壓縮 HBKStorage-Api.zip 檔案至 `C:\HBKStorage\HBKStorage.Api` 目錄
3. 複製 `C:\certificate\certificate.p12` 至 `C:\HBKStorage\HBKStorage.Api`
4. 修改 appsetting.json 內容
5. 修改 `Database:ConnectionString` 為對應的連線字串
6. 修改 `JWTOption:X509Certificate2Password` 為正確的密碼

```json=
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Error",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Database": {
    "ConnectionString": "Data Source=localhost;Initial Catalog=HBKStorage;Integrated Security=True"
  },
  "JWTOption": {
    "Issuer": "HBKStorage",
    "X509Certificate2Location": "certificate.p12",
    "X509Certificate2Password": "develop123465"
  },
  "AllowedHosts": "*"
}
```

### 建立 IIS 站台

1. 建立站台
![](https://i.imgur.com/BzxBgRe.png)
2. 將 .Net CLR 版本調整為沒有受控碼
![](https://i.imgur.com/OIK1KJY.png)
3. 重新啟動 IIS 站台

### 確認是否正常運作

1. 確認 `C:\HBKStorage\HBKStorage.Api\Logs` 內是否產生正確的 log 檔案

## 安裝 Sync Service

1. 建立 `C:\HBKStorage\HBKStorage.Sync` 目錄
2. 解壓縮 HBKStorage-Sync.zip 檔案至 `C:\HBKStorage\HBKStorage.Sync` 目錄
3. 修改 appsetting.json 檔案
4. 修改 `Database:ConnectionString` 為對應的連線字串
5. `sc create HBKStorage.Sync binPath="C:\HBKStorage\HBKStorage.Sync\HBKStorage.Sync.exe"`
6. `sc start HBKStorage.Sync`

```json=
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Database": {
    "ConnectionString": "Data Source=localhost;Initial Catalog=HBKStorage;Integrated Security=True"
  },
  "SyncTaskManagerOption": {
    "Identity": "Sync Service 1",
    "FetchLimit": "1",
    "FileEntityNoDivisor": "1",
    "FileEntityNoRemainder": "0",
    "IsExecutOnAllStoragProvider": "True",
    "StorageProviderIds": [],
    "TaskLimit": "1"
  },
  "DeleteFileEntityTaskManagerOption": {
    "Identity": "Delete Service 1",
    "FetchLimit": "100",
    "FileEntityNoDivisor": "1",
    "FileEntityNoRemainder": "0",
    "TaskLimit": "1"
  },
  "ExpireFileEntityTaskManagerOption": {
    "Identity": "Expire Service 1",
    "FetchLimit": "100",
    "FileEntityNoDivisor": "1",
    "FileEntityNoRemainder": "0"
  }
}

```

### 確認是否正常運作

1. 確認 `C:\HBKStorage\HBKStorage.Sync\Logs` 內是否產生正確的 log 檔案

## 安裝 Plugin(Option)

1. 建立 `C:\HBKStorage\HBKStorage.PluginIntegration` 目錄
2. 解壓縮 HBKStorage-PluginIntegration.zip 檔案至 `C:\HBKStorage\HBKStorage.PluginIntegration` 目錄
3. 修改 appsetting.json 檔案
4. 修改 `Database:ConnectionString` 為對應的連線字串
5. `sc create HBKStorage.PluginIntegration binPath="C:\HBKStorage\HBKStorage.PluginIntegration\HBKStorage.PluginIntegration.exe"`
6. `sc start HBKStorage.PluginIntegration`

```json=
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Database": {
    "ConnectionString": "Data Source=localhost;Initial Catalog=HBKStorage;Integrated Security=True"
  },
  "ImageCompressTaskOptions": {
    "Identity": "Compress-Image Plugin 1",
    "TaskLimit": 1,
    "IsExecutOnAllStoragProvider": true,
    "FileEntityNoDivisor": 1,
    "StorageProviderIds": [],
    "FileEntityNoRemainder": 0,
    "FetchLimit": 100,
    "MimeTypePartten": "image/%",
    "IdentityTag": "Compress-Image-Successfully",
    "ExceptionTag": "Compress-Image-Failed",
    "CompressLevels": [
      {
        "Name": "QuantityFirst",
        "Quantity": 80
      },
      {
        "Name": "QuantityMore",
        "Quantity": 65
      },
      {
        "Name": "Average",
        "Quantity": 50
      },
      {
        "Name": "CompressionMore",
        "Quantity": 35
      },
      {
        "Name": "CompressionFirst",
        "Quantity": 20
      }
    ]
  },
  "VideoConvertM3U8TaskManagerOptions": {
    "Identity": "Video-Convert-M3U8 Plugin 1",
    "TaskLimit": 1,
    "IsExecutOnAllStoragProvider": true,
    "FileEntityNoDivisor": 1,
    "StorageProviderIds": [],
    "FileEntityNoRemainder": 0,
    "FetchLimit": 100,
    "MimeTypePartten": "video/%",
    "IdentityTag": "Require-Video-Convert-M3U8",
    "ExceptionTag": "Video-Convert-M3U8-Failed",
    "FetchMode": 2,
    "JustExecuteOnRootFileEntity": false,
    "TSInterval": 10,
    "FFmpegLocation": "ffmpeg.exe",
    "WorkingDirectory": "Working"
  },
  "VideoMetadataTaskManagerOptions": {
    "Identity": "Video-Metadata Plugin 1",
    "TaskLimit": 1,
    "IsExecutOnAllStoragProvider": true,
    "FileEntityNoDivisor": 1,
    "StorageProviderIds": [],
    "FileEntityNoRemainder": 0,
    "FetchLimit": 100,
    "MimeTypePartten": "video/%",
    "IdentityTag": "Video-Metadata",
    "ExceptionTag": "Video-Metadata-Failed",
    "JustExecuteOnRootFileEntity": false,
    "ExceptionMimeTypes": [ "video/MP2T" ],
    "FFmpegLocation": "ffmpeg.exe",
    "WorkingDirectory": "Working",
    "PreviewsCount": 10
  },
  "VideoSubTitleCombineTaskOptions": {
    "Identity": "Video-SubTitle-Combine Plugin 1",
    "TaskLimit": 1,
    "IsExecutOnAllStoragProvider": true,
    "FileEntityNoDivisor": 1,
    "StorageProviderIds": [],
    "FileEntityNoRemainder": 0,
    "FetchLimit": 100,
    "MimeTypePartten": "video/%",
    "IdentityTag": "Require-Video-SubTitle-Combine",
    "ExceptionTag": "Video-SubTitle-Combine-Failed",
    "FetchMode": 2,
    "FFmpegLocation": "ffmpeg.exe",
    "WorkingDirectory": "Working"
  }
}

```

### 確認是否正常運作

1. 確認 `C:\HBKStorage\HBKStorage.PluginIntegration\Logs` 內是否產生正確的 log 檔案

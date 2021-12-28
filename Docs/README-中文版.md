<div align="center">
  <a href="https://github.com/nightsoul357/HBKStorage">
    <img src="https://user-images.githubusercontent.com/48483566/142714588-b05aac2a-2b12-40d8-be59-e32a885b56b2.png">
  </a>
  <h3 align="center">
    一個整合複數儲存個體並提供統一介面的服務
  </h3>
</div>

## Feature

-   **管理多種儲存個體**: 在同一個系統裡建立並使用多種類型的儲存個體
-   **組合儲存個體**: 將多個儲存個體組合為一個較大的儲存群組使用
-   **自動備份**: 透過設置同步規則，自動在不同儲存群組內建立副本
-   **自動清除**: 透過設置清除規則，自動清除不必要的檔案
-   **發行存取權杖**: 發行多種具有存取規則限制的權杖
-   **統一的上傳/下載接口**: 透過統一的 API 接口實現上傳及下載
-   **優化記憶體使用**: 上傳下載過程只會使用最小需求的記憶體
-   **加密儲存**: 以檔案加密形式將檔案儲放於實際的儲存個體上，且不會增加額外儲存成本
-   **負載平衡**: 透過建立多個檔案副本，提高實際存取效率
-   **下載指示**: 在下載檔案前，能對檔案內容進行最後加工(浮水印、Logo、Encrypt、Decrypt...)
-   **插件**: 支援豐富且可自定義的插件


## Getting Started

### Linux

Clone project and execute

```bash
# Install dokcer and docker-compose
apt-get update
apt-get install docker
apt-get install docker-compose
# Build and Start
dokcer-compose up -d
```

Dashboard 會在 `http://localhost:1080`, 可以使用預設的 API Key 進行登入。

[參數說明](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/Install%20Document(Linux).md)

### Windows

[More Information](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/Install%20Document(Windows).md)

## Dashboard

![image](https://user-images.githubusercontent.com/48483566/142719301-f0de6c6f-c94a-4341-8e02-59310873dbf8.png)

正常情況下，應該使用 API 控制檔案而不是 Dashboard，這個僅是提供簡易管理儲存群組的介面，以及測試用的上傳/下載檔案。

## Support Storage Type

| 名稱 | 上傳 | 下載 | 分段上傳 | 分段下載 |
| -------- | -------- | -------- | -------- | -------- |
| Local | O | O | X | O |
| FTP | O | O | X | X |
| Google Drive | O | O | X | O |
| AWS S3 | O | O | X | O |
| Mega | O | O | X | O |
| WebDAV | O | O | X | O |

## How it works

![HBKStorage](https://user-images.githubusercontent.com/48483566/142716208-c8c86813-eeda-47d5-a6b8-77a5f8d3eead.png)

## Documentation

- [API](https://app.swaggerhub.com/apis-docs/nightsoul357/hbk-storage_api/v1)
- [API Key](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/API%20Key.md)
- [Storage Credential](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/Storage%20Credential.md)
- [Policy Rule](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/Policy%20Rule.md)
- [File Access Token](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/File%20Access%20Token.md)
- [Donwload Indicate](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/Donwload%20Indicate.md)
- [Crtpto](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/Crypto.md)
- [External Download File Endpoint](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/External%20Download%20File%20Endpoint.md)
- [Situational Presentation](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/Situational%20Presentation.md)
- [Load Balance]()
- [Work on Azure CDN with token authentication]()
- [Plugin]()

## Coming Soon

- 分段上傳
- 掛載為網路硬碟
- Lock File
- Folder

## License

[MIT License](https://github.com/nightsoul357/HBK-Storage/blob/master/LICENSE) Copyright (c) 2021 nightsoul357

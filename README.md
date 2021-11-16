# HBK Storage

## Introduction

HBK Storage 提供了建立儲存個體集合的方式，可以組合不同總類的儲存方式建立一個儲存服務，並透過對外的統一接口存取儲存服務內的檔案，各個儲存個體之間還能透過同步策略建立檔案副本。
並且 HBK Storage 提供了多種存取檔案的限制方式，可以發行限制存取次數、限制存取規則...等不同的連結。

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

## Example

## Getting Started

### Linux

下載 Release 並執行下列指令:

```bash
# Install dokcer and docker-compose
apt-get update
apt-get install docker
apt-get install docker-compose
# Build and Start
dokcer-compose up -d
```

[More Information](https://)

### Windows

[More Information](https://)

## Support Storage Type

| 名稱 | 上傳 | 下載 | 分段上傳 | 分段下載 |
| -------- | -------- | -------- | -------- | -------- |
| Local | O | O | X | O |
| FTP | O | O | X | X |
| Google Drive | O | O | X | O |
| AWS S3 | O | O | X | O |
| Mega | O | O | X | O |

## How it works

## Documentation

- [Situational Presentation]()
- [Storage Credential]()
- [Policy Rule]()
- [File Access Token]()
- [Donwload Indicate]()
- [Crtpto]()
- [External Download File Endpoint]()
- [Load Balance]()
- [Work on Azure CDN with token authentication]()
- [Plugin]()

## Coming Soon

- 分段上傳
- 掛載為網路硬碟

## License

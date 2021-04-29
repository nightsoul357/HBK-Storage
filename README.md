# HBK Storage

## 簡介

HBK Storage 提供了建立儲存個體集合的方式，可以組合不同總類的儲存方式建立一個儲存服務，並透過對外的統一接口存取儲存服務內的檔案，各個儲存個體之間還能透過同步策略建立檔案副本。
並且 HBK Storage 提供了多種存取檔案的限制方式，可以發行限制存取次數、限制存取規則...等不同的連結。

## 適用情景

* 使用到不同種類的檔案儲存方式(ex: FTP、AWS S3)
* 單個儲存個體擴充成本過高或擴充後的開發難度上升
* 在不同儲存個體之間進行備份
* 發行受到規則限制的存取連結

## 名詞解釋

### Storage Provider

儲存服務，提供一完整檔案存取的服務，不同儲存服務之間的檔案無法存取。
一個 Storage Provider 內或包含一個或多個 Storage Group。

### Storage Group

儲存個體集合，為同一類型的儲存個體所組成的集合，在進行同步作業、檔案存取作業時，皆是以 Storage Group 作為單位。
一個 Storage Group 內會包含一個或多個同種類的 Storage。

### Storage

儲存個體，實際上進行儲存的單一儲存個體，例如 Amazon 的一個 S3 Bucket。
每個 Storage 都有獨立的驗證設定。

## 範例說明

### 範例設備

* 2 台 FTP Server 組成的 Storage-Group 1
* 2 台 AWS S3 組成的 Storage-Group 2
* 2 台 Local Storage 組成的 Storage-Group 3
* 2 台 Google Drive 組成的 Storage-Group 4

### 情景1-完整備份

![image](https://user-images.githubusercontent.com/48483566/114438887-ea4d5d00-9bfa-11eb-86b1-527d15b0a902.png)

將 Storage-Group 1 設置為 Main Storage Group，則所有的上傳檔案都會優先上傳至 Storage-Group 1。

再將 Storgae-Group2 ~ Storgae-Group4 的同步策略設置為**總是同步**，則上傳完成後一份檔案會被鏡像三份至其他三個 Storage-Group，並且每個 Storage-Group 內的檔案都能提供使用者存取。

### 情景2-完整備份但不設置 Main Storage Group

![image](https://user-images.githubusercontent.com/48483566/114439708-df46fc80-9bfb-11eb-9698-2155262ebe3b.png)

於情景1類似，但應未設置 Main Stroge Group，上傳時會選擇當下剩餘容量最多的 Storge Group 進行上傳。

### 情景3-就是要大(不備份)

![image](https://user-images.githubusercontent.com/48483566/114439959-27feb580-9bfc-11eb-9753-364eb33b4fd0.png)

將不同總類的 Storage Group 組成一個超大的儲存空間。
每次上傳都會挑剩餘容量最多的 Storge Group 進行上傳，且因為沒有設置同步，下載時也僅會從當初上傳的 Storage Group 進行下載。

### 情景4-設置同步規則

![image](https://user-images.githubusercontent.com/48483566/114440234-78761300-9bfc-11eb-9407-b7ad41103f9b.png)

將 Storage-Group 2 的同步規則設置為 When File Size > 100MB。

將 Storage-Group 3 的同步規則設置為 When File Mine Type is video/*。

將 Storage-Group 4 的同步規則設置為 When File Size < 100MB。

使不同的儲存群組負責不同的檔案類型。

## 同步規則

| 名稱 | 說明 |
| -------- | -------- |
| Alway Sync | 無論遇到什麼檔案都會同步至已設置為此的 Storage Group |
| Never Sync | 無論如何都不會將任何檔案同步至此 Storage Group，上傳至此 Storage Group 的所有檔案也都不會同步到其他 Storage Group|
| Sync By Policy | 根據設置的規則進行同步 |

### Sync Policy

TODO

## 發行外部存取連結類型

| 名稱 | 說明 |
| -------- | -------- |
| Normal | 具有時間限制、存取次數限制、存取目標檔案限制 |
| NormalNoLimit | 具有時間限制、存取目標檔案限制 |
| AllowTag | 具有時間限制、存取次數限制、存取目標檔案規則限制 |
| AllowTagNoLimit | 具有時間限制、存取目標檔案規則限制 |

## 支援的 Storage 類型

* Local
* Google Drive
* FTP
* AWS S3(minio)
* Mega

## 插件

### 取得多媒體 Metadata 插件

### 取得圖片壓縮副本插件

### 合併影片與字幕軌(強制渲染)插件

### 串流影音(m3u8)插件

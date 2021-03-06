# External Download File Endpoint

## 摘要

透過發行權仗或是存取 `Access Type` 為 `public` 檔案時的路徑。

## 存取權杖加入方式

存取權杖可使用 Query 或 Header 任一種放式加入，鍵值為 `esic`。

## 覆寫 Content-Disposition

請求內容 Header 內若加入 `Content-Disposition`，則回使用此值做為傳回時的 `Content-Disposition` 內容。

若未加入則傳回 `inline` 以及該檔案的檔案名稱。

### 連結

### 透過存取權杖

- URL: `/docs`
- **需加入存取權仗**
- 此方法適用 `Normal`、`NormalNoLimit` 兩種類型的權杖

### 透過存取權杖 + 覆寫檔案名稱

- URL: `/docs/filename/{filename}`
- **需加入存取權仗**
- 此方法適用 `Normal`、`NormalNoLimit` 兩種類型的權杖
- 此方法會覆寫傳回的檔案名稱，並且優先於 `Content-Disposition` 的覆寫，建議用於無法加入 Header 的場合

### 透過 File ID + 存取權杖

- URL: `/docs/fileEntityId/{fileEntityID}`
- **需加入存取權仗**
- 此方法適用 `AllowTag`、`AllowTagNoLimit` 兩種類型的權杖
- 此方法會嘗試判斷指定的檔案是否擁有合法的 Tag，若包含合法 Tag 則會回傳檔案內容

### 透過 File ID + 存取權杖 + 覆寫檔案名稱

- URL: `/docs/fileEntityId/{fileEntityID}/filename/{filename}`
- **需加入存取權仗**
- 此方法適用 `AllowTag`、`AllowTagNoLimit` 兩種類型的權杖。
- 此方法會嘗試判斷指定的檔案是否擁有合法的 Tag，若包含合法 Tag 則會回傳檔案內容
- 此方法會覆寫傳回的檔案名稱，並且優先於 `Content-Disposition` 的覆寫，建議用於無法加入 Header 的場合

### 存取公開檔案

- URL: `/docs/fileEntityId/{fileEntityID}`
- **不需加入存取權仗**
- 此方法適用於存取 `Access Type` 為 `Public` 之檔案

### 存取公開檔案 + 覆寫檔案名稱

- URL: `/docs/fileEntityId/{fileEntityID}/filename/{filename}`
- **不需加入存取權仗**
- 此方法適用於存取 `Access Type` 為 `Public` 之檔案
- 此方法會覆寫傳回的檔案名稱，並且優先於 `Content-Disposition` 的覆寫，建議用於無法加入 Header 的場合

## 關於覆寫檔案名稱的使用時機

### 網頁客戶端的 Disk Cache

使用覆寫檔案名稱的 URL 於網頁端載入檔案時，容易被客戶端快取，若希望快取檔案則建議使用此類型的 URL。

### 使用 Cloudflare 的快取機制時

因 Cloudflare 的快取僅能使用 URL 撰寫規則，透過覆寫檔案名稱的 URL 可以較容易定位其資源類型。
但僅建議使用在**存取公開檔案 + 覆寫檔案名稱**時，否則可能會使權杖驗證失效。

使用此快取時也可能造成存取次數計算錯誤。
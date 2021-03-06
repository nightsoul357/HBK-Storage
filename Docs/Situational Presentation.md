# Situational Presentation

## 簡易使用

當使用 Local Storage 做為檔案儲存方式，隨著系統運行檔案會越來越多，當原有的儲存個體不敷使用時，此時僅需要新增儲存個體便能繼續提供服務。

## 移轉資料

若系統已運行一段時間，希望將檔案的存放位置從 Local Storgae 移轉至 AWS S3 上，僅需要設置新的儲存群組，並設置同步規則即可完成。

## 資料備份，高可用性

系統運行期間，任何的檔案操作都會即時同步到指定的儲存個體，且若某儲存群組突然不可用時，系統會自動切換到其他可用群組繼續提供服務。

## 分類資料，節省成本

有的時候，我們會希望將**經常存取**的資料和**不常存取**的資料進行分類，並存放到不同的儲存個體已節省成本，此時可以透過設置不同的同步策略及清除策略來達成。

## 組合不同類型的儲存個體

若希望透過不同類型的儲存個體提供儲存服務時，可以設置複數儲存群組，並將上傳優先度和下載優先度設為相同的數值，此時系統會將複數不同類型的儲存群組視為一個大型儲存空間提供儲存服務。

## 加密資料，保護檔案(於客戶端)

提供使用者多媒體資源時，大部分的情況並不希望使用者能輕易的保存該多媒體資源，此時可以透過加密方法儲存檔案，並在客戶端實作特定的解密方式並重新渲染檔案內容。

## 加密資料，保護檔案(於伺服器端)

儲存檔案時，不希望將機密資料直接放置於第三方的儲存提供者時(Google、AWS、Azure...)，此時可以透過加密方法儲存檔案，並且在發布檔案連結時選擇由 HBK Storage Server 將檔案即時解密傳回，不須在客戶端進行額外的實作。

## 使用下載指示器

若希望在使用者下載檔案時能在對檔案內容做一些最後的處理，但又希望保留原檔案，且不希望浪費空間儲存修改後的檔案，抑或是根據不同的使用回應不同的修改內容，此時可以在發布檔案時加入下載指示器，讓 HBK Storage Server 在使用者請求檔案時，對檔案內容做最後的修改。

例如: 加密、解密、為圖片加入浮水印...等。

## 使用插件

若希望針對某個檔案產生其他額外資訊，或進行特定處理時可以使用插件。

例如: 為圖片產生不同解析度的副本、為影片產生每秒的預覽圖、轉檔影片...等。
# Install Document(Linux)

## Simple Host

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

## docker-compose.yml 重要參數說明

### API

| 名稱 | 說明 |
| -------- | -------- |
| RootKey:Key | 預設登入使用的 API Key |
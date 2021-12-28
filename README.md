<div align="center">
  <a href="https://github.com/nightsoul357/HBKStorage">
    <img src="https://user-images.githubusercontent.com/48483566/142714588-b05aac2a-2b12-40d8-be59-e32a885b56b2.png">
  </a>
  <h3 align="center">
    A integrate multiple storage and provider one interface service
  </h3>
</div>

English | [中文版](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/README-中文版.md)

## Feature

-   **Manage multiple storage entities**: Create and use multiple types of storage entities in the same system
-   **Combined storage unit**: Combine multiple storage entities into a larger storage group for use
-   **Automatic backup**: By setting synchronization rules, automatically create copies in different storage groups
-   **Automatically clear**: By setting clear rules, unnecessary files are automatically cleared
-   **Issue access token**: Issuing a variety of tokens with restrictions on access rules
-   **One upload/download interface**: Upload and download through one API interface
-   **Optimize memory usage**: Only the minimum required memory will be used during upload and download
-   **Encrypted storage**: Store the files on the actual storage entity in the form of file encryption without adding additional storage costs
-   **Load balancing**: Improve actual access efficiency by creating multiple file copies
-   **Download indicate**: Before downloading the file, the file content can be finalized (watermark, Logo, Encrypt, Decrypt...)
-   **Plugin**: Support rich and customizable plugin


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

Dashboard will listen at `http://localhost:1080`, Can use default API Key for login.

[Parameter](https://github.com/nightsoul357/HBK-Storage/blob/master/Docs/Install%20Document(Linux).md)

### Windows

[More Information](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/Install%20Document(Windows).md)

## Dashboard

![image](https://user-images.githubusercontent.com/48483566/142719301-f0de6c6f-c94a-4341-8e02-59310873dbf8.png)

## Support Storage Type

| Name | Upload | Download | Partial Upload | Partial Download |
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
- [API Key](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/API%20Key.md)
- [Storage Credential](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/Storage%20Credential.md)
- [Policy Rule](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/Policy%20Rule.md)
- [File Access Token](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/File%20Access%20Token.md)
- [Donwload Indicate](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/Donwload%20Indicate.md)
- [Crtpto](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/Crypto.md)
- [External Download File Endpoint](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/External%20Download%20File%20Endpoint.md)
- [Situational Presentation](https://github.com/nightsoul357/HBK-Storage/blob/develop/Docs/Situational%20Presentation.md)
- [Load Balance]()
- [Work on Azure CDN with token authentication]()
- [Plugin]()

## Coming Soon

- Folder
- Lock file
- Partial upload
- Mount to network disk

## License

[MIT License](https://github.com/nightsoul357/HBK-Storage/blob/master/LICENSE) Copyright (c) 2021 nightsoul357

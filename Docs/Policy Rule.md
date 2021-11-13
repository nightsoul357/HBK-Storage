# Policy Rule

## Sync Policy

### �W�h���g�d��

1. �P�B�Ҧ��j�� 100MB ���ɮ�
```
Size > 104857600
```

2. �P�B�Ҧ��v���ɮ�
```
MimeType.StartsWith("video/")
```

3. �P�B�Ҧ� Tag ���]�t Special File �B�j�� 100 MB ���ɮ�

```
FileEntityTag.Any(x => x.Value == "Special File") && Size > 104857600
```

### ����T

| ��� | ���� | ���� | �d�� |
| -------- | -------- | -------- | -------- |
| Name | �ɮצW�� | string | `test.mp4` |
| Size | �ɮפj�p | string | `215486325` |
| ExtendProperty | �X�R��T | string | `video/mp4` |
| MimeType | ���ں����C�髬�� | string  | `video/mp4` |
| CreateDateTime | �إ߮ɶ� | DateTimeOffset | `2019-11-12 22:01:33.8134699 +00:00` |
| FileEntityTag | ���ҲM�� | `List<FileEntityTag>`  |  |

#### FileEntityTag

| ��� | ���� | ���� | �d�� |
| -------- | -------- | -------- | -------- |
| Value | �� | string | `Special File` |

## Clear Policy

### �W�h���g�d��

1. �M���w����L�ƥ����ɮ�
```
ValidFileEntityStorageCount >= 2
```

2. �M���Ҧ��v���ɮ�
```
FileEntity.MimeType.StartsWith("video/")
```

3. �M���w����L�ƥ��B�j�� 100 MB ���ɮ�
```
ValidFileEntityStorageCount >= 2 &&  FileEntity.Size > 104857600
```

### ����T

| ��� | ���� | ���� | �d�� |
| -------- | -------- | -------- | -------- |
| FileEntity | �ɮ׸�T | FileEntity |  |
| FileEntityStorage | �� Storage Group ���x�s���ƥ���T | FileEntityStorage |  |
| ValidFileEntityStorageCount | ���Ī��ƥ��ƶq | int | `2` |

## �`�N

�ɶq�קK Sync �H�� Clear �P�ɱҰʪ����p�A�Y�������g���~�i��ɭP���_�P�B�βM���C
�p�U�ҥܬ����~���]�m���e:

| �\�� | �]�w |
| -------- | -------- |
| Sync Mode | Always |
| Clear Policy | `ValidFileEntityStorageCount >= 2` |

���]�w�|�ɭP�M���ɮ׫�A�P�B�A�ȷ|���զP�B���ɮסA�H��A�Q�M���A���_�`���C
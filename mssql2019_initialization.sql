USE [master]
GO
/****** Object:  Database [HBKStorage]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE DATABASE [HBKStorage]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'HBKStorage' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'HBKStorage_log' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [HBKStorage] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HBKStorage].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [HBKStorage] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [HBKStorage] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [HBKStorage] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [HBKStorage] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [HBKStorage] SET ARITHABORT OFF 
GO
ALTER DATABASE [HBKStorage] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [HBKStorage] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [HBKStorage] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [HBKStorage] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [HBKStorage] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [HBKStorage] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [HBKStorage] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [HBKStorage] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [HBKStorage] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [HBKStorage] SET  ENABLE_BROKER 
GO
ALTER DATABASE [HBKStorage] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [HBKStorage] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [HBKStorage] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [HBKStorage] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [HBKStorage] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [HBKStorage] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [HBKStorage] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [HBKStorage] SET RECOVERY FULL 
GO
ALTER DATABASE [HBKStorage] SET  MULTI_USER 
GO
ALTER DATABASE [HBKStorage] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [HBKStorage] SET DB_CHAINING OFF 
GO
ALTER DATABASE [HBKStorage] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [HBKStorage] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [HBKStorage] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [HBKStorage] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'HBKStorage', N'ON'
GO
ALTER DATABASE [HBKStorage] SET QUERY_STORE = OFF
GO
USE [HBKStorage]
GO
/****** Object:  Table [dbo].[FileEntity]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileEntity](
	[FileEntityID] [uniqueidentifier] NOT NULL,
	[FileEntityNo] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](511) NOT NULL,
	[Size] [bigint] NOT NULL,
	[ExtendProperty] [nvarchar](max) NULL,
	[MimeType] [nvarchar](255) NOT NULL,
	[IsMarkDelete] [bit] NOT NULL,
	[ParentFileEntityID] [uniqueidentifier] NULL,
	[ExpireDateTime] [datetimeoffset](7) NULL,
	[CreateDateTime] [datetimeoffset](7) NOT NULL,
	[UpdateDateTime] [datetimeoffset](7) NULL,
	[DeleteDateTime] [datetimeoffset](7) NULL,
	[Status] [bigint] NOT NULL,
 CONSTRAINT [PK_FileEntity] PRIMARY KEY NONCLUSTERED 
(
	[FileEntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_FileEntity]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_FileEntity] ON [dbo].[FileEntity]
(
	[FileEntityNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_FileEntityRecursive]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_FileEntityRecursive]
AS
WITH ChildrenCTE AS (SELECT          ParentFileEntityID AS RootFileEntityID, FileEntityID, 1 AS ChildLevel
                                                 FROM               dbo.FileEntity
                                                 UNION ALL
                                                 SELECT          Children.RootFileEntityID, FT.FileEntityID, Children.ChildLevel + 1 AS Expr1
                                                 FROM              dbo.FileEntity AS FT INNER JOIN
                                                                             ChildrenCTE AS Children ON FT.ParentFileEntityID = Children.FileEntityID)
    SELECT          RootFileEntityID, FileEntityID, ChildLevel
     FROM               ChildrenCTE AS Children
GO
/****** Object:  Table [dbo].[Storage]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Storage](
	[StorageID] [uniqueidentifier] NOT NULL,
	[StorageNo] [bigint] IDENTITY(1,1) NOT NULL,
	[StorageGroupID] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Type] [int] NOT NULL,
	[SizeLimit] [bigint] NOT NULL,
	[Credentials] [nvarchar](4000) NOT NULL,
	[CreateDateTime] [datetimeoffset](7) NOT NULL,
	[UpdateDateTime] [datetimeoffset](7) NULL,
	[DeleteDateTime] [datetimeoffset](7) NULL,
	[Status] [bigint] NOT NULL,
 CONSTRAINT [PK_Storage] PRIMARY KEY NONCLUSTERED 
(
	[StorageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_Storage]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_Storage] ON [dbo].[Storage]
(
	[StorageNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileEntityStorage]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileEntityStorage](
	[FileEntityStorageID] [uniqueidentifier] NOT NULL,
	[FileEntityStorageNo] [bigint] IDENTITY(1,1) NOT NULL,
	[FileEntityID] [uniqueidentifier] NOT NULL,
	[StorageID] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](1023) NOT NULL,
	[CreatorIdentity] [nvarchar](255) NOT NULL,
	[IsMarkDelete] [bit] NOT NULL,
	[CreateDateTime] [datetimeoffset](7) NOT NULL,
	[UpdateDateTime] [datetimeoffset](7) NULL,
	[DeleteDateTime] [datetimeoffset](7) NULL,
	[Status] [bigint] NOT NULL,
 CONSTRAINT [PK_FileEntityStorage] PRIMARY KEY NONCLUSTERED 
(
	[FileEntityStorageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileEntityStroage]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_FileEntityStroage] ON [dbo].[FileEntityStorage]
(
	[FileEntityStorageNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_StorageAnalysis]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_StorageAnalysis]
AS
SELECT          StorageID,
                                (SELECT          SUM(f.Size) AS Expr1
                                  FROM               dbo.FileEntityStorage AS fes INNER JOIN
                                                              dbo.FileEntity AS f ON fes.FileEntityID = f.FileEntityID
                                  WHERE           (fes.StorageID = s.StorageID) AND (fes.DeleteDateTime IS NULL) AND (f.DeleteDateTime IS NULL)) 
                            AS UsedSize
FROM              dbo.Storage AS s
GO
/****** Object:  Table [dbo].[StorageGroup]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StorageGroup](
	[StorageGroupID] [uniqueidentifier] NOT NULL,
	[StorageGroupNo] [bigint] IDENTITY(1,1) NOT NULL,
	[StorageProviderID] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Type] [int] NOT NULL,
	[SyncMode] [int] NOT NULL,
	[SyncPolicy] [nvarchar](511) NULL,
	[UploadPriority] [int] NOT NULL,
	[DownloadPriority] [int] NOT NULL,
	[CreateDateTime] [datetimeoffset](7) NOT NULL,
	[UpdateDateTime] [datetimeoffset](7) NULL,
	[DeleteDateTime] [datetimeoffset](7) NULL,
	[Status] [bigint] NOT NULL,
 CONSTRAINT [PK_StorageGroup] PRIMARY KEY NONCLUSTERED 
(
	[StorageGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_StorageGroup]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_StorageGroup] ON [dbo].[StorageGroup]
(
	[StorageGroupNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_StorageGroupAnalysis]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_StorageGroupAnalysis]
AS
SELECT          StorageGroupID,
                                (SELECT          SUM(SizeLimit) AS Expr1
                                  FROM               dbo.Storage AS s
                                  WHERE           (StorageGroupID = sg.StorageGroupID) AND (DeleteDateTime IS NULL)) AS SizeLimit,
                                (SELECT          SUM(vwsa.UsedSize) AS Expr1
                                  FROM               dbo.Storage AS s INNER JOIN
                                                              dbo.vw_StorageAnalysis AS vwsa ON s.StorageID = vwsa.StorageID
                                  WHERE           (s.StorageGroupID = sg.StorageGroupID) AND (s.DeleteDateTime IS NULL)) AS UsedSize
FROM              dbo.StorageGroup AS sg
GO
/****** Object:  Table [dbo].[AuthorizeKey]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthorizeKey](
	[AuthorizeKeyID] [uniqueidentifier] NOT NULL,
	[AuthorizeKeyNo] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](4000) NOT NULL,
	[KeyValue] [nvarchar](4000) NOT NULL,
	[CreateDateTime] [datetimeoffset](7) NOT NULL,
	[UpdateDateTime] [datetimeoffset](7) NULL,
	[DeleteDateTime] [datetimeoffset](7) NULL,
	[Type] [int] NOT NULL,
	[ExtendProperty] [nvarchar](4000) NULL,
	[Status] [bigint] NOT NULL,
 CONSTRAINT [PK_AuthorizeKey] PRIMARY KEY NONCLUSTERED 
(
	[AuthorizeKeyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuthorizeKey_AuthorizeKeyID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_AuthorizeKey_AuthorizeKeyID] ON [dbo].[AuthorizeKey]
(
	[AuthorizeKeyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuthorizeKeyScope]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthorizeKeyScope](
	[AuthorizeKeyScopeNo] [bigint] IDENTITY(1,1) NOT NULL,
	[AuthorizeKeyID] [uniqueidentifier] NOT NULL,
	[StorageProviderID] [uniqueidentifier] NOT NULL,
	[CreateDateTime] [datetimeoffset](7) NOT NULL,
	[UpdateDateTime] [datetimeoffset](7) NULL,
	[AllowOperationType] [int] NOT NULL,
 CONSTRAINT [PK_AuthorizeKeyScope] PRIMARY KEY CLUSTERED 
(
	[AuthorizeKeyScopeNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileAccessToken]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileAccessToken](
	[FileAccessTokenID] [uniqueidentifier] NOT NULL,
	[FileAccessTokenNo] [bigint] IDENTITY(1,1) NOT NULL,
	[StorageProviderID] [uniqueidentifier] NOT NULL,
	[StorageGroupID] [uniqueidentifier] NULL,
	[FileEntityID] [uniqueidentifier] NULL,
	[Token] [varchar](5000) NOT NULL,
	[AccessTimesLimit] [int] NOT NULL,
	[AccessTimes] [int] NOT NULL,
	[ExpireDateTime] [datetimeoffset](7) NOT NULL,
	[CreateDateTime] [datetimeoffset](7) NOT NULL,
	[UpdateDateTime] [datetimeoffset](7) NULL,
	[DeleteDateTime] [datetimeoffset](7) NULL,
	[Status] [bigint] NOT NULL,
 CONSTRAINT [PK_FileAccessToken] PRIMARY KEY NONCLUSTERED 
(
	[FileAccessTokenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileAccessToken]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_FileAccessToken] ON [dbo].[FileAccessToken]
(
	[FileAccessTokenNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileEntityStroageOperation]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileEntityStroageOperation](
	[FileEntityStroageOperationNo] [bigint] IDENTITY(1,1) NOT NULL,
	[FileEntityStroageID] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[SyncTargetStorageID] [uniqueidentifier] NULL,
	[CreateDateTime] [datetimeoffset](7) NOT NULL,
	[UpdateDateTime] [datetimeoffset](7) NULL,
	[Message] [nvarchar](511) NOT NULL,
	[ExtendProperty] [nvarchar](511) NOT NULL,
	[Status] [bigint] NOT NULL,
 CONSTRAINT [PK_FileEntityStroageOperation] PRIMARY KEY CLUSTERED 
(
	[FileEntityStroageOperationNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileEntityTag]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileEntityTag](
	[FileEntityTagNo] [bigint] IDENTITY(1,1) NOT NULL,
	[FileEntityID] [uniqueidentifier] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_FileEntityTag] PRIMARY KEY CLUSTERED 
(
	[FileEntityTagNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StorageProvider]    Script Date: 2021/11/7 下午 10:39:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StorageProvider](
	[StorageProviderID] [uniqueidentifier] NOT NULL,
	[StorageProviderNo] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[CreateDateTime] [datetimeoffset](7) NOT NULL,
	[UpdateDateTime] [datetimeoffset](7) NULL,
	[DeleteDateTime] [datetimeoffset](7) NULL,
	[Status] [bigint] NOT NULL,
 CONSTRAINT [PK_StorageProvider] PRIMARY KEY NONCLUSTERED 
(
	[StorageProviderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_StorageProvider]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_StorageProvider] ON [dbo].[StorageProvider]
(
	[StorageProviderNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuthorizeKeyScope_AuthorizeKeyID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_AuthorizeKeyScope_AuthorizeKeyID] ON [dbo].[AuthorizeKeyScope]
(
	[AuthorizeKeyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuthorizeKeyScope_StorageProviderID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_AuthorizeKeyScope_StorageProviderID] ON [dbo].[AuthorizeKeyScope]
(
	[StorageProviderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileAccessToken_FileEntityID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_FileAccessToken_FileEntityID] ON [dbo].[FileAccessToken]
(
	[FileEntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileAccessToken_StorageGroupID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_FileAccessToken_StorageGroupID] ON [dbo].[FileAccessToken]
(
	[StorageGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileAccessToken_StorageProviderID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_FileAccessToken_StorageProviderID] ON [dbo].[FileAccessToken]
(
	[StorageProviderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileEntity_ParentFileEntityID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_FileEntity_ParentFileEntityID] ON [dbo].[FileEntity]
(
	[ParentFileEntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileEntityStorage_FileEntityID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_FileEntityStorage_FileEntityID] ON [dbo].[FileEntityStorage]
(
	[FileEntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileEntityStorage_StorageID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_FileEntityStorage_StorageID] ON [dbo].[FileEntityStorage]
(
	[StorageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileEntityStroageOperation_FileEntityStroageID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_FileEntityStroageOperation_FileEntityStroageID] ON [dbo].[FileEntityStroageOperation]
(
	[FileEntityStroageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileEntityStroageOperation_SyncTargetStorageID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_FileEntityStroageOperation_SyncTargetStorageID] ON [dbo].[FileEntityStroageOperation]
(
	[SyncTargetStorageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FileEntityTag_FileEntityID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_FileEntityTag_FileEntityID] ON [dbo].[FileEntityTag]
(
	[FileEntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Storage_StorageGroupID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_Storage_StorageGroupID] ON [dbo].[Storage]
(
	[StorageGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StorageGroup_StorageProviderID]    Script Date: 2021/11/7 下午 10:39:42 ******/
CREATE NONCLUSTERED INDEX [IX_StorageGroup_StorageProviderID] ON [dbo].[StorageGroup]
(
	[StorageProviderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuthorizeKey] ADD  DEFAULT (newid()) FOR [AuthorizeKeyID]
GO
ALTER TABLE [dbo].[FileAccessToken] ADD  DEFAULT (newid()) FOR [FileAccessTokenID]
GO
ALTER TABLE [dbo].[FileEntity] ADD  DEFAULT (newid()) FOR [FileEntityID]
GO
ALTER TABLE [dbo].[FileEntityStorage] ADD  DEFAULT (newid()) FOR [FileEntityStorageID]
GO
ALTER TABLE [dbo].[Storage] ADD  DEFAULT (newid()) FOR [StorageID]
GO
ALTER TABLE [dbo].[StorageGroup] ADD  DEFAULT (newid()) FOR [StorageGroupID]
GO
ALTER TABLE [dbo].[StorageGroup] ADD  DEFAULT ((1)) FOR [UploadPriority]
GO
ALTER TABLE [dbo].[StorageGroup] ADD  DEFAULT ((1)) FOR [DownloadPriority]
GO
ALTER TABLE [dbo].[StorageProvider] ADD  DEFAULT (newid()) FOR [StorageProviderID]
GO
ALTER TABLE [dbo].[AuthorizeKeyScope]  WITH CHECK ADD  CONSTRAINT [FK_AuthorizeKeyScope_AuthorizeKey] FOREIGN KEY([AuthorizeKeyID])
REFERENCES [dbo].[AuthorizeKey] ([AuthorizeKeyID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AuthorizeKeyScope] CHECK CONSTRAINT [FK_AuthorizeKeyScope_AuthorizeKey]
GO
ALTER TABLE [dbo].[AuthorizeKeyScope]  WITH CHECK ADD  CONSTRAINT [FK_AuthorizeKeyScope_StorageProvider] FOREIGN KEY([StorageProviderID])
REFERENCES [dbo].[StorageProvider] ([StorageProviderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AuthorizeKeyScope] CHECK CONSTRAINT [FK_AuthorizeKeyScope_StorageProvider]
GO
ALTER TABLE [dbo].[FileAccessToken]  WITH CHECK ADD  CONSTRAINT [FK_FileAccessToken_FileEntity] FOREIGN KEY([FileEntityID])
REFERENCES [dbo].[FileEntity] ([FileEntityID])
GO
ALTER TABLE [dbo].[FileAccessToken] CHECK CONSTRAINT [FK_FileAccessToken_FileEntity]
GO
ALTER TABLE [dbo].[FileAccessToken]  WITH CHECK ADD  CONSTRAINT [FK_FileAccessToken_StorageGroup] FOREIGN KEY([StorageGroupID])
REFERENCES [dbo].[StorageGroup] ([StorageGroupID])
GO
ALTER TABLE [dbo].[FileAccessToken] CHECK CONSTRAINT [FK_FileAccessToken_StorageGroup]
GO
ALTER TABLE [dbo].[FileAccessToken]  WITH CHECK ADD  CONSTRAINT [FK_FileAccessToken_StorageProvider] FOREIGN KEY([StorageProviderID])
REFERENCES [dbo].[StorageProvider] ([StorageProviderID])
GO
ALTER TABLE [dbo].[FileAccessToken] CHECK CONSTRAINT [FK_FileAccessToken_StorageProvider]
GO
ALTER TABLE [dbo].[FileEntity]  WITH CHECK ADD  CONSTRAINT [FK_ChildFileEntity_ParentFileEntity] FOREIGN KEY([ParentFileEntityID])
REFERENCES [dbo].[FileEntity] ([FileEntityID])
GO
ALTER TABLE [dbo].[FileEntity] CHECK CONSTRAINT [FK_ChildFileEntity_ParentFileEntity]
GO
ALTER TABLE [dbo].[FileEntityStorage]  WITH CHECK ADD  CONSTRAINT [FK_FileEntityStroage_FileEntity] FOREIGN KEY([FileEntityID])
REFERENCES [dbo].[FileEntity] ([FileEntityID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileEntityStorage] CHECK CONSTRAINT [FK_FileEntityStroage_FileEntity]
GO
ALTER TABLE [dbo].[FileEntityStorage]  WITH CHECK ADD  CONSTRAINT [FK_FileEntityStroage_Storage] FOREIGN KEY([StorageID])
REFERENCES [dbo].[Storage] ([StorageID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileEntityStorage] CHECK CONSTRAINT [FK_FileEntityStroage_Storage]
GO
ALTER TABLE [dbo].[FileEntityStroageOperation]  WITH CHECK ADD  CONSTRAINT [FK_FileEntityStroageOperation_FileEntityStroage] FOREIGN KEY([FileEntityStroageID])
REFERENCES [dbo].[FileEntityStorage] ([FileEntityStorageID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileEntityStroageOperation] CHECK CONSTRAINT [FK_FileEntityStroageOperation_FileEntityStroage]
GO
ALTER TABLE [dbo].[FileEntityStroageOperation]  WITH CHECK ADD  CONSTRAINT [FK_FileEntityStroageOperation_Storage] FOREIGN KEY([SyncTargetStorageID])
REFERENCES [dbo].[Storage] ([StorageID])
GO
ALTER TABLE [dbo].[FileEntityStroageOperation] CHECK CONSTRAINT [FK_FileEntityStroageOperation_Storage]
GO
ALTER TABLE [dbo].[FileEntityTag]  WITH CHECK ADD  CONSTRAINT [FK_FileEntityTag_FileEntity] FOREIGN KEY([FileEntityID])
REFERENCES [dbo].[FileEntity] ([FileEntityID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileEntityTag] CHECK CONSTRAINT [FK_FileEntityTag_FileEntity]
GO
ALTER TABLE [dbo].[Storage]  WITH CHECK ADD  CONSTRAINT [FK_Storage_StorageGroup] FOREIGN KEY([StorageGroupID])
REFERENCES [dbo].[StorageGroup] ([StorageGroupID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Storage] CHECK CONSTRAINT [FK_Storage_StorageGroup]
GO
ALTER TABLE [dbo].[StorageGroup]  WITH CHECK ADD  CONSTRAINT [FK_StorageGroup_StorageProvider] FOREIGN KEY([StorageProviderID])
REFERENCES [dbo].[StorageProvider] ([StorageProviderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StorageGroup] CHECK CONSTRAINT [FK_StorageGroup_StorageProvider]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[22] 2[15] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Children"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 119
               Right = 225
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 2550
         Width = 2745
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_FileEntityRecursive'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_FileEntityRecursive'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "s"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 225
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_StorageAnalysis'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_StorageAnalysis'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "sg"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 231
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_StorageGroupAnalysis'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_StorageGroupAnalysis'
GO
USE [master]
GO
ALTER DATABASE [HBKStorage] SET  READ_WRITE 
GO

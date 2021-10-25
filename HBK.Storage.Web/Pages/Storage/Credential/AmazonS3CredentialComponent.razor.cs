using HBK.Storage.Web.DataSource;
using HBK.Storage.Web.DataSource.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Web.Pages.Storage.Credential
{
    public partial class AmazonS3CredentialComponent : ICredentialsComponent
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
        public string ServiceURL { get; set; }
        public bool ForcePathStyle { get; set; }

        [Parameter]
        public StorageCredentialsBase Credential
        {
            get
            {
                return new AmazonS3StorageCredentials()
                {
                    Storage_type = StorageType.Amazon_s3,
                    AccessKey = this.AccessKey,
                    BucketName = this.BucketName,
                    ForcePathStyle = this.ForcePathStyle,
                    Region = this.Region,
                    SecretKey = this.SecretKey,
                    ServiceURL = this.ServiceURL
                };
            }
            set
            {
                if (value != null)
                {
                    this.AccessKey = ((AmazonS3StorageCredentials)value).AccessKey;
                    this.SecretKey = ((AmazonS3StorageCredentials)value).SecretKey;
                    this.BucketName = ((AmazonS3StorageCredentials)value).BucketName;
                    this.ForcePathStyle = ((AmazonS3StorageCredentials)value).ForcePathStyle;
                    this.Region = ((AmazonS3StorageCredentials)value).Region;
                    this.ServiceURL = ((AmazonS3StorageCredentials)value).ServiceURL;
                }
            }
        }
    }
}

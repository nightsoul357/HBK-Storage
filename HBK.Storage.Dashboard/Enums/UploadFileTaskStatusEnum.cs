namespace HBK.Storage.Dashboard.Enums
{
    public enum UploadFileTaskStatusEnum : int
    {
        [System.Runtime.Serialization.EnumMember(Value = @"pending")]
        Pending = 1,
        [System.Runtime.Serialization.EnumMember(Value = @"uploading")]
        Uploading = 2,
        [System.Runtime.Serialization.EnumMember(Value = @"complete")]
        Complete = 3,
        [System.Runtime.Serialization.EnumMember(Value = @"terminate")]
        Terminate = 4
    }
}

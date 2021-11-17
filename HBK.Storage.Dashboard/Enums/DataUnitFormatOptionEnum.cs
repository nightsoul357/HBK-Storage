namespace HBK.Storage.Dashboard.Enums
{
    public enum DataUnitFormatOptionEnum : short
    {
        /// <summary>
        /// 預設
        /// </summary>
        Default = 0,
        /// <summary>
        /// 以 10^n 次方為基底，而非 2^(10n)
        /// </summary>
        DecBase = 0b01 << 0,
        /// <summary>
        /// 以 bit 為單位，而非 Byte
        /// </summary>
        Bit = 0b01 << 1,
        /// <summary>
        /// 顯示整個字，而非縮寫 (如 Kilobyte，而非 KB)
        /// </summary>
        Word = 0b01 << 2
    }
}

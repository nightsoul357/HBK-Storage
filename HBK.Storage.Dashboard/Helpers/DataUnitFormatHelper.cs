using HBK.Storage.Dashboard.Enums;

namespace HBK.Storage.Dashboard.Helpers
{
    public static class DataUnitFormatHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataUnitFormatOptionEnum"></param>
        /// <returns></returns>
        public static string Format(double? data, DataUnitFormatOptionEnum dataUnitFormatOptionEnum = DataUnitFormatOptionEnum.Default, int decimalPlace = 0)
        {
            if (!data.HasValue)
            {
                return String.Empty;
            }

            int divide = 1024;
            if (dataUnitFormatOptionEnum.HasFlag(DataUnitFormatOptionEnum.DecBase))
            {
                divide = 1000;
            }
            int exp = 0;
            while (data > 0 && data >= divide)
            {
                data /= divide;
                exp++;
            }
            if (exp >= DataUnitFormatHelper.expList.Length)
            {
                throw new FormatException("無法轉換數字至級數表示，超出級數範圍");
            }
            if (dataUnitFormatOptionEnum.HasFlag(DataUnitFormatOptionEnum.Word))
            {
                return $"{Math.Round(data.Value, decimalPlace)} {DataUnitFormatHelper.expList[exp]}{(dataUnitFormatOptionEnum.HasFlag(DataUnitFormatOptionEnum.Bit) ? "bit" : "byte")}";
            }
            else
            {
                return $"{Math.Round(data.Value, decimalPlace)} {DataUnitFormatHelper.expList[exp].FirstOrDefault()}{(dataUnitFormatOptionEnum.HasFlag(DataUnitFormatOptionEnum.Bit) ? "b" : "B")}";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataUnitFormatOptionEnum"></param>
        /// <returns></returns>
        public static string Format(long? data, DataUnitFormatOptionEnum dataUnitFormatOptionEnum = DataUnitFormatOptionEnum.Default, int decimalPlace = 0)
        {
            if (!data.HasValue)
            {
                return String.Empty;
            }
            return DataUnitFormatHelper.Format(Convert.ToDouble(data.Value), dataUnitFormatOptionEnum, decimalPlace);
        }

        /// <summary>
        /// 級數表示全名清單
        /// </summary>
        private static string[] expList = new string[] { "", "Kilo", "Mega", "Giga", "Tera", "Peta", "Exta", "Yotta", "Zetta" };
    }
}

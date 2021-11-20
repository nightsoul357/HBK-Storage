using Newtonsoft.Json;

namespace HBK.Storage.Dashboard.Helpers
{
    public static class JsonConvertHelper
    {
        public static T DeserializeObjectWithDefault<T>(string jsonText, T defaultValue)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                return defaultValue;
            }

            var result = JsonConvert.DeserializeObject<T>(jsonText);
            if (result == null)
            {
                return defaultValue;
            }
            return result;
        }
    }
}

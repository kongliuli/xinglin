using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demo_ReportPrinter.Helpers
{
    /// <summary>
    /// JSON辅助类
    /// </summary>
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _defaultOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {
                new JsonStringEnumConverter()
            }
        };

        /// <summary>
        /// 序列化对象为JSON字符串
        /// </summary>
        public static string Serialize<T>(T obj, JsonSerializerOptions options = null)
        {
            options ??= _defaultOptions;
            return JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// 反序列化JSON字符串为对象
        /// </summary>
        public static T Deserialize<T>(string json, JsonSerializerOptions options = null)
        {
            options ??= _defaultOptions;
            return JsonSerializer.Deserialize<T>(json, options);
        }

        /// <summary>
        /// 将对象保存为JSON文件
        /// </summary>
        public static async Task SaveToFileAsync<T>(T obj, string filePath, JsonSerializerOptions options = null)
        {
            var json = Serialize(obj, options);
            await File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// 从JSON文件加载对象
        /// </summary>
        public static async Task<T> LoadFromFileAsync<T>(string filePath, JsonSerializerOptions options = null)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("文件不存在", filePath);
            }

            var json = await File.ReadAllTextAsync(filePath);
            return Deserialize<T>(json, options);
        }
    }
}
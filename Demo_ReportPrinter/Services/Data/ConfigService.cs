using System;
using System.IO;
using System.Text.Json;

namespace Demo_ReportPrinter.Services.Data
{
    /// <summary>
    /// 配置服务
    /// </summary>
    public class ConfigService
    {
        private readonly string _configDirectory;
        private readonly string _appConfigFile;

        public ConfigService()
        {
            _configDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            _appConfigFile = Path.Combine(_configDirectory, "appsettings.json");

            EnsureConfigDirectory();
        }

        private void EnsureConfigDirectory()
        {
            if (!Directory.Exists(_configDirectory))
            {
                Directory.CreateDirectory(_configDirectory);
            }
        }

        /// <summary>
        /// 加载应用配置
        /// </summary>
        public async Task<T> LoadConfigAsync<T>(string fileName = "appsettings.json")
        {
            var configFile = Path.Combine(_configDirectory, fileName);

            if (!File.Exists(configFile))
            {
                return default;
            }

            var json = await File.ReadAllTextAsync(configFile);
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// 保存应用配置
        /// </summary>
        public async Task SaveConfigAsync<T>(T config, string fileName = "appsettings.json")
        {
            var configFile = Path.Combine(_configDirectory, fileName);
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(configFile, json);
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        public async Task<object> GetConfigValueAsync(string key, string fileName = "appsettings.json")
        {
            var config = await LoadConfigAsync<Dictionary<string, object>>(fileName);
            if (config != null && config.ContainsKey(key))
            {
                return config[key];
            }
            return null;
        }

        /// <summary>
        /// 设置配置值
        /// </summary>
        public async Task SetConfigValueAsync(string key, object value, string fileName = "appsettings.json")
        {
            var config = await LoadConfigAsync<Dictionary<string, object>>(fileName) ?? new Dictionary<string, object>();
            config[key] = value;
            await SaveConfigAsync(config, fileName);
        }
    }
}
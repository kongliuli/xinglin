using System;using System.Collections.Generic;using System.IO;using System.Linq;using System.Text;using System.Text.Json;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Configuration
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    public class ConfigurationManager
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string _configFilePath = "config.json";

        /// <summary>
        /// 配置设置
        /// </summary>
        public ConfigSettings Settings { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigurationManager()
        {
            Settings = new ConfigSettings();
            LoadConfig();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        public ConfigurationManager(string configFilePath)
        {
            _configFilePath = configFilePath;
            Settings = new ConfigSettings();
            LoadConfig();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadConfig()
        {
            if (File.Exists(_configFilePath))
            {
                try
                {
                    var json = File.ReadAllText(_configFilePath);
                    Settings = JsonSerializer.Deserialize<ConfigSettings>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载配置文件失败: {ex.Message}");
                    // 使用默认配置
                    Settings = new ConfigSettings();
                }
            }
            else
            {
                // 配置文件不存在，使用默认配置并保存
                Settings = new ConfigSettings();
                SaveConfig();
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            try
            {
                var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存配置文件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取模板目录
        /// </summary>
        /// <returns>模板目录路径</returns>
        public string GetTemplateDirectory()
        {
            var directory = Settings.TemplateDirectory;
            if (string.IsNullOrEmpty(directory))
            {
                directory = "Templates";
            }

            // 确保目录存在
            Directory.CreateDirectory(directory);
            return directory;
        }

        /// <summary>
        /// 获取数据目录
        /// </summary>
        /// <returns>数据目录路径</returns>
        public string GetDataDirectory()
        {
            var directory = Settings.DataDirectory;
            if (string.IsNullOrEmpty(directory))
            {
                directory = "InputData";
            }

            // 确保目录存在
            Directory.CreateDirectory(directory);
            return directory;
        }

        /// <summary>
        /// 获取资源目录
        /// </summary>
        /// <returns>资源目录路径</returns>
        public string GetAssetsDirectory()
        {
            var directory = Settings.AssetsDirectory;
            if (string.IsNullOrEmpty(directory))
            {
                directory = "Assets";
            }

            // 确保目录存在
            Directory.CreateDirectory(directory);
            return directory;
        }

        /// <summary>
        /// 获取默认模板ID
        /// </summary>
        /// <returns>默认模板ID</returns>
        public string GetDefaultTemplateId()
        {
            return Settings.DefaultTemplateId;
        }

        /// <summary>
        /// 设置默认模板ID
        /// </summary>
        /// <param name="templateId">模板ID</param>
        public void SetDefaultTemplateId(string templateId)
        {
            Settings.DefaultTemplateId = templateId;
            SaveConfig();
        }

        /// <summary>
        /// 获取运行模式
        /// </summary>
        /// <returns>运行模式</returns>
        public string GetRunningMode()
        {
            return Settings.RunningMode;
        }

        /// <summary>
        /// 设置运行模式
        /// </summary>
        /// <param name="mode">运行模式</param>
        public void SetRunningMode(string mode)
        {
            Settings.RunningMode = mode;
            SaveConfig();
        }
    }
}

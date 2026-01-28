using System;
using System.IO;
using System.Text.Json;
using Xinglin.ReportTemplateEditor.WPF.Models;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 快捷键配置服务
    /// </summary>
    public class ShortcutConfigService
    {
        private readonly string _configPath;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ShortcutConfigService()
        {
            // 获取配置文件路径
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolderPath = Path.Combine(appDataPath, "Xinglin", "ReportTemplateEditor");
            
            if (!Directory.Exists(appFolderPath))
            {
                Directory.CreateDirectory(appFolderPath);
            }
            
            _configPath = Path.Combine(appFolderPath, "shortcuts.json");
        }
        
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns>快捷键配置</returns>
        public ShortcutConfig LoadConfig()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = File.ReadAllText(_configPath);
                    var config = JsonSerializer.Deserialize<ShortcutConfig>(json);
                    return config ?? ShortcutConfig.GetDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load shortcut config: {ex.Message}");
            }
            
            return ShortcutConfig.GetDefault();
        }
        
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="config">快捷键配置</param>
        public void SaveConfig(ShortcutConfig config)
        {
            try
            {
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save shortcut config: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 重置为默认配置
        /// </summary>
        public void ResetToDefault()
        {
            var defaultConfig = ShortcutConfig.GetDefault();
            SaveConfig(defaultConfig);
        }
        
        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        /// <returns>配置文件路径</returns>
        public string GetConfigPath()
        {
            return _configPath;
        }
    }
}

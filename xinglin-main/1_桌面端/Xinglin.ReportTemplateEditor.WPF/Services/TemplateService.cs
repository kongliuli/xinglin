using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xinglin.ReportTemplateEditor.WPF.Models;
using Xinglin.ReportTemplateEditor.WPF.Core.Configuration;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 模板服务实现
    /// </summary>
    public class TemplateService : ITemplateService
    {
        private readonly string _templateDirectory;
        private readonly Dictionary<string, TemplateDefinition> _templateCache;
        private readonly object _cacheLock = new object();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateService()
        {
            var configManager = new ConfigurationManager();
            _templateDirectory = configManager.GetTemplateDirectory();
            _templateCache = new Dictionary<string, TemplateDefinition>();
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="templateDirectory">模板目录</param>
        public TemplateService(string templateDirectory)
        {
            _templateDirectory = templateDirectory;
            _templateCache = new Dictionary<string, TemplateDefinition>();
        }
        
        /// <summary>
        /// 加载模板
        /// </summary>
        public async Task<TemplateDefinition> LoadTemplateAsync(string templateId)
        {
            // 检查缓存
            if (_templateCache.TryGetValue(templateId, out var cachedTemplate))
            {
                return cachedTemplate;
            }
            
            // 加载模板
            var filePath = Path.Combine(_templateDirectory, $"{templateId}.json");
            var json = await File.ReadAllTextAsync(filePath);
            var template = JsonSerializer.Deserialize<TemplateDefinition>(json);
            
            // 缓存模板
            lock (_cacheLock)
            {
                _templateCache[templateId] = template;
            }
            
            return template;
        }
        
        /// <summary>
        /// 保存模板
        /// </summary>
        public async Task SaveTemplateAsync(TemplateDefinition template)
        {
            Directory.CreateDirectory(_templateDirectory);
            var filePath = Path.Combine(_templateDirectory, $"{template.TemplateId}.json");
            var json = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
            
            // 更新缓存
            lock (_cacheLock)
            {
                _templateCache[template.TemplateId] = template;
            }
        }
        
        /// <summary>
        /// 获取所有模板
        /// </summary>
        public async Task<List<TemplateDefinition>> GetAllTemplatesAsync()
        {
            if (!Directory.Exists(_templateDirectory))
                return new List<TemplateDefinition>();
            
            var templates = new List<TemplateDefinition>();
            foreach (var file in Directory.GetFiles(_templateDirectory, "*.json"))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                
                // 检查缓存
                if (_templateCache.TryGetValue(fileName, out var cachedTemplate))
                {
                    templates.Add(cachedTemplate);
                }
                else
                {
                    // 加载并缓存模板
                    var json = await File.ReadAllTextAsync(file);
                    var template = JsonSerializer.Deserialize<TemplateDefinition>(json);
                    templates.Add(template);
                    
                    lock (_cacheLock)
                    {
                        _templateCache[fileName] = template;
                    }
                }
            }
            
            return templates;
        }
        
        /// <summary>
        /// 删除模板
        /// </summary>
        public async Task DeleteTemplateAsync(string templateId)
        {
            var filePath = Path.Combine(_templateDirectory, $"{templateId}.json");
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                
                // 从缓存中移除
                lock (_cacheLock)
                {
                    _templateCache.Remove(templateId);
                }
            }
        }
        
        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCache()
        {
            lock (_cacheLock)
            {
                _templateCache.Clear();
            }
        }
        
        /// <summary>
        /// 获取缓存大小
        /// </summary>
        public int GetCacheSize()
        {
            lock (_cacheLock)
            {
                return _templateCache.Count;
            }
        }
    }
}

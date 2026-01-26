using System.IO;
using Newtonsoft.Json;
using Xinglin.Core.Models;
using Xinglin.Core.Services;

namespace Xinglin.Infrastructure.Services
{
    /// <summary>
    /// JSON模板序列化实现，负责将模板定义序列化为JSON字符串和从JSON字符串反序列化为模板定义
    /// </summary>
    public class JsonTemplateSerializer : ITemplateSerializer
    {
        private readonly JsonSerializerSettings _settings;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public JsonTemplateSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }
        
        /// <summary>
        /// 将模板定义序列化为字符串
        /// </summary>
        /// <param name="template">模板定义对象</param>
        /// <returns>序列化后的字符串</returns>
        public string Serialize(ReportTemplateDefinition template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            return JsonConvert.SerializeObject(template, _settings);
        }
        
        /// <summary>
        /// 将字符串反序列化为模板定义
        /// </summary>
        /// <param name="templateString">序列化后的字符串</param>
        /// <returns>模板定义对象</returns>
        public ReportTemplateDefinition Deserialize(string templateString)
        {
            if (string.IsNullOrEmpty(templateString))
                throw new ArgumentNullException(nameof(templateString));
            
            return JsonConvert.DeserializeObject<ReportTemplateDefinition>(templateString, _settings);
        }
        
        /// <summary>
        /// 将模板定义保存到文件
        /// </summary>
        /// <param name="template">模板定义对象</param>
        /// <param name="filePath">文件路径</param>
        public void SaveToFile(ReportTemplateDefinition template, string filePath)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
            
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            string json = Serialize(template);
            File.WriteAllText(filePath, json);
        }
        
        /// <summary>
        /// 从文件加载模板定义
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>模板定义对象</returns>
        public ReportTemplateDefinition LoadFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Template file not found: {filePath}", filePath);
            
            string json = File.ReadAllText(filePath);
            return Deserialize(json);
        }
    }
}
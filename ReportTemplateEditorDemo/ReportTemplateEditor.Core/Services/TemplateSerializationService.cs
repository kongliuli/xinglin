using System;
using System.IO;
using Newtonsoft.Json;
using ReportTemplateEditor.Core.Models;

namespace ReportTemplateEditor.Core.Services
{
    /// <summary>
    /// 模板序列化服务，负责模板的序列化、反序列化、保存和加载
    /// </summary>
    /// <remarks>
    /// 使用Newtonsoft.Json进行JSON序列化和反序列化
    /// 支持TypeNameHandling以正确处理多态类型
    /// </remarks>
    public class TemplateSerializationService
    {
        /// <summary>
        /// 将模板序列化为JSON字符串
        /// </summary>
        /// <param name="template">要序列化的模板</param>
        /// <returns>JSON字符串</returns>
        /// <exception cref="ArgumentNullException">当template参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// var service = new TemplateSerializationService();
        /// var template = new ReportTemplateDefinition { Name = "测试模板" };
        /// var json = service.SerializeTemplate(template);
        /// </code>
        /// </example>
        public string SerializeTemplate(ReportTemplateDefinition template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            return JsonConvert.SerializeObject(template, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        /// <summary>
        /// 从JSON字符串反序列化模板
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>模板对象</returns>
        /// <exception cref="ArgumentException">当json参数为null或空时抛出</exception>
        /// <exception cref="InvalidOperationException">当反序列化失败时抛出</exception>
        /// <example>
        /// <code>
        /// var service = new TemplateSerializationService();
        /// var json = "{\"Name\":\"测试模板\"}";
        /// var template = service.DeserializeTemplate(json);
        /// </code>
        /// </example>
        public ReportTemplateDefinition DeserializeTemplate(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentException("JSON content cannot be null or empty", nameof(json));

            try
            {
                var template = JsonConvert.DeserializeObject<ReportTemplateDefinition>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                if (template == null)
                    throw new InvalidOperationException("Failed to deserialize template");

                return template;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to deserialize template: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 将模板保存到文件
        /// </summary>
        /// <param name="template">要保存的模板</param>
        /// <param name="filePath">文件路径</param>
        /// <exception cref="ArgumentNullException">当template参数为null时抛出</exception>
        /// <exception cref="ArgumentException">当filePath参数为null或空时抛出</exception>
        /// <exception cref="InvalidOperationException">当保存失败时抛出</exception>
        /// <example>
        /// <code>
        /// var service = new TemplateSerializationService();
        /// var template = new ReportTemplateDefinition { Name = "测试模板" };
        /// service.SaveTemplateToFile(template, @"C:\Templates\test.json");
        /// </code>
        /// </example>
        public void SaveTemplateToFile(ReportTemplateDefinition template, string filePath)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            try
            {
                string json = SerializeTemplate(template);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save template to file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 从文件加载模板
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>模板对象</returns>
        /// <exception cref="ArgumentException">当filePath参数为null或空时抛出</exception>
        /// <exception cref="FileNotFoundException">当文件不存在时抛出</exception>
        /// <exception cref="InvalidOperationException">当加载失败时抛出</exception>
        /// <example>
        /// <code>
        /// var service = new TemplateSerializationService();
        /// var template = service.LoadTemplateFromFile(@"C:\Templates\test.json");
        /// </code>
        /// </example>
        public ReportTemplateDefinition LoadTemplateFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Template file not found: {filePath}", filePath);

            try
            {
                string json = File.ReadAllText(filePath);
                return DeserializeTemplate(json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load template from file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 验证模板的有效性
        /// </summary>
        /// <param name="template">要验证的模板</param>
        /// <returns>如果模板有效返回true，否则返回false</returns>
        /// <remarks>
        /// 验证规则：
        /// 1. 模板不为null
        /// 2. 页面宽度和高度大于0
        /// 3. 所有元素的坐标和尺寸有效
        /// 4. 所有元素在页面边界内
        /// </remarks>
        /// <example>
        /// <code>
        /// var service = new TemplateSerializationService();
        /// var template = new ReportTemplateDefinition { PageWidth = 210, PageHeight = 297 };
        /// var isValid = service.ValidateTemplate(template);
        /// // isValid = true
        /// </code>
        /// </example>
        public bool ValidateTemplate(ReportTemplateDefinition template)
        {
            if (template == null)
                return false;

            if (template.PageWidth <= 0 || template.PageHeight <= 0)
                return false;

            if (template.Elements == null)
                return true;

            foreach (var element in template.Elements)
            {
                if (element == null)
                    return false;

                if (element.X < 0 || element.Y < 0)
                    return false;

                if (element.Width <= 0 || element.Height <= 0)
                    return false;

                if (element.X + element.Width > template.PageWidth)
                    return false;

                if (element.Y + element.Height > template.PageHeight)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 将模板导出为JSON字符串
        /// </summary>
        /// <param name="template">要导出的模板</param>
        /// <returns>JSON字符串</returns>
        /// <example>
        /// <code>
        /// var service = new TemplateSerializationService();
        /// var template = new ReportTemplateDefinition { Name = "测试模板" };
        /// var json = service.ExportTemplateToJson(template);
        /// </code>
        /// </example>
        public string ExportTemplateToJson(ReportTemplateDefinition template)
        {
            return SerializeTemplate(template);
        }
    }
}

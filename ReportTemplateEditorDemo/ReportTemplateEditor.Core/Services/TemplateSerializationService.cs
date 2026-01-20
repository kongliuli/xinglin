using System;
using System.IO;
using Newtonsoft.Json;
using ReportTemplateEditor.Core.Models;

namespace ReportTemplateEditor.Core.Services
{
    public class TemplateSerializationService
    {
        public string SerializeTemplate(ReportTemplateDefinition template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            return JsonConvert.SerializeObject(template, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

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

        public string ExportTemplateToJson(ReportTemplateDefinition template)
        {
            return SerializeTemplate(template);
        }
    }
}

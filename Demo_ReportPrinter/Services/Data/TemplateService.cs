using System;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Services.Data
{
    /// <summary>
    /// 模板版本信息
    /// </summary>
    public class TemplateVersion
    {
        public string VersionId { get; set; }
        public string TemplateId { get; set; }
        public string VersionNumber { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FilePath { get; set; }
    }

    /// <summary>
    /// 模板服务实现
    /// </summary>
    public class TemplateService : ITemplateService
    {
        private readonly string _templatesDirectory;
        private readonly string _templatesIndexFile;
        private readonly string _versionsDirectory;

        public TemplateService()
        {
            _templatesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            _templatesIndexFile = Path.Combine(_templatesDirectory, "templates.json");
            _versionsDirectory = Path.Combine(_templatesDirectory, "Versions");

            EnsureTemplatesDirectory();
        }

        private void EnsureTemplatesDirectory()
        {
            if (!Directory.Exists(_templatesDirectory))
            {
                Directory.CreateDirectory(_templatesDirectory);
            }

            if (!Directory.Exists(_versionsDirectory))
            {
                Directory.CreateDirectory(_versionsDirectory);
            }
        }

        public async Task<List<TemplateData>> GetAllTemplatesAsync()
        {
            if (!File.Exists(_templatesIndexFile))
            {
                return new List<TemplateData>();
            }

            var json = await File.ReadAllTextAsync(_templatesIndexFile);
            return JsonSerializer.Deserialize<List<TemplateData>>(json) ?? new List<TemplateData>();
        }

        public async Task<List<TemplateData>> GetTemplatesByCategoryAsync(string category)
        {
            var templates = await GetAllTemplatesAsync();
            return templates.Where(t => t.Config.Category == category).ToList();
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            var templates = await GetAllTemplatesAsync();
            return templates.Select(t => t.Config.Category).Distinct().ToList();
        }

        public async Task<List<TemplateData>> SearchTemplatesAsync(string searchTerm)
        {
            var templates = await GetAllTemplatesAsync();
            return templates.Where(t => 
                t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                t.Config.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public async Task<TemplateData> GetTemplateByIdAsync(string templateId)
        {
            var templateFile = Path.Combine(_templatesDirectory, $"{templateId}.json");

            if (!File.Exists(templateFile))
            {
                throw new FileNotFoundException("模板文件不存在", templateFile);
            }

            var json = await File.ReadAllTextAsync(templateFile);
            return JsonSerializer.Deserialize<TemplateData>(json);
        }

        public async Task SaveTemplateAsync(TemplateData template)
        {
            // 保存当前版本
            await SaveTemplateVersion(template);

            template.ModifiedDate = DateTime.Now;

            // 保存模板文件
            var templateFile = Path.Combine(_templatesDirectory, $"{template.TemplateId}.json");
            var json = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(templateFile, json);

            // 更新模板索引
            await UpdateTemplatesIndexAsync(template);
        }

        public async Task DeleteTemplateAsync(string templateId)
        {
            var templateFile = Path.Combine(_templatesDirectory, $"{templateId}.json");
            if (File.Exists(templateFile))
            {
                File.Delete(templateFile);
            }

            // 删除模板版本
            await DeleteTemplateVersions(templateId);

            // 更新模板索引
            await RemoveFromTemplatesIndexAsync(templateId);
        }

        public Task<TemplateData> LoadDefaultTemplateAsync()
        {
            // 创建默认模板
            var defaultTemplate = new TemplateData
            {
                TemplateId = Guid.NewGuid().ToString(),
                Name = "默认模板",
                Description = "系统默认模板",
                Layout = new LayoutMetadata(),
                Config = new TemplateConfig
                {
                    Version = "1.0",
                    IsLocked = false,
                    Author = "System",
                    Category = "默认",
                    Metadata = new Dictionary<string, object>()
                }
            };

            // 添加一些默认控件
            defaultTemplate.Layout.EditableElements.Add(new ControlElement
            {
                ElementId = Guid.NewGuid().ToString(),
                Type = ControlType.TextBox,
                DisplayName = "文本框",
                X = 50,
                Y = 50,
                Width = 200,
                Height = 30,
                Value = "默认文本",
                EditState = EditableState.Editable
            });

            return Task.FromResult(defaultTemplate);
        }

        public Task<TemplateData> DuplicateTemplateAsync(TemplateData template)
        {
            // 创建新模板，复制原始模板的所有属性
            var duplicatedTemplate = new TemplateData
            {
                TemplateId = Guid.NewGuid().ToString(),
                Name = template.Name,
                Description = template.Description,
                Layout = new LayoutMetadata
                {
                    PaperWidth = template.Layout.PaperWidth,
                    PaperHeight = template.Layout.PaperHeight,
                    PaperType = template.Layout.PaperType,
                    FixedElements = new ObservableCollection<ControlElement>(),
                    EditableElements = new ObservableCollection<ControlElement>()
                },
                Config = new TemplateConfig
                {
                    Version = template.Config.Version,
                    IsLocked = template.Config.IsLocked,
                    Author = template.Config.Author,
                    Category = template.Config.Category,
                    Metadata = new Dictionary<string, object>(template.Config.Metadata)
                }
            };

            // 复制固定元素
            foreach (var element in template.Layout.FixedElements)
            {
                var duplicatedElement = new ControlElement
                {
                    ElementId = Guid.NewGuid().ToString(),
                    Type = element.Type,
                    DisplayName = element.DisplayName,
                    X = element.X,
                    Y = element.Y,
                    Width = element.Width,
                    Height = element.Height,
                    Value = element.Value,
                    EditState = element.EditState
                };
                duplicatedTemplate.Layout.FixedElements.Add(duplicatedElement);
            }

            // 复制可编辑元素，生成新的ElementId
            foreach (var element in template.Layout.EditableElements)
            {
                var duplicatedElement = new ControlElement
                {
                    ElementId = Guid.NewGuid().ToString(),
                    Type = element.Type,
                    DisplayName = element.DisplayName,
                    X = element.X,
                    Y = element.Y,
                    Width = element.Width,
                    Height = element.Height,
                    Value = element.Value,
                    EditState = element.EditState
                };
                duplicatedTemplate.Layout.EditableElements.Add(duplicatedElement);
            }

            return Task.FromResult(duplicatedTemplate);
        }

        public async Task<List<TemplateVersion>> GetTemplateVersionsAsync(string templateId)
        {
            var versionDirectory = Path.Combine(_versionsDirectory, templateId);
            if (!Directory.Exists(versionDirectory))
            {
                return new List<TemplateVersion>();
            }

            var versionFiles = Directory.GetFiles(versionDirectory, "*.json");
            var versions = new List<TemplateVersion>();

            foreach (var file in versionFiles)
            {
                var json = await File.ReadAllTextAsync(file);
                var version = JsonSerializer.Deserialize<TemplateVersion>(json);
                if (version != null)
                {
                    versions.Add(version);
                }
            }

            return versions.OrderByDescending(v => v.CreatedDate).ToList();
        }

        public async Task<TemplateData> GetTemplateVersionAsync(string versionId)
        {
            // 查找版本文件
            var versionFiles = Directory.GetFiles(_versionsDirectory, $"*{versionId}*.json", SearchOption.AllDirectories);
            if (versionFiles.Length == 0)
            {
                throw new FileNotFoundException("版本文件不存在");
            }

            var versionFile = versionFiles.First();
            var json = await File.ReadAllTextAsync(versionFile);
            var version = JsonSerializer.Deserialize<TemplateVersion>(json);

            if (version == null || !File.Exists(version.FilePath))
            {
                throw new FileNotFoundException("版本文件不存在");
            }

            var templateJson = await File.ReadAllTextAsync(version.FilePath);
            return JsonSerializer.Deserialize<TemplateData>(templateJson);
        }

        private async Task SaveTemplateVersion(TemplateData template)
        {
            var versionDirectory = Path.Combine(_versionsDirectory, template.TemplateId);
            if (!Directory.Exists(versionDirectory))
            {
                Directory.CreateDirectory(versionDirectory);
            }

            // 生成版本号
            var versions = await GetTemplateVersionsAsync(template.TemplateId);
            var versionNumber = versions.Count > 0 ? $"{double.Parse(versions.First().VersionNumber) + 0.1}" : "1.0";

            // 保存模板版本
            var versionId = Guid.NewGuid().ToString();
            var versionFilePath = Path.Combine(versionDirectory, $"version_{versionId}.json");
            var templateVersionFilePath = Path.Combine(versionDirectory, $"template_{versionId}.json");

            // 保存模板数据
            var templateJson = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(templateVersionFilePath, templateJson);

            // 保存版本信息
            var version = new TemplateVersion
            {
                VersionId = versionId,
                TemplateId = template.TemplateId,
                VersionNumber = versionNumber,
                Author = template.Config.Author,
                Description = $"版本 {versionNumber}",
                CreatedDate = DateTime.Now,
                FilePath = templateVersionFilePath
            };

            var versionJson = JsonSerializer.Serialize(version, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(versionFilePath, versionJson);
        }

        private Task DeleteTemplateVersions(string templateId)
        {
            var versionDirectory = Path.Combine(_versionsDirectory, templateId);
            if (Directory.Exists(versionDirectory))
            {
                Directory.Delete(versionDirectory, true);
            }
            return Task.CompletedTask;
        }

        private async Task UpdateTemplatesIndexAsync(TemplateData template)
        {
            var templates = await GetAllTemplatesAsync();
            var existingTemplate = templates.FirstOrDefault(t => t.TemplateId == template.TemplateId);

            if (existingTemplate != null)
            {
                existingTemplate.Name = template.Name;
                existingTemplate.Description = template.Description;
                existingTemplate.ModifiedDate = template.ModifiedDate;
            }
            else
            {
                templates.Add(template);
            }

            var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_templatesIndexFile, json);
        }

        private async Task RemoveFromTemplatesIndexAsync(string templateId)
        {
            var templates = await GetAllTemplatesAsync();
            templates.RemoveAll(t => t.TemplateId == templateId);

            var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_templatesIndexFile, json);
        }
    }
}
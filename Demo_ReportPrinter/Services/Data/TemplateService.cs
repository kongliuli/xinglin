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

        public async Task<Result<List<TemplateData>>> GetAllTemplatesAsync()
        {
            try
            {
                if (!File.Exists(_templatesIndexFile))
                {
                    return Result<List<TemplateData>>.Success(new List<TemplateData>());
                }

                var json = await File.ReadAllTextAsync(_templatesIndexFile);
                var templates = JsonSerializer.Deserialize<List<TemplateData>>(json) ?? new List<TemplateData>();
                return Result<List<TemplateData>>.Success(templates);
            }
            catch (IOException ex)
            {
                return Result<List<TemplateData>>.Failure($"读取模板索引文件失败：{ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result<List<TemplateData>>.Failure($"解析模板索引文件失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<List<TemplateData>>.Failure($"获取模板列表失败：{ex.Message}");
            }
        }

        public async Task<Result<TemplateData>> GetTemplateByIdAsync(string templateId)
        {
            try
            {
                var templateFile = Path.Combine(_templatesDirectory, $"{templateId}.json");

                if (!File.Exists(templateFile))
                {
                    return Result<TemplateData>.Failure("模板文件不存在");
                }

                var json = await File.ReadAllTextAsync(templateFile);
                var template = JsonSerializer.Deserialize<TemplateData>(json);

                if (template == null)
                {
                    return Result<TemplateData>.Failure("模板数据已损坏");
                }

                return Result<TemplateData>.Success(template);
            }
            catch (IOException ex)
            {
                return Result<TemplateData>.Failure($"读取模板文件失败：{ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result<TemplateData>.Failure($"解析模板文件失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<TemplateData>.Failure($"获取模板失败：{ex.Message}");
            }
        }

        public async Task<Result> SaveTemplateAsync(TemplateData template)
        {
            try
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

                return Result.Success();
            }
            catch (IOException ex)
            {
                return Result.Failure($"保存模板文件失败：{ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result.Failure($"序列化模板数据失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Failure($"保存模板失败：{ex.Message}");
            }
        }

        public async Task<Result> DeleteTemplateAsync(string templateId)
        {
            try
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

                return Result.Success();
            }
            catch (IOException ex)
            {
                return Result.Failure($"删除模板文件失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Failure($"删除模板失败：{ex.Message}");
            }
        }

        public Task<Result<TemplateData>> LoadDefaultTemplateAsync()
        {
            try
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

                return Task.FromResult(Result<TemplateData>.Success(defaultTemplate));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<TemplateData>.Failure($"加载默认模板失败：{ex.Message}"));
            }
        }

        public Task<Result<TemplateData>> DuplicateTemplateAsync(TemplateData template)
        {
            try
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

                return Task.FromResult(Result<TemplateData>.Success(duplicatedTemplate));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<TemplateData>.Failure($"复制模板失败：{ex.Message}"));
            }
        }

        public async Task<Result> RebuildTemplatesIndexAsync()
        {
            try
            {
                // 扫描Templates目录中的所有.json文件
                var templateFiles = Directory.GetFiles(_templatesDirectory, "*.json")
                    .Where(file => Path.GetFileName(file) != "templates.json") // 排除索引文件本身
                    .ToList();

                var templates = new List<TemplateData>();

                // 解析每个模板文件
                foreach (var file in templateFiles)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var template = JsonSerializer.Deserialize<TemplateData>(json);
                        if (template != null)
                        {
                            templates.Add(template);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"解析模板文件失败 {file}: {ex.Message}");
                    }
                }

                // 保存更新后的索引
                var indexJson = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_templatesIndexFile, indexJson);

                return Result.Success();
            }
            catch (IOException ex)
            {
                return Result.Failure($"重建模板索引失败：{ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result.Failure($"序列化模板索引失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Failure($"重建模板索引失败：{ex.Message}");
            }
        }

        public async Task<Result<TemplateData>> ImportTemplateAsync(string sourceFilePath)
        {
            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    return Result<TemplateData>.Failure("源模板文件不存在");
                }

                // 读取源模板文件
                var json = await File.ReadAllTextAsync(sourceFilePath);
                var template = JsonSerializer.Deserialize<TemplateData>(json);
                if (template == null)
                {
                    return Result<TemplateData>.Failure("无效的模板文件格式");
                }

                // 生成新的模板ID，确保唯一性
                template.TemplateId = Guid.NewGuid().ToString();
                template.CreatedDate = DateTime.Now;
                template.ModifiedDate = DateTime.Now;

                // 保存到目标位置
                var templateFile = Path.Combine(_templatesDirectory, $"{template.TemplateId}.json");
                var templateJson = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(templateFile, templateJson);

                // 更新索引
                await UpdateTemplatesIndexAsync(template);

                return Result<TemplateData>.Success(template);
            }
            catch (IOException ex)
            {
                return Result<TemplateData>.Failure($"读取源模板文件失败：{ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result<TemplateData>.Failure($"解析模板文件失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<TemplateData>.Failure($"导入模板失败：{ex.Message}");
            }
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
            try
            {
                var templates = (await GetAllTemplatesAsync()).Value;
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
            catch (Exception ex)
            {
                Console.WriteLine($"更新模板索引失败：{ex.Message}");
            }
        }

        private async Task RemoveFromTemplatesIndexAsync(string templateId)
        {
            try
            {
                var templates = (await GetAllTemplatesAsync()).Value;
                templates.RemoveAll(t => t.TemplateId == templateId);

                var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_templatesIndexFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新模板索引失败：{ex.Message}");
            }
        }
    }
}
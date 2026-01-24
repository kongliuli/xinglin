using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xinglin.Core.Services;

// 模板服务接口
public interface ITemplateService
{
    Task<IEnumerable<TemplateDto>> GetTemplatesAsync();
    Task<TemplateDto> GetTemplateAsync(Guid templateId);
    Task<Guid> SaveTemplateAsync(TemplateDto template);
    Task<bool> DeleteTemplateAsync(Guid templateId);
    Task<TemplateDto> GetLatestTemplateAsync(string templateName);
    Task<IEnumerable<TemplateVersionDto>> GetTemplateVersionsAsync(Guid templateId);
}

// 模板数据传输对象
public class TemplateDto
{
    public Guid TemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// 模板版本数据传输对象
public class TemplateVersionDto
{
    public Guid VersionId { get; set; }
    public Guid TemplateId { get; set; }
    public string VersionNumber { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

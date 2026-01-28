using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xinglin.Core.Services;

namespace Xinglin.Infrastructure.Services;

public class TemplateService : ITemplateService
{
    private readonly List<TemplateDto> _templates = new List<TemplateDto>();
    private readonly List<TemplateVersionDto> _templateVersions = new List<TemplateVersionDto>();
    private readonly object _lock = new object();

    public async Task<IEnumerable<TemplateDto>> GetTemplatesAsync()
    {
        lock (_lock)
        {
            return _templates.ToList();
        }
    }

    public async Task<TemplateDto> GetTemplateAsync(Guid templateId)
    {
        lock (_lock)
        {
            var template = _templates.FirstOrDefault(t => t.TemplateId == templateId);
            if (template == null)
            {
                throw new KeyNotFoundException($"Template not found: {templateId}");
            }
            return template;
        }
    }

    public async Task<Guid> SaveTemplateAsync(TemplateDto template)
    {
        lock (_lock)
        {
            if (template.TemplateId == Guid.Empty)
            {
                template.TemplateId = Guid.NewGuid();
                template.CreatedAt = DateTime.UtcNow;
                _templates.Add(template);
            }
            else
            {
                var existingTemplate = _templates.FirstOrDefault(t => t.TemplateId == template.TemplateId);
                if (existingTemplate != null)
                {
                    existingTemplate.Name = template.Name;
                    existingTemplate.Version = template.Version;
                    existingTemplate.Content = template.Content;
                    existingTemplate.IsPublished = template.IsPublished;
                    existingTemplate.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    template.CreatedAt = DateTime.UtcNow;
                    _templates.Add(template);
                }
            }

            // Create version history
            var version = new TemplateVersionDto
            {
                VersionId = Guid.NewGuid(),
                TemplateId = template.TemplateId,
                VersionNumber = template.Version,
                Content = template.Content,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = template.CreatedBy
            };
            _templateVersions.Add(version);

            return template.TemplateId;
        }
    }

    public async Task<bool> DeleteTemplateAsync(Guid templateId)
    {
        lock (_lock)
        {
            var template = _templates.FirstOrDefault(t => t.TemplateId == templateId);
            if (template == null)
            {
                return false;
            }

            _templates.Remove(template);
            _templateVersions.RemoveAll(v => v.TemplateId == templateId);
            return true;
        }
    }

    public async Task<TemplateDto> GetLatestTemplateAsync(string templateName)
    {
        lock (_lock)
        {
            var templates = _templates.Where(t => t.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (templates.Count == 0)
            {
                return null;
            }

            return templates.OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt).First();
        }
    }

    public async Task<IEnumerable<TemplateVersionDto>> GetTemplateVersionsAsync(Guid templateId)
    {
        lock (_lock)
        {
            return _templateVersions
                .Where(v => v.TemplateId == templateId)
                .OrderByDescending(v => v.CreatedAt)
                .ToList();
        }
    }
}
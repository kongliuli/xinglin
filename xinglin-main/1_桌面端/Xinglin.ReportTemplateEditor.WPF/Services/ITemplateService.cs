using System.Collections.Generic;
using System.Threading.Tasks;
using Xinglin.ReportTemplateEditor.WPF.Models;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 模板服务接口
    /// </summary>
    public interface ITemplateService
    {
        Task<TemplateDefinition> LoadTemplateAsync(string templateId);
        Task SaveTemplateAsync(TemplateDefinition template);
        Task<List<TemplateDefinition>> GetAllTemplatesAsync();
        Task DeleteTemplateAsync(string templateId);
    }
}

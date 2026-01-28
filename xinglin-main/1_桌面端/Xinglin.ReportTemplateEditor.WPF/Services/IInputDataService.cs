using System.Collections.Generic;
using System.Threading.Tasks;
using Xinglin.ReportTemplateEditor.WPF.Models;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 录入数据服务接口
    /// </summary>
    public interface IInputDataService
    {
        Task<InputData> LoadInputDataAsync(string recordId);
        Task SaveInputDataAsync(InputData inputData);
        Task<List<InputData>> GetInputDataByTemplateIdAsync(string templateId);
    }
}

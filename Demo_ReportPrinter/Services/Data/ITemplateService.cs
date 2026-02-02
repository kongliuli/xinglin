using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Services.Data
{
    /// <summary>
    /// 模板服务接口
    /// </summary>
    public interface ITemplateService
    {
        /// <summary>
        /// 获取所有模板
        /// </summary>
        Task<Result<List<TemplateData>>> GetAllTemplatesAsync();

        /// <summary>
        /// 根据ID获取模板
        /// </summary>
        Task<Result<TemplateData>> GetTemplateByIdAsync(string templateId);

        /// <summary>
        /// 保存模板
        /// </summary>
        Task<Result> SaveTemplateAsync(TemplateData template);

        /// <summary>
        /// 删除模板
        /// </summary>
        Task<Result> DeleteTemplateAsync(string templateId);

        /// <summary>
        /// 加载默认模板
        /// </summary>
        Task<Result<TemplateData>> LoadDefaultTemplateAsync();

        /// <summary>
        /// 复制模板
        /// </summary>
        Task<Result<TemplateData>> DuplicateTemplateAsync(TemplateData template);

        /// <summary>
        /// 重建模板索引
        /// </summary>
        Task<Result> RebuildTemplatesIndexAsync();

        /// <summary>
        /// 导入模板
        /// </summary>
        Task<Result<TemplateData>> ImportTemplateAsync(string sourceFilePath);
    }
}
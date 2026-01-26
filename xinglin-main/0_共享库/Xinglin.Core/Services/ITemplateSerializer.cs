namespace Xinglin.Core.Services
{
    /// <summary>
    /// 模板序列化接口，负责将模板定义序列化为字符串和从字符串反序列化为模板定义
    /// </summary>
    public interface ITemplateSerializer
    {
        /// <summary>
        /// 将模板定义序列化为字符串
        /// </summary>
        /// <param name="template">模板定义对象</param>
        /// <returns>序列化后的字符串</returns>
        string Serialize(Models.ReportTemplateDefinition template);
        
        /// <summary>
        /// 将字符串反序列化为模板定义
        /// </summary>
        /// <param name="templateString">序列化后的字符串</param>
        /// <returns>模板定义对象</returns>
        Models.ReportTemplateDefinition Deserialize(string templateString);
        
        /// <summary>
        /// 将模板定义保存到文件
        /// </summary>
        /// <param name="template">模板定义对象</param>
        /// <param name="filePath">文件路径</param>
        void SaveToFile(Models.ReportTemplateDefinition template, string filePath);
        
        /// <summary>
        /// 从文件加载模板定义
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>模板定义对象</returns>
        Models.ReportTemplateDefinition LoadFromFile(string filePath);
    }
}
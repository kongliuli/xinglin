using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Models.SharedData
{
    /// <summary>
    /// 标签模板
    /// </summary>
    public class LabelTemplate
    {
        /// <summary>
        /// 标签ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 标签文本
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 关联的数据路径引用ID
        /// </summary>
        public string? DefaultDataPathRef { get; set; }
    }

    /// <summary>
    /// 标签类别
    /// </summary>
    public class LabelCategory
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 标签列表
        /// </summary>
        public List<LabelTemplate> Labels { get; set; } = new List<LabelTemplate>();
    }

    /// <summary>
    /// 标签模板数据
    /// </summary>
    public class LabelTemplatesData
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// 类别字典
        /// </summary>
        public Dictionary<string, LabelCategory> Categories { get; set; } = new();
    }
}
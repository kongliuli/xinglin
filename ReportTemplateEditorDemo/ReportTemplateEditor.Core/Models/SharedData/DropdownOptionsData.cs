using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Models.SharedData
{
    /// <summary>
    /// 下拉选项类别
    /// </summary>
    public class DropdownCategory
    {
        /// <summary>
        /// 类别ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 类别名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 选项列表
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();
    }

    /// <summary>
    /// 下拉选项数据
    /// </summary>
    public class DropdownOptionsData
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// 类别字典
        /// </summary>
        public Dictionary<string, DropdownCategory> Categories { get; set; } = new();
    }
}
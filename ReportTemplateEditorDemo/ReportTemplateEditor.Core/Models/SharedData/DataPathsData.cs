using System.Collections.Generic;

namespace ReportTemplateEditor.Core.Models.SharedData
{
    /// <summary>
    /// 数据路径模板
    /// </summary>
    public class DataPathTemplate
    {
        /// <summary>
        /// 路径ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 路径名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 数据路径（支持{index}占位符）
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string FormatString { get; set; } = string.Empty;
    }

    /// <summary>
    /// 数据路径组
    /// </summary>
    public class DataPathGroup
    {
        /// <summary>
        /// 组名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 路径列表
        /// </summary>
        public List<DataPathTemplate> Paths { get; set; } = new List<DataPathTemplate>();
    }

    /// <summary>
    /// 数据路径数据
    /// </summary>
    public class DataPathsData
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// 路径组字典
        /// </summary>
        public Dictionary<string, DataPathGroup> Groups { get; set; } = new();
    }
}
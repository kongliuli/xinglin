namespace Xinglin.Core.Models
{
    /// <summary>
    /// 报告模板模型
    /// </summary>
    public class ReportTemplate
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模板版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 医院ID
        /// </summary>
        public string HospitalId { get; set; }

        /// <summary>
        /// 模板内容（JSON格式）
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否为默认模板
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool IsForceUpdate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
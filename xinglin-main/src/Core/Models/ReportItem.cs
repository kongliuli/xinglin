namespace Xinglin.Core.Models
{
    /// <summary>
    /// 报告项目模型
    /// </summary>
    public class ReportItem
    {
        /// <summary>
        /// 报告编号（外键）
        /// </summary>
        public string ReportNumber { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 项目代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 项目值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 参考范围
        /// </summary>
        public string ReferenceRange { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 结果状态（如：正常、异常、临界）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        public string Description { get; set; }
    }
}
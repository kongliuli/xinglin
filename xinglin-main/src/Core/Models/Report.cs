namespace Xinglin.Core.Models
{
    /// <summary>
    /// 报告模型
    /// </summary>
    public class Report
    {
        /// <summary>
        /// 报告编号
        /// </summary>
        public required string ReportNumber { get; set; }

        /// <summary>
        /// 病人信息
        /// </summary>
        public required Patient Patient { get; set; }

        /// <summary>
        /// 报告项目列表
        /// </summary>
        public required List<ReportItem> ReportItems { get; set; }

        /// <summary>
        /// 报告模板ID
        /// </summary>
        public string? TemplateId { get; set; }

        /// <summary>
        /// 模板版本
        /// </summary>
        public string? TemplateVersion { get; set; }

        /// <summary>
        /// 报告类型
        /// </summary>
        public required string ReportType { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public required string Operator { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string? Reviewer { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        public required DateTime EntryTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public required DateTime UpdateTime { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ReviewTime { get; set; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime? PrintTime { get; set; }

        /// <summary>
        /// 报告状态（如：草稿、已审核、已打印）
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }
    }
}
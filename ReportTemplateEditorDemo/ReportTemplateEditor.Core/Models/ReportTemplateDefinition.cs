using ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Core.Models
{
    /// <summary>
    /// 报告模板定义
    /// </summary>
    public class ReportTemplateDefinition
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; } = "未命名模板";

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 模板版本
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// 模板类型
        /// </summary>
        public string Type { get; set; } = "病理报告";

        /// <summary>
        /// 医院ID
        /// </summary>
        public string HospitalId { get; set; } = string.Empty;

        /// <summary>
        /// 页面宽度（毫米）
        /// </summary>
        public double PageWidth { get; set; } = 210;

        /// <summary>
        /// 页面高度（毫米）
        /// </summary>
        public double PageHeight { get; set; } = 297;

        /// <summary>
        /// 左边距（毫米）
        /// </summary>
        public double MarginLeft { get; set; } = 10;

        /// <summary>
        /// 右边距（毫米）
        /// </summary>
        public double MarginRight { get; set; } = 10;

        /// <summary>
        /// 上边距（毫米）
        /// </summary>
        public double MarginTop { get; set; } = 10;

        /// <summary>
        /// 下边距（毫米）
        /// </summary>
        public double MarginBottom { get; set; } = 10;

        /// <summary>
        /// 页面方向
        /// </summary>
        public string Orientation { get; set; } = "Portrait";

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// 模板元素集合
        /// </summary>
        public List<ElementBase> Elements { get; set; } = new List<ElementBase>();

        /// <summary>
        /// 数据绑定集合
        /// </summary>
        public List<TemplateDataBinding> DataBindings { get; set; } = new List<TemplateDataBinding>();

        /// <summary>
        /// 是否为默认模板
        /// </summary>
        public bool IsDefault { get; set; } = false;

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool IsForceUpdate { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 转换为现有系统的ReportTemplate格式
        /// </summary>
        /// <returns></returns>
        public string ToExistingReportTemplateContent()
        {
            // 这里可以实现与现有系统ReportTemplate的转换逻辑
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// 数据绑定
    /// </summary>
    public class TemplateDataBinding
    {
        /// <summary>
        /// 绑定唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 元素ID
        /// </summary>
        public string ElementId { get; set; } = string.Empty;

        /// <summary>
        /// 数据路径
        /// </summary>
        public string DataPath { get; set; } = string.Empty;

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string FormatString { get; set; } = string.Empty;
    }
}
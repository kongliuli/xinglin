namespace Demo_ReportPrinter.Models.Configuration
{
    /// <summary>
    /// 下拉框选项配置
    /// </summary>
    public class DropdownOptionConfig
    {
        /// <summary>
        /// 选项ID
        /// </summary>
        public string OptionId { get; set; }

        /// <summary>
        /// 选项显示文本
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// 选项值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否默认选中
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; }
    }
}
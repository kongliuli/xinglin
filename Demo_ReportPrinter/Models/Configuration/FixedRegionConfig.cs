namespace Demo_ReportPrinter.Models.Configuration
{
    /// <summary>
    /// 固定区域配置
    /// </summary>
    public class FixedRegionConfig
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 区域X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 区域Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 区域宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 区域高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; }

        /// <summary>
        /// 边框宽度
        /// </summary>
        public double BorderWidth { get; set; }
    }
}
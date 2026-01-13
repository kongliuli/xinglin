using System;

namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 椭圆元素类
    /// </summary>
    public class EllipseElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type { get { return "Ellipse"; } }
        
        /// <summary>
        /// 填充颜色
        /// </summary>
        public string FillColor { get; set; } = "#FFFFFF";
        
        /// <summary>
        /// 边框颜色
        /// </summary>
        public string StrokeColor { get; set; } = "#000000";
        
        /// <summary>
        /// 边框宽度
        /// </summary>
        public double StrokeWidth { get; set; } = 1;
        
        /// <summary>
        /// 边框样式
        /// </summary>
        public string StrokeStyle { get; set; } = "Solid";
    }
}
using System;

namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 矩形元素类
    /// </summary>
    public class RectangleElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type { get { return "Rectangle"; } }
        
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
        
        /// <summary>
        /// 圆角半径
        /// </summary>
        public double CornerRadius { get; set; } = 0;
    }
}
using System;

namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 线条元素类
    /// </summary>
    public class LineElement : ElementBase
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type { get { return "Line"; } }
        
        /// <summary>
        /// 线条起点X坐标
        /// </summary>
        public double StartX { get; set; } = 0;
        
        /// <summary>
        /// 线条起点Y坐标
        /// </summary>
        public double StartY { get; set; } = 0;
        
        /// <summary>
        /// 线条终点X坐标
        /// </summary>
        public double EndX { get; set; } = 100;
        
        /// <summary>
        /// 线条终点Y坐标
        /// </summary>
        public double EndY { get; set; } = 100;
        
        /// <summary>
        /// 线条样式
        /// </summary>
        public string LineStyle { get; set; } = "Solid";
        
        /// <summary>
        /// 线条宽度
        /// </summary>
        public double LineWidth { get; set; } = 1;
        
        /// <summary>
        /// 线条颜色
        /// </summary>
        public string LineColor { get; set; } = "#000000";
        
        /// <summary>
        /// 线条起点样式
        /// </summary>
        public string StartLineCap { get; set; } = "Flat";
        
        /// <summary>
        /// 线条终点样式
        /// </summary>
        public string EndLineCap { get; set; } = "Flat";
    }
}
using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;

namespace Xinglin.ReportTemplateEditor.WPF.Core.Elements
{
    /// <summary>
    /// 线条元素
    /// </summary>
    public class LineElement : ElementBase
    {
        /// <summary>
        /// 起点X坐标
        /// </summary>
        public double StartX { get; set; }

        /// <summary>
        /// 起点Y坐标
        /// </summary>
        public double StartY { get; set; }

        /// <summary>
        /// 终点X坐标
        /// </summary>
        public double EndX { get; set; }

        /// <summary>
        /// 终点Y坐标
        /// </summary>
        public double EndY { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LineElement()
        {
            ElementType = "Line";
            StartX = 0;
            StartY = 0;
            EndX = 200;
            EndY = 0;
            Width = 200;
            Height = 2;
        }
    }
}

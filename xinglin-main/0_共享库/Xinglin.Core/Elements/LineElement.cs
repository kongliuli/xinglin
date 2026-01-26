using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xinglin.Core.Elements
{
    /// <summary>
    /// 线条元素，用于绘制直线
    /// </summary>
    public class LineElement : ElementBase
    {
        private string _lineColor = "#000000";
        private double _lineWidth = 1;
        private string _lineStyle = "Solid";
        private double _startX;
        private double _startY;
        private double _endX;
        private double _endY;

        /// <summary>
        /// 元素类型
        /// </summary>
        public override string Type => "Line";

        /// <summary>
        /// 线条颜色
        /// </summary>
        public string LineColor 
        { 
            get => _lineColor; 
            set => SetProperty(ref _lineColor, value); 
        }

        /// <summary>
        /// 线条宽度
        /// </summary>
        public double LineWidth 
        { 
            get => _lineWidth; 
            set => SetProperty(ref _lineWidth, value); 
        }

        /// <summary>
        /// 线条样式（Solid, Dash, Dot等）
        /// </summary>
        public string LineStyle 
        { 
            get => _lineStyle; 
            set => SetProperty(ref _lineStyle, value); 
        }

        /// <summary>
        /// 起点X坐标
        /// </summary>
        public double StartX 
        { 
            get => _startX; 
            set => SetProperty(ref _startX, value); 
        }

        /// <summary>
        /// 起点Y坐标
        /// </summary>
        public double StartY 
        { 
            get => _startY; 
            set => SetProperty(ref _startY, value); 
        }

        /// <summary>
        /// 终点X坐标
        /// </summary>
        public double EndX 
        { 
            get => _endX; 
            set => SetProperty(ref _endX, value); 
        }

        /// <summary>
        /// 终点Y坐标
        /// </summary>
        public double EndY 
        { 
            get => _endY; 
            set => SetProperty(ref _endY, value); 
        }
    }
}

using System;
using System.ComponentModel;
using ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Core.Models.Controls
{
    /// <summary>
    /// 控件基类
    /// </summary>
    public abstract class ControlBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 控件类型
        /// </summary>
        public ControlType Type { get; set; }

        /// <summary>
        /// X坐标（毫米）
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标（毫米）
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 宽度（毫米）
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度（毫米）
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Z轴顺序
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; } = "#000000";

        /// <summary>
        /// 边框宽度
        /// </summary>
        public double BorderWidth { get; set; } = 0;

        /// <summary>
        /// 边框样式
        /// </summary>
        public string BorderStyle { get; set; } = "Solid";

        /// <summary>
        /// 圆角半径
        /// </summary>
        public double CornerRadius { get; set; } = 0;

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";

        /// <summary>
        /// 透明度
        /// </summary>
        public double Opacity { get; set; } = 1.0;

        /// <summary>
        /// 阴影颜色
        /// </summary>
        public string ShadowColor { get; set; } = "#000000";

        /// <summary>
        /// 阴影深度
        /// </summary>
        public double ShadowDepth { get; set; } = 0;

        /// <summary>
        /// 水平对齐
        /// </summary>
        public string HorizontalAlignment { get; set; } = "Left";

        /// <summary>
        /// 垂直对齐
        /// </summary>
        public string VerticalAlignment { get; set; } = "Top";

        /// <summary>
        /// 旋转角度
        /// </summary>
        public double Rotation { get; set; } = 0;

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;

namespace ReportTemplateEditor.Core.Models.Elements
{
    /// <summary>
    /// 模板元素基类
    /// </summary>
    public abstract class ElementBase
    {
        /// <summary>
        /// 元素唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 元素类型
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// 旋转角度
        /// </summary>
        public double Rotation { get; set; } = 0;

        /// <summary>
        /// Z轴顺序
        /// </summary>
        public int ZIndex { get; set; } = 0;

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";

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
        /// 不透明度
        /// </summary>
        public double Opacity { get; set; } = 1;

        /// <summary>
        /// 阴影颜色
        /// </summary>
        public string ShadowColor { get; set; } = "#000000";

        /// <summary>
        /// 阴影深度
        /// </summary>
        public double ShadowDepth { get; set; } = 0;

        /// <summary>
        /// 水平对齐方式
        /// </summary>
        public string HorizontalAlignment { get; set; } = "Left";

        /// <summary>
        /// 垂直对齐方式
        /// </summary>
        public string VerticalAlignment { get; set; } = "Top";
    }
}
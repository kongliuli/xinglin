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

        /// <summary>
        /// 是否忽略全局字体设置
        /// </summary>
        public bool IgnoreGlobalFontSize { get; set; } = false;

        /// <summary>
        /// 字体家族
        /// </summary>
        public string FontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; } = 12;

        /// <summary>
        /// 字体粗细
        /// </summary>
        public string FontWeight { get; set; } = "Normal";

        /// <summary>
        /// 字体样式
        /// </summary>
        public string FontStyle { get; set; } = "Normal";

        /// <summary>
        /// 前景颜色
        /// </summary>
        public string ForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; } = "Left";

        /// <summary>
        /// 边框圆角半径
        /// </summary>
        public double BorderRadius { get; set; } = 0;

        /// <summary>
        /// 验证元素的有效性
        /// </summary>
        public virtual bool Validate()
        {
            if (Width <= 0)
                throw new InvalidOperationException("Width must be greater than 0");

            if (Height <= 0)
                throw new InvalidOperationException("Height must be greater than 0");

            if (X < 0)
                throw new InvalidOperationException("X position cannot be negative");

            if (Y < 0)
                throw new InvalidOperationException("Y position cannot be negative");

            if (Opacity < 0 || Opacity > 1)
                throw new InvalidOperationException("Opacity must be between 0 and 1");

            if (BorderWidth < 0)
                throw new InvalidOperationException("BorderWidth cannot be negative");

            if (CornerRadius < 0)
                throw new InvalidOperationException("CornerRadius cannot be negative");

            if (ShadowDepth < 0)
                throw new InvalidOperationException("ShadowDepth cannot be negative");

            return true;
        }

        /// <summary>
        /// 验证元素在指定画布边界内是否有效
        /// </summary>
        public virtual bool ValidateBounds(double canvasWidth, double canvasHeight)
        {
            if (X < 0 || Y < 0)
                return false;

            if (X + Width > canvasWidth)
                return false;

            if (Y + Height > canvasHeight)
                return false;

            return true;
        }
    }
}
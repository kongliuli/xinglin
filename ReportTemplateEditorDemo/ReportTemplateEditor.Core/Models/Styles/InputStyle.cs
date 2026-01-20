using System;

namespace ReportTemplateEditor.Core.Models.Styles
{
    /// <summary>
    /// 输入框样式类
    /// 用于定义输入框的字体、颜色、边框等样式属性
    /// </summary>
    public class InputStyle
    {
        /// <summary>
        /// 字体名称
        /// </summary>
        public string FontFamily { get; set; } = "Microsoft YaHei";

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; } = 11;

        /// <summary>
        /// 字体粗细
        /// </summary>
        public string FontWeight { get; set; } = "Normal";

        /// <summary>
        /// 字体样式
        /// </summary>
        public string FontStyle { get; set; } = "Normal";

        /// <summary>
        /// 文本颜色
        /// </summary>
        public string ForegroundColor { get; set; } = "#000000";

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
        public double BorderWidth { get; set; } = 1;

        /// <summary>
        /// 圆角半径
        /// </summary>
        public double CornerRadius { get; set; } = 0;

        /// <summary>
        /// 文本对齐方式
        /// </summary>
        public string TextAlignment { get; set; } = "Left";

        /// <summary>
        /// 垂直对齐方式
        /// </summary>
        public string VerticalAlignment { get; set; } = "Center";

        /// <summary>
        /// 内边距
        /// </summary>
        public Thickness Padding { get; set; } = new Thickness(5);

        /// <summary>
        /// 外边距
        /// </summary>
        public Thickness Margin { get; set; } = new Thickness(0);

        /// <summary>
        /// 输入框宽度
        /// </summary>
        public double Width { get; set; } = 100;

        /// <summary>
        /// 输入框高度
        /// </summary>
        public double Height { get; set; } = 30;

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// 创建默认样式
        /// </summary>
        public static InputStyle Default => new InputStyle();

        /// <summary>
        /// 创建紧凑样式
        /// </summary>
        public static InputStyle Compact => new InputStyle
        {
            FontSize = 10,
            Padding = new Thickness(3),
            Height = 25
        };

        /// <summary>
        /// 创建宽松样式
        /// </summary>
        public static InputStyle Loose => new InputStyle
        {
            FontSize = 12,
            Padding = new Thickness(8),
            Height = 35
        };

        /// <summary>
        /// 克隆当前样式
        /// </summary>
        public InputStyle Clone()
        {
            return new InputStyle
            {
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontWeight = FontWeight,
                FontStyle = FontStyle,
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
                BorderColor = BorderColor,
                BorderWidth = BorderWidth,
                CornerRadius = CornerRadius,
                TextAlignment = TextAlignment,
                VerticalAlignment = VerticalAlignment,
                Padding = Padding,
                Margin = Margin,
                Width = Width,
                Height = Height,
                IsReadOnly = IsReadOnly
            };
        }

        /// <summary>
        /// 验证样式的有效性
        /// </summary>
        public bool Validate()
        {
            if (FontSize <= 0)
                throw new InvalidOperationException("FontSize must be greater than 0");

            if (string.IsNullOrEmpty(FontFamily))
                throw new InvalidOperationException("FontFamily cannot be null or empty");

            if (Width <= 0)
                throw new InvalidOperationException("Width must be greater than 0");

            if (Height <= 0)
                throw new InvalidOperationException("Height must be greater than 0");

            if (BorderWidth < 0)
                throw new InvalidOperationException("BorderWidth cannot be negative");

            if (CornerRadius < 0)
                throw new InvalidOperationException("CornerRadius cannot be negative");

            return true;
        }
    }
}

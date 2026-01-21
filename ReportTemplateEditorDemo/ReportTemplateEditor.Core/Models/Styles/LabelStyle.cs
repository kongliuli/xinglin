using System;

namespace ReportTemplateEditor.Core.Models.Styles
{
    /// <summary>
    /// 标签样式类
    /// 用于定义标签的字体、颜色、对齐等样式属性
    /// </summary>
    public class LabelStyle
    {
        /// <summary>
        /// 字体名称
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
        /// 文本颜色
        /// </summary>
        public string ForegroundColor { get; set; } = "#000000";

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; } = "#FFFFFF";

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
        public Thickness Padding { get; set; } = new Thickness(2);

        /// <summary>
        /// 外边距
        /// </summary>
        public Thickness Margin { get; set; } = new Thickness(0, 0, 5, 0);

        /// <summary>
        /// 创建默认样式
        /// </summary>
        public static LabelStyle Default => new LabelStyle();

        /// <summary>
        /// 创建粗体样式
        /// </summary>
        public static LabelStyle Bold => new LabelStyle
        {
            FontWeight = "Bold"
        };

        /// <summary>
        /// 克隆当前样式
        /// </summary>
        public LabelStyle Clone()
        {
            return new LabelStyle
            {
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontWeight = FontWeight,
                FontStyle = FontStyle,
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
                TextAlignment = TextAlignment,
                VerticalAlignment = VerticalAlignment,
                Padding = Padding,
                Margin = Margin
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

            return true;
        }
    }

    /// <summary>
    /// 厚度结构体
    /// </summary>
    public struct Thickness
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public Thickness(double uniformLength)
        {
            Left = Top = Right = Bottom = uniformLength;
        }

        public Thickness(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static implicit operator Thickness(double uniformLength)
        {
            return new Thickness(uniformLength);
        }
    }
}

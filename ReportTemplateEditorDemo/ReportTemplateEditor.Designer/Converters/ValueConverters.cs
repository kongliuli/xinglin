using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ReportTemplateEditor.Designer.Converters
{
    /// <summary>
    /// 布尔值到可见性转换器
    /// </summary>
    /// <remarks>
    /// 将布尔值转换为Visibility枚举
    /// true转换为Visible，false转换为Collapsed
    /// </remarks>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将布尔值转换为可见性
        /// </summary>
        /// <param name="value">布尔值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>Visibility枚举值</returns>
        /// <example>
        /// <code>
        /// var converter = new BooleanToVisibilityConverter();
        /// var visibility = converter.Convert(true, null, null, null);
        /// // visibility = Visibility.Visible
        /// </code>
        /// </example>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        /// <summary>
        /// 将可见性转换为布尔值
        /// </summary>
        /// <param name="value">Visibility值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>布尔值</returns>
        /// <example>
        /// <code>
        /// var converter = new BooleanToVisibilityConverter();
        /// var boolValue = converter.ConvertBack(Visibility.Visible, null, null, null);
        /// // boolValue = true
        /// </code>
        /// </example>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }

            return false;
        }
    }

    /// <summary>
    /// 布尔值反转转换器
    /// </summary>
    /// <remarks>
    /// 将布尔值取反
    /// true转换为false，false转换为true
    /// </remarks>
    public class BooleanInverseConverter : IValueConverter
    {
        /// <summary>
        /// 将布尔值取反
        /// </summary>
        /// <param name="value">布尔值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>取反后的布尔值</returns>
        /// <example>
        /// <code>
        /// var converter = new BooleanInverseConverter();
        /// var inverted = converter.Convert(true, null, null, null);
        /// // inverted = false
        /// </code>
        /// </example>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }

            return true;
        }

        /// <summary>
        /// 将布尔值取反
        /// </summary>
        /// <param name="value">布尔值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>取反后的布尔值</returns>
        /// <example>
        /// <code>
        /// var converter = new BooleanInverseConverter();
        /// var inverted = converter.ConvertBack(true, null, null, null);
        /// // inverted = false
        /// </code>
        /// </example>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }

            return false;
        }
    }

    /// <summary>
    /// 颜色字符串到画笔转换器
    /// </summary>
    /// <remarks>
    /// 将十六进制颜色字符串转换为Brush对象
    /// 支持格式：#RRGGBB、#AARRGGBB、RRGGBB、AARRGGBB
    /// </remarks>
    public class ColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// 将颜色字符串转换为画笔
        /// </summary>
        /// <param name="value">颜色字符串</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>Brush对象</returns>
        /// <example>
        /// <code>
        /// var converter = new ColorToBrushConverter();
        /// var brush = converter.Convert("#FF0000", null, null, null);
        /// // brush = SolidColorBrush { Color = #FFFF0000 }
        /// </code>
        /// </example>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString && !string.IsNullOrEmpty(colorString))
            {
                try
                {
                    var converter = new BrushConverter();
                    return converter.ConvertFromString(colorString);
                }
                catch
                {
                    return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                }
            }

            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
        }

        /// <summary>
        /// 将画笔转换为颜色字符串
        /// </summary>
        /// <param name="value">Brush对象</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>颜色字符串</returns>
        /// <example>
        /// <code>
        /// var converter = new ColorToBrushConverter();
        /// var colorString = converter.ConvertBack(new SolidColorBrush(Colors.Red), null, null, null);
        /// // colorString = "#FFFF0000"
        /// </code>
        /// </example>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Media.SolidColorBrush brush)
            {
                var color = brush.Color;
                return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            }

            return "#FF000000";
        }
    }

    /// <summary>
    /// 元素类型到图标转换器
    /// </summary>
    /// <remarks>
    /// 根据元素类型返回对应的图标路径
    /// </remarks>
    public class ElementToIconConverter : IValueConverter
    {
        /// <summary>
        /// 将元素类型转换为图标路径
        /// </summary>
        /// <param name="value">元素类型</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>图标路径</returns>
        /// <example>
        /// <code>
        /// var converter = new ElementToIconConverter();
        /// var iconPath = converter.Convert("Text", null, null, null);
        /// // iconPath = "pack://application:,,,/Resources/text-icon.png"
        /// </code>
        /// </example>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string elementType)
            {
                return elementType switch
                {
                    "Text" => "pack://application:,,,/Resources/text-icon.png",
                    "Label" => "pack://application:,,,/Resources/label-icon.png",
                    "Image" => "pack://application:,,,/Resources/image-icon.png",
                    "Table" => "pack://application:,,,/Resources/table-icon.png",
                    "TestItem" => "pack://application:,,,/Resources/test-item-icon.png",
                    "Line" => "pack://application:,,,/Resources/line-icon.png",
                    "Rectangle" => "pack://application:,,,/Resources/rectangle-icon.png",
                    "Ellipse" => "pack://application:,,,/Resources/ellipse-icon.png",
                    "Barcode" => "pack://application:,,,/Resources/barcode-icon.png",
                    "Signature" => "pack://application:,,,/Resources/signature-icon.png",
                    "AutoNumber" => "pack://application:,,,/Resources/auto-number-icon.png",
                    "LabelInputBox" => "pack://application:,,,/Resources/label-input-box-icon.png",
                    _ => "pack://application:,,,/Resources/default-icon.png"
                };
            }

            return "pack://application:,,,/Resources/default-icon.png";
        }

        /// <summary>
        /// 转换回元素类型（未实现）
        /// </summary>
        /// <param name="value">图标路径</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>元素类型</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 缩放级别到文本转换器
    /// </summary>
    /// <remarks>
    /// 将缩放级别（0.1-5.0）转换为百分比文本
    /// </remarks>
    public class ZoomLevelToTextConverter : IValueConverter
    {
        /// <summary>
        /// 将缩放级别转换为百分比文本
        /// </summary>
        /// <param name="value">缩放级别</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>百分比文本</returns>
        /// <example>
        /// <code>
        /// var converter = new ZoomLevelToTextConverter();
        /// var text = converter.Convert(1.5, null, null, null);
        /// // text = "150%"
        /// </code>
        /// </example>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double zoomLevel)
            {
                return $"{zoomLevel * 100:P0}";
            }

            return "100%";
        }

        /// <summary>
        /// 转换回缩放级别
        /// </summary>
        /// <param name="value">百分比文本</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>缩放级别</returns>
        /// <example>
        /// <code>
        /// var converter = new ZoomLevelToTextConverter();
        /// var zoomLevel = converter.ConvertBack("150%", null, null, null);
        /// // zoomLevel = 1.5
        /// </code>
        /// </example>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text && text.EndsWith("%"))
            {
                if (double.TryParse(text.TrimEnd('%'), out double percentage))
                {
                    return percentage / 100.0;
                }
            }

            return 1.0;
        }
    }

    /// <summary>
    /// 多值转换器
    /// </summary>
    /// <remarks>
    /// 根据参数返回不同的值
    /// 用于条件绑定场景
    /// </remarks>
    public class MultiValueConverter : IValueConverter
    {
        /// <summary>
        /// 根据参数返回不同的值
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（true返回第一个值，false返回第二个值）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>转换后的值</returns>
        /// <example>
        /// <code>
        /// var converter = new MultiValueConverter();
        /// var result = converter.Convert("原始值", null, "转换后的值", null);
        /// // result = "转换后的值"
        /// </code>
        /// </example>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is bool boolParam)
            {
                return boolParam ? value : parameter;
            }

            return value;
        }

        /// <summary>
        /// 转换回原始值
        /// </summary>
        /// <param name="value">转换后的值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>原始值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// 空值到可见性转换器
    /// </summary>
    /// <remarks>
    /// 将null或空字符串转换为Collapsed，否则转换为Visible
    /// </remarks>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将空值转换为可见性
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>Visibility枚举值</returns>
        /// <example>
        /// <code>
        /// var converter = new NullToVisibilityConverter();
        /// var visibility = converter.Convert(null, null, null, null);
        /// // visibility = Visibility.Collapsed
        /// var visibility2 = converter.Convert("有内容", null, null, null);
        /// // visibility2 = Visibility.Visible
        /// </code>
        /// </example>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (value is string str && string.IsNullOrEmpty(str)))
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        /// <summary>
        /// 转换回原始值
        /// </summary>
        /// <param name="value">Visibility值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>原始值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 整数到可见性转换器
    /// </summary>
    /// <remarks>
    /// 将整数值转换为Visibility枚举
    /// 0转换为Collapsed，非0转换为Visible
    /// </remarks>
    public class IntToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将整数值转换为可见性
        /// </summary>
        /// <param name="value">整数值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>Visibility枚举值</returns>
        /// <example>
        /// <code>
        /// var converter = new IntToVisibilityConverter();
        /// var visibility = converter.Convert(0, null, null, null);
        /// // visibility = Visibility.Collapsed
        /// var visibility2 = converter.Convert(1, null, null, null);
        /// // visibility2 = Visibility.Visible
        /// </code>
        /// </example>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue > 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        /// <summary>
        /// 转换回整数值
        /// </summary>
        /// <param name="value">Visibility值</param>
        /// <param name="targetType">目标类型（未使用）</param>
        /// <param name="parameter">转换参数（未使用）</param>
        /// <param name="culture">区域信息（未使用）</param>
        /// <returns>整数值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible ? 1 : 0;
            }

            return 0;
        }
    }
}

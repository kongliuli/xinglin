using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Demo_ReportPrinter.Converters
{
    /// <summary>
    /// 布尔值转颜色转换器
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string param)
                {
                    var parts = param.Split('|');
                    if (parts.Length == 2)
                    {
                        var trueColor = (Color)ColorConverter.ConvertFromString(parts[0]);
                        var falseColor = (Color)ColorConverter.ConvertFromString(parts[1]);
                        return new SolidColorBrush(boolValue ? trueColor : falseColor);
                    }
                }
                // 如果没有参数，使用默认颜色
                return boolValue ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC")) : new SolidColorBrush(Colors.Black);
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

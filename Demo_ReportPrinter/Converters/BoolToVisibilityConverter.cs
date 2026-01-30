using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Demo_ReportPrinter.Converters
{
    /// <summary>
    /// 布尔值到可见性转换器
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value is bool && (bool)value;
            bool inverse = parameter is string && parameter.ToString().Equals("Inverse", StringComparison.OrdinalIgnoreCase);

            if (inverse)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool inverse = parameter is string && parameter.ToString().Equals("Inverse", StringComparison.OrdinalIgnoreCase);
                return inverse ? visibility != Visibility.Visible : visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
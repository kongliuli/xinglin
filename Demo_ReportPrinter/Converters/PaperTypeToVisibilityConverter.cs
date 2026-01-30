using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Converters
{
    /// <summary>
    /// 纸张类型到可见性转换器
    /// </summary>
    public class PaperTypeToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">文化</param>
        /// <returns>转换后的值</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PaperSizeType paperType)
            {
                return paperType == PaperSizeType.Custom ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// 反向转换
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">文化</param>
        /// <returns>转换后的值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
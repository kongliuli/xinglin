using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Converters
{
    /// <summary>
    /// 可编辑状态转换器
    /// </summary>
    public class EditableStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditableState state)
            {
                switch (state)
                {
                    case EditableState.Editable:
                        return Brushes.Blue;
                    case EditableState.ReadOnly:
                        return Brushes.Gray;
                    case EditableState.Locked:
                        return Brushes.Red;
                    default:
                        return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
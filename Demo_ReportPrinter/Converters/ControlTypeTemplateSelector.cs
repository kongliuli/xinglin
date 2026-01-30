using System.Windows; 
using System.Windows.Controls; 
using Demo_ReportPrinter.Models.CoreEntities; 

namespace Demo_ReportPrinter.Converters 
{
    /// <summary>
    /// 控件类型模板选择器
    /// </summary>
    public class ControlTypeTemplateSelector : DataTemplateSelector 
    {
        public DataTemplate TextBoxTemplate { get; set; }
        public DataTemplate ComboBoxTemplate { get; set; }
        public DataTemplate DatePickerTemplate { get; set; }
        public DataTemplate CheckBoxTemplate { get; set; }
        public DataTemplate TableTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate ChartTemplate { get; set; }
        public DataTemplate FixedTextTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) 
        {
            if (item is ControlElement controlElement) 
            {
                switch (controlElement.Type) 
                {
                    case ControlType.TextBox:
                        return TextBoxTemplate ?? GetDefaultTemplate(container);
                    case ControlType.ComboBox:
                        return ComboBoxTemplate ?? GetDefaultTemplate(container);
                    case ControlType.DatePicker:
                        return DatePickerTemplate ?? GetDefaultTemplate(container);
                    case ControlType.CheckBox:
                        return CheckBoxTemplate ?? GetDefaultTemplate(container);
                    case ControlType.Table:
                        return TableTemplate ?? GetDefaultTemplate(container);
                    case ControlType.Image:
                        return ImageTemplate ?? GetDefaultTemplate(container);
                    case ControlType.Chart:
                        return ChartTemplate ?? GetDefaultTemplate(container);
                    case ControlType.FixedText:
                        return FixedTextTemplate ?? GetDefaultTemplate(container);
                    default:
                        return GetDefaultTemplate(container);
                }
            }
            return base.SelectTemplate(item, container);
        }

        /// <summary>
        /// 获取默认模板
        /// </summary>
        /// <param name="container">容器</param>
        /// <returns>默认模板</returns>
        private DataTemplate GetDefaultTemplate(DependencyObject container)
        {
            // 创建一个默认的文本框模板
            var factory = new FrameworkElementFactory(typeof(TextBox));
            factory.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding("Value") { Mode = System.Windows.Data.BindingMode.TwoWay });
            factory.SetValue(TextBox.MarginProperty, new Thickness(2));
            
            return new DataTemplate { VisualTree = factory };
        }
    }
}
// �ο��ļ� - ��ע�͵�
/*
using System.Windows;
using System.Windows.Controls;
using Demo_ReportPrinter.Models.CoreEntities;

namespace Demo_ReportPrinter.Views
{
    /// <summary>
    /// 控件模板选择�?    /// 根据控件类型自动选择对应的DataTemplate
    /// </summary>
    public class ControlTemplateSelector : DataTemplateSelector
    {
        #region 模板属�?
        /// <summary>
        /// 文本框模�?        /// </summary>
        public DataTemplate TextBoxTemplate { get; set; }

        /// <summary>
        /// 下拉框模�?        /// </summary>
        public DataTemplate ComboBoxTemplate { get; set; }

        /// <summary>
        /// 日期选择模板
        /// </summary>
        public DataTemplate DatePickerTemplate { get; set; }

        /// <summary>
        /// 复选框模板
        /// </summary>
        public DataTemplate CheckBoxTemplate { get; set; }

        /// <summary>
        /// 表格模板
        /// </summary>
        public DataTemplate TableTemplate { get; set; }

        /// <summary>
        /// 图片模板
        /// </summary>
        public DataTemplate ImageTemplate { get; set; }

        /// <summary>
        /// 图表模板
        /// </summary>
        public DataTemplate ChartTemplate { get; set; }

        /// <summary>
        /// 固定文本模板
        /// </summary>
        public DataTemplate FixedTextTemplate { get; set; }

        /// <summary>
        /// 线条模板
        /// </summary>
        public DataTemplate LineTemplate { get; set; }

        /// <summary>
        /// 矩形模板
        /// </summary>
        public DataTemplate RectangleTemplate { get; set; }

        /// <summary>
        /// 圆形模板
        /// </summary>
        public DataTemplate CircleTemplate { get; set; }

        #endregion

        #region 模板选择逻辑

        /// <summary>
        /// 根据控件类型选择对应的模�?        /// </summary>
        /// <param name="item">控件元素</param>
        /// <param name="container">容器</param>
        /// <returns>对应的数据模�?/returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ControlElement element)
            {
                switch (element.Type)
                {
                    case ControlType.TextBox:
                        return TextBoxTemplate;

                    case ControlType.ComboBox:
                        return ComboBoxTemplate;

                    case ControlType.DatePicker:
                        return DatePickerTemplate;

                    case ControlType.CheckBox:
                        return CheckBoxTemplate;

                    case ControlType.Table:
                        return TableTemplate;

                    case ControlType.Image:
                        return ImageTemplate;

                    case ControlType.Chart:
                        return ChartTemplate;

                    case ControlType.FixedText:
                        return FixedTextTemplate;

                    case ControlType.Line:
                        return LineTemplate;

                    case ControlType.Rectangle:
                        return RectangleTemplate;

                    case ControlType.Circle:
                        return CircleTemplate;

                    default:
                        return FixedTextTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }

        #endregion
    }
}

*/

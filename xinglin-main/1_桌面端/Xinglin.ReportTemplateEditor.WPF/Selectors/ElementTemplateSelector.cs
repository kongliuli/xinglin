using System.Windows;
using System.Windows.Controls;
using Xinglin.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Selectors;

public class ElementTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextElementTemplate { get; set; }
    public DataTemplate ImageElementTemplate { get; set; }
    public DataTemplate LineElementTemplate { get; set; }
    public DataTemplate TableElementTemplate { get; set; }
    
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is TextElement)
        {
            return TextElementTemplate;
        }
        else if (item is ImageElement)
        {
            return ImageElementTemplate;
        }
        else if (item is LineElement)
        {
            return LineElementTemplate;
        }
        else if (item is TableElement)
        {
            return TableElementTemplate;
        }
        
        return base.SelectTemplate(item, container);
    }
}
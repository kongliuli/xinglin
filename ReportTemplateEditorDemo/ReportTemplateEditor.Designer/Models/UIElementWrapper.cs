using System.Windows;
using System.Windows.Controls;
using TemplateElements = ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Designer.Models
{
    public class UIElementWrapper
    {
        public TemplateElements.ElementBase ModelElement { get; set; }
        public UIElement UiElement { get; set; }
        public Border SelectionBorder { get; set; }

        public UIElementWrapper()
        {
        }

        public UIElementWrapper(TemplateElements.ElementBase modelElement, UIElement uiElement, Border selectionBorder = null)
        {
            ModelElement = modelElement;
            UiElement = uiElement;
            SelectionBorder = selectionBorder;
        }

        public void UpdatePosition(double x, double y)
        {
            if (ModelElement != null)
            {
                ModelElement.X = x;
                ModelElement.Y = y;
            }

            if (UiElement != null)
            {
                Canvas.SetLeft(UiElement, x);
                Canvas.SetTop(UiElement, y);
            }

            if (SelectionBorder != null)
            {
                Canvas.SetLeft(SelectionBorder, x);
                Canvas.SetTop(SelectionBorder, y);
            }
        }

        public void UpdateSize(double width, double height)
        {
            if (ModelElement != null)
            {
                ModelElement.Width = width;
                ModelElement.Height = height;
            }

            if (UiElement is FrameworkElement frameworkElement)
            {
                frameworkElement.Width = width;
                frameworkElement.Height = height;
            }

            if (SelectionBorder != null)
            {
                SelectionBorder.Width = width;
                SelectionBorder.Height = height;
            }
        }

        public void SetSelection(bool isSelected)
        {
            if (SelectionBorder != null)
            {
                SelectionBorder.Visibility = isSelected ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public bool IsSelected
        {
            get
            {
                return SelectionBorder != null && SelectionBorder.Visibility == Visibility.Visible;
            }
        }
    }
}

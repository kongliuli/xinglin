using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ReportTemplateEditor.Designer.Models;
using ReportTemplateEditor.Engine;
using TemplateElements = ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Designer.Helpers
{
    public static class ElementHelper
    {
        public static Border CreateSelectionBorder(UIElement uiElement)
        {
            var border = new Border
            {
                BorderBrush = Brushes.Blue,
                BorderThickness = new Thickness(1),
                Visibility = Visibility.Hidden,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true,
                IsHitTestVisible = false
            };

            if (uiElement is FrameworkElement fe)
            {
                border.Width = fe.Width;
                border.Height = fe.Height;
            }

            return border;
        }

        public static void SetTextElementContent(TextBlock textBlock, TemplateElements.TextElement textElement, object boundData, DataBindingEngine dataBindingEngine)
        {
            if (boundData != null && !string.IsNullOrEmpty(textElement.DataBindingPath))
            {
                try
                {
                    object value = dataBindingEngine.GetValue(boundData, textElement.DataBindingPath);
                    textBlock.Text = value?.ToString() ?? textElement.Text;
                }
                catch
                {
                    textBlock.Text = textElement.Text;
                }
            }
            else
            {
                textBlock.Text = textElement.Text;
            }
        }

        public static void SetTestItemElementContent(
            TemplateElements.TestItemElement testItem,
            TextBlock itemNameText,
            TextBlock resultText,
            TextBlock unitText,
            TextBlock referenceText,
            object boundData,
            DataBindingEngine dataBindingEngine)
        {
            if (boundData != null)
            {
                if (!string.IsNullOrEmpty(testItem.ItemNameDataPath))
                {
                    object value = dataBindingEngine.GetValue(boundData, testItem.ItemNameDataPath);
                    itemNameText.Text = value?.ToString() ?? testItem.ItemName;
                }
                else
                {
                    itemNameText.Text = testItem.ItemName;
                }

                if (!string.IsNullOrEmpty(testItem.ResultDataPath))
                {
                    object value = dataBindingEngine.GetValue(boundData, testItem.ResultDataPath);
                    resultText.Text = value?.ToString() ?? testItem.Result;
                }
                else
                {
                    resultText.Text = testItem.Result;
                }

                if (!string.IsNullOrEmpty(testItem.UnitDataPath))
                {
                    object value = dataBindingEngine.GetValue(boundData, testItem.UnitDataPath);
                    unitText.Text = value?.ToString() ?? testItem.Unit;
                }
                else
                {
                    unitText.Text = testItem.Unit;
                }

                if (!string.IsNullOrEmpty(testItem.ReferenceRangeDataPath))
                {
                    object value = dataBindingEngine.GetValue(boundData, testItem.ReferenceRangeDataPath);
                    referenceText.Text = value?.ToString() ?? testItem.ReferenceRange;
                }
                else
                {
                    referenceText.Text = testItem.ReferenceRange;
                }
            }
            else
            {
                itemNameText.Text = testItem.ItemName;
                resultText.Text = testItem.Result;
                unitText.Text = testItem.Unit;
                referenceText.Text = testItem.ReferenceRange;
            }
        }

        public static void UpdateElementSize(UIElementWrapper wrapper)
        {
            if (wrapper?.UiElement is FrameworkElement fe && fe.ActualWidth > 0 && fe.ActualHeight > 0)
            {
                wrapper.ModelElement.Width = fe.ActualWidth;
                wrapper.ModelElement.Height = fe.ActualHeight;

                if (wrapper.SelectionBorder != null)
                {
                    wrapper.SelectionBorder.Width = fe.ActualWidth;
                    wrapper.SelectionBorder.Height = fe.ActualHeight;
                }
            }
        }

        public static void ApplyGlobalFontSizeToElement(TemplateElements.ElementBase element, double globalFontSize, UIElementWrapper wrapper)
        {
            if (element == null || element.IgnoreGlobalFontSize)
                return;

            if (element is TemplateElements.TextElement textElement && wrapper?.UiElement is TextBlock textBlock)
            {
                textElement.FontSize = globalFontSize;
                textBlock.FontSize = globalFontSize;
            }
            else if (element is TemplateElements.LabelElement labelElement && wrapper?.UiElement is TextBlock labelBlock)
            {
                labelElement.FontSize = globalFontSize;
                labelBlock.FontSize = globalFontSize;
            }
        }

        public static void UpdateAllBindings(
            List<UIElementWrapper> elementWrappers,
            object boundData,
            DataBindingEngine dataBindingEngine)
        {
            if (elementWrappers == null || boundData == null)
                return;

            foreach (var wrapper in elementWrappers)
            {
                if (wrapper.ModelElement is TemplateElements.TextElement textElement && wrapper.UiElement is TextBlock textBlock)
                {
                    SetTextElementContent(textBlock, textElement, boundData, dataBindingEngine);
                }
                else if (wrapper.ModelElement is TemplateElements.TestItemElement testItem)
                {
                    var container = wrapper.UiElement as Panel;
                    if (container != null)
                    {
                        var itemNameText = container.FindName("itemNameText") as TextBlock;
                        var resultText = container.FindName("resultText") as TextBlock;
                        var unitText = container.FindName("unitText") as TextBlock;
                        var referenceText = container.FindName("referenceText") as TextBlock;

                        if (itemNameText != null && resultText != null && unitText != null && referenceText != null)
                        {
                            SetTestItemElementContent(testItem, itemNameText, resultText, unitText, referenceText, boundData, dataBindingEngine);
                        }
                    }
                }
            }
        }
    }
}

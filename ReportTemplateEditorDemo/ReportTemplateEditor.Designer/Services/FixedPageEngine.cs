using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models;

namespace ReportTemplateEditor.Designer.Services
{
    public class FixedPageEngine
    {
        private readonly UIElementFactory _uiElementFactory;

        public FixedPageEngine(UIElementFactory uiElementFactory)
        {
            _uiElementFactory = uiElementFactory ?? throw new ArgumentNullException(nameof(uiElementFactory));
        }

        public FixedPage GenerateFixedPage(ReportTemplateDefinition template, List<ElementBase> elements)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            double mmToPx = 96.0 / 25.4;
            double pageWidth = template.PageWidth * mmToPx;
            double pageHeight = template.PageHeight * mmToPx;

            var fixedPage = new FixedPage
            {
                Width = pageWidth,
                Height = pageHeight,
                Background = Brushes.White
            };

            foreach (var element in elements)
            {
                var uiElement = CreateUIElement(element);
                if (uiElement != null)
                {
                    double x = element.X * mmToPx;
                    double y = element.Y * mmToPx;
                    double width = element.Width * mmToPx;
                    double height = element.Height * mmToPx;

                    FixedPage.SetLeft(uiElement, x);
                    FixedPage.SetTop(uiElement, y);

                    if (uiElement is FrameworkElement frameworkElement)
                    {
                        frameworkElement.Width = width;
                        frameworkElement.Height = height;
                    }

                    fixedPage.Children.Add(uiElement);
                }
            }

            return fixedPage;
        }

        private UIElement CreateUIElement(ElementBase element)
        {
            return _uiElementFactory.CreateUIElement(element);
        }
    }
}
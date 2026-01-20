using System;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models.Widgets;

namespace ReportTemplateEditor.Core.Services
{
    public class ElementCreationService
    {
        private readonly WidgetRegistry _widgetRegistry;

        public ElementCreationService()
        {
            _widgetRegistry = WidgetRegistry.Instance;
        }

        public ElementCreationService(WidgetRegistry widgetRegistry)
        {
            _widgetRegistry = widgetRegistry ?? throw new ArgumentNullException(nameof(widgetRegistry));
        }

        public ElementBase CreateElement(string elementType, double x = 0, double y = 0)
        {
            var element = _widgetRegistry.CreateElement(elementType);

            if (element != null)
            {
                element.X = x;
                element.Y = y;
                SetDefaultProperties(element);
            }

            return element;
        }

        public TextElement CreateTextElement(double x = 0, double y = 0, string text = "文本")
        {
            var element = (TextElement)CreateElement("Text", x, y);
            element.Text = text;
            return element;
        }

        public LabelElement CreateLabelElement(double x = 0, double y = 0, string text = "标签")
        {
            var element = (LabelElement)CreateElement("Label", x, y);
            element.Text = text;
            return element;
        }

        public ImageElement CreateImageElement(double x = 0, double y = 0, string imagePath = "")
        {
            var element = (ImageElement)CreateElement("Image", x, y);
            element.ImagePath = imagePath;
            return element;
        }

        public TableElement CreateTableElement(double x = 0, double y = 0, int rows = 3, int columns = 3)
        {
            var element = (TableElement)CreateElement("Table", x, y);
            element.Rows = rows;
            element.Columns = columns;
            return element;
        }

        public TestItemElement CreateTestItemElement(double x = 0, double y = 0)
        {
            return (TestItemElement)CreateElement("TestItem", x, y);
        }

        public LineElement CreateLineElement(double x = 0, double y = 0)
        {
            return (LineElement)CreateElement("Line", x, y);
        }

        public RectangleElement CreateRectangleElement(double x = 0, double y = 0)
        {
            return (RectangleElement)CreateElement("Rectangle", x, y);
        }

        public EllipseElement CreateEllipseElement(double x = 0, double y = 0)
        {
            return (EllipseElement)CreateElement("Ellipse", x, y);
        }

        public BarcodeElement CreateBarcodeElement(double x = 0, double y = 0)
        {
            return (BarcodeElement)CreateElement("Barcode", x, y);
        }

        public SignatureElement CreateSignatureElement(double x = 0, double y = 0)
        {
            return (SignatureElement)CreateElement("Signature", x, y);
        }

        public AutoNumberElement CreateAutoNumberElement(double x = 0, double y = 0)
        {
            return (AutoNumberElement)CreateElement("AutoNumber", x, y);
        }

        private void SetDefaultProperties(ElementBase element)
        {
            if (element == null)
                return;

            if (element.Width <= 0)
                element.Width = 100;

            if (element.Height <= 0)
                element.Height = 50;

            if (string.IsNullOrEmpty(element.FontFamily))
                element.FontFamily = "Microsoft YaHei";

            if (element.FontSize <= 0)
                element.FontSize = 12;

            if (string.IsNullOrEmpty(element.ForegroundColor))
                element.ForegroundColor = "#000000";

            if (string.IsNullOrEmpty(element.BackgroundColor))
                element.BackgroundColor = "#FFFFFF";
        }

        public bool ValidateElement(ElementBase element)
        {
            if (element == null)
                return false;

            if (element.Width <= 0 || element.Height <= 0)
                return false;

            if (element.X < 0 || element.Y < 0)
                return false;

            return true;
        }

        public ElementBase CloneElement(ElementBase source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var cloned = CreateElement(source.Type, source.X, source.Y);

            if (cloned != null)
            {
                cloned.Width = source.Width;
                cloned.Height = source.Height;
                cloned.FontFamily = source.FontFamily;
                cloned.FontSize = source.FontSize;
                cloned.FontWeight = source.FontWeight;
                cloned.FontStyle = source.FontStyle;
                cloned.ForegroundColor = source.ForegroundColor;
                cloned.BackgroundColor = source.BackgroundColor;
                cloned.TextAlignment = source.TextAlignment;
                cloned.VerticalAlignment = source.VerticalAlignment;
                cloned.BorderWidth = source.BorderWidth;
                cloned.BorderColor = source.BorderColor;
                cloned.BorderRadius = source.BorderRadius;
            }

            return cloned;
        }
    }
}

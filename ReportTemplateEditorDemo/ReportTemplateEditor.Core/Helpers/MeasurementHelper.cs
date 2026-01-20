using System;

namespace ReportTemplateEditor.Core.Helpers
{
    public static class MeasurementHelper
    {
        private const double MM_TO_PIXEL_RATIO = 3.7795275591;

        public static double MmToPixel(double mm)
        {
            return mm * MM_TO_PIXEL_RATIO;
        }

        public static double PixelToMm(double pixel)
        {
            return pixel / MM_TO_PIXEL_RATIO;
        }

        public static double CalculateTextWidth(string text, double fontSize, double fontFamilyWidthRatio = 0.6)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return text.Length * fontSize * fontFamilyWidthRatio;
        }

        public static double CalculateTextHeight(string text, double fontSize, double lineHeightRatio = 1.2)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            int lineCount = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            return lineCount * fontSize * lineHeightRatio;
        }

        public static double CalculateCellWidth(double contentWidth, double cellPadding)
        {
            return contentWidth + cellPadding * 2;
        }

        public static double CalculateCellHeight(double contentHeight, double cellPadding)
        {
            return contentHeight + cellPadding * 2;
        }
    }
}

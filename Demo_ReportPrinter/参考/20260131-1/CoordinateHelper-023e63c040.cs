// �ο��ļ� - ��ע�͵�
/*
using System;
using System.Windows;

namespace Demo_ReportPrinter.Helpers
{
    /// <summary>
    /// 坐标转换辅助�?    /// 提供逻辑坐标（毫米）与屏幕坐标（像素）之间的双向转换
    /// </summary>
    public static class CoordinateHelper
    {
        #region 常量

        /// <summary>
        /// 毫米转像素转换系数（96 DPI: 1mm = 3.7795px�?        /// 计算公式: 96 / 25.4 = 3.779527559
        /// </summary>
        public const double MmToPixel96DPI = 96.0 / 25.4; // 3.779527559

        /// <summary>
        /// 像素转毫米转换系数（96 DPI: 1px = 0.264583mm�?        /// 计算公式: 25.4 / 96 = 0.264583333
        /// </summary>
        public const double PixelToMm96DPI = 25.4 / 96.0; // 0.264583333

        /// <summary>
        /// 默认DPI�?        /// </summary>
        public const double DefaultDPI = 96.0;

        #endregion

        #region 基础转换方法

        /// <summary>
        /// 毫米转像素（指定DPI�?        /// </summary>
        /// <param name="mm">毫米�?/param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <returns>像素�?/returns>
        public static double MmToPixel(double mm, double dpi = DefaultDPI)
        {
            return mm * dpi / 25.4;
        }

        /// <summary>
        /// 像素转毫米（指定DPI�?        /// </summary>
        /// <param name="pixel">像素�?/param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <returns>毫米�?/returns>
        public static double PixelToMm(double pixel, double dpi = DefaultDPI)
        {
            return pixel * 25.4 / dpi;
        }

        /// <summary>
        /// 毫米转像素（96 DPI，高性能版本�?        /// </summary>
        /// <param name="mm">毫米�?/param>
        /// <returns>像素�?/returns>
        public static double MmToPixel96(double mm)
        {
            return mm * MmToPixel96DPI;
        }

        /// <summary>
        /// 像素转毫米（96 DPI，高性能版本�?        /// </summary>
        /// <param name="pixel">像素�?/param>
        /// <returns>毫米�?/returns>
        public static double PixelToMm96(double pixel)
        {
            return pixel * PixelToMm96DPI;
        }

        #endregion

        #region Point转换

        /// <summary>
        /// 逻辑坐标（毫米）转换为屏幕坐标（像素�?        /// </summary>
        /// <param name="logicalPoint">逻辑坐标�?/param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <returns>屏幕坐标�?/returns>
        public static Point LogicalToScreen(Point logicalPoint, double dpi = DefaultDPI)
        {
            return new Point(
                MmToPixel(logicalPoint.X, dpi),
                MmToPixel(logicalPoint.Y, dpi)
            );
        }

        /// <summary>
        /// 屏幕坐标（像素）转换为逻辑坐标（毫米）
        /// </summary>
        /// <param name="screenPoint">屏幕坐标�?/param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <returns>逻辑坐标�?/returns>
        public static Point ScreenToLogical(Point screenPoint, double dpi = DefaultDPI)
        {
            return new Point(
                PixelToMm(screenPoint.X, dpi),
                PixelToMm(screenPoint.Y, dpi)
            );
        }

        /// <summary>
        /// 逻辑坐标（毫米）转换为屏幕坐标（像素�?6 DPI，高性能版本�?        /// </summary>
        /// <param name="logicalPoint">逻辑坐标�?/param>
        /// <returns>屏幕坐标�?/returns>
        public static Point LogicalToScreen96(Point logicalPoint)
        {
            return new Point(
                logicalPoint.X * MmToPixel96DPI,
                logicalPoint.Y * MmToPixel96DPI
            );
        }

        /// <summary>
        /// 屏幕坐标（像素）转换为逻辑坐标（毫米，96 DPI，高性能版本�?        /// </summary>
        /// <param name="screenPoint">屏幕坐标�?/param>
        /// <returns>逻辑坐标�?/returns>
        public static Point ScreenToLogical96(Point screenPoint)
        {
            return new Point(
                screenPoint.X * PixelToMm96DPI,
                screenPoint.Y * PixelToMm96DPI
            );
        }

        #endregion

        #region Size转换

        /// <summary>
        /// 逻辑尺寸（毫米）转换为屏幕尺寸（像素�?        /// </summary>
        /// <param name="logicalSize">逻辑尺寸</param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <returns>屏幕尺寸</returns>
        public static Size LogicalToScreen(Size logicalSize, double dpi = DefaultDPI)
        {
            return new Size(
                MmToPixel(logicalSize.Width, dpi),
                MmToPixel(logicalSize.Height, dpi)
            );
        }

        /// <summary>
        /// 屏幕尺寸（像素）转换为逻辑尺寸（毫米）
        /// </summary>
        /// <param name="screenSize">屏幕尺寸</param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <returns>逻辑尺寸</returns>
        public static Size ScreenToLogical(Size screenSize, double dpi = DefaultDPI)
        {
            return new Size(
                PixelToMm(screenSize.Width, dpi),
                PixelToMm(screenSize.Height, dpi)
            );
        }

        /// <summary>
        /// 逻辑尺寸（毫米）转换为屏幕尺寸（像素�?6 DPI，高性能版本�?        /// </summary>
        /// <param name="logicalSize">逻辑尺寸</param>
        /// <returns>屏幕尺寸</returns>
        public static Size LogicalToScreen96(Size logicalSize)
        {
            return new Size(
                logicalSize.Width * MmToPixel96DPI,
                logicalSize.Height * MmToPixel96DPI
            );
        }

        /// <summary>
        /// 屏幕尺寸（像素）转换为逻辑尺寸（毫米，96 DPI，高性能版本�?        /// </summary>
        /// <param name="screenSize">屏幕尺寸</param>
        /// <returns>逻辑尺寸</returns>
        public static Size ScreenToLogical96(Size screenSize)
        {
            return new Size(
                screenSize.Width * PixelToMm96DPI,
                screenSize.Height * PixelToMm96DPI
            );
        }

        #endregion

        #region Rect转换

        /// <summary>
        /// 逻辑矩形（毫米）转换为屏幕矩形（像素�?        /// </summary>
        /// <param name="logicalRect">逻辑矩形</param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <returns>屏幕矩形</returns>
        public static Rect LogicalToScreen(Rect logicalRect, double dpi = DefaultDPI)
        {
            return new Rect(
                MmToPixel(logicalRect.X, dpi),
                MmToPixel(logicalRect.Y, dpi),
                MmToPixel(logicalRect.Width, dpi),
                MmToPixel(logicalRect.Height, dpi)
            );
        }

        /// <summary>
        /// 屏幕矩形（像素）转换为逻辑矩形（毫米）
        /// </summary>
        /// <param name="screenRect">屏幕矩形</param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <returns>逻辑矩形</returns>
        public static Rect ScreenToLogical(Rect screenRect, double dpi = DefaultDPI)
        {
            return new Rect(
                PixelToMm(screenRect.X, dpi),
                PixelToMm(screenRect.Y, dpi),
                PixelToMm(screenRect.Width, dpi),
                PixelToMm(screenRect.Height, dpi)
            );
        }

        #endregion

        #region 缩放转换

        /// <summary>
        /// 应用缩放（逻辑坐标 �?显示坐标�?        /// </summary>
        /// <param name="point">逻辑坐标�?/param>
        /// <param name="scale">缩放比例</param>
        /// <returns>显示坐标�?/returns>
        public static Point ApplyScale(Point point, double scale)
        {
            return new Point(
                point.X * scale,
                point.Y * scale
            );
        }

        /// <summary>
        /// 移除缩放（显示坐�?�?逻辑坐标�?        /// </summary>
        /// <param name="point">显示坐标�?/param>
        /// <param name="scale">缩放比例</param>
        /// <returns>逻辑坐标�?/returns>
        public static Point RemoveScale(Point point, double scale)
        {
            if (scale == 0)
                return point;

            return new Point(
                point.X / scale,
                point.Y / scale
            );
        }

        /// <summary>
        /// 应用缩放（逻辑尺寸 �?显示尺寸�?        /// </summary>
        /// <param name="size">逻辑尺寸</param>
        /// <param name="scale">缩放比例</param>
        /// <returns>显示尺寸</returns>
        public static Size ApplyScale(Size size, double scale)
        {
            return new Size(
                size.Width * scale,
                size.Height * scale
            );
        }

        /// <summary>
        /// 移除缩放（显示尺�?�?逻辑尺寸�?        /// </summary>
        /// <param name="size">显示尺寸</param>
        /// <param name="scale">缩放比例</param>
        /// <returns>逻辑尺寸</returns>
        public static Size RemoveScale(Size size, double scale)
        {
            if (scale == 0)
                return size;

            return new Size(
                size.Width / scale,
                size.Height / scale
            );
        }

        /// <summary>
        /// 完整转换：逻辑坐标（毫米）�?屏幕坐标（像素）�?显示坐标（像素，考虑缩放�?        /// </summary>
        /// <param name="logicalPoint">逻辑坐标点（毫米�?/param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <param name="scale">缩放比例，默�?.0</param>
        /// <returns>显示坐标点（像素�?/returns>
        public static Point LogicalToDisplay(Point logicalPoint, double dpi = DefaultDPI, double scale = 1.0)
        {
            Point screenPoint = LogicalToScreen(logicalPoint, dpi);
            return ApplyScale(screenPoint, scale);
        }

        /// <summary>
        /// 完整转换：显示坐标（像素，考虑缩放）→ 屏幕坐标（像素）�?逻辑坐标（毫米）
        /// </summary>
        /// <param name="displayPoint">显示坐标点（像素�?/param>
        /// <param name="scale">缩放比例，默�?.0</param>
        /// <param name="dpi">DPI值，默认96</param>
        /// <returns>逻辑坐标点（毫米�?/returns>
        public static Point DisplayToLogical(Point displayPoint, double scale = 1.0, double dpi = DefaultDPI)
        {
            Point screenPoint = RemoveScale(displayPoint, scale);
            return ScreenToLogical(screenPoint, dpi);
        }

        #endregion

        #region 网格对齐辅助方法

        /// <summary>
        /// 将值对齐到网格
        /// </summary>
        /// <param name="value">原始�?/param>
        /// <param name="gridSize">网格大小</param>
        /// <returns>对齐后的�?/returns>
        public static double SnapToGrid(double value, double gridSize = 10.0)
        {
            if (gridSize <= 0)
                return value;

            return Math.Round(value / gridSize) * gridSize;
        }

        /// <summary>
        /// 将点对齐到网�?        /// </summary>
        /// <param name="point">原始�?/param>
        /// <param name="gridSize">网格大小</param>
        /// <returns>对齐后的�?/returns>
        public static Point SnapToGrid(Point point, double gridSize = 10.0)
        {
            return new Point(
                SnapToGrid(point.X, gridSize),
                SnapToGrid(point.Y, gridSize)
            );
        }

        /// <summary>
        /// 计算网格对齐的偏移量
        /// </summary>
        /// <param name="value">当前�?/param>
        /// <param name="gridSize">网格大小</param>
        /// <returns>对齐需要的偏移�?/returns>
        public static double CalculateSnapOffset(double value, double gridSize = 10.0)
        {
            if (gridSize <= 0)
                return 0;

            double remainder = value % gridSize;
            return remainder < gridSize / 2 ? -remainder : (gridSize - remainder);
        }

        #endregion

        #region 边界检查辅助方�?
        /// <summary>
        /// 限制值在指定范围�?        /// </summary>
        /// <param name="value">原始�?/param>
        /// <param name="minValue">最小�?/param>
        /// <param name="maxValue">最大�?/param>
        /// <returns>限制后的�?/returns>
        public static double Clamp(double value, double minValue, double maxValue)
        {
            return Math.Max(minValue, Math.Min(maxValue, value));
        }

        /// <summary>
        /// 检查点是否在矩形内
        /// </summary>
        /// <param name="point">要检查的�?/param>
        /// <param name="rect">矩形区域</param>
        /// <returns>是否在矩形内</returns>
        public static bool IsPointInRect(Point point, Rect rect)
        {
            return point.X >= rect.X &&
                   point.Y >= rect.Y &&
                   point.X <= rect.X + rect.Width &&
                   point.Y <= rect.Y + rect.Height;
        }

        /// <summary>
        /// 检查点是否在画布边界内
        /// </summary>
        /// <param name="point">要检查的�?/param>
        /// <param name="canvasWidth">画布宽度</param>
        /// <param name="canvasHeight">画布高度</param>
        /// <param name="padding">内边�?/param>
        /// <returns>是否在边界内</returns>
        public static bool IsPointInCanvas(Point point, double canvasWidth, double canvasHeight, double padding = 5.0)
        {
            return point.X >= padding &&
                   point.Y >= padding &&
                   point.X <= canvasWidth - padding &&
                   point.Y <= canvasHeight - padding;
        }

        /// <summary>
        /// 限制矩形在边界内
        /// </summary>
        /// <param name="rect">原始矩形</param>
        /// <param name="boundsWidth">边界宽度</param>
        /// <param name="boundsHeight">边界高度</param>
        /// <param name="padding">内边�?/param>
        /// <returns>限制后的矩形</returns>
        public static Rect ClampToBounds(Rect rect, double boundsWidth, double boundsHeight, double padding = 5.0)
        {
            double x = Clamp(rect.X, padding, boundsWidth - rect.Width - padding);
            double y = Clamp(rect.Y, padding, boundsHeight - rect.Height - padding);
            return new Rect(x, y, rect.Width, rect.Height);
        }

        #endregion

        #region 距离计算辅助方法

        /// <summary>
        /// 计算两点之间的距�?        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>距离</returns>
        public static double Distance(Point point1, Point point2)
        {
            double dx = point2.X - point1.X;
            double dy = point2.Y - point1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 计算点到线段的距�?        /// </summary>
        /// <param name="point">�?/param>
        /// <param name="lineStart">线段起点</param>
        /// <param name="lineEnd">线段终点</param>
        /// <returns>距离</returns>
        public static double DistanceToLine(Point point, Point lineStart, Point lineEnd)
        {
            double A = point.X - lineStart.X;
            double B = point.Y - lineStart.Y;
            double C = lineEnd.X - lineStart.X;
            double D = lineEnd.Y - lineStart.Y;

            double dot = A * C + B * D;
            double lenSq = C * C + D * D;
            double param = -1.0;

            if (lenSq != 0)
                param = dot / lenSq;

            double xx, yy;

            if (param < 0)
            {
                xx = lineStart.X;
                yy = lineStart.Y;
            }
            else if (param > 1)
            {
                xx = lineEnd.X;
                yy = lineEnd.Y;
            }
            else
            {
                xx = lineStart.X + param * C;
                yy = lineStart.Y + param * D;
            }

            double dx = point.X - xx;
            double dy = point.Y - yy;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion

        #region 格式化输出辅助方�?
        /// <summary>
        /// 格式化毫米�?        /// </summary>
        /// <param name="mm">毫米�?/param>
        /// <param name="decimals">小数位数</param>
        /// <returns>格式化字符串</returns>
        public static string FormatMm(double mm, int decimals = 2)
        {
            return $"{mm.ToString($"F{decimals}")} mm";
        }

        /// <summary>
        /// 格式化像素�?        /// </summary>
        /// <param name="pixel">像素�?/param>
        /// <param name="decimals">小数位数</param>
        /// <returns>格式化字符串</returns>
        public static string FormatPixel(double pixel, int decimals = 0)
        {
            return $"{Math.Round(pixel).ToString($"F{decimals}")} px";
        }

        /// <summary>
        /// 格式化点（毫米）
        /// </summary>
        /// <param name="point">�?/param>
        /// <param name="decimals">小数位数</param>
        /// <returns>格式化字符串</returns>
        public static string FormatPointMm(Point point, int decimals = 2)
        {
            return $"({FormatMm(point.X, decimals)}, {FormatMm(point.Y, decimals)})";
        }

        /// <summary>
        /// 格式化点（像素）
        /// </summary>
        /// <param name="point">�?/param>
        /// <param name="decimals">小数位数</param>
        /// <returns>格式化字符串</returns>
        public static string FormatPointPixel(Point point, int decimals = 0)
        {
            return $"({FormatPixel(point.X, decimals)}, {FormatPixel(point.Y, decimals)})";
        }

        #endregion
    }
}

*/

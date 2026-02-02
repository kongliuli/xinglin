// �ο��ļ� - ��ע�͵�
/*
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Helpers;

namespace Demo_ReportPrinter.Behaviors
{
    /// <summary>
    /// 元素对齐工具 - 提供多种对齐方式
    /// </summary>
    public static class AlignmentTools
    {
        #region 对齐方式枚举

        public enum AlignmentType
        {
            Left,
            CenterHorizontal,
            Right,
            Top,
            CenterVertical,
            Bottom,
            DistributeHorizontal,
            DistributeVertical,
            SameWidth,
            SameHeight,
            SameSize
        }

        #endregion

        #region 水平对齐

        /// <summary>
        /// 左对�?        /// </summary>
        public static void AlignLeft(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 2)
                return;

            double left = double.MaxValue;
            foreach (var element in elements)
            {
                left = Math.Min(left, element.X);
            }

            foreach (var element in elements)
            {
                element.X = left;
            }
        }

        /// <summary>
        /// 水平居中对齐
        /// </summary>
        public static void AlignCenterHorizontal(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 2)
                return;

            // 计算平均中心�?            double centerX = 0;
            foreach (var element in elements)
            {
                centerX += element.X + element.Width / 2;
            }
            centerX /= elements.Count;

            // 将所有元素的中心点对齐到平均中心�?            foreach (var element in elements)
            {
                element.X = centerX - element.Width / 2;
            }
        }

        /// <summary>
        /// 右对�?        /// </summary>
        public static void AlignRight(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 2)
                return;

            double right = double.MinValue;
            foreach (var element in elements)
            {
                right = Math.Max(right, element.X + element.Width);
            }

            foreach (var element in elements)
            {
                element.X = right - element.Width;
            }
        }

        #endregion

        #region 垂直对齐

        /// <summary>
        /// 顶部对齐
        /// </summary>
        public static void AlignTop(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 2)
                return;

            double top = double.MaxValue;
            foreach (var element in elements)
            {
                top = Math.Min(top, element.Y);
            }

            foreach (var element in elements)
            {
                element.Y = top;
            }
        }

        /// <summary>
        /// 垂直居中对齐
        /// </summary>
        public static void AlignCenterVertical(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 2)
                return;

            // 计算平均中心�?            double centerY = 0;
            foreach (var element in elements)
            {
                centerY += element.Y + element.Height / 2;
            }
            centerY /= elements.Count;

            // 将所有元素的中心点对齐到平均中心�?            foreach (var element in elements)
            {
                element.Y = centerY - element.Height / 2;
            }
        }

        /// <summary>
        /// 底部对齐
        /// </summary>
        public static void AlignBottom(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 2)
                return;

            double bottom = double.MinValue;
            foreach (var element in elements)
            {
                bottom = Math.Max(bottom, element.Y + element.Height);
            }

            foreach (var element in elements)
            {
                element.Y = bottom - element.Height;
            }
        }

        #endregion

        #region 分布对齐

        /// <summary>
        /// 水平分布对齐
        /// </summary>
        public static void DistributeHorizontal(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 3)
                return;

            // 按X坐标排序
            var sortedElements = elements.OrderBy(e => e.X).ToList();

            double minX = sortedElements[0].X;
            double maxX = sortedElements[sortedElements.Count - 1].X + sortedElements[sortedElements.Count - 1].Width;
            double totalWidth = maxX - minX;
            double spacing = totalWidth / (sortedElements.Count - 1);

            for (int i = 1; i < sortedElements.Count - 1; i++)
            {
                sortedElements[i].X = minX + i * spacing - sortedElements[i].Width / 2;
            }
        }

        /// <summary>
        /// 垂直分布对齐
        /// </summary>
        public static void DistributeVertical(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 3)
                return;

            // 按Y坐标排序
            var sortedElements = elements.OrderBy(e => e.Y).ToList();

            double minY = sortedElements[0].Y;
            double maxY = sortedElements[sortedElements.Count - 1].Y + sortedElements[sortedElements.Count - 1].Height;
            double totalHeight = maxY - minY;
            double spacing = totalHeight / (sortedElements.Count - 1);

            for (int i = 1; i < sortedElements.Count - 1; i++)
            {
                sortedElements[i].Y = minY + i * spacing - sortedElements[i].Height / 2;
            }
        }

        #endregion

        #region 尺寸对齐

        /// <summary>
        /// 相同宽度
        /// </summary>
        public static void SameWidth(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 2)
                return;

            double width = double.MinValue;
            foreach (var element in elements)
            {
                width = Math.Max(width, element.Width);
            }

            foreach (var element in elements)
            {
                element.Width = width;
            }
        }

        /// <summary>
        /// 相同高度
        /// </summary>
        public static void SameHeight(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 2)
                return;

            double height = double.MinValue;
            foreach (var element in elements)
            {
                height = Math.Max(height, element.Height);
            }

            foreach (var element in elements)
            {
                element.Height = height;
            }
        }

        /// <summary>
        /// 相同尺寸（宽度和高度�?        /// </summary>
        public static void SameSize(List<ControlElement> elements)
        {
            if (elements == null || elements.Count < 2)
                return;

            double maxWidth = double.MinValue;
            double maxHeight = double.MinValue;
            foreach (var element in elements)
            {
                maxWidth = Math.Max(maxWidth, element.Width);
                maxHeight = Math.Max(maxHeight, element.Height);
            }

            foreach (var element in elements)
            {
                element.Width = maxWidth;
                element.Height = maxHeight;
            }
        }

        #endregion

        #region 网格对齐

        /// <summary>
        /// 对齐到网�?        /// </summary>
        public static void SnapToGrid(List<ControlElement> elements, double gridSize = 10)
        {
            if (elements == null)
                return;

            foreach (var element in elements)
            {
                element.X = CoordinateHelper.SnapToGrid(element.X, gridSize);
                element.Y = CoordinateHelper.SnapToGrid(element.Y, gridSize);
                element.Width = CoordinateHelper.SnapToGrid(element.Width, gridSize);
                element.Height = CoordinateHelper.SnapToGrid(element.Height, gridSize);
            }
        }

        #endregion

        #region 画布对齐

        /// <summary>
        /// 居中对齐到画�?        /// </summary>
        public static void CenterToCanvas(List<ControlElement> elements, double canvasWidth, double canvasHeight)
        {
            if (elements == null || elements.Count == 0)
                return;

            double centerX = canvasWidth / 2;
            double centerY = canvasHeight / 2;

            // 计算选中区域的中心点
            double selectionCenterX = 0;
            double selectionCenterY = 0;
            foreach (var element in elements)
            {
                selectionCenterX += element.X + element.Width / 2;
                selectionCenterY += element.Y + element.Height / 2;
            }
            selectionCenterX /= elements.Count;
            selectionCenterY /= elements.Count;

            // 计算偏移�?            double offsetX = centerX - selectionCenterX;
            double offsetY = centerY - selectionCenterY;

            // 应用偏移�?            foreach (var element in elements)
            {
                element.X += offsetX;
                element.Y += offsetY;
            }
        }

        /// <summary>
        /// 水平居中对齐到画�?        /// </summary>
        public static void CenterHorizontalToCanvas(List<ControlElement> elements, double canvasWidth)
        {
            if (elements == null || elements.Count == 0)
                return;

            double centerX = canvasWidth / 2;

            // 计算选中区域的中心点
            double selectionCenterX = 0;
            foreach (var element in elements)
            {
                selectionCenterX += element.X + element.Width / 2;
            }
            selectionCenterX /= elements.Count;

            // 计算偏移�?            double offsetX = centerX - selectionCenterX;

            // 应用偏移�?            foreach (var element in elements)
            {
                element.X += offsetX;
            }
        }

        /// <summary>
        /// 垂直居中对齐到画�?        /// </summary>
        public static void CenterVerticalToCanvas(List<ControlElement> elements, double canvasHeight)
        {
            if (elements == null || elements.Count == 0)
                return;

            double centerY = canvasHeight / 2;

            // 计算选中区域的中心点
            double selectionCenterY = 0;
            foreach (var element in elements)
            {
                selectionCenterY += element.Y + element.Height / 2;
            }
            selectionCenterY /= elements.Count;

            // 计算偏移�?            double offsetY = centerY - selectionCenterY;

            // 应用偏移�?            foreach (var element in elements)
            {
                element.Y += offsetY;
            }
        }

        #endregion

        #region 综合对齐方法

        /// <summary>
        /// 执行对齐操作
        /// </summary>
        public static void Align(List<ControlElement> elements, AlignmentType alignmentType)
        {
            switch (alignmentType)
            {
                case AlignmentType.Left:
                    AlignLeft(elements);
                    break;
                case AlignmentType.CenterHorizontal:
                    AlignCenterHorizontal(elements);
                    break;
                case AlignmentType.Right:
                    AlignRight(elements);
                    break;
                case AlignmentType.Top:
                    AlignTop(elements);
                    break;
                case AlignmentType.CenterVertical:
                    AlignCenterVertical(elements);
                    break;
                case AlignmentType.Bottom:
                    AlignBottom(elements);
                    break;
                case AlignmentType.DistributeHorizontal:
                    DistributeHorizontal(elements);
                    break;
                case AlignmentType.DistributeVertical:
                    DistributeVertical(elements);
                    break;
                case AlignmentType.SameWidth:
                    SameWidth(elements);
                    break;
                case AlignmentType.SameHeight:
                    SameHeight(elements);
                    break;
                case AlignmentType.SameSize:
                    SameSize(elements);
                    break;
            }
        }

        /// <summary>
        /// 对齐到画�?        /// </summary>
        public static void AlignToCanvas(List<ControlElement> elements, AlignmentType alignmentType, double canvasWidth, double canvasHeight)
        {
            switch (alignmentType)
            {
                case AlignmentType.CenterHorizontal:
                    CenterHorizontalToCanvas(elements, canvasWidth);
                    break;
                case AlignmentType.CenterVertical:
                    CenterVerticalToCanvas(elements, canvasHeight);
                    break;
                default:
                    if (alignmentType == AlignmentType.CenterHorizontal || alignmentType == AlignmentType.CenterVertical)
                    {
                        CenterToCanvas(elements, canvasWidth, canvasHeight);
                    }
                    break;
            }
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 获取元素的边界矩�?        /// </summary>
        public static Rect GetBounds(List<ControlElement> elements)
        {
            if (elements == null || elements.Count == 0)
                return Rect.Empty;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var element in elements)
            {
                minX = Math.Min(minX, element.X);
                minY = Math.Min(minY, element.Y);
                maxX = Math.Max(maxX, element.X + element.Width);
                maxY = Math.Max(maxY, element.Y + element.Height);
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// 获取元素的中心点
        /// </summary>
        public static Point GetCenter(List<ControlElement> elements)
        {
            var bounds = GetBounds(elements);
            if (bounds.IsEmpty)
                return new Point(0, 0);

            return new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
        }

        #endregion
    }
}

*/

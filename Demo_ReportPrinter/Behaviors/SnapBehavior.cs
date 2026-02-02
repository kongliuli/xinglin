using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Helpers;

// 解决 Point 类型二义性
using WPoint = System.Windows.Point;

// 使用系统的 Point 类型
using Point = System.Windows.Point;

namespace Demo_ReportPrinter.Behaviors
{
    /// <summary>
    /// 吸附对齐行为 - 提供网格对齐、元素对齐等功能
    /// </summary>
    public class SnapBehavior
    {
        #region 私有字段

        private readonly Canvas _parentCanvas;
        private readonly List<ControlElement> _allElements;
        private readonly FrameworkElement _targetElement;
        private readonly double _snapDistance;
        private readonly double _gridSize;
        private readonly bool _enableSnapToGrid;
        private readonly bool _enableSnapToElements;

        private List<Line> _snapLines;
        private ControlElement _currentElement;

        #endregion

        #region 构造函数

        /// <summary>
        /// 吸附对齐行为
        /// </summary>
        /// <param name="parentCanvas">父级画布</param>
        /// <param name="targetElement">目标元素（正在拖拽/调整大小的元素）</param>
        /// <param name="allElements">所有元素列表</param>
        public SnapBehavior(Canvas parentCanvas, FrameworkElement targetElement, List<ControlElement> allElements)
            : this(parentCanvas, targetElement, allElements,
                   Constants.Constants.DragDrop.SnapDistance,
                   Constants.Constants.DragDrop.GridSize,
                   Constants.Constants.DragDrop.EnableSnapToGrid)
        {
        }

        /// <summary>
        /// 吸附对齐行为（完整参数）
        /// </summary>
        /// <param name="parentCanvas">父级画布</param>
        /// <param name="targetElement">目标元素</param>
        /// <param name="allElements">所有元素列表</param>
        /// <param name="snapDistance">吸附距离</param>
        /// <param name="gridSize">网格大小</param>
        /// <param name="enableSnapToGrid">是否启用网格吸附</param>
        public SnapBehavior(Canvas parentCanvas, FrameworkElement targetElement, List<ControlElement> allElements,
                           double snapDistance, double gridSize, bool enableSnapToGrid)
        {
            _parentCanvas = parentCanvas;
            _targetElement = targetElement;
            _allElements = allElements ?? new List<ControlElement>();
            _snapDistance = snapDistance;
            _gridSize = gridSize;
            _enableSnapToGrid = enableSnapToGrid;
            _enableSnapToElements = true;
            _snapLines = new List<Line>();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 计算吸附位置
        /// </summary>
        /// <param name="currentPosition">当前位置</param>
        /// <param name="elementSize">元素尺寸</param>
        /// <returns>吸附后的位置</returns>
        public Point CalculateSnapPosition(Point currentPosition, Size elementSize)
        {
            Point snappedPosition = currentPosition;

            // 网格吸附
            if (_enableSnapToGrid)
            {
                snappedPosition = SnapToGrid(snappedPosition);
            }

            // 元素吸附
            if (_enableSnapToElements)
            {
                snappedPosition = SnapToElements(snappedPosition, elementSize);
            }

            return snappedPosition;
        }

        /// <summary>
        /// 计算吸附尺寸
        /// </summary>
        /// <param name="currentSize">当前尺寸</param>
        /// <param name="elementPosition">元素位置</param>
        /// <returns>吸附后的尺寸</returns>
        public Size CalculateSnapSize(Size currentSize, Point elementPosition)
        {
            Size snappedSize = currentSize;

            // 网格吸附
            if (_enableSnapToGrid)
            {
                snappedSize = SnapSizeToGrid(snappedSize);
            }

            // 元素吸附（对齐到其他元素的宽度/高度）
            if (_enableSnapToElements)
            {
                snappedSize = SnapSizeToElements(snappedSize, elementPosition);
            }

            return snappedSize;
        }

        /// <summary>
        /// 绘制吸附辅助线
        /// </summary>
        /// <param name="snapPosition">吸附位置</param>
        /// <param name="elementSize">元素尺寸</param>
        public void DrawSnapLines(Point snapPosition, Size elementSize)
        {
            // 清除旧的对齐线
            ClearSnapLines();

            if (!_parentCanvas.Children.Contains(_targetElement))
                return;

            // 绘制垂直对齐线
            DrawVerticalSnapLine(snapPosition.X);
            DrawVerticalSnapLine(snapPosition.X + elementSize.Width);

            // 绘制水平对齐线
            DrawHorizontalSnapLine(snapPosition.Y);
            DrawHorizontalSnapLine(snapPosition.Y + elementSize.Height);
        }

        /// <summary>
        /// 清除吸附辅助线
        /// </summary>
        public void ClearSnapLines()
        {
            if (_parentCanvas != null)
            {
                foreach (var line in _snapLines)
                {
                    _parentCanvas.Children.Remove(line);
                }
                _snapLines.Clear();
            }
        }

        #endregion

        #region 网格吸附

        /// <summary>
        /// 网格吸附（位置）
        /// </summary>
        private Point SnapToGrid(Point position)
        {
            double snappedX = CoordinateHelper.SnapToGrid(position.X, _gridSize);
            double snappedY = CoordinateHelper.SnapToGrid(position.Y, _gridSize);

            // 检查是否在吸附距离内
            if (Math.Abs(snappedX - position.X) > _snapDistance)
                snappedX = position.X;

            if (Math.Abs(snappedY - position.Y) > _snapDistance)
                snappedY = position.Y;

            return new Point(snappedX, snappedY);
        }

        /// <summary>
        /// 网格吸附（尺寸）
        /// </summary>
        private Size SnapSizeToGrid(Size size)
        {
            double snappedWidth = CoordinateHelper.SnapToGrid(size.Width, _gridSize);
            double snappedHeight = CoordinateHelper.SnapToGrid(size.Height, _gridSize);

            // 检查是否在吸附距离内
            if (Math.Abs(snappedWidth - size.Width) > _snapDistance)
                snappedWidth = size.Width;

            if (Math.Abs(snappedHeight - size.Height) > _snapDistance)
                snappedHeight = size.Height;

            return new Size(snappedWidth, snappedHeight);
        }

        #endregion

        #region 元素吸附

        /// <summary>
        /// 元素吸附（对齐到其他元素）
        /// </summary>
        private Point SnapToElements(Point position, Size elementSize)
        {
            Point snappedPosition = position;
            double minSnapDistance = _snapDistance;

            foreach (var otherElement in _allElements)
            {
                if (otherElement == _currentElement)
                    continue;

                // 左边对齐
                double leftSnap = CheckSnap(position.X, otherElement.X, minSnapDistance);
                if (leftSnap >= 0)
                {
                    snappedPosition.X = leftSnap;
                }

                // 右边对齐
                double rightSnap = CheckSnap(position.X + elementSize.Width, otherElement.X + otherElement.Width, minSnapDistance);
                if (rightSnap >= 0)
                {
                    snappedPosition.X = rightSnap - elementSize.Width;
                }

                // 水平居中对齐
                double centerX = position.X + elementSize.Width / 2;
                double otherCenterX = otherElement.X + otherElement.Width / 2;
                double centerSnapX = CheckSnap(centerX, otherCenterX, minSnapDistance);
                if (centerSnapX >= 0)
                {
                    snappedPosition.X = centerSnapX - elementSize.Width / 2;
                }

                // 顶部对齐
                double topSnap = CheckSnap(position.Y, otherElement.Y, minSnapDistance);
                if (topSnap >= 0)
                {
                    snappedPosition.Y = topSnap;
                }

                // 底部对齐
                double bottomSnap = CheckSnap(position.Y + elementSize.Height, otherElement.Y + otherElement.Height, minSnapDistance);
                if (bottomSnap >= 0)
                {
                    snappedPosition.Y = bottomSnap - elementSize.Height;
                }

                // 垂直居中对齐
                double centerY = position.Y + elementSize.Height / 2;
                double otherCenterY = otherElement.Y + otherElement.Height / 2;
                double centerSnapY = CheckSnap(centerY, otherCenterY, minSnapDistance);
                if (centerSnapY >= 0)
                {
                    snappedPosition.Y = centerSnapY - elementSize.Height / 2;
                }
            }

            return snappedPosition;
        }

        /// <summary>
        /// 检查是否可以吸附
        /// </summary>
        /// <param name="value1">值1</param>
        /// <param name="value2">值2</param>
        /// <param name="snapDistance">吸附距离</param>
        /// <returns>吸附后的值，如果不在吸附距离内则返回-1</returns>
        private double CheckSnap(double value1, double value2, double snapDistance)
        {
            double distance = Math.Abs(value1 - value2);
            if (distance <= snapDistance)
            {
                return value2;
            }
            return -1;
        }

        /// <summary>
        /// 元素吸附（尺寸对齐）
        /// </summary>
        private Size SnapSizeToElements(Size size, Point elementPosition)
        {
            Size snappedSize = size;
            double minSnapDistance = _snapDistance;

            foreach (var otherElement in _allElements)
            {
                if (otherElement == _currentElement)
                    continue;

                // 宽度对齐
                double widthSnap = CheckSnap(size.Width, otherElement.Width, minSnapDistance);
                if (widthSnap >= 0)
                {
                    snappedSize.Width = widthSnap;
                }

                // 高度对齐
                double heightSnap = CheckSnap(size.Height, otherElement.Height, minSnapDistance);
                if (heightSnap >= 0)
                {
                    snappedSize.Height = heightSnap;
                }
            }

            return snappedSize;
        }

        #endregion

        #region 辅助线绘制

        /// <summary>
        /// 绘制垂直对齐线
        /// </summary>
        private void DrawVerticalSnapLine(double x)
        {
            if (_parentCanvas == null)
                return;

            var line = new Line
            {
                X1 = x,
                Y1 = 0,
                X2 = x,
                Y2 = _parentCanvas.ActualHeight,
                Stroke = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new double[] { 4, 4 }),
                Opacity = 0.7
            };
            
            // 设置 Z 索引
            Panel.SetZIndex(line, int.MaxValue);

            _snapLines.Add(line);
            _parentCanvas.Children.Add(line);
        }

        /// <summary>
        /// 绘制水平对齐线
        /// </summary>
        private void DrawHorizontalSnapLine(double y)
        {
            if (_parentCanvas == null)
                return;

            var line = new Line
            {
                X1 = 0,
                Y1 = y,
                X2 = _parentCanvas.ActualWidth,
                Y2 = y,
                Stroke = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new double[] { 4, 4 }),
                Opacity = 0.7
            };
            
            // 设置 Z 索引
            Panel.SetZIndex(line, int.MaxValue);

            _snapLines.Add(line);
            _parentCanvas.Children.Add(line);
        }

        #endregion

        #region 公共静态方法

        /// <summary>
        /// 查找最近的吸附点（静态方法）
        /// </summary>
        /// <param name="currentValue">当前值</param>
        /// <param name="snapPoints">吸附点列表</param>
        /// <param name="snapDistance">吸附距离</param>
        /// <returns>最近的吸附点，如果不在吸附距离内则返回当前值</returns>
        public static double FindNearestSnapPoint(double currentValue, List<double> snapPoints, double snapDistance)
        {
            if (snapPoints == null || snapPoints.Count == 0)
                return currentValue;

            double nearestPoint = currentValue;
            double minDistance = snapDistance;

            foreach (var point in snapPoints)
            {
                double distance = Math.Abs(currentValue - point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = point;
                }
            }

            return nearestPoint;
        }

        /// <summary>
        /// 获取所有元素的吸附点
        /// </summary>
        /// <param name="elements">元素列表</param>
        /// <returns>吸附点列表</returns>
        public static List<double> GetAllSnapPoints(List<ControlElement> elements)
        {
            var snapPoints = new List<double>();

            foreach (var element in elements)
            {
                snapPoints.Add(element.X);                    // 左边
                snapPoints.Add(element.X + element.Width);    // 右边
                snapPoints.Add(element.X + element.Width / 2); // 水平中心
                snapPoints.Add(element.Y);                    // 顶部
                snapPoints.Add(element.Y + element.Height);   // 底部
                snapPoints.Add(element.Y + element.Height / 2);// 垂直中心
            }

            return snapPoints;
        }

        #endregion
    }
}
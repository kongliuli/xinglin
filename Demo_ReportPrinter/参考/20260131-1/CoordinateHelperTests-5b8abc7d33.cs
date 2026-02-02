// �ο��ļ� - ��ע�͵�
/*
using System;
using System.Windows;
using Demo_ReportPrinter.Helpers;
using NUnit.Framework;

namespace Demo_ReportPrinter.Tests
{
    /// <summary>
    /// 坐标转换辅助类单元测�?    /// </summary>
    [TestFixture]
    public class CoordinateHelperTests
    {
        #region 常量测试

        [Test]
        public void Test_Constants_MmToPixel96DPI()
        {
            // 验证转换常量
            // 96 DPI: 1mm = 96 / 25.4 = 3.779527559 pixels
            Assert.AreEqual(3.7795, CoordinateHelper.MmToPixel96DPI, 0.0001, "毫米到像素转换常量应�?.7795");
        }

        [Test]
        public void Test_Constants_PixelToMm96DPI()
        {
            // 验证转换常量
            // 96 DPI: 1px = 25.4 / 96 = 0.264583333 mm
            Assert.AreEqual(0.2646, CoordinateHelper.PixelToMm96DPI, 0.0001, "像素到毫米转换常量应�?.2646");
        }

        #endregion

        #region 基础转换测试

        [Test]
        public void Test_MmToPixel_DefaultDPI()
        {
            // 测试毫米转像素（默认96 DPI�?            double mm = 100;
            double pixel = CoordinateHelper.MmToPixel(mm);

            // 100mm × 3.7795 = 377.95px
            Assert.AreEqual(377.95, pixel, 0.01, "100mm应转换为377.95px");
        }

        [Test]
        public void Test_PixelToMm_DefaultDPI()
        {
            // 测试像素转毫米（默认96 DPI�?            double pixel = 377.95;
            double mm = CoordinateHelper.PixelToMm(pixel);

            // 377.95px ÷ 3.7795 = 100mm
            Assert.AreEqual(100, mm, 0.01, "377.95px应转换为100mm");
        }

        [Test]
        public void Test_MmToPixel_CustomDPI()
        {
            // 测试毫米转像素（自定义DPI�?            double mm = 100;
            double pixel = CoordinateHelper.MmToPixel(mm, 300); // 300 DPI

            // 100mm × (300 / 25.4) = 1181.1px
            Assert.AreEqual(1181.1, pixel, 0.1, "100mm @ 300 DPI应转换为1181.1px");
        }

        [Test]
        public void Test_PixelToMm_CustomDPI()
        {
            // 测试像素转毫米（自定义DPI�?            double pixel = 1181.1;
            double mm = CoordinateHelper.PixelToMm(pixel, 300); // 300 DPI

            // 1181.1px ÷ (300 / 25.4) = 100mm
            Assert.AreEqual(100, mm, 0.01, "1181.1px @ 300 DPI应转换为100mm");
        }

        [Test]
        public void Test_MmToPixel96_HighPerformance()
        {
            // 测试高性能版本的毫米转像素�?6 DPI�?            double mm = 210; // A4 宽度
            double pixel = CoordinateHelper.MmToPixel96(mm);

            // 210mm × 3.779527559 = 793.7px
            Assert.AreEqual(793.7, pixel, 0.1, "210mm应转换为793.7px");
        }

        [Test]
        public void Test_PixelToMm96_HighPerformance()
        {
            // 测试高性能版本的像素转毫米�?6 DPI�?            double pixel = 793.7;
            double mm = CoordinateHelper.PixelToMm96(pixel);

            // 793.7px ÷ 3.779527559 = 210mm
            Assert.AreEqual(210, mm, 0.1, "793.7px应转换为210mm");
        }

        #endregion

        #region Point 转换测试

        [Test]
        public void Test_LogicalToScreen_Point()
        {
            // 测试逻辑坐标（毫米）转换为屏幕坐标（像素�?            var logicalPoint = new Point(100, 200);
            var screenPoint = CoordinateHelper.LogicalToScreen(logicalPoint);

            Assert.AreEqual(377.95, screenPoint.X, 0.01, "X坐标转换错误");
            Assert.AreEqual(755.9, screenPoint.Y, 0.01, "Y坐标转换错误");
        }

        [Test]
        public void Test_ScreenToLogical_Point()
        {
            // 测试屏幕坐标（像素）转换为逻辑坐标（毫米）
            var screenPoint = new Point(377.95, 755.9);
            var logicalPoint = CoordinateHelper.ScreenToLogical(screenPoint);

            Assert.AreEqual(100, logicalPoint.X, 0.01, "X坐标转换错误");
            Assert.AreEqual(200, logicalPoint.Y, 0.01, "Y坐标转换错误");
        }

        [Test]
        public void Test_LogicalToScreen96_Point()
        {
            // 测试高性能版本的点转换
            var logicalPoint = new Point(210, 297); // A4 尺寸
            var screenPoint = CoordinateHelper.LogicalToScreen96(logicalPoint);

            Assert.AreEqual(793.7, screenPoint.X, 0.1, "A4宽度像素值错�?);
            Assert.AreEqual(1122.5, screenPoint.Y, 0.1, "A4高度像素值错�?);
        }

        [Test]
        public void Test_ScreenToLogical96_Point()
        {
            // 测试高性能版本的点转换
            var screenPoint = new Point(793.7, 1122.5); // A4 像素尺寸
            var logicalPoint = CoordinateHelper.ScreenToLogical96(screenPoint);

            Assert.AreEqual(210, logicalPoint.X, 0.1, "A4宽度毫米值错�?);
            Assert.AreEqual(297, logicalPoint.Y, 0.1, "A4高度毫米值错�?);
        }

        #endregion

        #region Size 转换测试

        [Test]
        public void Test_LogicalToScreen_Size()
        {
            // 测试逻辑尺寸（毫米）转换为屏幕尺寸（像素�?            var logicalSize = new Size(100, 200);
            var screenSize = CoordinateHelper.LogicalToScreen(logicalSize);

            Assert.AreEqual(377.95, screenSize.Width, 0.01, "宽度转换错误");
            Assert.AreEqual(755.9, screenSize.Height, 0.01, "高度转换错误");
        }

        [Test]
        public void Test_ScreenToLogical_Size()
        {
            // 测试屏幕尺寸（像素）转换为逻辑尺寸（毫米）
            var screenSize = new Size(377.95, 755.9);
            var logicalSize = CoordinateHelper.ScreenToLogical(screenSize);

            Assert.AreEqual(100, logicalSize.Width, 0.01, "宽度转换错误");
            Assert.AreEqual(200, logicalSize.Height, 0.01, "高度转换错误");
        }

        [Test]
        public void Test_LogicalToScreen96_Size()
        {
            // 测试高性能版本的尺寸转�?            var logicalSize = new Size(148, 210); // A5 尺寸
            var screenSize = CoordinateHelper.LogicalToScreen96(logicalSize);

            Assert.AreEqual(559.2, screenSize.Width, 0.1, "A5宽度像素值错�?);
            Assert.AreEqual(793.7, screenSize.Height, 0.1, "A5高度像素值错�?);
        }

        [Test]
        public void Test_ScreenToLogical96_Size()
        {
            // 测试高性能版本的尺寸转�?            var screenSize = new Size(559.2, 793.7); // A5 像素尺寸
            var logicalSize = CoordinateHelper.ScreenToLogical96(screenSize);

            Assert.AreEqual(148, logicalSize.Width, 0.1, "A5宽度毫米值错�?);
            Assert.AreEqual(210, logicalSize.Height, 0.1, "A5高度毫米值错�?);
        }

        #endregion

        #region Rect 转换测试

        [Test]
        public void Test_LogicalToScreen_Rect()
        {
            // 测试逻辑矩形（毫米）转换为屏幕矩形（像素�?            var logicalRect = new Rect(10, 20, 100, 200);
            var screenRect = CoordinateHelper.LogicalToScreen(logicalRect);

            Assert.AreEqual(37.8, screenRect.X, 0.1, "X坐标转换错误");
            Assert.AreEqual(75.6, screenRect.Y, 0.1, "Y坐标转换错误");
            Assert.AreEqual(377.95, screenRect.Width, 0.01, "宽度转换错误");
            Assert.AreEqual(755.9, screenRect.Height, 0.01, "高度转换错误");
        }

        [Test]
        public void Test_ScreenToLogical_Rect()
        {
            // 测试屏幕矩形（像素）转换为逻辑矩形（毫米）
            var screenRect = new Rect(37.8, 75.6, 377.95, 755.9);
            var logicalRect = CoordinateHelper.ScreenToLogical(screenRect);

            Assert.AreEqual(10, logicalRect.X, 0.01, "X坐标转换错误");
            Assert.AreEqual(20, logicalRect.Y, 0.01, "Y坐标转换错误");
            Assert.AreEqual(100, logicalRect.Width, 0.01, "宽度转换错误");
            Assert.AreEqual(200, logicalRect.Height, 0.01, "高度转换错误");
        }

        #endregion

        #region 缩放转换测试

        [Test]
        public void Test_ApplyScale_Point()
        {
            // 测试应用缩放（逻辑坐标 �?显示坐标�?            var point = new Point(100, 200);
            var scaledPoint = CoordinateHelper.ApplyScale(point, 1.5);

            Assert.AreEqual(150, scaledPoint.X, 0.01, "X坐标缩放错误");
            Assert.AreEqual(300, scaledPoint.Y, 0.01, "Y坐标缩放错误");
        }

        [Test]
        public void Test_RemoveScale_Point()
        {
            // 测试移除缩放（显示坐�?�?逻辑坐标�?            var scaledPoint = new Point(150, 300);
            var point = CoordinateHelper.RemoveScale(scaledPoint, 1.5);

            Assert.AreEqual(100, point.X, 0.01, "X坐标缩放移除错误");
            Assert.AreEqual(200, point.Y, 0.01, "Y坐标缩放移除错误");
        }

        [Test]
        public void Test_ApplyScale_Size()
        {
            // 测试应用缩放（逻辑尺寸 �?显示尺寸�?            var size = new Size(100, 200);
            var scaledSize = CoordinateHelper.ApplyScale(size, 2.0);

            Assert.AreEqual(200, scaledSize.Width, 0.01, "宽度缩放错误");
            Assert.AreEqual(400, scaledSize.Height, 0.01, "高度缩放错误");
        }

        [Test]
        public void Test_RemoveScale_Size()
        {
            // 测试移除缩放（显示尺�?�?逻辑尺寸�?            var scaledSize = new Size(200, 400);
            var size = CoordinateHelper.RemoveScale(scaledSize, 2.0);

            Assert.AreEqual(100, size.Width, 0.01, "宽度缩放移除错误");
            Assert.AreEqual(200, size.Height, 0.01, "高度缩放移除错误");
        }

        [Test]
        public void Test_LogicalToDisplay_FullConversion()
        {
            // 测试完整转换：逻辑坐标（毫米）�?屏幕坐标（像素）�?显示坐标（像素，考虑缩放�?            var logicalPoint = new Point(100, 200); // 毫米
            var displayPoint = CoordinateHelper.LogicalToDisplay(logicalPoint, 96, 1.5);

            // 步骤1: 毫米 �?像素: (100, 200) �?(377.95, 755.9)
            // 步骤2: 像素 �?显示坐标（缩�?.5�? (377.95, 755.9) �?(566.9, 1133.8)
            Assert.AreEqual(566.9, displayPoint.X, 0.1, "完整转换X坐标错误");
            Assert.AreEqual(1133.8, displayPoint.Y, 0.1, "完整转换Y坐标错误");
        }

        [Test]
        public void Test_DisplayToLogical_FullConversion()
        {
            // 测试完整转换：显示坐标（像素，考虑缩放）→ 屏幕坐标（像素）�?逻辑坐标（毫米）
            var displayPoint = new Point(566.9, 1133.8); // 显示像素
            var logicalPoint = CoordinateHelper.DisplayToLogical(displayPoint, 1.5, 96);

            // 步骤1: 显示坐标 �?像素: (566.9, 1133.8) �?(377.9, 755.9)
            // 步骤2: 像素 �?毫米: (377.9, 755.9) �?(100, 200)
            Assert.AreEqual(100, logicalPoint.X, 0.1, "完整转换X坐标错误");
            Assert.AreEqual(200, logicalPoint.Y, 0.1, "完整转换Y坐标错误");
        }

        #endregion

        #region 网格对齐测试

        [Test]
        public void Test_SnapToGrid_DefaultGridSize()
        {
            // 测试网格对齐（默认网格大�?0px�?            double value = 13.5;
            double snappedValue = CoordinateHelper.SnapToGrid(value);

            // 13.5 对齐到最近的10的倍数 = 10
            Assert.AreEqual(10, snappedValue, "网格对齐失败");
        }

        [Test]
        public void Test_SnapToGrid_CustomGridSize()
        {
            // 测试网格对齐（自定义网格大小5px�?            double value = 13.5;
            double snappedValue = CoordinateHelper.SnapToGrid(value, 5);

            // 13.5 对齐到最近的5的倍数 = 15
            Assert.AreEqual(15, snappedValue, "网格对齐失败");
        }

        [Test]
        public void Test_SnapToGrid_Point()
        {
            // 测试点对�?            var point = new Point(13.5, 27.8);
            var snappedPoint = CoordinateHelper.SnapToGrid(point, 10);

            Assert.AreEqual(10, snappedPoint.X, "X坐标网格对齐失败");
            Assert.AreEqual(30, snappedPoint.Y, "Y坐标网格对齐失败");
        }

        [Test]
        public void Test_CalculateSnapOffset()
        {
            // 测试计算对齐偏移�?            double value = 13.5;
            double gridSize = 10;
            double offset = CoordinateHelper.CalculateSnapOffset(value, gridSize);

            // 13.5 % 10 = 3.5
            // 3.5 < 5，所以偏移量 = -3.5
            Assert.AreEqual(-3.5, offset, 0.01, "对齐偏移量计算错�?);
        }

        [Test]
        public void Test_CalculateSnapOffset_RoundUp()
        {
            // 测试计算对齐偏移量（向上取整�?            double value = 16.5;
            double gridSize = 10;
            double offset = CoordinateHelper.CalculateSnapOffset(value, gridSize);

            // 16.5 % 10 = 6.5
            // 6.5 >= 5，所以偏移量 = 10 - 6.5 = 3.5
            Assert.AreEqual(3.5, offset, 0.01, "对齐偏移量计算错�?);
        }

        #endregion

        #region 边界检查测�?
        [Test]
        public void Test_Clamp()
        {
            // 测试限制值在范围�?            Assert.AreEqual(50, CoordinateHelper.Clamp(30, 50, 100), "下限限制失败");
            Assert.AreEqual(100, CoordinateHelper.Clamp(150, 50, 100), "上限限制失败");
            Assert.AreEqual(75, CoordinateHelper.Clamp(75, 50, 100), "中间值不应改�?);
        }

        [Test]
        public void Test_IsPointInRect()
        {
            // 测试点是否在矩形�?            var rect = new Rect(10, 10, 100, 100);

            Assert.IsTrue(CoordinateHelper.IsPointInRect(new Point(50, 50), rect), "点应在矩形内");
            Assert.IsTrue(CoordinateHelper.IsPointInRect(new Point(10, 10), rect), "边界点应在矩形内");
            Assert.IsTrue(CoordinateHelper.IsPointInRect(new Point(110, 110), rect), "边界点应在矩形内");
            Assert.IsFalse(CoordinateHelper.IsPointInRect(new Point(5, 50), rect), "点应在矩形外");
            Assert.IsFalse(CoordinateHelper.IsPointInRect(new Point(50, 120), rect), "点应在矩形外");
        }

        [Test]
        public void Test_IsPointInCanvas()
        {
            // 测试点是否在画布边界�?            double canvasWidth = 800;
            double canvasHeight = 600;
            double padding = 10;

            Assert.IsTrue(CoordinateHelper.IsPointInCanvas(new Point(400, 300), canvasWidth, canvasHeight, padding), "点应在画布内");
            Assert.IsFalse(CoordinateHelper.IsPointInCanvas(new Point(5, 300), canvasWidth, canvasHeight, padding), "点应在画布外（左边界�?);
            Assert.IsFalse(CoordinateHelper.IsPointInCanvas(new Point(795, 300), canvasWidth, canvasHeight, padding), "点应在画布外（右边界�?);
        }

        [Test]
        public void Test_ClampToBounds()
        {
            // 测试限制矩形在边界内
            var rect = new Rect(-10, -10, 100, 100);
            double boundsWidth = 800;
            double boundsHeight = 600;
            double padding = 10;

            var clampedRect = CoordinateHelper.ClampToBounds(rect, boundsWidth, boundsHeight, padding);

            Assert.AreEqual(10, clampedRect.X, "X坐标应限制在边界�?);
            Assert.AreEqual(10, clampedRect.Y, "Y坐标应限制在边界�?);
            Assert.AreEqual(100, clampedRect.Width, "宽度不应改变");
            Assert.AreEqual(100, clampedRect.Height, "高度不应改变");
        }

        #endregion

        #region 距离计算测试

        [Test]
        public void Test_Distance()
        {
            // 测试计算两点之间的距�?            var point1 = new Point(0, 0);
            var point2 = new Point(3, 4);
            double distance = CoordinateHelper.Distance(point1, point2);

            // �?3² + 4²) = 5
            Assert.AreEqual(5, distance, 0.01, "距离计算错误");
        }

        [Test]
        public void Test_Distance_SamePoint()
        {
            // 测试同一点的距离
            var point = new Point(10, 10);
            double distance = CoordinateHelper.Distance(point, point);

            Assert.AreEqual(0, distance, "同一点的距离应为0");
        }

        [Test]
        public void Test_DistanceToLine_PointOnLine()
        {
            // 测试点到线段的距离（点在线段上）
            var point = new Point(50, 50);
            var lineStart = new Point(0, 0);
            var lineEnd = new Point(100, 100);
            double distance = CoordinateHelper.DistanceToLine(point, lineStart, lineEnd);

            Assert.AreEqual(0, distance, 0.01, "在线段上的点到线段距离应�?");
        }

        [Test]
        public void Test_DistanceToLine_PointOffLine()
        {
            // 测试点到线段的距离（点在线段外）
            var point = new Point(50, 0);
            var lineStart = new Point(0, 50);
            var lineEnd = new Point(100, 50);
            double distance = CoordinateHelper.DistanceToLine(point, lineStart, lineEnd);

            // 点到直线的垂直距�?= 50
            Assert.AreEqual(50, distance, 0.01, "点到线段距离计算错误");
        }

        #endregion

        #region 格式化输出测�?
        [Test]
        public void Test_FormatMm()
        {
            // 测试格式化毫米�?            string formatted = CoordinateHelper.FormatMm(100.5678, 2);
            Assert.AreEqual("100.57 mm", formatted, "毫米格式化错�?);
        }

        [Test]
        public void Test_FormatPixel()
        {
            // 测试格式化像素�?            string formatted = CoordinateHelper.FormatPixel(793.7, 0);
            Assert.AreEqual("794 px", formatted, "像素格式化错�?);
        }

        [Test]
        public void Test_FormatPointMm()
        {
            // 测试格式化点（毫米）
            var point = new Point(100.5678, 200.3456);
            string formatted = CoordinateHelper.FormatPointMm(point, 2);
            Assert.AreEqual("(100.57 mm, 200.35 mm)", formatted, "点（毫米）格式化错误");
        }

        [Test]
        public void Test_FormatPointPixel()
        {
            // 测试格式化点（像素）
            var point = new Point(793.7, 1122.5);
            string formatted = CoordinateHelper.FormatPointPixel(point, 0);
            Assert.AreEqual("(794 px, 1123 px)", formatted, "点（像素）格式化错误");
        }

        #endregion

        #region 实际应用场景测试

        [Test]
        public void Test_Scenario_A4PaperCoordinateConversion()
        {
            // 场景：A4纸张元素位置转换
            var logicalPosition = new Point(50, 100); // 毫米
            var screenPosition = CoordinateHelper.LogicalToScreen96(logicalPosition);

            Assert.AreEqual(188.98, screenPosition.X, 0.1, "A4纸张X坐标转换错误");
            Assert.AreEqual(377.95, screenPosition.Y, 0.1, "A4纸张Y坐标转换错误");
        }

        [Test]
        public void Test_Scenario_ElementWithScale()
        {
            // 场景：缩放画布上的元素坐标转�?            var logicalPosition = new Point(100, 200); // 毫米
            double scale = 1.5;
            var displayPosition = CoordinateHelper.LogicalToDisplay(logicalPosition, 96, scale);

            Assert.AreEqual(566.9, displayPosition.X, 0.1, "缩放后X坐标错误");
            Assert.AreEqual(1133.8, displayPosition.Y, 0.1, "缩放后Y坐标错误");
        }

        [Test]
        public void Test_Scenario_GridSnapWithCanvasBounds()
        {
            // 场景：网格对�?+ 边界限制
            var elementPosition = new Point(13.5, 27.8);
            double canvasWidth = 800;
            double canvasHeight = 600;
            double padding = 10;

            // 网格对齐
            var snappedPosition = CoordinateHelper.SnapToGrid(elementPosition, 10);
            Assert.AreEqual(10, snappedPosition.X, "X坐标网格对齐失败");
            Assert.AreEqual(30, snappedPosition.Y, "Y坐标网格对齐失败");

            // 边界检�?            bool inBounds = CoordinateHelper.IsPointInCanvas(snappedPosition, canvasWidth, canvasHeight, padding);
            Assert.IsTrue(inBounds, "对齐后的位置应在画布边界�?);
        }

        #endregion
    }
}

*/

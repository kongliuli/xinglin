// �ο��ļ� - ��ע�͵�
/*
using System;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Helpers;
using NUnit.Framework;

namespace Demo_ReportPrinter.Tests
{
    /// <summary>
    /// 纸张规格系统单元测试
    /// </summary>
    [TestFixture]
    public class PaperSizeTests
    {
        #region 纸张规格常量测试

        [Test]
        public void Test_PaperSizeConstants_A4Dimensions()
        {
            // 验证 A4 纸张尺寸常量
            Assert.AreEqual(210, Constants.Constants.PaperSizes.A4Width, 0.01, "A4宽度应为210mm");
            Assert.AreEqual(297, Constants.Constants.PaperSizes.A4Height, 0.01, "A4高度应为297mm");
        }

        [Test]
        public void Test_PaperSizeConstants_A5Dimensions()
        {
            // 验证 A5 纸张尺寸常量
            Assert.AreEqual(148, Constants.Constants.PaperSizes.A5Width, 0.01, "A5宽度应为148mm");
            Assert.AreEqual(210, Constants.Constants.PaperSizes.A5Height, 0.01, "A5高度应为210mm");
        }

        [Test]
        public void Test_PaperSizeConstants_A3Dimensions()
        {
            // 验证 A3 纸张尺寸常量
            Assert.AreEqual(297, Constants.Constants.PaperSizes.A3Width, 0.01, "A3宽度应为297mm");
            Assert.AreEqual(420, Constants.Constants.PaperSizes.A3Height, 0.01, "A3高度应为420mm");
        }

        [Test]
        public void Test_PaperSizeConstants_LetterDimensions()
        {
            // 验证 Letter 纸张尺寸常量
            Assert.AreEqual(216, Constants.Constants.PaperSizes.LetterWidth, 0.01, "Letter宽度应为216mm");
            Assert.AreEqual(279, Constants.Constants.PaperSizes.LetterHeight, 0.01, "Letter高度应为279mm");
        }

        [Test]
        public void Test_PaperSizeConstants_LegalDimensions()
        {
            // 验证 Legal 纸张尺寸常量
            Assert.AreEqual(216, Constants.Constants.PaperSizes.LegalWidth, 0.01, "Legal宽度应为216mm");
            Assert.AreEqual(356, Constants.Constants.PaperSizes.LegalHeight, 0.01, "Legal高度应为356mm");
        }

        #endregion

        #region 纸张规格信息测试

        [Test]
        public void Test_PaperSizeInfo_A4PortraitPixelDimensions()
        {
            // 获取 A4 纸张规格
            var paper = PaperSizeConstants.GetByType(PaperSizeType.A4);

            // 验证纵向像素尺寸
            Assert.AreEqual(210, paper.Width, 0.01, "A4宽度应为210mm");
            Assert.AreEqual(297, paper.Height, 0.01, "A4高度应为297mm");

            // 验证像素转换�?6 DPI: 1mm = 3.7795px�?            Assert.AreEqual(793.7, paper.PortraitPixelWidth, 0.1, "A4纵向宽度像素应为793.7px");
            Assert.AreEqual(1122.5, paper.PortraitPixelHeight, 0.1, "A4纵向高度像素应为1122.5px");
        }

        [Test]
        public void Test_PaperSizeInfo_A4LandscapePixelDimensions()
        {
            // 获取 A4 纸张规格
            var paper = PaperSizeConstants.GetByType(PaperSizeType.A4);

            // 验证横向像素尺寸
            Assert.AreEqual(1122.5, paper.LandscapePixelWidth, 0.1, "A4横向宽度像素应为1122.5px");
            Assert.AreEqual(793.7, paper.LandscapePixelHeight, 0.1, "A4横向高度像素应为793.7px");
        }

        [Test]
        public void Test_PaperSizeInfo_A5PortraitPixelDimensions()
        {
            // 获取 A5 纸张规格
            var paper = PaperSizeConstants.GetByType(PaperSizeType.A5);

            // 验证纵向像素尺寸
            Assert.AreEqual(148, paper.Width, 0.01, "A5宽度应为148mm");
            Assert.AreEqual(210, paper.Height, 0.01, "A5高度应为210mm");

            // 验证像素转换
            Assert.AreEqual(559.2, paper.PortraitPixelWidth, 0.1, "A5纵向宽度像素应为559.2px");
            Assert.AreEqual(793.7, paper.PortraitPixelHeight, 0.1, "A5纵向高度像素应为793.7px");
        }

        [Test]
        public void Test_PaperSizeInfo_A5LandscapePixelDimensions()
        {
            // 获取 A5 纸张规格
            var paper = PaperSizeConstants.GetByType(PaperSizeType.A5);

            // 验证横向像素尺寸
            Assert.AreEqual(793.7, paper.LandscapePixelWidth, 0.1, "A5横向宽度像素应为793.7px");
            Assert.AreEqual(559.2, paper.LandscapePixelHeight, 0.1, "A5横向高度像素应为559.2px");
        }

        #endregion

        #region LayoutMetadata 测试

        [Test]
        public void Test_LayoutMetadata_A4Portrait()
        {
            var layout = new LayoutMetadata();
            layout.SetPaperSize(PaperSizeType.A4);
            layout.IsLandscape = false;

            // 验证逻辑坐标
            Assert.AreEqual(PaperSizeType.A4, layout.PaperType, "纸张类型应为A4");
            Assert.AreEqual(210, layout.ActualWidth, 0.01, "实际宽度应为210mm");
            Assert.AreEqual(297, layout.ActualHeight, 0.01, "实际高度应为297mm");
            Assert.AreEqual(210, layout.PaperWidth, 0.01, "纸张宽度应为210mm");
            Assert.AreEqual(297, layout.PaperHeight, 0.01, "纸张高度应为297mm");
        }

        [Test]
        public void Test_LayoutMetadata_A4Landscape()
        {
            var layout = new LayoutMetadata();
            layout.SetPaperSize(PaperSizeType.A4);
            layout.IsLandscape = true;

            // 验证逻辑坐标
            Assert.AreEqual(PaperSizeType.A4, layout.PaperType, "纸张类型应为A4");
            Assert.AreEqual(297, layout.ActualWidth, 0.01, "实际宽度应为297mm");
            Assert.AreEqual(210, layout.ActualHeight, 0.01, "实际高度应为210mm");
            Assert.AreEqual(210, layout.PaperWidth, 0.01, "纸张宽度应为210mm");
            Assert.AreEqual(297, layout.PaperHeight, 0.01, "纸张高度应为297mm");
        }

        [Test]
        public void Test_LayoutMetadata_A5Portrait()
        {
            var layout = new LayoutMetadata();
            layout.SetPaperSize(PaperSizeType.A5);
            layout.IsLandscape = false;

            // 验证逻辑坐标
            Assert.AreEqual(PaperSizeType.A5, layout.PaperType, "纸张类型应为A5");
            Assert.AreEqual(148, layout.ActualWidth, 0.01, "实际宽度应为148mm");
            Assert.AreEqual(210, layout.ActualHeight, 0.01, "实际高度应为210mm");
            Assert.AreEqual(148, layout.PaperWidth, 0.01, "纸张宽度应为148mm");
            Assert.AreEqual(210, layout.PaperHeight, 0.01, "纸张高度应为210mm");
        }

        [Test]
        public void Test_LayoutMetadata_A5Landscape()
        {
            var layout = new LayoutMetadata();
            layout.SetPaperSize(PaperSizeType.A5);
            layout.IsLandscape = true;

            // 验证逻辑坐标
            Assert.AreEqual(PaperSizeType.A5, layout.PaperType, "纸张类型应为A5");
            Assert.AreEqual(210, layout.ActualWidth, 0.01, "实际宽度应为210mm");
            Assert.AreEqual(148, layout.ActualHeight, 0.01, "实际高度应为148mm");
            Assert.AreEqual(148, layout.PaperWidth, 0.01, "纸张宽度应为148mm");
            Assert.AreEqual(210, layout.PaperHeight, 0.01, "纸张高度应为210mm");
        }

        [Test]
        public void Test_LayoutMetadata_ToggleLandscape()
        {
            var layout = new LayoutMetadata();
            layout.SetPaperSize(PaperSizeType.A4);
            layout.IsLandscape = false;

            // 初始状态：纵向
            Assert.AreEqual(210, layout.ActualWidth, 0.01);
            Assert.AreEqual(297, layout.ActualHeight, 0.01);

            // 切换为横�?            layout.IsLandscape = true;
            Assert.AreEqual(297, layout.ActualWidth, 0.01, "切换横向后宽度应�?97mm");
            Assert.AreEqual(210, layout.ActualHeight, 0.01, "切换横向后高度应�?10mm");

            // 再次切换为纵�?            layout.IsLandscape = false;
            Assert.AreEqual(210, layout.ActualWidth, 0.01, "切换纵向后宽度应�?10mm");
            Assert.AreEqual(297, layout.ActualHeight, 0.01, "切换纵向后高度应�?97mm");
        }

        [Test]
        public void Test_LayoutMetadata_CustomPaperSize()
        {
            var layout = new LayoutMetadata();
            layout.SetCustomPaperSize(250, 300);

            // 验证自定义纸张尺�?            Assert.AreEqual(PaperSizeType.Custom, layout.PaperType, "纸张类型应为Custom");
            Assert.AreEqual(250, layout.PaperWidth, 0.01, "纸张宽度应为250mm");
            Assert.AreEqual(300, layout.PaperHeight, 0.01, "纸张高度应为300mm");
        }

        #endregion

        #region PaperSizeConstants 工具方法测试

        [Test]
        public void Test_PaperSizeConstants_GetByType_A4()
        {
            var paper = PaperSizeConstants.GetByType(PaperSizeType.A4);

            Assert.IsNotNull(paper, "应返回有效的纸张规格");
            Assert.AreEqual(PaperSizeType.A4, paper.Type, "纸张类型应为A4");
            Assert.AreEqual("A4", paper.Name, "纸张名称应为A4");
        }

        [Test]
        public void Test_PaperSizeConstants_GetByType_Unknown()
        {
            // 获取不存在的纸张类型
            var paper = PaperSizeConstants.GetByType(PaperSizeType.Custom);

            Assert.IsNotNull(paper, "应返回默认的自定义纸张规�?);
            Assert.AreEqual(PaperSizeType.Custom, paper.Type);
        }

        [Test]
        public void Test_PaperSizeConstants_GetDisplayWidth_A4Portrait()
        {
            var width = PaperSizeConstants.GetDisplayWidth(PaperSizeType.A4, false);

            // 210mm × 3.7795 = 793.7px
            Assert.AreEqual(793.7, width, 0.1, "A4纵向显示宽度应为793.7px");
        }

        [Test]
        public void Test_PaperSizeConstants_GetDisplayWidth_A4Landscape()
        {
            var width = PaperSizeConstants.GetDisplayWidth(PaperSizeType.A4, true);

            // 297mm × 3.7795 = 1122.5px
            Assert.AreEqual(1122.5, width, 0.1, "A4横向显示宽度应为1122.5px");
        }

        [Test]
        public void Test_PaperSizeConstants_GetDisplayHeight_A5Portrait()
        {
            var height = PaperSizeConstants.GetDisplayHeight(PaperSizeType.A5, false);

            // 210mm × 3.7795 = 793.7px
            Assert.AreEqual(793.7, height, 0.1, "A5纵向显示高度应为793.7px");
        }

        [Test]
        public void Test_PaperSizeConstants_GetDisplayHeight_A5Landscape()
        {
            var height = PaperSizeConstants.GetDisplayHeight(PaperSizeType.A5, true);

            // 148mm × 3.7795 = 559.2px
            Assert.AreEqual(559.2, height, 0.1, "A5横向显示高度应为559.2px");
        }

        #endregion

        #region 纸张尺寸对照表测�?
        [Test]
        public void Test_PaperSizeComparisonTable()
        {
            // A4 纸张
            var a4 = PaperSizeConstants.GetByType(PaperSizeType.A4);
            Assert.AreEqual(793.7, a4.PortraitPixelWidth, 0.1);
            Assert.AreEqual(1122.5, a4.PortraitPixelHeight, 0.1);

            // A5 纸张
            var a5 = PaperSizeConstants.GetByType(PaperSizeType.A5);
            Assert.AreEqual(559.2, a5.PortraitPixelWidth, 0.1);
            Assert.AreEqual(793.7, a5.PortraitPixelHeight, 0.1);

            // A3 纸张
            var a3 = PaperSizeConstants.GetByType(PaperSizeType.A3);
            Assert.AreEqual(1122.5, a3.PortraitPixelWidth, 0.1);
            Assert.AreEqual(1587.4, a3.PortraitPixelHeight, 0.1);

            // Letter 纸张
            var letter = PaperSizeConstants.GetByType(PaperSizeType.Letter);
            Assert.AreEqual(816.3, letter.PortraitPixelWidth, 0.1);
            Assert.AreEqual(1054.5, letter.PortraitPixelHeight, 0.1);

            // Legal 纸张
            var legal = PaperSizeConstants.GetByType(PaperSizeType.Legal);
            Assert.AreEqual(816.3, legal.PortraitPixelWidth, 0.1);
            Assert.AreEqual(1345.5, legal.PortraitPixelHeight, 0.1);
        }

        #endregion
    }
}

*/

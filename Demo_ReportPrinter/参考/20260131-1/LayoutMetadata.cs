// �ο��ļ� - ��ע�͵�
/*
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Demo_ReportPrinter.Models.CoreEntities
{
    /// <summary>
    /// 布局元数�?- 包含所有控件的位置、大小等信息
    /// </summary>
    public partial class LayoutMetadata : ObservableObject
    {
        [ObservableProperty]
        private double _paperWidth = 210; // A4宽度(mm)

        [ObservableProperty]
        private double _paperHeight = 297; // A4高度(mm)

        [ObservableProperty]
        private PaperSizeType _paperType = PaperSizeType.A4;

        /// <summary>
        /// 纸张方向
        /// </summary>
        [ObservableProperty]
        private bool _isLandscape;

        /// <summary>
        /// 固定内容区域 - 不可编辑
        /// </summary>
        public ObservableCollection<ControlElement> FixedElements { get; set; }

        /// <summary>
        /// 可编辑内容区�?- 可拖拽、调整大�?        /// </summary>
        public ObservableCollection<ControlElement> EditableElements { get; set; }

        /// <summary>
        /// 实际显示宽度
        /// </summary>
        public double ActualWidth => IsLandscape ? PaperHeight : PaperWidth;

        /// <summary>
        /// 实际显示高度
        /// </summary>
        public double ActualHeight => IsLandscape ? PaperWidth : PaperHeight;

        public LayoutMetadata()
        {
            FixedElements = new ObservableCollection<ControlElement>();
            EditableElements = new ObservableCollection<ControlElement>();
        }

        /// <summary>
        /// 设置纸张大小
        /// </summary>
        /// <param name="paperType">纸张类型</param>
        public void SetPaperSize(PaperSizeType paperType)
        {
            PaperType = paperType;
            var paperInfo = PaperSizeConstants.GetByType(paperType);
            PaperWidth = paperInfo.Width;
            PaperHeight = paperInfo.Height;
        }

        /// <summary>
        /// 设置自定义纸张大�?        /// </summary>
        /// <param name="width">宽度(mm)</param>
        /// <param name="height">高度(mm)</param>
        public void SetCustomPaperSize(double width, double height)
        {
            PaperType = PaperSizeType.Custom;
            PaperWidth = width;
            PaperHeight = height;
        }
    }
}
*/

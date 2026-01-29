using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xinglin.ReportTemplateEditor.WPF.Services;

namespace Xinglin.ReportTemplateEditor.WPF.Views
{
    /// <summary>
    /// EditPanelView.xaml 的交互逻辑
    /// </summary>
    public partial class EditPanelView : UserControl
    {
        private ZoomManager _zoomManager;
        private GridHelper _gridHelper;

        /// <summary>
        /// 缩放滑块控件
        /// </summary>
        private Slider ZoomSlider => zoomSlider;

        /// <summary>
        /// 缩放文本控件
        /// </summary>
        private TextBlock ZoomText => zoomText;

        public EditPanelView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 初始化ZoomManager
            _zoomManager = new ZoomManager(DesignCanvas, ZoomSlider, ZoomText, CanvasScrollViewer);
            
            // 初始化GridHelper
            _gridHelper = new GridHelper(DesignCanvas);
            _gridHelper.DrawGrid();
            
            // 订阅缩放变化事件
            _zoomManager.ZoomChanged += OnZoomChanged;
        }
        
        /// <summary>
        /// 控件拖拽开始事件处理
        /// </summary>
        private void ListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is StackPanel stackPanel && stackPanel.Tag is string widgetType)
            {
                // 创建拖拽数据
                var dragData = new DataObject("WidgetType", widgetType);
                
                // 开始拖拽
                DragDrop.DoDragDrop(stackPanel, dragData, DragDropEffects.Copy);
            }
        }
        
        /// <summary>
        /// 画布拖拽放置事件处理
        /// </summary>
        private void DesignCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WidgetType"))
            {
                string widgetType = e.Data.GetData("WidgetType") as string;
                if (!string.IsNullOrEmpty(widgetType))
                {
                    // 获取鼠标在画布上的位置
                    Point position = e.GetPosition(DesignCanvas);
                    
                    // 应用网格吸附
                    Point snappedPosition = _gridHelper.SnapToGrid(position);
                    
                    // 调用ViewModel的方法添加控件
                    var viewModel = DataContext as dynamic;
                    if (viewModel != null && viewModel.AddWidgetCommand != null)
                    {
                        viewModel.AddWidgetCommand.Execute(new Tuple<string, double, double>(widgetType, snappedPosition.X, snappedPosition.Y));
                    }
                }
            }
        }
        
        /// <summary>
        /// 缩放变化事件处理
        /// </summary>
        private void OnZoomChanged(double zoomLevel)
        {
            // 计算并更新网格大小
            _gridHelper.CalculateAndSetGridSize(DesignCanvas.Width, DesignCanvas.Height, zoomLevel);
        }
        
        /// <summary>
        /// 鼠标按下事件处理
        /// </summary>
        private void DesignCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 聚焦画布
            DesignCanvas.Focus();
        }

        /// <summary>
        /// 画布拖拽经过事件处理
        /// </summary>
        private void DesignCanvas_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("WidgetType"))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        /// <summary>
        /// 画布鼠标左键按下事件处理
        /// </summary>
        private void DesignCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 处理鼠标左键按下事件
        }

        /// <summary>
        /// 画布鼠标右键按下事件处理
        /// </summary>
        private void DesignCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 处理鼠标右键按下事件
        }

        /// <summary>
        /// 画布鼠标右键释放事件处理
        /// </summary>
        private void DesignCanvas_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 处理鼠标右键释放事件
        }

        /// <summary>
        /// 画布鼠标移动事件处理
        /// </summary>
        private void DesignCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // 处理鼠标移动事件
        }

        /// <summary>
        /// 画布鼠标左键释放事件处理
        /// </summary>
        private void DesignCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 处理鼠标左键释放事件
        }

        /// <summary>
        /// 画布鼠标滚轮事件处理
        /// </summary>
        private void DesignCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 处理鼠标滚轮事件
        }
    }
}
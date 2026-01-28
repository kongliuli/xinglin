using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xinglin.ReportTemplateEditor.WPF.Views
{
    /// <summary>
    /// EditPanelView.xaml 的交互逻辑
    /// </summary>
    public partial class EditPanelView : UserControl
    {
        public EditPanelView()
        {
            InitializeComponent();
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
    }
}
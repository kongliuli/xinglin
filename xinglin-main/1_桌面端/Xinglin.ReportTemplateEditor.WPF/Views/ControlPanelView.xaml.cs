using System.Windows.Controls;

namespace Xinglin.ReportTemplateEditor.WPF.Views
{
    /// <summary>
    /// ControlPanelView.xaml 的交互逻辑
    /// </summary>
    public partial class ControlPanelView : UserControl
    {
        public ControlPanelView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 控件容器
        /// </summary>
        public StackPanel ControlContainer => (StackPanel)FindName("ControlsContainer");
    }
}
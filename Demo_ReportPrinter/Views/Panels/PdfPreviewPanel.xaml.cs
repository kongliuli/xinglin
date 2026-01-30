using System.Windows.Controls;
using Demo_ReportPrinter.ViewModels;

namespace Demo_ReportPrinter.Views
{
    /// <summary>
    /// PdfPreviewPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PdfPreviewPanel : UserControl
    {
        public PdfPreviewPanel()
        {
            InitializeComponent();
        }

        private void PdfWebView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // 将WebView2控件传递给ViewModel
            if (DataContext is PdfPreviewViewModel viewModel)
            {
                viewModel.SetWebView(PdfWebView);
            }
        }
    }
}
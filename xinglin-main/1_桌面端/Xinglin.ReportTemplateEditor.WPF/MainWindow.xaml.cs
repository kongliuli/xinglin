using System.Windows;
using Xinglin.ReportTemplateEditor.WPF.ViewModels;

namespace Xinglin.ReportTemplateEditor.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
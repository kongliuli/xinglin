using System.Windows;
using Xinglin.ReportTemplateEditor.WPF.ViewModels;
using Xinglin.ReportTemplateEditor.WPF.Services;

namespace Xinglin.ReportTemplateEditor.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private KeyboardShortcutService _keyboardShortcutService;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        
        // 初始化快捷键服务
        _keyboardShortcutService = new KeyboardShortcutService(this);
        
        // 注册默认快捷键
        if (DataContext is MainViewModel mainViewModel)
        {
            _keyboardShortcutService.RegisterDefaultShortcuts(mainViewModel);
        }
    }
    
    /// <summary>
    /// 获取快捷键服务
    /// </summary>
    public KeyboardShortcutService KeyboardShortcutService => _keyboardShortcutService;
}
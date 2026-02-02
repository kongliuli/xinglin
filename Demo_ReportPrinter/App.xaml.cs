using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Demo_ReportPrinter.Services.Shared;
using Demo_ReportPrinter.Services.Data;
using Demo_ReportPrinter.Services.Pdf;
using Demo_ReportPrinter.Services.Validation;
using Demo_ReportPrinter.ViewModels;

namespace Demo_ReportPrinter;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        
        // 注册共享服务
        services.AddSingleton<ISharedDataService, SharedDataService>();
        services.AddSingleton<IMessageService, MessageService>();
        
        // 注册数据服务
        services.AddSingleton<ITemplateService, TemplateService>();
        
        // 注册PDF服务
        services.AddSingleton<IPdfService, PdfService>();
        
        // 注册验证服务
        services.AddSingleton<IValidationService, ValidationService>();
        
        // 注册ViewModel
        services.AddTransient<MainViewModel>();
        services.AddTransient<TemplateEditorViewModel>();
        services.AddTransient<DataEntryViewModel>();
        services.AddTransient<PdfPreviewViewModel>();
        services.AddTransient<TemplateTreeViewModel>();
        services.AddTransient<DynamicDataEntryViewModel>();
        
        // 注册MainWindow
        services.AddTransient<MainWindow>();
        
        _serviceProvider = services.BuildServiceProvider();
        
        // 创建并显示主窗口
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
        
        base.OnStartup(e);
    }
}


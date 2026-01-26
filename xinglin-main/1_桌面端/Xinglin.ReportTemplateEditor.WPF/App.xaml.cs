using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Autofac;
using Xinglin.Infrastructure.DependencyInjection;
using Xinglin.Infrastructure.Services;
using Xinglin.Security;

namespace Xinglin.ReportTemplateEditor.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IPermissionStatusService _permissionStatusService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // 检查命令行参数，获取运行模式
            string runMode = GetRunModeFromArguments(e.Args);
            
            // 加载配置
            var configPath = GetConfigPath(runMode);
            
            // 构建依赖注入容器
            var container = DIContainerBuilder.Build(configPath);
            
            // 初始化授权状态管理
            _permissionStatusService = container.Resolve<IPermissionStatusService>();
            
            // 检查授权状态
            CheckAuthorizationStatus();
            
            // 继续正常启动
            // MainWindow.xaml会在StartupUri中指定，自动创建
        }
        catch (Exception ex)
        {
            MessageBox.Show($"应用启动失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    /// <summary>
    /// 从命令行参数获取运行模式
    /// </summary>
    /// <param name="args">命令行参数</param>
    /// <returns>运行模式</returns>
    private string GetRunModeFromArguments(string[] args)
    {
        // 默认运行模式
        const string defaultMode = "SingleClient";
        
        if (args == null || args.Length == 0)
        {
            return defaultMode;
        }
        
        // 查找运行模式参数
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("--mode", StringComparison.OrdinalIgnoreCase) || args[i].Equals("-m", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 < args.Length)
                {
                    return args[i + 1];
                }
                break;
            }
        }
        
        return defaultMode;
    }

    /// <summary>
    /// 获取配置文件路径
    /// </summary>
    /// <param name="runMode">运行模式</param>
    /// <returns>配置文件路径</returns>
    private string GetConfigPath(string runMode)
    {
        // 配置文件命名规则：config_{mode}.json
        string configFileName = $"config_{runMode}.json";
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
        
        // 如果配置文件不存在，使用默认配置文件
        if (!File.Exists(configPath))
        {
            configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        }
        
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"配置文件不存在: {configPath}");
        }
        
        return configPath;
    }

    /// <summary>
    /// 检查授权状态
    /// </summary>
    private void CheckAuthorizationStatus()
    {
        var status = _permissionStatusService.GetCurrentStatus();
        
        switch (status)
        {
            case AuthorizationStatus.Normal:
                // 正常状态，继续运行
                break;
            case AuthorizationStatus.LicensePending:
                // 许可证待激活，显示提示
                MessageBox.Show("许可证待激活，请联系管理员获取完整权限", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                break;
            case AuthorizationStatus.Trial:
                // 试用模式，显示剩余天数
                MessageBox.Show("当前处于试用模式，试用期为3天", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                break;
            case AuthorizationStatus.Locked:
                // 已锁定，禁止运行
                MessageBox.Show("系统已锁定，请联系管理员获取授权", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                break;
        }
    }
}
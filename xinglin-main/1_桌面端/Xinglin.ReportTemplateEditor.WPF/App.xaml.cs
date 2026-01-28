using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace Xinglin.ReportTemplateEditor.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 添加全局异常处理
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        DispatcherUnhandledException += App_DispatcherUnhandledException;

        try
        {
            // 检查授权状态
            CheckAuthorizationStatus();
            
            // 直接创建和显示MainWindow
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
        catch (XamlParseException ex)
        {
            // 显示XAML解析错误信息
            MessageBox.Show($"XAML解析失败: {ex.Message}\n{ex.StackTrace}\n{ex.BaseUri}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
        catch (Exception ex)
        {
            // 显示错误信息
            MessageBox.Show($"应用启动失败: {ex.Message}\n{ex.StackTrace}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        // 显示UI线程异常信息
        //MessageBox.Show($"UI线程异常: {e.Exception.Message}\n{e.Exception.StackTrace}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
        Shutdown();
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // 显示非UI线程异常信息
        Exception ex = e.ExceptionObject as Exception;
        MessageBox.Show($"非UI线程异常: {ex?.Message}\n{ex?.StackTrace}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        Shutdown();
    }

    /// <summary>
    /// 检查授权状态
    /// </summary>
    private void CheckAuthorizationStatus()
    {
        try
        {
            // 简单的授权状态检查，显示试用天数
            int remainingDays = GetRemainingTrialDays();
            MessageBox.Show($"当前处于试用模式，剩余试用期为{remainingDays}天", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"授权状态检查失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            // 即使授权检查失败，也继续启动应用程序
        }
    }

    /// <summary>
    /// 获取剩余试用天数
    /// </summary>
    /// <returns>剩余试用天数</returns>
    private int GetRemainingTrialDays()
    {
        const int TrialDays = 3;
        string trialInfoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trialinfo.dat");
        
        if (System.IO.File.Exists(trialInfoPath))
        {
            try
            {
                var trialContent = System.IO.File.ReadAllText(trialInfoPath);
                var trialInfo = System.Text.Json.JsonSerializer.Deserialize<TrialInfo>(trialContent);
                
                if (trialInfo != null)
                {
                    double daysPassed = (DateTime.Now - trialInfo.FirstLaunchTime).TotalDays;
                    int remainingDays = Math.Max(0, TrialDays - (int)Math.Ceiling(daysPassed));
                    return remainingDays;
                }
            }
            catch (Exception)
            {
                // 读取试用信息失败，返回默认值
            }
        }
        else
        {
            // 首次启动，创建试用信息
            var trialInfo = new TrialInfo { FirstLaunchTime = DateTime.Now };
            var trialContent = System.Text.Json.JsonSerializer.Serialize(trialInfo);
            System.IO.File.WriteAllText(trialInfoPath, trialContent);
        }
        
        return TrialDays;
    }

    /// <summary>
    /// 试用信息类
    /// </summary>
    private class TrialInfo
    {
        public DateTime FirstLaunchTime { get; set; }
        public int LaunchCount { get; set; } = 1;
    }
}
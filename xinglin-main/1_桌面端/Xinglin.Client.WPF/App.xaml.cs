using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Autofac;
using Xinglin.Client.Core.Integration;
using Xinglin.Core.Communication;
using Xinglin.Infrastructure.DependencyInjection;

namespace Xinglin.Client.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // 配置文件路径
            string configPath = "appsettings.json";
            
            // 如果配置文件不存在，复制示例配置文件
            if (!File.Exists(configPath))
            {
                var exampleConfigPath = "appsettings.example.json";
                if (File.Exists(exampleConfigPath))
                {
                    File.Copy(exampleConfigPath, configPath);
                }
                else
                {
                    throw new FileNotFoundException($"配置文件 {configPath} 和示例配置文件 {exampleConfigPath} 都不存在");
                }
            }
            
            try
            {
                // 构建依赖注入容器
                var container = DIContainerBuilder.Build(configPath);
                
                // 配置通信
                ConfigureCommunication(container);
                
                // 检查授权状态
                CheckAuthorizationStatus(container);
                
                // 初始化模块
                InitializeModules(container);
                
                // 获取并显示主窗口
                var mainWindow = container.Resolve<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"应用程序启动失败: {ex.Message}", "启动错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
        
        private void CheckAuthorizationStatus(IContainer container)
        {
            // 获取授权相关服务
            var machineCodeService = container.Resolve<Xinglin.Security.IMachineCodeService>();
            var permissionStatusService = container.Resolve<Xinglin.Security.IPermissionStatusService>();
            var verificationService = container.Resolve<Xinglin.Security.IVerificationService>();
            
            // 生成并保存机器码
            var machineCode = machineCodeService.GenerateMachineCode();
            
            // 检查授权状态
            var authStatus = permissionStatusService.GetCurrentStatus();
            
            switch (authStatus)
            {
                case Xinglin.Security.AuthorizationStatus.Normal:
                    // 正常授权状态，继续启动
                    break;
                case Xinglin.Security.AuthorizationStatus.LicensePending:
                    // 许可证待激活，提示用户
                    MessageBox.Show("许可证待激活，请联系管理员获取授权", "授权提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case Xinglin.Security.AuthorizationStatus.Trial:
                    // 试用模式，提示用户
                    var remainingDays = CalculateRemainingTrialDays();
                    MessageBox.Show($"当前处于试用模式，剩余 {remainingDays} 天", "授权提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case Xinglin.Security.AuthorizationStatus.Locked:
                    // 已锁定，禁止启动
                    MessageBox.Show("应用程序已锁定，请联系管理员获取授权", "授权错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    break;
                default:
                    // 其他状态，禁止启动
                    MessageBox.Show("授权状态异常，请联系管理员", "授权错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    break;
            }
        }
        
        private int CalculateRemainingTrialDays()
        {
            const string trialInfoFileName = "trialinfo.dat";
            const int TrialDays = 3;
            
            if (File.Exists(trialInfoFileName))
            {
                try
                {
                    var trialContent = File.ReadAllText(trialInfoFileName);
                    var trialInfo = System.Text.Json.JsonSerializer.Deserialize<TrialInfo>(trialContent);
                    if (trialInfo != null)
                    {
                        var trialDaysPassed = (DateTime.Now - trialInfo.FirstLaunchTime).TotalDays;
                        var remainingDays = (int)(TrialDays - trialDaysPassed);
                        return Math.Max(0, remainingDays);
                    }
                    return TrialDays;
                }
                catch (Exception)
                {
                    return TrialDays;
                }
            }
            else
            {
                return TrialDays;
            }
        }
        
        private class TrialInfo
        {
            public DateTime FirstLaunchTime { get; set; }
        }
    
    private void ConfigureCommunication(IContainer container)
    {
        // 获取通信代理并初始化
        var communicationProxy = container.Resolve<ICommunicationProxy>();
        var config = container.Resolve<SystemConfig>();
        
        // 根据运行模式配置通信代理
        switch (config.RunningMode)
        {
            case "SingleClient":
                // 单例客户端模式，无需实际通信
                break;
            case "ClientInternalServer":
                // 客户端-内网服务端组合模式，配置内网服务通信
                communicationProxy.InitializeAsync(new Xinglin.Core.Communication.CommunicationConfig
                {
                    BaseUrl = config.InternalServerConfig.BaseUrl,
                    ApiKey = config.InternalServerConfig.ApiKey,
                    UseHttps = false
                }).Wait();
                break;
            case "FullSystem":
                // 完整系统模式，配置内网服务通信
                communicationProxy.InitializeAsync(new Xinglin.Core.Communication.CommunicationConfig
                {
                    BaseUrl = config.InternalServerConfig.BaseUrl,
                    ApiKey = config.InternalServerConfig.ApiKey,
                    UseHttps = false
                }).Wait();
                
                // 配置服务端通信网关
                var serverGateway = container.Resolve<IServerGateway>();
                serverGateway.InitializeAsync(new Xinglin.Core.Communication.ServerGatewayConfig
                {
                    InternalServer = new Xinglin.Core.Communication.InternalServerConfig
                    {
                        BaseUrl = config.InternalServerConfig.BaseUrl,
                        ApiKey = config.InternalServerConfig.ApiKey
                    },
                    ExternalServer = new Xinglin.Core.Communication.ExternalServerConfig
                    {
                        BaseUrl = config.ExternalServerConfig.BaseUrl,
                        ApiKey = config.ExternalServerConfig.ApiKey
                    },
                    MasterServer = new Xinglin.Core.Communication.MasterServerConfig
                    {
                        BaseUrl = config.MasterServerConfig.BaseUrl,
                        ApiKey = config.MasterServerConfig.ApiKey
                    }
                }).Wait();
                break;
        }
    }
    
    private void InitializeModules(IContainer container)
    {
        // 获取并初始化所有模块
        var modules = container.Resolve<IEnumerable<IModuleIntegration>>();
        foreach (var module in modules)
        {
            module.InitializeModule();
        }
    }
}


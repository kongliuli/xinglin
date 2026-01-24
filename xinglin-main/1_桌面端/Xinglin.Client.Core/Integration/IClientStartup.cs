using System.Windows;
using Autofac;

namespace Xinglin.Client.Core.Integration;

// 客户端启动接口
public interface IClientStartup
{
    void ConfigureServices(ContainerBuilder builder);
    void ConfigureApplication(Application application);
    void ConfigureCommunication(IContainer container);
}

// 模块集成接口
public interface IModuleIntegration
{
    string ModuleName { get; }
    void RegisterServices(ContainerBuilder builder);
    void InitializeModule();
    void ShutdownModule();
}

// 客户端集成配置类
public class ClientIntegrationConfig
{
    public bool EnableLogging { get; set; } = true;
    public bool EnableAutoUpdates { get; set; } = false;
    public string LogLevel { get; set; } = "Information";
    public string ConfigPath { get; set; } = "appsettings.json";
}

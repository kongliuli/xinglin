using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using Xinglin.Core.Communication;
using Xinglin.Core.Logging;
using Xinglin.Infrastructure.Logging;
using Xinglin.Infrastructure.Communication;
using Xinglin.Infrastructure.Services;
using Xinglin.Security;

namespace Xinglin.Infrastructure.DependencyInjection;

public class SystemConfig
{
    public string RunningMode { get; set; } = "SingleClient";
    public ClientConfig ClientConfig { get; set; } = new ClientConfig();
    public InternalServerConfig InternalServerConfig { get; set; } = new InternalServerConfig();
    public ExternalServerConfig ExternalServerConfig { get; set; } = new ExternalServerConfig();
    public MasterServerConfig MasterServerConfig { get; set; } = new MasterServerConfig();
    public List<ModuleConfig> Modules { get; set; } = new List<ModuleConfig>();
    public Dictionary<string, string> Dependencies { get; set; } = new Dictionary<string, string>();
}

public class ClientConfig
{
    public string AppName { get; set; } = "杏林病理检验报告系统";
    public string Version { get; set; } = "1.0.0";
    public string MachineCode { get; set; } = string.Empty;
}

public class InternalServerConfig
{
    public string BaseUrl { get; set; } = "http://localhost:5000";
    public string ApiKey { get; set; } = string.Empty;
}

public class ExternalServerConfig
{
    public string BaseUrl { get; set; } = "https://external.xinglin.com";
    public string ApiKey { get; set; } = string.Empty;
}

public class MasterServerConfig
{
    public string BaseUrl { get; set; } = "https://master.xinglin.com";
    public string ApiKey { get; set; } = string.Empty;
}

public class ModuleConfig
{
    public string Name { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public List<string> Dependencies { get; set; } = new List<string>();
}

public static class DIContainerBuilder
{
    public static IContainer Build(string configPath)
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"配置文件 {configPath} 不存在");
        }

        var configJson = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<SystemConfig>(configJson);
        
        if (config == null)
        {
            throw new InvalidOperationException("配置文件格式无效");
        }

        return Build(config, configPath);
    }

    public static IContainer Build(SystemConfig config)
    {
        return Build(config, null);
    }

    public static IContainer Build(SystemConfig config, string configFilePath)
    {
        var builder = new ContainerBuilder();
        
        // 注册核心服务
        builder.RegisterInstance(config).As<SystemConfig>().SingleInstance();
        builder.Register(c => new ConfigurationManager(config, configFilePath)).As<IConfigurationManager>().SingleInstance();
        
        // 注册日志服务
        builder.RegisterType<Logging.LogConfig>().AsSelf().SingleInstance();
        builder.RegisterType<Logging.Logger>().As<Core.Logging.ILogger>().SingleInstance();
        
        // 注册异常处理服务
        builder.RegisterType<Exceptions.ExceptionHandler>().AsSelf().SingleInstance();
        
        // 注册授权验证相关服务
        builder.RegisterType<Services.MachineCodeService>().As<Security.IMachineCodeService>().SingleInstance();
        builder.RegisterType<Services.LicenseService>().As<Security.ILicenseService>().SingleInstance();
        builder.RegisterType<Services.PermissionStatusService>().As<Security.IPermissionStatusService>().SingleInstance();
        
        // 注册模板相关服务
        builder.RegisterType<Services.TemplateService>().As<Core.Services.ITemplateService>().SingleInstance();
        builder.RegisterType<Services.TemplateSerializationService>().As<Core.Services.ITemplateSerializer>().SingleInstance();
        
        // 根据运行模式注册组件
        RegisterComponentsByRunningMode(builder, config);
        
        // 注册授权验证服务（依赖于通信代理，需在通信代理注册后注册）
        builder.RegisterType<Services.VerificationService>().As<Security.IVerificationService>().SingleInstance();
        
        // 注册模块
        RegisterModules(builder, config);
        
        return builder.Build();
    }

    private static void RegisterComponentsByRunningMode(ContainerBuilder builder, SystemConfig config)
    {
        switch (config.RunningMode)
        {
            case "SingleClient":
                RegisterSingleClientComponents(builder);
                break;
            case "ClientInternalServer":
                RegisterClientInternalServerComponents(builder);
                break;
            case "FullSystem":
                RegisterFullSystemComponents(builder);
                break;
            case "MasterServerOnly":
                RegisterMasterServerComponents(builder);
                break;
            default:
                throw new NotSupportedException($"不支持的运行模式: {config.RunningMode}");
        }
    }

    private static void RegisterSingleClientComponents(ContainerBuilder builder)
    {
        // 注册单例客户端模式组件
        builder.RegisterType<MockCommunicationProxy>().As<ICommunicationProxy>().SingleInstance();
    }

    private static void RegisterClientInternalServerComponents(ContainerBuilder builder)
    {
        // 注册客户端-内网服务端组合模式组件
        builder.RegisterType<HttpCommunicationProxy>().As<ICommunicationProxy>().SingleInstance();
    }

    private static void RegisterFullSystemComponents(ContainerBuilder builder)
    {
        // 注册完整系统模式组件
        builder.RegisterType<HttpCommunicationProxy>().As<ICommunicationProxy>().SingleInstance();
        builder.RegisterType<ServerGateway>().As<IServerGateway>().SingleInstance();
    }

    private static void RegisterMasterServerComponents(ContainerBuilder builder)
    {
        // 注册主控服务端模式组件
        builder.RegisterType<ServerGateway>().As<IServerGateway>().SingleInstance();
    }

    private static void RegisterModules(ContainerBuilder builder, SystemConfig config)
    {
        foreach (var moduleConfig in config.Modules.Where(m => m.Enabled))
        {
            try
            {
                // 动态加载模块程序集
                var assemblyName = $"Xinglin.{moduleConfig.Name}";
                var assembly = Assembly.Load(assemblyName);
                
                // 注册模块中的服务
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"加载模块 {moduleConfig.Name} 失败: {ex.Message}", ex);
            }
        }
    }
}

public interface IConfigurationManager
{
    SystemConfig GetSystemConfig();
    T GetSection<T>(string sectionPath);
    void UpdateSection<T>(string sectionPath, T value);
    void Save();
}

public class ConfigurationManager : IConfigurationManager
{
    private readonly SystemConfig _config;
    private readonly string _configFilePath;

    public ConfigurationManager(SystemConfig config, string configFilePath = null)
    {
        _config = config;
        _configFilePath = configFilePath;
    }

    public SystemConfig GetSystemConfig()
    {
        return _config;
    }

    public T GetSection<T>(string sectionPath)
    {
        if (string.IsNullOrEmpty(sectionPath))
        {
            throw new ArgumentNullException(nameof(sectionPath));
        }

        // 处理简单的配置节路径，如 "ClientConfig"、"InternalServerConfig"
        var sections = sectionPath.Split('.');
        object current = _config;

        foreach (var section in sections)
        {
            var property = current.GetType().GetProperty(section, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (property == null)
            {
                throw new KeyNotFoundException($"配置节 '{sectionPath}' 不存在");
            }
            current = property.GetValue(current);
        }

        return (T)current;
    }

    public void UpdateSection<T>(string sectionPath, T value)
    {
        if (string.IsNullOrEmpty(sectionPath))
        {
            throw new ArgumentNullException(nameof(sectionPath));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        // 处理简单的配置节路径，如 "ClientConfig"、"InternalServerConfig"
        var sections = sectionPath.Split('.');
        object current = _config;

        for (int i = 0; i < sections.Length - 1; i++)
        {
            var property = current.GetType().GetProperty(sections[i], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (property == null)
            {
                throw new KeyNotFoundException($"配置节 '{sectionPath}' 不存在");
            }
            current = property.GetValue(current);
        }

        var targetProperty = current.GetType().GetProperty(sections.Last(), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (targetProperty == null)
        {
            throw new KeyNotFoundException($"配置节 '{sectionPath}' 不存在");
        }

        targetProperty.SetValue(current, value);
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(_configFilePath))
        {
            throw new InvalidOperationException("配置文件路径未设置，无法保存配置");
        }

        var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configFilePath, json);
    }
}

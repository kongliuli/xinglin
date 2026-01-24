using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Xinglin.Core.Communication;
using Xinglin.Infrastructure.Communication;

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

        return Build(config);
    }

    public static IContainer Build(SystemConfig config)
    {
        var builder = new ContainerBuilder();
        
        // 注册核心服务
        builder.RegisterInstance(config).As<SystemConfig>().SingleInstance();
        builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>().SingleInstance();
        
        // 根据运行模式注册组件
        RegisterComponentsByRunningMode(builder, config);
        
        // 注册模块
        RegisterModules(builder, config);
        
        // 注册依赖关系
        RegisterDependencies(builder, config);
        
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

    private static void RegisterDependencies(ContainerBuilder builder, SystemConfig config)
    {
        foreach (var dependency in config.Dependencies)
        {
            try
            {
                // 获取接口类型
                var interfaceType = Type.GetType(dependency.Key);
                if (interfaceType == null)
                {
                    throw new TypeLoadException($"找不到接口类型: {dependency.Key}");
                }
                
                // 获取实现类型
                var implementationType = Type.GetType(dependency.Value);
                if (implementationType == null)
                {
                    throw new TypeLoadException($"找不到实现类型: {dependency.Value}");
                }
                
                // 注册依赖关系
                builder.RegisterType(implementationType).As(interfaceType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"注册依赖关系 {dependency.Key} -> {dependency.Value} 失败: {ex.Message}", ex);
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

    public ConfigurationManager(SystemConfig config)
    {
        _config = config;
    }

    public SystemConfig GetSystemConfig()
    {
        return _config;
    }

    public T GetSection<T>(string sectionPath)
    {
        // 简单实现，根据sectionPath获取配置节
        // 实际项目中可以使用更复杂的配置路径解析
        throw new NotImplementedException("GetSection 方法尚未实现");
    }

    public void UpdateSection<T>(string sectionPath, T value)
    {
        // 简单实现，根据sectionPath更新配置节
        // 实际项目中可以使用更复杂的配置路径解析
        throw new NotImplementedException("UpdateSection 方法尚未实现");
    }

    public void Save()
    {
        // 保存配置到文件
        // 实际项目中需要实现配置持久化
        throw new NotImplementedException("Save 方法尚未实现");
    }
}

using System.IO;
using System.Text.Json;
using Xunit;
using Autofac;
using Xinglin.Infrastructure.DependencyInjection;

namespace Xinglin.ModularTests;

// 依赖注入测试类
public class DependencyInjectionTests
{
    [Fact]
    public void TestContainerBuildFromConfig()
    {
        // 创建临时配置文件
        var tempConfig = new SystemConfig
        {
            RunningMode = "SingleClient",
            Modules = new List<ModuleConfig>
            {
                new ModuleConfig { Name = "TestModule", Enabled = true }
            },
            Dependencies = new Dictionary<string, string>
            {
                { "Xinglin.Core.Services.IReportService", "Xinglin.Core.Services.ReportService" }
            }
        };
        
        var configJson = JsonSerializer.Serialize(tempConfig);
        var configPath = "temp_test_config.json";
        File.WriteAllText(configPath, configJson);
        
        try
        {
            // 测试容器构建
            var container = DIContainerBuilder.Build(configPath);
            
            // 验证容器不为空
            Assert.NotNull(container);
            
            // 验证配置能够被解析
            var config = container.Resolve<SystemConfig>();
            Assert.NotNull(config);
            Assert.Equal("SingleClient", config.RunningMode);
        }
        finally
        {
            // 清理临时文件
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
        }
    }
    
    [Fact]
    public void TestContainerBuildFromSystemConfig()
    {
        // 创建系统配置
        var config = new SystemConfig
        {
            RunningMode = "ClientInternalServer",
            InternalServerConfig = new InternalServerConfig
            {
                BaseUrl = "http://localhost:5000",
                ApiKey = "test-api-key"
            }
        };
        
        // 测试容器构建
        var container = DIContainerBuilder.Build(config);
        
        // 验证容器不为空
        Assert.NotNull(container);
        
        // 验证配置能够被解析
        var resolvedConfig = container.Resolve<SystemConfig>();
        Assert.NotNull(resolvedConfig);
        Assert.Equal("ClientInternalServer", resolvedConfig.RunningMode);
        Assert.Equal("http://localhost:5000", resolvedConfig.InternalServerConfig.BaseUrl);
        Assert.Equal("test-api-key", resolvedConfig.InternalServerConfig.ApiKey);
    }
    
    [Fact]
    public void TestContainerBuildWithInvalidConfig()
    {
        // 创建无效配置
        var invalidConfig = "{}";
        var configPath = "temp_invalid_config.json";
        File.WriteAllText(configPath, invalidConfig);
        
        try
        {
            // 测试无效配置构建容器应该抛出异常
            Assert.Throws<InvalidOperationException>(() => DIContainerBuilder.Build(configPath));
        }
        finally
        {
            // 清理临时文件
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
        }
    }
}

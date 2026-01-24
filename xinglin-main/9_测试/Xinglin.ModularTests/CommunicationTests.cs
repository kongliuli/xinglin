using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xinglin.Core.Communication;
using Xinglin.Core.Services;
using Xinglin.Infrastructure.Communication;

namespace Xinglin.ModularTests;

// 通信代理测试类
public class CommunicationTests
{
    [Fact]
    public async Task TestMockCommunicationProxy()
    {
        // 创建模拟通信代理
        var mockProxy = new MockCommunicationProxy();
        
        // 初始化通信代理
        await mockProxy.InitializeAsync(new CommunicationConfig        {
            BaseUrl = "http://localhost:5000",
            ApiKey = "test-api-key",
            UseHttps = false
        });
        
        // 测试连接状态
        var isConnected = await mockProxy.IsConnectedAsync();
        Assert.True(isConnected);
        
        // 测试GET请求
        var templates = await mockProxy.GetAsync<IEnumerable<TemplateDto>>("templates");
        Assert.NotNull(templates);
        Assert.NotEmpty(templates);
        
        // 测试POST请求
        var report = new ReportDto        {
            ReportNumber = "TEST20260124-0001",
            Patient = new PatientDto            {
                Name = "测试患者",
                Gender = "男",
                Age = 25,
                IdCard = "110101199001011234",
                Phone = "13800138000"
            },
            TemplateId = Guid.NewGuid(),
            Content = new Dictionary<string, object>            {
                { "项目1", "阳性" },
                { "项目2", "阴性" }
            },
            Status = "已完成",
            CreatedBy = "test_user",
            CreatedAt = DateTime.Now
        };
        
        var reportId = await mockProxy.SendRequestAsync<ReportDto, Guid>("reports/create", report, HttpMethod.Post);
        Assert.NotEqual(Guid.Empty, reportId);
    }
    
    [Fact]
    public async Task TestHttpCommunicationProxyInitialization()
    {
        // 创建HTTP通信代理
        var httpProxy = new HttpCommunicationProxy();
        
        // 测试初始化
        await httpProxy.InitializeAsync(new CommunicationConfig        {
            BaseUrl = "http://localhost:5000",
            ApiKey = "test-api-key",
            UseHttps = false,
            Timeout = 10
        });
        
        // 测试连接状态（预期会失败，因为没有实际服务器）
        var isConnected = await httpProxy.IsConnectedAsync();
        Assert.False(isConnected);
    }
    
    [Fact]
    public async Task TestCommunicationProxyNotInitializedException()
    {
        // 创建HTTP通信代理
        var httpProxy = new HttpCommunicationProxy();
        
        // 测试未初始化时发送请求会抛出异常
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await httpProxy.SendRequestAsync<string, string>("test", "test");
        });
    }
}

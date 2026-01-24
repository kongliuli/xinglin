using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xinglin.Core.Communication;
using Xinglin.Core.Services;

namespace Xinglin.Infrastructure.Communication;

// 模拟通信代理实现，用于测试
public class MockCommunicationProxy : ICommunicationProxy
{
    private CommunicationConfig _config;
    private readonly Dictionary<Guid, ReportDto> _mockReports;
    private readonly Dictionary<Guid, TemplateDto> _mockTemplates;

    public MockCommunicationProxy()
    {
        _mockReports = new Dictionary<Guid, ReportDto>();
        _mockTemplates = new Dictionary<Guid, TemplateDto>();
        
        // 初始化模拟数据
        InitializeMockData();
    }

    public async Task InitializeAsync(CommunicationConfig config)
    {
        _config = config;
        // 模拟初始化，无需实际连接
        await Task.CompletedTask;
    }

    public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(string endpoint, TRequest request, HttpMethod method = null)
    {
        // 模拟请求处理
        if (endpoint.Contains("reports/create"))
        {
            var reportRequest = request as ReportDto;
            if (reportRequest != null)
            {
                var reportId = Guid.NewGuid();
                reportRequest.ReportId = reportId;
                _mockReports[reportId] = reportRequest;
                return await Task.FromResult((TResponse)(object)reportId);
            }
        }
        
        if (endpoint.Contains("reports/archive"))
        {
            return await Task.FromResult((TResponse)(object)Guid.NewGuid());
        }
        
        if (endpoint.Contains("templates/save"))
        {
            var templateRequest = request as TemplateDto;
            if (templateRequest != null)
            {
                var templateId = Guid.NewGuid();
                templateRequest.TemplateId = templateId;
                _mockTemplates[templateId] = templateRequest;
                return await Task.FromResult((TResponse)(object)templateId);
            }
        }
        
        return await Task.FromResult(default(TResponse));
    }

    public async Task<TResponse> GetAsync<TResponse>(string endpoint)
    {
        // 模拟GET请求处理
        if (endpoint.Contains("reports/"))
        {
            var parts = endpoint.Split('/');
            if (parts.Length > 2 && Guid.TryParse(parts[2], out var reportId))
            {
                if (_mockReports.TryGetValue(reportId, out var report))
                {
                    return await Task.FromResult((TResponse)(object)report);
                }
            }
        }
        
        if (endpoint.Contains("templates"))
        {
            var templates = new List<TemplateDto>(_mockTemplates.Values);
            return await Task.FromResult((TResponse)(object)templates);
        }
        
        return await Task.FromResult(default(TResponse));
    }

    public async Task<bool> IsConnectedAsync()
    {
        // 模拟连接状态，始终返回已连接
        return await Task.FromResult(true);
    }

    private void InitializeMockData()
    {
        // 添加模拟报告数据
        var reportId = Guid.NewGuid();
        _mockReports[reportId] = new ReportDto
        {
            ReportId = reportId,
            ReportNumber = "LAB20260124-0001",
            Patient = new PatientDto
            {
                PatientId = Guid.NewGuid(),
                Name = "张三",
                Gender = "男",
                Age = 30,
                IdCard = "110101199001011234",
                Phone = "13800138000",
                HospitalId = "HOS001"
            },
            TemplateId = Guid.NewGuid(),
            Content = new Dictionary<string, object>
            {
                { "项目1", "阳性" },
                { "项目2", "阴性" },
                { "项目3", "123.45" }
            },
            Status = "已完成",
            CreatedBy = "admin",
            CreatedAt = DateTime.Now
        };
        
        // 添加模拟模板数据
        var templateId = Guid.NewGuid();
        _mockTemplates[templateId] = new TemplateDto
        {
            TemplateId = templateId,
            Name = "常规病理报告模板",
            Version = "1.0.0",
            Content = "{\"template\": \"常规病理报告\"}",
            IsPublished = true,
            CreatedBy = "admin",
            CreatedAt = DateTime.Now
        };
    }
}

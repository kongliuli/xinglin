using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xinglin.Core.Communication;

namespace Xinglin.Infrastructure.Communication;

// HTTP通信代理实现
public class HttpCommunicationProxy : ICommunicationProxy
{
    private HttpClient _httpClient;
    private CommunicationConfig _config;

    public HttpCommunicationProxy()
    {
        _httpClient = new HttpClient();
    }

    public async Task InitializeAsync(CommunicationConfig config)
    {
        _config = config;
        _httpClient.BaseAddress = new Uri(config.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(config.Timeout);
        
        // 添加API密钥到请求头
        if (!string.IsNullOrEmpty(config.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", config.ApiKey);
        }
        
        // 配置HTTPS
        if (config.UseHttps && !config.AllowInsecureConnections)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }

    public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(string endpoint, TRequest request, HttpMethod method = null)
    {
        if (_config == null)
        {
            throw new InvalidOperationException("通信代理尚未初始化");
        }
        
        method ??= HttpMethod.Post;
        var jsonContent = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(new HttpRequestMessage(method, endpoint) { Content = content });
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseContent);
    }

    public async Task<TResponse> GetAsync<TResponse>(string endpoint)
    {
        if (_config == null)
        {
            throw new InvalidOperationException("通信代理尚未初始化");
        }
        
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseContent);
    }

    public async Task<bool> IsConnectedAsync()
    {
        if (_config == null)
        {
            throw new InvalidOperationException("通信代理尚未初始化");
        }
        
        try
        {
            var response = await _httpClient.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

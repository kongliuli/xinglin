using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xinglin.Core.Communication;

namespace Xinglin.Infrastructure.Communication;

// 服务端通信网关实现
public class ServerGateway : IServerGateway
{
    private readonly Dictionary<string, HttpClient> _httpClients;
    private readonly Dictionary<string, string> _serverBaseUrls;
    private readonly Dictionary<string, string> _serverApiKeys;

    public ServerGateway()
    {
        _httpClients = new Dictionary<string, HttpClient>();
        _serverBaseUrls = new Dictionary<string, string>();
        _serverApiKeys = new Dictionary<string, string>();
    }

    public async Task InitializeAsync(ServerGatewayConfig config)
    {
        // 初始化内网服务端客户端
        InitializeHttpClient("internal", config.InternalServer.BaseUrl, config.InternalServer.ApiKey);
        
        // 初始化外网服务端客户端
        InitializeHttpClient("external", config.ExternalServer.BaseUrl, config.ExternalServer.ApiKey);
        
        // 初始化主控服务端客户端
        InitializeHttpClient("master", config.MasterServer.BaseUrl, config.MasterServer.ApiKey);
    }

    private void InitializeHttpClient(string serverType, string baseUrl, string apiKey)
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(baseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        // 添加API密钥到请求头
        if (!string.IsNullOrEmpty(apiKey))
        {
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        }
        
        _httpClients[serverType] = httpClient;
        _serverBaseUrls[serverType] = baseUrl;
        _serverApiKeys[serverType] = apiKey;
    }

    public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(string serverType, string endpoint, TRequest request, HttpMethod method = null)
    {
        if (!_httpClients.TryGetValue(serverType, out var httpClient))
        {
            throw new NotSupportedException($"不支持的服务器类型: {serverType}");
        }
        
        method ??= HttpMethod.Post;
        var jsonContent = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        var response = await httpClient.SendAsync(new HttpRequestMessage(method, endpoint) { Content = content });
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseContent);
    }

    public async Task<bool> IsServerConnectedAsync(string serverType)
    {
        if (!_httpClients.TryGetValue(serverType, out var httpClient))
        {
            throw new NotSupportedException($"不支持的服务器类型: {serverType}");
        }
        
        try
        {
            var response = await httpClient.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

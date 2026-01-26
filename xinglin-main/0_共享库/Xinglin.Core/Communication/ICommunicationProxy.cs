using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xinglin.Core.Communication;

// 通信代理接口
public interface ICommunicationProxy
{
    Task<TResponse> SendRequestAsync<TRequest, TResponse>(string endpoint, TRequest request, HttpMethod? method = null);
    Task<TResponse> GetAsync<TResponse>(string endpoint);
    Task<bool> IsConnectedAsync();
    Task InitializeAsync(CommunicationConfig config);
}

// 通信配置类
public class CommunicationConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30;
    public bool UseHttps { get; set; } = true;
    public bool AllowInsecureConnections { get; set; } = false;
}

// 服务端通信网关接口
public interface IServerGateway
{
    Task<TResponse> SendRequestAsync<TRequest, TResponse>(string serverType, string endpoint, TRequest request, HttpMethod? method = null);
    Task<bool> IsServerConnectedAsync(string serverType);
    Task InitializeAsync(ServerGatewayConfig config);
}

// 服务端网关配置类
public class ServerGatewayConfig
{
    public InternalServerConfig InternalServer { get; set; } = new InternalServerConfig();
    public ExternalServerConfig ExternalServer { get; set; } = new ExternalServerConfig();
    public MasterServerConfig MasterServer { get; set; } = new MasterServerConfig();
}

// 内网服务器配置
public class InternalServerConfig
{
    public string BaseUrl { get; set; } = "http://localhost:5000";
    public string ApiKey { get; set; } = string.Empty;
}

// 外网服务器配置
public class ExternalServerConfig
{
    public string BaseUrl { get; set; } = "https://external.xinglin.com";
    public string ApiKey { get; set; } = string.Empty;
}

// 主控服务器配置
public class MasterServerConfig
{
    public string BaseUrl { get; set; } = "https://master.xinglin.com";
    public string ApiKey { get; set; } = string.Empty;
}

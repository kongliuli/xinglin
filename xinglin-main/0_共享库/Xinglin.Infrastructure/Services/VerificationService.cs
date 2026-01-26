using System;using System.Net.Http;using System.Text.Json;using System.Threading.Tasks;using Xinglin.Core.Communication;using Xinglin.Security;

namespace Xinglin.Infrastructure.Services;

/// <summary>
/// 授权验证服务实现，负责与总控系统进行通信
/// </summary>
public class VerificationService : IVerificationService
{
    private readonly ICommunicationProxy _communicationProxy;
    private readonly IMachineCodeService _machineCodeService;
    private bool _isMasterServerAvailable = true;

    public VerificationService(ICommunicationProxy communicationProxy, IMachineCodeService machineCodeService)
    {
        _communicationProxy = communicationProxy;
        _machineCodeService = machineCodeService;
    }

    /// <summary>
    /// 与总控系统验证机器码
    /// </summary>
    /// <param name="machineCode">机器码</param>
    /// <returns>是否验证成功</returns>
    public async Task<bool> VerifyWithMasterServerAsync(string machineCode)
    {
        if (!IsMasterServerAvailable())
        {
            return false;
        }

        try
        {
            var request = new MachineCodeVerificationRequest
            {
                MachineCode = machineCode,
                ProductVersion = GetProductVersion(),
                Timestamp = DateTime.UtcNow
            };

            var response = await _communicationProxy.SendRequestAsync<MachineCodeVerificationRequest, MachineCodeVerificationResponse>(
                "/api/verification/verify-machine-code",
                request,
                System.Net.Http.HttpMethod.Post
            );

            return response?.IsVerified ?? false;
        }
        catch (Exception)
        {
            _isMasterServerAvailable = false;
            return false;
        }
    }

    /// <summary>
    /// 从总控系统获取许可证
    /// </summary>
    /// <param name="machineCode">机器码</param>
    /// <param name="licenseKey">许可证密钥</param>
    /// <returns>许可证信息</returns>
    public async Task<LicenseInfo> GetLicenseFromMasterServerAsync(string machineCode, string licenseKey)
    {
        if (!IsMasterServerAvailable())
        {
            return null;
        }

        try
        {
            var request = new LicenseRequest
            {
                MachineCode = machineCode,
                LicenseKey = licenseKey,
                ProductVersion = GetProductVersion(),
                Timestamp = DateTime.UtcNow
            };

            var response = await _communicationProxy.SendRequestAsync<LicenseRequest, LicenseResponse>(
                "/api/verification/get-license",
                request,
                System.Net.Http.HttpMethod.Post
            );

            if (response?.IsSuccess ?? false && !string.IsNullOrEmpty(response.LicenseContent))
            {
                // 解密许可证内容
                var decryptedContent = DecryptLicense(response.LicenseContent);
                return JsonSerializer.Deserialize<LicenseInfo>(decryptedContent);
            }

            return null;
        }
        catch (Exception)
        {
            _isMasterServerAvailable = false;
            return null;
        }
    }

    /// <summary>
    /// 检查总控系统是否可用
    /// </summary>
    /// <returns>是否可用</returns>
    public bool IsMasterServerAvailable()
    {
        if (!_isMasterServerAvailable)
        {
            return false;
        }

        try
        {
            // 尝试连接总控系统
            var response = _communicationProxy.SendRequestAsync<object, object>(
                "/api/verification/ping",
                null,
                System.Net.Http.HttpMethod.Get
            ).Result;

            return response != null;
        }
        catch (Exception)
        {
            _isMasterServerAvailable = false;
            return false;
        }
    }

    /// <summary>
    /// 获取产品版本
    /// </summary>
    /// <returns>产品版本号</returns>
    private string GetProductVersion()
    {
        // 从程序集获取版本号
        var assembly = System.Reflection.Assembly.GetEntryAssembly();
        var version = assembly?.GetName().Version;
        return version?.ToString() ?? "1.0.0";
    }

    /// <summary>
    /// 解密许可证内容
    /// </summary>
    /// <param name="encryptedContent">加密的许可证内容</param>
    /// <returns>解密后的内容</returns>
    private string DecryptLicense(string encryptedContent)
    {
        // 简单的解密实现，实际项目中应使用更安全的加密算法
        var bytes = Convert.FromBase64String(encryptedContent);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// 机器码验证请求
    /// </summary>
    private class MachineCodeVerificationRequest
    {
        public string MachineCode { get; set; } = string.Empty;
        public string ProductVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// 机器码验证响应
    /// </summary>
    private class MachineCodeVerificationResponse
    {
        public bool IsVerified { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ValidUntil { get; set; }
    }

    /// <summary>
    /// 许可证请求
    /// </summary>
    private class LicenseRequest
    {
        public string MachineCode { get; set; } = string.Empty;
        public string LicenseKey { get; set; } = string.Empty;
        public string ProductVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// 许可证响应
    /// </summary>
    private class LicenseResponse
    {
        public bool IsSuccess { get; set; }
        public string LicenseContent { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
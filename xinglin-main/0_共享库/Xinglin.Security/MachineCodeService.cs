using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinglin.Security;

/// <summary>
/// 机器码服务接口
/// </summary>
public interface IMachineCodeService
{
    string GenerateMachineCode();
    bool VerifyMachineCode(string machineCode);
    string GetHardwareInfo();
}

/// <summary>
/// 许可证信息
/// </summary>
public class LicenseInfo
{
    public string MachineCode { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public DateTime ActivationDate { get; set; }
    public List<string> Permissions { get; set; } = new List<string>();
    public LicenseStatus Status { get; set; }
    public string ProductVersion { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
}

/// <summary>
/// 许可证状态枚举
/// </summary>
public enum LicenseStatus
{
    Invalid,
    Valid,
    Expired,
    Revoked,
    Trial
}

/// <summary>
/// 授权状态枚举
/// </summary>
public enum AuthorizationStatus
{
    Normal,
    LicensePending,
    Trial,
    Locked
}

/// <summary>
/// 许可证服务接口
/// </summary>
public interface ILicenseService
{
    bool ImportLicense(string licenseContent);
    LicenseInfo GetCurrentLicense();
    bool ValidateLicense();
    LicenseStatus GetLicenseStatus();
    bool ActivateLicense(string licenseKey);
    void ExportLicenseInfo(string filePath);
}

/// <summary>
/// 授权验证服务接口
/// </summary>
public interface IVerificationService
{
    Task<bool> VerifyWithMasterServerAsync(string machineCode);
    Task<LicenseInfo> GetLicenseFromMasterServerAsync(string machineCode, string licenseKey);
    bool IsMasterServerAvailable();
}

/// <summary>
/// 验证结果
/// </summary>
public class VerificationResult
{
    public bool IsVerified { get; set; }
    public AuthorizationStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public LicenseInfo LicenseInfo { get; set; } = new LicenseInfo();
}

/// <summary>
/// 权限状态管理接口
/// </summary>
public interface IPermissionStatusService
{
    AuthorizationStatus GetCurrentStatus();
    void SetStatus(AuthorizationStatus status);
    bool CheckPermission(string permission);
    void UpdateStatus();
    bool IsFeatureAvailable(string featureName);
}
using System;
using System.Threading.Tasks;

namespace Xinglin.Core.Services;

// 授权服务接口
public interface IAuthorizationService
{
    Task<bool> ActivateSoftwareAsync(string activationCode);
    Task<LicenseDto> GetLicenseInfoAsync();
    Task<bool> SendHeartbeatAsync();
    Task<bool> ValidateLicenseAsync();
    Task<bool> UpdateLicenseAsync(LicenseUpdateDto updateDto);
    Task<string> GenerateMachineCodeAsync();
}

// 许可证数据传输对象
public class LicenseDto
{
    public Guid LicenseId { get; set; }
    public string MachineCode { get; set; } = string.Empty;
    public string ActivationCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LicenseStatus Status { get; set; }
    public LicensePermissionsDto Permissions { get; set; } = new LicensePermissionsDto();
    public string HospitalName { get; set; } = string.Empty;
    public string HospitalId { get; set; } = string.Empty;
}

// 许可证状态枚举
public enum LicenseStatus
{
    Inactive,
    Active,
    Expired,
    Suspended,
    Revoked
}

// 许可证权限数据传输对象
public class LicensePermissionsDto
{
    public bool AllowReportCreation { get; set; } = true;
    public bool AllowReportPrinting { get; set; } = true;
    public bool AllowTemplateEditing { get; set; } = true;
    public bool AllowDataExport { get; set; } = true;
    public bool AllowMultiUser { get; set; } = false;
    public int MaxConcurrentUsers { get; set; } = 1;
}

// 许可证更新数据传输对象
public class LicenseUpdateDto
{
    public DateTime? EndDate { get; set; }
    public LicenseStatus? Status { get; set; }
    public LicensePermissionsDto? Permissions { get; set; }
}

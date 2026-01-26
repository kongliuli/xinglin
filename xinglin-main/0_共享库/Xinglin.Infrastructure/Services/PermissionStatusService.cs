using System;using System.IO;using System.Text.Json;using Xinglin.Security;

namespace Xinglin.Infrastructure.Services;

/// <summary>
/// 权限状态管理服务实现，负责管理系统的授权状态
/// </summary>
public class PermissionStatusService : IPermissionStatusService
{
    private const string StatusFileName = "authorizationstatus.dat";
    private const int TrialDays = 3;

    private readonly ILicenseService _licenseService;
    private readonly IVerificationService _verificationService;
    private readonly IMachineCodeService _machineCodeService;

    private AuthorizationStatus _currentStatus = AuthorizationStatus.Trial;
    private TrialInfo _trialInfo = new TrialInfo();

    public PermissionStatusService(
        ILicenseService licenseService,
        IVerificationService verificationService,
        IMachineCodeService machineCodeService)
    {
        _licenseService = licenseService;
        _verificationService = verificationService;
        _machineCodeService = machineCodeService;

        LoadStatus();
        LoadTrialInfo();
        UpdateStatus();
    }

    /// <summary>
    /// 获取当前授权状态
    /// </summary>
    /// <returns>授权状态</returns>
    public AuthorizationStatus GetCurrentStatus()
    {
        return _currentStatus;
    }

    /// <summary>
    /// 设置授权状态
    /// </summary>
    /// <param name="status">新的授权状态</param>
    public void SetStatus(AuthorizationStatus status)
    {
        _currentStatus = status;
        SaveStatus();
    }

    /// <summary>
    /// 检查权限
    /// </summary>
    /// <param name="permission">权限名称</param>
    /// <returns>是否有权限</returns>
    public bool CheckPermission(string permission)
    {
        // 检查当前状态
        if (_currentStatus == AuthorizationStatus.Locked)
        {
            return false;
        }

        // 检查许可证权限
        var license = _licenseService.GetCurrentLicense();
        if (license != null && license.Status == LicenseStatus.Valid)
        {
            return license.Permissions.Contains(permission) || license.Permissions.Contains("*/*");
        }

        // 试用模式下的权限限制
        if (_currentStatus == AuthorizationStatus.Trial)
        {
            var restrictedPermissions = new[] { "export", "save", "advanced" };
            return !restrictedPermissions.Contains(permission);
        }

        // 许可证待激活状态下的权限限制
        if (_currentStatus == AuthorizationStatus.LicensePending)
        {
            var restrictedPermissions = new[] { "export", "save", "advanced", "print" };
            return !restrictedPermissions.Contains(permission);
        }

        return false;
    }

    /// <summary>
    /// 更新授权状态
    /// </summary>
    public void UpdateStatus()
    {
        // 检查许可证有效性
        var license = _licenseService.GetCurrentLicense();
        if (_licenseService.ValidateLicense())
        {
            _currentStatus = AuthorizationStatus.Normal;
            SaveStatus();
            return;
        }

        // 检查总控系统是否可用
        var machineCode = _machineCodeService.GenerateMachineCode();
        var isVerified = _verificationService.IsMasterServerAvailable();

        if (isVerified)
        {
            // 总控系统可用，检查机器码验证
            _verificationService.VerifyWithMasterServerAsync(machineCode).Wait();
            // 这里可以根据验证结果设置状态
            _currentStatus = AuthorizationStatus.LicensePending;
        }
        else
        {
            // 总控系统不可用，检查试用状态
            CheckTrialStatus();
        }

        SaveStatus();
    }

    /// <summary>
    /// 检查功能是否可用
    /// </summary>
    /// <param name="featureName">功能名称</param>
    /// <returns>是否可用</returns>
    public bool IsFeatureAvailable(string featureName)
    {
        // 检查当前状态
        if (_currentStatus == AuthorizationStatus.Locked)
        {
            return false;
        }

        // 检查许可证权限
        var license = _licenseService.GetCurrentLicense();
        if (license != null && license.Status == LicenseStatus.Valid)
        {
            return license.Permissions.Contains($"feature/{featureName}") || license.Permissions.Contains("*/*");
        }

        // 试用模式下的功能限制
        if (_currentStatus == AuthorizationStatus.Trial)
        {
            var restrictedFeatures = new[] { "advanced_reporting", "batch_processing", "custom_templates" };
            return !restrictedFeatures.Contains(featureName);
        }

        // 许可证待激活状态下的功能限制
        if (_currentStatus == AuthorizationStatus.LicensePending)
        {
            var restrictedFeatures = new[] { "advanced_reporting", "batch_processing", "custom_templates", "export" };
            return !restrictedFeatures.Contains(featureName);
        }

        return false;
    }

    /// <summary>
    /// 检查试用状态
    /// </summary>
    private void CheckTrialStatus()
    {
        // 计算试用天数
        var trialDaysPassed = (DateTime.Now - _trialInfo.FirstLaunchTime).TotalDays;
        if (trialDaysPassed > TrialDays)
        {
            // 试用期满，锁定系统
            _currentStatus = AuthorizationStatus.Locked;
        }
        else
        {
            // 继续试用
            _currentStatus = AuthorizationStatus.Trial;
        }
    }

    /// <summary>
    /// 加载授权状态
    /// </summary>
    private void LoadStatus()
    {
        if (File.Exists(StatusFileName))
        {
            try
            {
                var statusContent = File.ReadAllText(StatusFileName);
                _currentStatus = JsonSerializer.Deserialize<AuthorizationStatus>(statusContent);
            }
            catch (Exception)
            {
                _currentStatus = AuthorizationStatus.Trial;
            }
        }
    }

    /// <summary>
    /// 保存授权状态
    /// </summary>
    private void SaveStatus()
    {
        var statusContent = JsonSerializer.Serialize(_currentStatus);
        File.WriteAllText(StatusFileName, statusContent);
    }

    /// <summary>
    /// 加载试用信息
    /// </summary>
    private void LoadTrialInfo()
    {
        const string trialInfoFileName = "trialinfo.dat";
        if (File.Exists(trialInfoFileName))
        {
            try
            {
                var trialContent = File.ReadAllText(trialInfoFileName);
                _trialInfo = JsonSerializer.Deserialize<TrialInfo>(trialContent);
            }
            catch (Exception)
            {
                _trialInfo = new TrialInfo { FirstLaunchTime = DateTime.Now };
                SaveTrialInfo();
            }
        }
        else
        {
            _trialInfo = new TrialInfo { FirstLaunchTime = DateTime.Now };
            SaveTrialInfo();
        }
    }

    /// <summary>
    /// 保存试用信息
    /// </summary>
    private void SaveTrialInfo()
    {
        const string trialInfoFileName = "trialinfo.dat";
        var trialContent = JsonSerializer.Serialize(_trialInfo);
        File.WriteAllText(trialInfoFileName, trialContent);
    }

    /// <summary>
    /// 试用信息
    /// </summary>
    private class TrialInfo
    {
        public DateTime FirstLaunchTime { get; set; }
        public int LaunchCount { get; set; } = 1;
    }
}
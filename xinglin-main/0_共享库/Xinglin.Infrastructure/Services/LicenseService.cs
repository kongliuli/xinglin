using System;using System.IO;using System.Security.Cryptography;using System.Text;using System.Text.Json;using Xinglin.Security;

namespace Xinglin.Infrastructure.Services;

/// <summary>
/// 许可证服务实现，负责许可证的导入、验证和管理
/// </summary>
public class LicenseService : ILicenseService
{
    private const string LicenseFileName = "license.dat";
    private const string TrialInfoFileName = "trialinfo.dat";
    private const int TrialDays = 3;

    private LicenseInfo _currentLicense = new LicenseInfo();
    private TrialInfo _trialInfo = new TrialInfo();

    public LicenseService()
    {
        LoadLicense();
        LoadTrialInfo();
    }

    /// <summary>
    /// 导入许可证
    /// </summary>
    /// <param name="licenseContent">许可证内容</param>
    /// <returns>是否导入成功</returns>
    public bool ImportLicense(string licenseContent)
    {
        if (string.IsNullOrEmpty(licenseContent))
        {
            return false;
        }

        try
        {
            // 解密许可证内容
            var decryptedContent = DecryptLicense(licenseContent);
            var license = JsonSerializer.Deserialize<LicenseInfo>(decryptedContent);
            
            if (license == null)
            {
                return false;
            }

            // 验证许可证有效性
            if (ValidateLicense(license))
            {
                // 保存许可证
                File.WriteAllText(LicenseFileName, licenseContent);
                _currentLicense = license;
                return true;
            }

            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 获取当前许可证信息
    /// </summary>
    /// <returns>许可证信息</returns>
    public LicenseInfo GetCurrentLicense()
    {
        return _currentLicense;
    }

    /// <summary>
    /// 验证许可证
    /// </summary>
    /// <returns>是否有效</returns>
    public bool ValidateLicense()
    {
        return ValidateLicense(_currentLicense);
    }

    /// <summary>
    /// 获取许可证状态
    /// </summary>
    /// <returns>许可证状态</returns>
    public LicenseStatus GetLicenseStatus()
    {
        return _currentLicense.Status;
    }

    /// <summary>
    /// 激活许可证
    /// </summary>
    /// <param name="licenseKey">许可证密钥</param>
    /// <returns>是否激活成功</returns>
    public bool ActivateLicense(string licenseKey)
    {
        if (string.IsNullOrEmpty(licenseKey))
        {
            return false;
        }

        try
        {
            // 解析许可证密钥
            var license = ParseLicenseKey(licenseKey);
            
            if (license == null || !ValidateLicense(license))
            {
                return false;
            }

            // 保存许可证
            var encryptedLicense = EncryptLicense(JsonSerializer.Serialize(license));
            File.WriteAllText(LicenseFileName, encryptedLicense);
            _currentLicense = license;
            
            // 清除试用信息
            if (File.Exists(TrialInfoFileName))
            {
                File.Delete(TrialInfoFileName);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 导出许可证信息
    /// </summary>
    /// <param name="filePath">保存路径</param>
    public void ExportLicenseInfo(string filePath)
    {
        var licenseInfo = GetCurrentLicense();
        var jsonContent = JsonSerializer.Serialize(licenseInfo, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, jsonContent);
    }

    /// <summary>
    /// 加载许可证
    /// </summary>
    private void LoadLicense()
    {
        if (File.Exists(LicenseFileName))
        {
            try
            {
                var licenseContent = File.ReadAllText(LicenseFileName);
                var decryptedContent = DecryptLicense(licenseContent);
                var license = JsonSerializer.Deserialize<LicenseInfo>(decryptedContent);
                
                if (license != null && ValidateLicense(license))
                {
                    _currentLicense = license;
                }
            }
            catch (Exception)
            {
                // 许可证文件无效，使用默认值
                _currentLicense = new LicenseInfo();
            }
        }
    }

    /// <summary>
    /// 加载试用信息
    /// </summary>
    private void LoadTrialInfo()
    {
        if (File.Exists(TrialInfoFileName))
        {
            try
            {
                var trialContent = File.ReadAllText(TrialInfoFileName);
                _trialInfo = JsonSerializer.Deserialize<TrialInfo>(trialContent);
            }
            catch (Exception)
            {
                // 试用信息文件无效，创建新的
                _trialInfo = new TrialInfo { FirstLaunchTime = DateTime.Now };
                SaveTrialInfo();
            }
        }
        else
        {
            // 首次启动，创建试用信息
            _trialInfo = new TrialInfo { FirstLaunchTime = DateTime.Now };
            SaveTrialInfo();
        }
    }

    /// <summary>
    /// 保存试用信息
    /// </summary>
    private void SaveTrialInfo()
    {
        var trialContent = JsonSerializer.Serialize(_trialInfo);
        File.WriteAllText(TrialInfoFileName, trialContent);
    }

    /// <summary>
    /// 验证许可证
    /// </summary>
    /// <param name="license">许可证信息</param>
    /// <returns>是否有效</returns>
    private bool ValidateLicense(LicenseInfo license)
    {
        if (license == null)
        {
            return false;
        }

        // 检查许可证状态
        if (license.Status == LicenseStatus.Revoked)
        {
            return false;
        }

        // 检查许可证过期日期
        if (license.ExpiryDate < DateTime.Now)
        {
            license.Status = LicenseStatus.Expired;
            return false;
        }

        // 检查许可证类型
        if (license.LicenseType == "Trial")
        {
            license.Status = LicenseStatus.Trial;
        }
        else
        {
            license.Status = LicenseStatus.Valid;
        }

        return true;
    }

    /// <summary>
    /// 解密许可证
    /// </summary>
    /// <param name="encryptedContent">加密的许可证内容</param>
    /// <returns>解密后的许可证内容</returns>
    private string DecryptLicense(string encryptedContent)
    {
        // 简单的解密实现，实际项目中应使用更安全的加密算法
        var bytes = Convert.FromBase64String(encryptedContent);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// 加密许可证
    /// </summary>
    /// <param name="content">许可证内容</param>
    /// <returns>加密后的许可证内容</returns>
    private string EncryptLicense(string content)
    {
        // 简单的加密实现，实际项目中应使用更安全的加密算法
        var bytes = Encoding.UTF8.GetBytes(content);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 解析许可证密钥
    /// </summary>
    /// <param name="licenseKey">许可证密钥</param>
    /// <returns>许可证信息</returns>
    private LicenseInfo ParseLicenseKey(string licenseKey)
    {
        // 简单的许可证密钥解析，实际项目中应使用更复杂的解析逻辑
        try
        {
            // 许可证密钥格式：MachineCode|ExpiryDate|Permissions|LicenseType
            var parts = licenseKey.Split('|');
            
            if (parts.Length < 4)
            {
                return null;
            }

            return new LicenseInfo
            {
                MachineCode = parts[0],
                LicenseKey = licenseKey,
                ExpiryDate = DateTime.Parse(parts[1]),
                ActivationDate = DateTime.Now,
                Permissions = parts[2].Split(',').ToList(),
                LicenseType = parts[3],
                Status = LicenseStatus.Valid,
                ProductVersion = "1.0.0"
            };
        }
        catch (Exception)
        {
            return null;
        }
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
using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Xinglin.Security;

namespace Xinglin.Infrastructure.Services;

/// <summary>
/// 机器码服务实现，基于硬件信息生成唯一机器码
/// </summary>
public class MachineCodeService : IMachineCodeService
{
    private const string MachineCodeFileName = "machinecode.dat";
    private string _machineCode = string.Empty;
    private string _hardwareInfo = string.Empty;

    /// <summary>
    /// 生成机器码
    /// </summary>
    /// <returns>唯一的机器码</returns>
    public string GenerateMachineCode()
    {
        if (!string.IsNullOrEmpty(_machineCode))
        {
            return _machineCode;
        }

        // 尝试从文件读取机器码
        if (File.Exists(MachineCodeFileName))
        {
            _machineCode = File.ReadAllText(MachineCodeFileName);
            if (!string.IsNullOrEmpty(_machineCode))
            {
                return _machineCode;
            }
        }

        // 生成新的机器码
        _hardwareInfo = GetHardwareInfo();
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_hardwareInfo));
            _machineCode = Convert.ToBase64String(hashBytes);
        }

        // 保存机器码到文件
        File.WriteAllText(MachineCodeFileName, _machineCode);
        return _machineCode;
    }

    /// <summary>
    /// 验证机器码
    /// </summary>
    /// <param name="machineCode">要验证的机器码</param>
    /// <returns>是否有效</returns>
    public bool VerifyMachineCode(string machineCode)
    {
        if (string.IsNullOrEmpty(machineCode))
        {
            return false;
        }

        var currentMachineCode = GenerateMachineCode();
        return string.Equals(currentMachineCode, machineCode, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 获取硬件信息
    /// </summary>
    /// <returns>硬件信息字符串</returns>
    public string GetHardwareInfo()
    {
        if (!string.IsNullOrEmpty(_hardwareInfo))
        {
            return _hardwareInfo;
        }

        var hardwareInfo = new StringBuilder();

        try
        {
            // 使用WMI获取硬件信息
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystemProduct");
            foreach (var obj in searcher.Get())
            {
                hardwareInfo.AppendLine(obj["UUID"]?.ToString() ?? string.Empty);
                break;
            }

            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (var obj in searcher.Get())
            {
                hardwareInfo.AppendLine(obj["SerialNumber"]?.ToString() ?? string.Empty);
                break;
            }

            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE Index = 0");
            foreach (var obj in searcher.Get())
            {
                hardwareInfo.AppendLine(obj["SerialNumber"]?.ToString() ?? string.Empty);
                break;
            }

            // 对于不支持WMI的环境，使用其他方式获取硬件信息
            if (hardwareInfo.Length == 0)
            {
                // 使用环境变量和系统信息
                hardwareInfo.AppendLine(Environment.MachineName);
                hardwareInfo.AppendLine(Environment.OSVersion.ToString());
                hardwareInfo.AppendLine(Environment.ProcessorCount.ToString());
                hardwareInfo.AppendLine(Environment.Version.ToString());
                hardwareInfo.AppendLine(Environment.UserName);
            }
        }
        catch (Exception ex)
        {
            // 异常情况下使用系统信息作为备选
            hardwareInfo.AppendLine(Environment.MachineName);
            hardwareInfo.AppendLine(Environment.OSVersion.ToString());
            hardwareInfo.AppendLine(Environment.ProcessorCount.ToString());
            hardwareInfo.AppendLine(Environment.Version.ToString());
            hardwareInfo.AppendLine(Environment.UserName);
        }

        _hardwareInfo = hardwareInfo.ToString();
        return _hardwareInfo;
    }
}
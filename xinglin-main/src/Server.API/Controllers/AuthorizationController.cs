using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xinglin.Core.Interfaces;
using Xinglin.Core.Models;
using Xinglin.Core.Models.Authorization;
using Xinglin.Infrastructure.Data;

namespace Xinglin.Server.API.Controllers
{
    /// <summary>
    /// 授权服务控制器
    /// 用于管理软件的授权状态、心跳校验和权限控制
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly CloudDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public AuthorizationController(CloudDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 激活软件
        /// </summary>
        /// <param name="request">激活请求</param>
        /// <returns>激活结果</returns>
        [HttpPost("activate")]
        public async Task<IActionResult> ActivateSoftware([FromBody] ActivationRequest request)
        {
            try
            {
                // 验证激活码
                var activationCode = await _dbContext.ActivationCodes
                    .FirstOrDefaultAsync(ac => ac.Code == request.ActivationCode && ac.Status == "未使用");

                if (activationCode == null)
                {
                    return BadRequest(new { Message = "无效的激活码" });
                }

                // 检查设备数量限制
                if (activationCode.UsedDeviceCount >= activationCode.DeviceCount)
                {
                    return BadRequest(new { Message = "激活码已达到设备数量限制" });
                }

                // 创建机器码记录
                var machineCode = new MachineCode
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = request.MachineCode,
                    HospitalId = activationCode.HospitalId,
                    DeviceName = request.DeviceName,
                    DeviceType = request.DeviceType,
                    OperatingSystem = request.OperatingSystem,
                    FirstRegisterTime = DateTime.Now,
                    LastHeartbeatTime = DateTime.Now,
                    Status = "活跃"
                };

                // 更新激活码状态
                activationCode.UsedDeviceCount++;
                if (activationCode.UsedDeviceCount >= activationCode.DeviceCount)
                {
                    activationCode.Status = "已使用";
                }

                // 保存数据
                await _dbContext.MachineCodes.AddAsync(machineCode);
                await _dbContext.SaveChangesAsync();

                // 获取医院信息
                var hospital = await _dbContext.Hospitals.FindAsync(activationCode.HospitalId);

                return Ok(new AuthorizationResult
                {
                    Success = true,
                    Message = "软件激活成功",
                    ExpiryDate = activationCode.EndTime,
                    HospitalId = activationCode.HospitalId,
                    HospitalName = hospital?.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"软件激活失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <param name="request">心跳请求</param>
        /// <returns>心跳结果</returns>
        [HttpPost("heartbeat")]
        public async Task<IActionResult> SendHeartbeat([FromBody] HeartbeatRequest request)
        {
            try
            {
                // 查询机器码
                var machineCode = await _dbContext.MachineCodes
                    .FirstOrDefaultAsync(mc => mc.Code == request.MachineCode);

                if (machineCode == null)
                {
                    return BadRequest(new { Message = "无效的机器码" });
                }

                // 检查授权是否过期
                var activationCode = await _dbContext.ActivationCodes
                    .FirstOrDefaultAsync(ac => ac.HospitalId == machineCode.HospitalId && ac.Status != "已禁用");

                if (activationCode == null || DateTime.Now > activationCode.EndTime)
                {
                    machineCode.Status = "过期";
                    await _dbContext.SaveChangesAsync();
                    return BadRequest(new { Message = "授权已过期" });
                }

                // 更新心跳时间
                machineCode.LastHeartbeatTime = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                // 检查模板是否需要更新
                var latestTemplate = await _dbContext.ReportTemplates
                    .Where(t => t.HospitalId == machineCode.HospitalId || t.HospitalId == null)
                    .OrderByDescending(t => t.CreateTime)
                    .FirstOrDefaultAsync();

                bool needUpdateTemplate = false;
                bool isForceUpdateTemplate = false;

                if (latestTemplate != null && latestTemplate.Version != request.CurrentTemplateVersion)
                {
                    needUpdateTemplate = true;
                    isForceUpdateTemplate = latestTemplate.IsForceUpdate;
                }

                // 获取医院权限
                var permissions = await GetHospitalPermissions(machineCode.HospitalId);

                return Ok(new HeartbeatResult
                {
                    IsAuthorized = true,
                    Message = "心跳成功",
                    NeedUpdateTemplate = needUpdateTemplate,
                    IsForceUpdateTemplate = isForceUpdateTemplate,
                    LatestTemplate = latestTemplate,
                    Permissions = permissions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"心跳失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取医院可用模板列表
        /// </summary>
        /// <param name="machineCode">机器码</param>
        /// <returns>模板列表</returns>
        [HttpGet("templates/{machineCode}")]
        public async Task<IActionResult> GetAvailableTemplates(string machineCode)
        {
            try
            {
                // 查询机器码
                var machineCodeEntity = await _dbContext.MachineCodes
                    .FirstOrDefaultAsync(mc => mc.Code == machineCode);

                if (machineCodeEntity == null)
                {
                    return BadRequest(new { Message = "无效的机器码" });
                }

                // 获取医院可用模板
                var templates = await _dbContext.ReportTemplates
                    .Where(t => t.HospitalId == machineCodeEntity.HospitalId || t.HospitalId == null)
                    .OrderByDescending(t => t.CreateTime)
                    .ToListAsync();

                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"获取模板列表失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取医院权限列表
        /// </summary>
        /// <param name="hospitalId">医院ID</param>
        /// <returns>权限列表</returns>
        private async Task<Dictionary<string, bool>> GetHospitalPermissions(string hospitalId)
        {
            // 这里应该从数据库获取医院的权限配置
            // 为了演示，我们返回模拟数据
            return new Dictionary<string, bool>
            {
                { "Print", true },
                { "ExportPDF", true },
                { "ModifyReportedReport", false },
                { "ViewHistoryReports", true },
                { "ManageTemplates", false }
            };
        }
    }

    /// <summary>
    /// 激活请求
    /// </summary>
    public class ActivationRequest
    {
        /// <summary>
        /// 机器码
        /// </summary>
        public string MachineCode { get; set; }

        /// <summary>
        /// 激活码
        /// </summary>
        public string ActivationCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        public string OperatingSystem { get; set; }
    }

    /// <summary>
    /// 心跳请求
    /// </summary>
    public class HeartbeatRequest
    {
        /// <summary>
        /// 机器码
        /// </summary>
        public string MachineCode { get; set; }

        /// <summary>
        /// 当前模板版本
        /// </summary>
        public string CurrentTemplateVersion { get; set; }
    }
}
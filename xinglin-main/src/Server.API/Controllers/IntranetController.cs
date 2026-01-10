using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xinglin.Core.Models;
using Xinglin.Infrastructure.Data;

namespace Xinglin.Server.API.Controllers
{
    /// <summary>
    /// 内网服务控制器
    /// 用于报告归档和安全查询
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IntranetController : ControllerBase
    {
        private readonly IntranetDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public IntranetController(IntranetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 归档报告
        /// </summary>
        /// <param name="report">报告信息</param>
        /// <returns>归档结果，包含报告编号</returns>
        [HttpPost("archive-report")]
        public async Task<IActionResult> ArchiveReport([FromBody] Report report)
        {
            try
            {
                // 生成报告编号
                string reportNumber = GenerateReportNumber();
                report.ReportNumber = reportNumber;

                // 保存报告
                await _dbContext.Reports.AddAsync(report);
                await _dbContext.SaveChangesAsync();

                return Ok(new { ReportNumber = reportNumber, Message = "报告归档成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"报告归档失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 根据报告编号查询报告
        /// </summary>
        /// <param name="reportNumber">报告编号</param>
        /// <returns>报告信息</returns>
        [HttpGet("report/{reportNumber}")]
        public async Task<IActionResult> GetReportByNumber(string reportNumber)
        {
            try
            {
                var report = await _dbContext.Reports
                    .Include(r => r.ReportItems)
                    .FirstOrDefaultAsync(r => r.ReportNumber == reportNumber);

                if (report == null)
                {
                    return NotFound(new { Message = "报告不存在" });
                }

                // 脱敏处理，隐藏内部敏感信息
                var desensitizedReport = new
                {
                    report.ReportNumber,
                    report.Patient,
                    report.ReportItems,
                    report.TemplateId,
                    report.TemplateVersion,
                    report.ReportType,
                    report.EntryTime,
                    report.PrintTime,
                    report.Status
                };

                // 记录查询日志
                await LogReportQuery(reportNumber, Request.HttpContext.Connection.RemoteIpAddress?.ToString());

                return Ok(desensitizedReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"查询报告失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 根据病人ID查询报告列表
        /// </summary>
        /// <param name="patientId">病人ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>报告列表</returns>
        [HttpGet("reports/patient/{patientId}")]
        public async Task<IActionResult> GetReportsByPatientId(string patientId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var query = _dbContext.Reports
                    .Where(r => r.Patient.Id == patientId);

                if (startDate.HasValue)
                {
                    query = query.Where(r => r.EntryTime >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(r => r.EntryTime <= endDate.Value);
                }

                var reports = await query
                    .Include(r => r.ReportItems)
                    .ToListAsync();

                // 脱敏处理
                var desensitizedReports = reports.Select(r => new
                {
                    r.ReportNumber,
                    r.Patient,
                    r.ReportType,
                    r.EntryTime,
                    r.Status
                });

                return Ok(desensitizedReports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"查询报告列表失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 生成报告编号
        /// 格式：LAB+当前日期+序号
        /// 示例：LAB20260110-00123
        /// </summary>
        /// <returns>报告编号</returns>
        private string GenerateReportNumber()
        {
            string datePrefix = DateTime.Now.ToString("yyyyMMdd");
            string serialNumber = "00001";

            // 查询当天最后一个报告编号
            var lastReport = _dbContext.Reports
                .Where(r => r.ReportNumber.StartsWith($"LAB{datePrefix}"))
                .OrderByDescending(r => r.ReportNumber)
                .FirstOrDefault();

            if (lastReport != null)
            {
                string lastSerial = lastReport.ReportNumber.Split('-')[1];
                int.TryParse(lastSerial, out int serial);
                serialNumber = (serial + 1).ToString("D5");
            }

            return $"LAB{datePrefix}-{serialNumber}";
        }

        /// <summary>
        /// 记录报告查询日志
        /// </summary>
        /// <param name="reportNumber">报告编号</param>
        /// <param name="ipAddress">IP地址</param>
        private async Task LogReportQuery(string reportNumber, string ipAddress)
        {
            var log = new ReportQueryLog
            {
                Id = Guid.NewGuid().ToString(),
                ReportNumber = reportNumber,
                QueryIdentifier = ipAddress,
                QueryTime = DateTime.Now,
                QueryMethod = "API",
                QueryResult = "成功",
                IpAddress = ipAddress,
                BrowserInfo = Request.Headers.UserAgent.ToString()
            };

            await _dbContext.ReportQueryLogs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
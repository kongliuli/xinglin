using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Xinglin.Server.API.Controllers
{
    /// <summary>
    /// 外网服务控制器
    /// 用于对接微信OAuth，转发查询请求，生成HTML报告
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WechatController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="httpClientFactory">HTTP客户端工厂</param>
        public WechatController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// 微信登录授权
        /// </summary>
        /// <param name="code">微信授权码</param>
        /// <returns>登录结果</returns>
        [HttpGet("login")]
        public async Task<IActionResult> WechatLogin(string code)
        {
            try
            {
                // 这里应该调用微信API获取openid和session_key
                // 为了演示，我们返回模拟数据
                var result = new
                {
                    OpenId = Guid.NewGuid().ToString(),
                    SessionKey = "mock_session_key",
                    Token = Guid.NewGuid().ToString()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"微信登录失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 查询报告
        /// </summary>
        /// <param name="request">查询请求</param>
        /// <returns>报告信息</returns>
        [HttpPost("query-report")]
        public async Task<IActionResult> QueryReport([FromBody] ReportQueryRequest request)
        {
            try
            {
                // 验证身份
                if (string.IsNullOrEmpty(request.OpenId) && string.IsNullOrEmpty(request.Token))
                {
                    return Unauthorized(new { Message = "身份验证失败" });
                }

                // 调用内网服务查询报告
                string intranetApiUrl = "https://intranet-server/api/intranet/report/" + request.ReportNumber;
                var response = await _httpClient.GetAsync(intranetApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var reportJson = await response.Content.ReadAsStringAsync();
                    var report = JsonSerializer.Deserialize<JsonElement>(reportJson);

                    // 渲染HTML报告
                    string htmlReport = RenderHtmlReport(report);

                    return Ok(new { Report = report, HtmlReport = htmlReport });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { Message = $"查询报告失败: {error}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"查询报告失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 根据身份证号和姓名查询报告列表
        /// </summary>
        /// <param name="request">查询请求</param>
        /// <returns>报告列表</returns>
        [HttpPost("query-reports-by-idcard")]
        public async Task<IActionResult> QueryReportsByIdCard([FromBody] IdCardReportQueryRequest request)
        {
            try
            {
                // 验证身份
                if (string.IsNullOrEmpty(request.OpenId) && string.IsNullOrEmpty(request.Token))
                {
                    return Unauthorized(new { Message = "身份验证失败" });
                }

                // 调用内网服务查询报告列表
                string intranetApiUrl = "https://intranet-server/api/intranet/reports/by-idcard";
                var response = await _httpClient.PostAsJsonAsync(intranetApiUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    var reports = await response.Content.ReadFromJsonAsync<JsonElement>();
                    return Ok(reports);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { Message = $"查询报告列表失败: {error}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"查询报告列表失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 渲染HTML报告
        /// </summary>
        /// <param name="report">报告数据</param>
        /// <returns>HTML报告</returns>
        private string RenderHtmlReport(JsonElement report)
        {
            // 使用StringBuilder来构建HTML报告，避免直接使用带有大量双引号的字符串
            var htmlBuilder = new System.Text.StringBuilder();

            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine("    <title>病理检验报告</title>");
            htmlBuilder.AppendLine("    <style>");
            htmlBuilder.AppendLine("        body { font-family: Arial, sans-serif; margin: 20px; }");
            htmlBuilder.AppendLine("        .report-header { text-align: center; border-bottom: 2px solid #000; padding-bottom: 10px; margin-bottom: 20px; }");
            htmlBuilder.AppendLine("        .report-title { font-size: 24px; font-weight: bold; }");
            htmlBuilder.AppendLine("        .patient-info { margin-bottom: 20px; }");
            htmlBuilder.AppendLine("        .patient-info table { width: 100%; border-collapse: collapse; }");
            htmlBuilder.AppendLine("        .patient-info th, .patient-info td { border: 1px solid #ccc; padding: 8px; text-align: left; }");
            htmlBuilder.AppendLine("        .report-items { margin-top: 20px; }");
            htmlBuilder.AppendLine("        .report-items table { width: 100%; border-collapse: collapse; }");
            htmlBuilder.AppendLine("        .report-items th, .report-items td { border: 1px solid #ccc; padding: 8px; text-align: left; }");
            htmlBuilder.AppendLine("        .report-footer { margin-top: 20px; text-align: right; }");
            htmlBuilder.AppendLine("    </style>");
            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            htmlBuilder.AppendLine("    <div class='report-header'>");
            htmlBuilder.AppendLine("        <div class='report-title'>病理检验报告</div>");
            htmlBuilder.AppendLine($"        <div>报告编号: {report.GetProperty("ReportNumber").GetString()}</div>");
            htmlBuilder.AppendLine($"        <div>报告日期: {report.GetProperty("EntryTime").GetDateTime().ToString("yyyy-MM-dd HH:mm:ss")}</div>");
            htmlBuilder.AppendLine("    </div>");
            htmlBuilder.AppendLine();
            htmlBuilder.AppendLine("    <div class='patient-info'>");
            htmlBuilder.AppendLine("        <h3>病人信息</h3>");
            htmlBuilder.AppendLine("        <table>");
            htmlBuilder.AppendLine("            <tr>");
            htmlBuilder.AppendLine($"                <th>姓名</th>");
            htmlBuilder.AppendLine($"                <td>{report.GetProperty("Patient").GetProperty("Name").GetString()}</td>");
            htmlBuilder.AppendLine($"                <th>性别</th>");
            htmlBuilder.AppendLine($"                <td>{report.GetProperty("Patient").GetProperty("Gender").GetString()}</td>");
            htmlBuilder.AppendLine($"                <th>年龄</th>");
            htmlBuilder.AppendLine($"                <td>{report.GetProperty("Patient").GetProperty("Age").GetInt32()}</td>");
            htmlBuilder.AppendLine("            </tr>");
            htmlBuilder.AppendLine("            <tr>");
            htmlBuilder.AppendLine($"                <th>身份证号</th>");
            htmlBuilder.AppendLine($"                <td>{report.GetProperty("Patient").GetProperty("IdCardNumber").GetString()}</td>");
            htmlBuilder.AppendLine($"                <th>联系方式</th>");
            htmlBuilder.AppendLine($"                <td>{report.GetProperty("Patient").GetProperty("ContactNumber").GetString()}</td>");
            htmlBuilder.AppendLine($"                <th>科室</th>");
            htmlBuilder.AppendLine($"                <td>{report.GetProperty("Patient").GetProperty("Department").GetString()}</td>");
            htmlBuilder.AppendLine("            </tr>");
            htmlBuilder.AppendLine("        </table>");
            htmlBuilder.AppendLine("    </div>");
            htmlBuilder.AppendLine();
            htmlBuilder.AppendLine("    <div class='report-items'>");
            htmlBuilder.AppendLine("        <h3>检验项目</h3>");
            htmlBuilder.AppendLine("        <table>");
            htmlBuilder.AppendLine("            <thead>");
            htmlBuilder.AppendLine("                <tr>");
            htmlBuilder.AppendLine("                    <th>项目名称</th>");
            htmlBuilder.AppendLine("                    <th>项目值</th>");
            htmlBuilder.AppendLine("                    <th>参考范围</th>");
            htmlBuilder.AppendLine("                    <th>单位</th>");
            htmlBuilder.AppendLine("                    <th>结果状态</th>");
            htmlBuilder.AppendLine("                </tr>");
            htmlBuilder.AppendLine("            </thead>");
            htmlBuilder.AppendLine("            <tbody>");
            htmlBuilder.AppendLine(RenderReportItems(report.GetProperty("ReportItems")));
            htmlBuilder.AppendLine("            </tbody>");
            htmlBuilder.AppendLine("        </table>");
            htmlBuilder.AppendLine("    </div>");
            htmlBuilder.AppendLine();
            htmlBuilder.AppendLine("    <div class='report-footer'>");
            htmlBuilder.AppendLine($"        <div>报告状态: {report.GetProperty("Status").GetString()}</div>");
            htmlBuilder.AppendLine("    </div>");
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

            return htmlBuilder.ToString();
        }

        /// <summary>
        /// 渲染报告项目
        /// </summary>
        /// <param name="reportItems">报告项目数组</param>
        /// <returns>HTML报告项目行</returns>
        private string RenderReportItems(JsonElement reportItems)
        {
            var html = new System.Text.StringBuilder();

            foreach (var item in reportItems.EnumerateArray())
            {
                html.AppendLine("                <tr>");
                html.AppendLine($"                    <td>{item.GetProperty("Name").GetString()}</td>");
                html.AppendLine($"                    <td>{item.GetProperty("Value").GetString()}</td>");
                html.AppendLine($"                    <td>{item.GetProperty("ReferenceRange").GetString()}</td>");
                html.AppendLine($"                    <td>{item.GetProperty("Unit").GetString()}</td>");
                html.AppendLine($"                    <td>{item.GetProperty("Status").GetString()}</td>");
                html.AppendLine("                </tr>");
            }

            return html.ToString();
        }
    }

    /// <summary>
    /// 报告查询请求
    /// </summary>
    public class ReportQueryRequest
    {
        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 登录令牌
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 报告编号
        /// </summary>
        public string ReportNumber { get; set; }
    }

    /// <summary>
    /// 身份证报告查询请求
    /// </summary>
    public class IdCardReportQueryRequest
    {
        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 登录令牌
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCardNumber { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xinglin.Core.Services;

// 报告服务接口
public interface IReportService
{
    Task<Guid> CreateReportAsync(ReportDto report);
    Task<ReportDto> GetReportAsync(Guid reportId);
    Task<Guid> ArchiveReportAsync(Guid reportId);
    Task<byte[]> GenerateReportPreviewAsync(Guid reportId);
    Task<IEnumerable<ReportDto>> SearchReportsAsync(ReportSearchCriteria criteria);
}

// 报告数据传输对象
public class ReportDto
{
    public Guid ReportId { get; set; }
    public string ReportNumber { get; set; } = string.Empty;
    public PatientDto Patient { get; set; } = new PatientDto();
    public Guid TemplateId { get; set; }
    public Dictionary<string, object> Content { get; set; } = new Dictionary<string, object>();
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
}

// 患者数据传输对象
public class PatientDto
{
    public Guid PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }
    public string IdCard { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string HospitalId { get; set; } = string.Empty;
}

// 报告搜索条件
public class ReportSearchCriteria
{
    public string? ReportNumber { get; set; }
    public string? PatientName { get; set; }
    public string? IdCard { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
}

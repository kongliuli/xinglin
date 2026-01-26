using Microsoft.Extensions.DependencyInjection;
using Xinglin.Core.Communication;
using Xinglin.Core.Rendering;
using Xinglin.Core.Services;
using Xinglin.Infrastructure.Communication;
using Xinglin.Infrastructure.Rendering;
using Xinglin.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 注册核心服务
builder.Services.AddScoped<ICommunicationProxy, HttpCommunicationProxy>();
builder.Services.AddScoped<ITemplateRenderer, PdfSharpTemplateRenderer>();
builder.Services.AddScoped<ITemplateSerializer, JsonTemplateSerializer>();
builder.Services.AddScoped<ITemplateService, TemplateSerializationService>();

// 配置API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 开发环境配置
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 健康检查API
app.MapGet("/api/health", () =>
{
    return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
})
.WithName("GetHealth")
.WithOpenApi();

// 系统管理API
app.MapGet("/api/system/config", () =>
{
    // TODO: 实现获取系统配置
    return Results.Ok(new { message = "System configuration endpoint" });
})
.WithName("GetSystemConfig")
.WithOpenApi();

app.MapPut("/api/system/config", () =>
{
    // TODO: 实现更新系统配置
    return Results.Ok(new { message = "System configuration updated" });
})
.WithName("UpdateSystemConfig")
.WithOpenApi();

app.MapGet("/api/system/modules", () =>
{
    // TODO: 实现获取系统模块列表
    return Results.Ok(new { message = "System modules endpoint" });
})
.WithName("GetSystemModules")
.WithOpenApi();

app.MapPut("/api/system/modules/{id}/status", (string id, bool enabled) =>
{
    // TODO: 实现更新系统模块状态
    return Results.Ok(new { message = $"Module {id} status updated to {enabled}" });
})
.WithName("UpdateModuleStatus")
.WithOpenApi();

// 权限管理API
app.MapGet("/api/permissions/users", () =>
{
    // TODO: 实现获取用户列表
    return Results.Ok(new { message = "Users endpoint" });
})
.WithName("GetUsers")
.WithOpenApi();

app.MapGet("/api/permissions/roles", () =>
{
    // TODO: 实现获取角色列表
    return Results.Ok(new { message = "Roles endpoint" });
})
.WithName("GetRoles")
.WithOpenApi();

app.MapPost("/api/permissions/assign", () =>
{
    // TODO: 实现分配权限
    return Results.Ok(new { message = "Permission assigned" });
})
.WithName("AssignPermission")
.WithOpenApi();

// 日志管理API
app.MapGet("/api/logs", () =>
{
    // TODO: 实现获取日志列表
    return Results.Ok(new { message = "Logs endpoint" });
})
.WithName("GetLogs")
.WithOpenApi();

app.MapGet("/api/logs/search", () =>
{
    // TODO: 实现搜索日志
    return Results.Ok(new { message = "Logs search endpoint" });
})
.WithName("SearchLogs")
.WithOpenApi();

app.Run();

// 数据传输对象
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

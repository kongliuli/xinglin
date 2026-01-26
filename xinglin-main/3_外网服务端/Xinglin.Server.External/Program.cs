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

// 远程模板访问API
app.MapGet("/api/remote/templates", async (ITemplateService templateService) =>
{
    var templates = await templateService.GetTemplatesAsync();
    return Results.Ok(templates);
})
.WithName("GetRemoteTemplates")
.WithOpenApi();

app.MapGet("/api/remote/templates/{id}", async (string id, ITemplateService templateService) =>
{
    if (!Guid.TryParse(id, out var templateId))
    {
        return Results.BadRequest("Invalid template ID format");
    }
    
    var template = await templateService.GetTemplateAsync(templateId);
    if (template == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(template);
})
.WithName("GetRemoteTemplate")
.WithOpenApi();

app.MapGet("/api/remote/templates/latest/{name}", async (string name, ITemplateService templateService) =>
{
    if (string.IsNullOrEmpty(name))
    {
        return Results.BadRequest("Template name cannot be empty");
    }
    
    var template = await templateService.GetLatestTemplateAsync(name);
    if (template == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(template);
})
.WithName("GetLatestRemoteTemplate")
.WithOpenApi();

// 数据同步API
app.MapPost("/api/sync/templates", async (IEnumerable<TemplateDto> templates, ITemplateService templateService) =>
{
    if (templates == null || !templates.Any())
    {
        return Results.BadRequest("No templates provided for synchronization");
    }
    
    var synchronizedIds = new List<Guid>();
    
    foreach (var template in templates)
    {
        var savedId = await templateService.SaveTemplateAsync(template);
        synchronizedIds.Add(savedId);
    }
    
    return Results.Ok(new { synchronizedCount = synchronizedIds.Count, templateIds = synchronizedIds });
})
.WithName("SyncTemplates")
.WithOpenApi();

// 授权验证API
app.MapPost("/api/auth/validate", (string apiKey) =>
{
    // TODO: 实现授权验证逻辑
    return Results.Ok(new { isValid = true, message = "API key validated successfully" });
})
.WithName("ValidateApiKey")
.WithOpenApi();

app.Run();

// 数据传输对象
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

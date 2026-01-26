using Microsoft.AspNetCore.Mvc;
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

// 模板管理API
app.MapGet("/api/templates", async (ITemplateService templateService) =>
{
    var templates = await templateService.GetTemplatesAsync();
    return Results.Ok(templates);
})
.WithName("GetTemplates")
.WithOpenApi();

app.MapGet("/api/templates/{id}", async (string id, ITemplateService templateService) =>
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
.WithName("GetTemplate")
.WithOpenApi();

app.MapPost("/api/templates", async (TemplateDto template, ITemplateService templateService) =>
{
    if (template == null)
    {
        return Results.BadRequest("Template data cannot be null");
    }
    
    var templateId = await templateService.SaveTemplateAsync(template);
    return Results.Created($"/api/templates/{templateId}", new { TemplateId = templateId });
})
.WithName("CreateTemplate")
.WithOpenApi();

app.MapPut("/api/templates/{id}", async (string id, TemplateDto template, ITemplateService templateService) =>
{
    if (!Guid.TryParse(id, out var templateId))
    {
        return Results.BadRequest("Invalid template ID format");
    }
    
    if (template == null)
    {
        return Results.BadRequest("Template data cannot be null");
    }
    
    template.TemplateId = templateId;
    var savedId = await templateService.SaveTemplateAsync(template);
    return Results.Ok(new { TemplateId = savedId });
})
.WithName("UpdateTemplate")
.WithOpenApi();

app.MapDelete("/api/templates/{id}", async (string id, ITemplateService templateService) =>
{
    if (!Guid.TryParse(id, out var templateId))
    {
        return Results.BadRequest("Invalid template ID format");
    }
    
    var result = await templateService.DeleteTemplateAsync(templateId);
    if (result)
    {
        return Results.Ok();
    }
    
    return Results.NotFound();
})
.WithName("DeleteTemplate")
.WithOpenApi();

// 模板版本管理API
app.MapGet("/api/templates/{id}/versions", async (string id, ITemplateService templateService) =>
{
    if (!Guid.TryParse(id, out var templateId))
    {
        return Results.BadRequest("Invalid template ID format");
    }
    
    var versions = await templateService.GetTemplateVersionsAsync(templateId);
    return Results.Ok(versions);
})
.WithName("GetTemplateVersions")
.WithOpenApi();

// 报表生成API
app.MapPost("/api/reports/generate", (ITemplateRenderer renderer) =>
{
    // TODO: 实现报表生成
    return Results.Ok(new { message = "Report generation endpoint", timestamp = DateTime.UtcNow });
})
.WithName("GenerateReport")
.WithOpenApi();

app.Run();

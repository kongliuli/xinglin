using Microsoft.EntityFrameworkCore;
using Xinglin.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 配置CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWechat", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 添加数据库上下文
builder.Services.AddDbContext<IntranetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IntranetDb")));

builder.Services.AddDbContext<CloudDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CloudDb")));

// 添加控制器
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowWechat");
app.UseAuthorization();

// 路由配置
app.MapControllers();

app.Run();

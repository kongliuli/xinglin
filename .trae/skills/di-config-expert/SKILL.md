---
name: di-config-expert
description: "C# DI配置专家技能：负责Startup/Program.cs服务注册、生命周期管理(Scoped/Transient/Singleton)、工厂模式注册、第三方容器集成。使用时需要配置依赖注入、管理服务生命周期、使用工厂模式注册服务、集成第三方DI容器。"
---

# di-config-expert Skill

为C#项目提供专业的依赖注入(DI)配置和管理能力，确保项目具有清晰、可维护的服务注册和生命周期管理。

## When to Use This Skill

Trigger when any of these applies:
- 需要在Startup/Program.cs中注册服务
- 需要管理服务生命周期(Scoped/Transient/Singleton)
- 需要使用工厂模式注册服务
- 需要集成第三方DI容器
- 需要优化现有DI配置
- 需要解决服务解析问题

## Not For / Boundaries

- 不负责具体业务逻辑的实现
- 不替代开发人员进行服务设计
- 不处理DI容器的高级配置（仅通用配置）
- 不负责DI容器的安装和基础设置

## Quick Reference

### Common Patterns

**Pattern 1:** 基本服务注册
```csharp
// 在Program.cs或Startup.ConfigureServices中
builder.Services.AddTransient<IMyService, MyService>();
builder.Services.AddScoped<IAnotherService, AnotherService>();
builder.Services.AddSingleton<IThirdService, ThirdService>();
```

**Pattern 2:** 工厂模式注册
```csharp
// 使用工厂方法注册服务
builder.Services.AddTransient<IMyService>(provider => {
    var dependency = provider.GetRequiredService<IDependency>();
    return new MyService(dependency, "custom parameter");
});

// 使用工厂类注册服务
builder.Services.AddSingleton<MyServiceFactory>();
builder.Services.AddTransient<IMyService>(provider => {
    var factory = provider.GetRequiredService<MyServiceFactory>();
    return factory.CreateService();
});
```

**Pattern 3:** 服务生命周期
```csharp
// Transient - 每次请求时创建新实例
builder.Services.AddTransient<ITransientService, TransientService>();

// Scoped - 每个请求范围创建一个实例
builder.Services.AddScoped<IScopedService, ScopedService>();

// Singleton - 应用程序生命周期内只创建一个实例
builder.Services.AddSingleton<ISingletonService, SingletonService>();
```

**Pattern 4:** 第三方容器集成 (Autofac)
```csharp
// 安装NuGet包: Autofac.Extensions.DependencyInjection

// 在Program.cs中
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder => {
    builder.RegisterType<MyService>().As<IMyService>();
    builder.RegisterType<AnotherService>().As<IAnotherService>().InstancePerLifetimeScope();
});
```

**Pattern 5:** 批量注册
```csharp
// 注册同一程序集中的所有服务
builder.Services.Scan(scan => scan
    .FromAssemblyOf<IMyService>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithTransientLifetime());
```

## Examples

### Example 1: 基本服务注册
- Input: 需要注册几个基本服务及其实现
- Steps:
  1. 在Program.cs或Startup.ConfigureServices中添加服务注册
  2. 为不同的服务选择合适的生命周期
  3. 确保接口和实现正确映射
- Expected output / acceptance: 服务可以通过构造函数注入使用

### Example 2: 使用工厂模式注册服务
- Input: 需要使用工厂模式注册一个需要自定义参数的服务
- Steps:
  1. 创建工厂方法或工厂类
  2. 使用AddTransient/AddScoped/AddSingleton的工厂重载
  3. 在工厂中解析依赖并创建服务实例
- Expected output / acceptance: 服务可以通过工厂方法正确创建并使用

### Example 3: 集成Autofac容器
- Input: 需要集成Autofac作为DI容器
- Steps:
  1. 安装Autofac.Extensions.DependencyInjection包
  2. 配置UseServiceProviderFactory
  3. 使用ConfigureContainer<ContainerBuilder>配置Autofac
  4. 使用Autofac的注册语法注册服务
- Expected output / acceptance: Autofac容器正确集成并管理服务生命周期

## References

- `references/index.md`: DI配置最佳实践导航
- `references/service-lifetimes.md`: 服务生命周期详解
- `references/factory-pattern.md`: 工厂模式注册指南
- `references/third-party-containers.md`: 第三方容器集成指南

## Maintenance

- Sources: Microsoft官方文档和行业最佳实践
- Last updated: 2026-01-20
- Known limits: 不支持复杂的DI容器高级配置
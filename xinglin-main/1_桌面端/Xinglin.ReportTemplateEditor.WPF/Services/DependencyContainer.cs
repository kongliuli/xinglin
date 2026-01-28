using System;
using System.Collections.Generic;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 依赖注入容器
    /// </summary>
    public class DependencyContainer
    {
        private readonly Dictionary<Type, Func<object>> _registrations;
        private readonly Dictionary<Type, object> _singletons;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DependencyContainer()
        {
            _registrations = new Dictionary<Type, Func<object>>();
            _singletons = new Dictionary<Type, object>();
            RegisterDefaults();
        }

        /// <summary>
        /// 注册默认服务
        /// </summary>
        private void RegisterDefaults()
        {
            // 注册单例服务
            RegisterSingleton<TemplateManager>(() => new TemplateManager());
            RegisterSingleton<CommandManagerService>(() => new CommandManagerService());
            RegisterSingleton<GlobalExceptionHandler>(() => new GlobalExceptionHandler());
            RegisterSingleton<ShortcutConfigService>(() => new ShortcutConfigService());
            RegisterSingleton<TemplateService>(() => new TemplateService());
            RegisterSingleton<InputDataService>(() => new InputDataService());
            RegisterSingleton<DragDropService>(() => new DragDropService());
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="factory">服务工厂</param>
        public void Register<T>(Func<object> factory)
        {
            _registrations[typeof(T)] = factory;
        }

        /// <summary>
        /// 注册单例服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="factory">服务工厂</param>
        public void RegisterSingleton<T>(Func<object> factory)
        {
            _registrations[typeof(T)] = () =>
            {
                if (!_singletons.ContainsKey(typeof(T)))
                {
                    _singletons[typeof(T)] = factory();
                }
                return _singletons[typeof(T)];
            };
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例</returns>
        public T Resolve<T>()
        {
            if (_registrations.TryGetValue(typeof(T), out var factory))
            {
                return (T)factory();
            }
            throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <returns>服务实例</returns>
        public object Resolve(Type type)
        {
            if (_registrations.TryGetValue(type, out var factory))
            {
                return factory();
            }
            throw new InvalidOperationException($"Service {type.Name} not registered");
        }
    }

    /// <summary>
    /// 依赖注入容器扩展
    /// </summary>
    public static class DependencyContainerExtensions
    {
        /// <summary>
        /// 解析服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="container">依赖注入容器</param>
        /// <returns>服务实例</returns>
        public static T GetService<T>(this DependencyContainer container)
        {
            return container.Resolve<T>();
        }
    }
}

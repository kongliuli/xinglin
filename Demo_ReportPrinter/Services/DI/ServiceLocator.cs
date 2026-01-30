using Demo_ReportPrinter.Services.Data;
using Demo_ReportPrinter.Services.Pdf;
using Demo_ReportPrinter.Services.Shared;
using Demo_ReportPrinter.Services.Validation;

namespace Demo_ReportPrinter.Services.DI
{
    /// <summary>
    /// 服务定位器，用于依赖注入
    /// </summary>
    public class ServiceLocator
    {
        private static readonly ServiceLocator _instance = new();
        private readonly Dictionary<Type, object> _services = new();

        public static ServiceLocator Instance => _instance;

        private ServiceLocator()
        {
            RegisterServices();
        }

        private void RegisterServices()
        {
            // 注册共享服务
            Register<ISharedDataService, SharedDataService>();
            Register<IMessageService, MessageService>();

            // 注册数据服务
            Register<ITemplateService, TemplateService>();

            // 注册PDF服务
            Register<IPdfService, PdfService>();

            // 注册验证服务
            Register<IValidationService, ValidationService>();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            var service = Activator.CreateInstance<TImplementation>();
            _services[typeof(TInterface)] = service;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        public T GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new InvalidOperationException($"服务 {typeof(T).Name} 未注册");
        }
    }
}

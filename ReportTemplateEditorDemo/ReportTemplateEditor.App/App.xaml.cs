using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using ReportTemplateEditor.App.Services;
using ReportTemplateEditor.App.ViewModels;
using ReportTemplateEditor.App.Views;
using ReportTemplateEditor.Core.Services;
using System.Windows;

using WpfApplication = System.Windows.Application;

namespace ReportTemplateEditor.App
{
    public partial class App : WpfApplication
    {
        public static ServiceProvider ServiceProvider { get; private set; } = null!;
        public static string BuiltInTemplatesPath { get; private set; } = string.Empty;
        public static string SharedDataPath { get; private set; } = string.Empty;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeBuiltInTemplatesPath();
            InitializeSharedDataPath();

            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            QuestPDF.Settings.License = LicenseType.Professional;
            var mainWindow = ServiceProvider.GetRequiredService<Views.MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITemplateLoaderService>(sp => new TemplateLoaderService(SharedDataPath));
            services.AddSingleton<IFileWatcherService, FileWatcherService>();
            services.AddSingleton<IControlGeneratorService, ControlGeneratorService>();
            services.AddSingleton<IPdfPreviewService, PdfPreviewService>();
            services.AddSingleton<IPrintService, PrintService>();
            services.AddSingleton<TemplateTreeViewModel>();
            services.AddSingleton<ControlPanelViewModel>();
            services.AddSingleton<PdfPreviewViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<Views.MainWindow>();
        }

        private void InitializeBuiltInTemplatesPath()
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory;
            BuiltInTemplatesPath = System.IO.Path.Combine(appBasePath, "Templates");
        }

        private void InitializeSharedDataPath()
        {
            var appBasePath = AppDomain.CurrentDomain.BaseDirectory;
            SharedDataPath = System.IO.Path.Combine(appBasePath, "SharedData");
        }
    }
}
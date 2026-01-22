using System.Configuration;
using System.Data;
using System.Windows;
using QuestPDF.Infrastructure;

namespace ReportTemplateEditor.Designer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static string SharedDataPath { get; private set; } = string.Empty;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        QuestPDF.Settings.License = LicenseType.Professional;
        InitializeSharedDataPath();
    }

    private void InitializeSharedDataPath()
    {
        var appBasePath = AppDomain.CurrentDomain.BaseDirectory;
        SharedDataPath = System.IO.Path.Combine(appBasePath, "SharedData");
    }
}


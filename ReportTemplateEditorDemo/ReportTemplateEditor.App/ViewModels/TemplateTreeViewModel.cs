using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportTemplateEditor.App.Models;
using ReportTemplateEditor.App.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ReportTemplateEditor.App.ViewModels
{
    public partial class TemplateTreeViewModel : ViewModelBase
    {
        private readonly ITemplateLoaderService _templateLoaderService;
        private readonly IFileWatcherService _fileWatcherService;

        [ObservableProperty]
        private ObservableCollection<TemplateTreeItem> _treeItems = new ObservableCollection<TemplateTreeItem>();

        [ObservableProperty]
        private TemplateTreeItem? _selectedItem;

        [ObservableProperty]
        private string _rootDirectory = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusMessage = "就绪";

        public ICommand LoadTemplatesCommand { get; }
        public ICommand LoadBuiltInTemplatesCommand { get; }
        public ICommand RefreshCommand { get; }

        public TemplateTreeViewModel(ITemplateLoaderService templateLoaderService, IFileWatcherService fileWatcherService)
        {
            _templateLoaderService = templateLoaderService;
            _fileWatcherService = fileWatcherService;

            LoadTemplatesCommand = new RelayCommand<string>(LoadTemplates);
            LoadBuiltInTemplatesCommand = new RelayCommand<string>(LoadBuiltInTemplates);
            RefreshCommand = new RelayCommand(Refresh);

            _fileWatcherService.FileChanged += OnFileChanged;
        }

        private void LoadTemplates(string? directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                directoryPath = RootDirectory;
            }

            if (string.IsNullOrEmpty(directoryPath) || !System.IO.Directory.Exists(directoryPath))
            {
                StatusMessage = "请选择有效的模板目录";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "正在加载模板...";

                RootDirectory = directoryPath;
                TreeItems = _templateLoaderService.LoadTemplatesFromDirectory(directoryPath);

                _fileWatcherService.StopWatching();
                _fileWatcherService.StartWatching(directoryPath);

                StatusMessage = $"已加载 {GetTemplateCount()} 个模板文件";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"加载失败: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void LoadBuiltInTemplates(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !System.IO.Directory.Exists(directoryPath))
            {
                StatusMessage = "内置模板目录不存在";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "正在加载内置模板...";

                RootDirectory = directoryPath;
                TreeItems = _templateLoaderService.LoadTemplatesFromDirectory(directoryPath);

                _fileWatcherService.StopWatching();
                _fileWatcherService.StartWatching(directoryPath);

                StatusMessage = $"已加载 {GetTemplateCount()} 个内置模板文件";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"加载内置模板失败: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Refresh()
        {
            if (!string.IsNullOrEmpty(RootDirectory))
            {
                LoadTemplates(RootDirectory);
            }
        }

        private void OnFileChanged(object? sender, FileChangedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Refresh();
                StatusMessage = $"检测到文件变更: {System.IO.Path.GetFileName(e.FilePath)}";
            });
        }

        private int GetTemplateCount()
        {
            int count = 0;
            foreach (var item in TreeItems)
            {
                count += CountTemplates(item);
            }
            return count;
        }

        private int CountTemplates(TemplateTreeItem item)
        {
            int count = 0;
            if (item.Type == TreeItemType.TemplateFile)
            {
                count++;
            }
            foreach (var child in item.Children)
            {
                count += CountTemplates(child);
            }
            return count;
        }
    }
}

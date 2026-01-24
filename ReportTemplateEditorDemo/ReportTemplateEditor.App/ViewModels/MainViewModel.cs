using CommunityToolkit.Mvvm.Input;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.App.Models;
using ReportTemplateEditor.App.Services;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ReportTemplateEditor.App.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly ITemplateLoaderService _templateLoaderService;
        private readonly TemplateService _templateService;

        public TemplateTreeViewModel TemplateTreeViewModel { get; }
        public ControlPanelViewModel ControlPanelViewModel { get; }
        public PdfPreviewViewModel PdfPreviewViewModel { get; }

        [ObservableProperty]
        private string _windowTitle = "报告模板编辑器";

        [ObservableProperty]
        private string _statusMessage = "就绪";

        [ObservableProperty]
        private bool _isBusy;

        public ICommand OpenOtherDirectoryCommand { get; }
        public ICommand OpenBuiltInTemplatesCommand { get; }
        public ICommand OpenDesignerCommand { get; }
        public ICommand LoadTemplateCommand { get; }
        public ICommand SaveTemplateCommand { get; }
        public ICommand ExitCommand { get; }

        public MainViewModel(
            ITemplateLoaderService templateLoaderService,
            TemplateService templateService,
            TemplateTreeViewModel templateTreeViewModel,
            ControlPanelViewModel controlPanelViewModel,
            PdfPreviewViewModel pdfPreviewViewModel)
        {
            _templateLoaderService = templateLoaderService;
            _templateService = templateService;
            TemplateTreeViewModel = templateTreeViewModel;
            ControlPanelViewModel = controlPanelViewModel;
            PdfPreviewViewModel = pdfPreviewViewModel;

            OpenOtherDirectoryCommand = new RelayCommand(OpenOtherDirectory);
            OpenBuiltInTemplatesCommand = new RelayCommand(OpenBuiltInTemplates);
            OpenDesignerCommand = new RelayCommand(OpenDesigner);
            LoadTemplateCommand = new RelayCommand<TemplateTreeItem>(LoadTemplateAsync);
            SaveTemplateCommand = new RelayCommand(SaveTemplate, CanExecuteSaveTemplate);
            ExitCommand = new RelayCommand(Exit);

            TemplateTreeViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TemplateTreeViewModel.SelectedItem))
                {
                    OnTemplateTreeSelectedItemChanged();
                }
            };

            InitializeBuiltInTemplates();
        }

        private void InitializeBuiltInTemplates()
        {
            if (!string.IsNullOrEmpty(App.BuiltInTemplatesPath) && System.IO.Directory.Exists(App.BuiltInTemplatesPath))
            {
                TemplateTreeViewModel.LoadBuiltInTemplatesCommand.Execute(App.BuiltInTemplatesPath);
            }
        }

        private void OpenOtherDirectory()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "选择模板文件目录",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TemplateTreeViewModel.LoadTemplatesCommand.Execute(dialog.SelectedPath);
                StatusMessage = $"已选择目录: {dialog.SelectedPath}";
            }
        }

        private void OpenBuiltInTemplates()
        {
            if (!string.IsNullOrEmpty(App.BuiltInTemplatesPath))
            {
                TemplateTreeViewModel.LoadBuiltInTemplatesCommand.Execute(App.BuiltInTemplatesPath);
                StatusMessage = $"已打开内置模板目录";
            }
        }

        private async void LoadTemplateAsync(TemplateTreeItem? item)
        {
            if (!ValidateTemplateItem(item))
            {
                return;
            }

            try
            {
                IsBusy = true;
                StatusMessage = $"正在加载模板: {item.Name}";
                System.Diagnostics.Debug.WriteLine($"开始加载模板: {item.Name}, 路径: {item.FullPath}");

                // 使用 TemplateService 加载模板
                var template = await _templateService.LoadTemplateAsync(item.FullPath);
                System.Diagnostics.Debug.WriteLine($"模板加载成功: {template.Name}, 包含 {template.Elements.Count} 个元素");
                WindowTitle = $"报告模板编辑器 - {template.Name}";

                // 创建并设置样本数据
                System.Diagnostics.Debug.WriteLine("开始创建样本数据");
                var data = CreateSampleDataForTemplate(template);
                System.Diagnostics.Debug.WriteLine($"样本数据创建成功");
                _templateService.SetCurrentData(data);

                // 手动更新所有视图模型
                System.Diagnostics.Debug.WriteLine("开始更新ControlPanelViewModel");
                ControlPanelViewModel.LoadTemplateCommand.Execute(template);
                ControlPanelViewModel.UpdateDataCommand.Execute(data);
                System.Diagnostics.Debug.WriteLine("ControlPanelViewModel更新成功");
                
                System.Diagnostics.Debug.WriteLine("开始更新PdfPreviewViewModel");
                PdfPreviewViewModel.LoadTemplateCommand.Execute(template);
                PdfPreviewViewModel.UpdateDataCommand.Execute(data);
                System.Diagnostics.Debug.WriteLine("PdfPreviewViewModel更新成功");

                StatusMessage = $"已加载模板: {template.Name}，包含 {template.Elements.Count} 个元素，路径: {item.FullPath}";
                System.Diagnostics.Debug.WriteLine($"模板加载完成: {template.Name}");
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"加载失败: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"加载模板失败: {item.Name}, 错误: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"堆栈跟踪: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"内部异常: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"内部异常堆栈跟踪: {ex.InnerException.StackTrace}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool ValidateTemplateItem([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] TemplateTreeItem? item)
        {
            if (item == null)
            {
                StatusMessage = "未选择模板文件";
                return false;
            }

            if (item.Type != TreeItemType.TemplateFile)
            {
                StatusMessage = $"选择了 {item.Type}，请选择模板文件";
                return false;
            }

            if (string.IsNullOrEmpty(item.FullPath))
            {
                StatusMessage = "模板文件路径为空";
                return false;
            }

            if (!System.IO.File.Exists(item.FullPath))
            {
                StatusMessage = $"模板文件不存在: {item.FullPath}";
                return false;
            }

            return true;
        }

        private async Task<ReportTemplateDefinition> LoadTemplateDataAsync(string filePath)
        {
            return await _templateLoaderService.LoadTemplateFromFileAsync(filePath);
        }

        private object CreateSampleDataForTemplate(ReportTemplateDefinition template)
        {
            var data = new System.Dynamic.ExpandoObject();
            var dataDict = (System.Collections.Generic.IDictionary<string, object?>)data;

            foreach (var element in template.Elements)
            {
                if (element is ReportTemplateEditor.Core.Models.Elements.TextElement textElement &&
                    !string.IsNullOrEmpty(textElement.DataBindingPath))
                {
                    if (!dataDict.ContainsKey(textElement.DataBindingPath))
                    {
                        dataDict[textElement.DataBindingPath] = textElement.Text;
                    }
                }
            }

            return data;
        }

        private void UpdateViewModelsWithTemplate(ReportTemplateDefinition template, object data)
        {
            ControlPanelViewModel.LoadTemplateCommand.Execute(template);
            ControlPanelViewModel.UpdateDataCommand.Execute(data);
            PdfPreviewViewModel.LoadTemplateCommand.Execute(template);
            PdfPreviewViewModel.UpdateDataCommand.Execute(data);
        }

        private void SaveTemplate()
        {
            var template = _templateService.CurrentTemplate;
            if (template == null)
            {
                StatusMessage = "没有可保存的模板";
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "模板文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                Title = "保存模板",
                FileName = string.IsNullOrEmpty(template.FilePath) 
                    ? $"{template.Name}.json" 
                    : System.IO.Path.GetFileName(template.FilePath),
                InitialDirectory = string.IsNullOrEmpty(template.FilePath)
                    ? App.SharedDataPath
                    : System.IO.Path.GetDirectoryName(template.FilePath)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    StatusMessage = $"正在保存模板: {template.Name}";

                    // 使用 TemplateService 保存模板
                    _templateService.SaveTemplate(template, saveFileDialog.FileName);
                    WindowTitle = $"报告模板编辑器 - {template.Name}";
                    StatusMessage = $"模板已保存到: {saveFileDialog.FileName}";
                }
                catch (System.Exception ex)
                {
                    StatusMessage = $"保存失败: {ex.Message}";
                    System.Diagnostics.Debug.WriteLine($"保存模板失败: {ex}");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private bool CanExecuteSaveTemplate()
        {
            return _templateService.CurrentTemplate != null;
        }

        private void Exit()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OpenDesigner()
        {
            try
            {
                string designerExePath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ReportTemplateEditor.Designer.exe"
                );

                if (System.IO.File.Exists(designerExePath))
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = designerExePath,
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(startInfo);
                    StatusMessage = "已启动设计器";
                }
                else
                {
                    var message = "设计器可执行文件不存在。请先构建ReportTemplateEditor.Designer项目。\n" +
                                 "构建步骤：\n" +
                                 "1. 在Visual Studio中打开解决方案\n" +
                                 "2. 右键点击ReportTemplateEditor.Designer项目\n" +
                                 "3. 选择'生成'或'重新生成'\n" +
                                 "4. 生成完成后再次尝试打开设计器";
                    StatusMessage = message;
                    System.Windows.MessageBox.Show(message, "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                var message = $"启动设计器失败: {ex.Message}\n请确保Designer项目已正确构建。";
                StatusMessage = message;
                System.Windows.MessageBox.Show(message, "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"启动设计器失败: {ex}");
            }
        }

        private void OnTemplateTreeSelectedItemChanged()
        {
            var selectedItem = TemplateTreeViewModel.SelectedItem;
            if (selectedItem != null)
            {
                System.Diagnostics.Debug.WriteLine($"选中项: {selectedItem.Name}, 类型: {selectedItem.Type}");
                LoadTemplateAsync(selectedItem);
            }
        }

        partial void OnIsBusyChanged(bool value)
        {
            if (SaveTemplateCommand is CommunityToolkit.Mvvm.Input.IRelayCommand relayCommand)
            {
                relayCommand.NotifyCanExecuteChanged();
            }
        }
    }
}
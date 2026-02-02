using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Services.Data;
using Demo_ReportPrinter.Services.Pdf;
using Demo_ReportPrinter.Services.Shared;
using Demo_ReportPrinter.ViewModels.Base;
using System.Windows;

namespace Demo_ReportPrinter.ViewModels
{
    /// <summary>
    /// 主窗口ViewModel
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        private readonly ISharedDataService _sharedDataService;
        private readonly ITemplateService _templateService;
        private readonly IPdfService _pdfService;

        [ObservableProperty]
        private string _windowTitle = "WPF模板编辑器";

        public MainViewModel(
            ISharedDataService sharedDataService,
            ITemplateService templateService,
            IPdfService pdfService)
        {
            _sharedDataService = sharedDataService;
            _templateService = templateService;
            _pdfService = pdfService;
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            IsLoading = true;
            ClearError();

            try
            {
                // 初始化应用数据
                await LoadInitialDataAsync();

                // 注册消息监听
                RegisterMessageHandlers();
            }
            catch (Exception ex)
            {
                SetError($"初始化应用失败：{ex.Message}");
                ShowErrorMessage("初始化失败", $"应用初始化过程中发生错误：{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadInitialDataAsync()
        {
            // 加载初始模板
            var result = await _templateService.LoadDefaultTemplateAsync();

            if (result.IsSuccess)
            {
                _sharedDataService.CurrentTemplate = result.Value;
            }
            else
            {
                SetError(result.ErrorMessage);
                ShowErrorMessage("加载模板失败", result.ErrorMessage);
            }
        }

        private void RegisterMessageHandlers()
        {
            // 监听模板变更消息
            RegisterMessageHandler<Services.Shared.TemplateSelectedMessage>((message) =>
            {
                // 处理模板选中逻辑
            });
        }

        [RelayCommand]
        private void ExitApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        [RelayCommand]
        private async Task CreateNewTemplate()
        {
            IsLoading = true;
            ClearError();

            try
            {
                // 实现新建模板逻辑
                var result = await _templateService.LoadDefaultTemplateAsync();

                if (result.IsSuccess)
                {
                    var newTemplate = result.Value;
                    newTemplate.Name = "新模板";
                    newTemplate.Description = "新建的模板";
                    
                    _sharedDataService.CurrentTemplate = newTemplate;
                }
                else
                {
                    SetError(result.ErrorMessage);
                    ShowErrorMessage("创建模板失败", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                SetError($"创建模板失败：{ex.Message}");
                ShowErrorMessage("创建模板失败", $"创建新模板时发生错误：{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task OpenTemplate()
        {
            IsLoading = true;
            ClearError();

            try
            {
                // 实现打开模板逻辑
                var result = await _templateService.GetAllTemplatesAsync();

                if (result.IsSuccess)
                {
                    var templates = result.Value;
                    if (templates.Any())
                    {
                        _sharedDataService.CurrentTemplate = templates.First();
                    }
                    else
                    {
                        SetError("没有可用的模板");
                        ShowErrorMessage("打开模板失败", "没有找到可用的模板");
                    }
                }
                else
                {
                    SetError(result.ErrorMessage);
                    ShowErrorMessage("加载模板失败", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                SetError($"打开模板失败：{ex.Message}");
                ShowErrorMessage("打开模板失败", $"打开模板时发生错误：{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SaveTemplate()
        {
            IsLoading = true;
            ClearError();

            try
            {
                // 实现保存模板逻辑
                if (_sharedDataService.CurrentTemplate != null)
                {
                    var result = await _templateService.SaveTemplateAsync(_sharedDataService.CurrentTemplate);

                    if (result.IsSuccess)
                    {
                        ShowSuccessMessage("保存成功", "模板已成功保存");
                    }
                    else
                    {
                        SetError(result.ErrorMessage);
                        ShowErrorMessage("保存模板失败", result.ErrorMessage);
                    }
                }
                else
                {
                    SetError("没有可保存的模板");
                    ShowErrorMessage("保存模板失败", "请先创建或选择一个模板");
                }
            }
            catch (Exception ex)
            {
                SetError($"保存模板失败：{ex.Message}");
                ShowErrorMessage("保存模板失败", $"保存模板时发生错误：{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task GeneratePdf()
        {
            IsLoading = true;
            ClearError();

            try
            {
                // 实现生成PDF逻辑
                if (_sharedDataService.CurrentTemplate != null)
                {
                    var result = await _pdfService.GeneratePdfAsync(_sharedDataService.UserData, _sharedDataService.CurrentTemplate.TemplateId);

                    if (result.IsSuccess)
                    {
                        // 发送PDF生成完成消息
                        _sharedDataService.BroadcastDataChange("PdfGenerated", result.Value);
                        ShowSuccessMessage("生成成功", "PDF文件已成功生成");
                    }
                    else
                    {
                        SetError(result.ErrorMessage);
                        ShowErrorMessage("生成PDF失败", result.ErrorMessage);
                    }
                }
                else
                {
                    SetError("没有可生成PDF的模板");
                    ShowErrorMessage("生成PDF失败", "请先选择一个模板");
                }
            }
            catch (Exception ex)
            {
                SetError($"生成PDF失败：{ex.Message}");
                ShowErrorMessage("生成PDF失败", $"生成PDF时发生错误：{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task PreviewPdf()
        {
            IsLoading = true;
            ClearError();

            try
            {
                // 实现预览PDF逻辑
                if (_sharedDataService.CurrentTemplate != null)
                {
                    var result = await _pdfService.GeneratePdfAsync(_sharedDataService.UserData, _sharedDataService.CurrentTemplate.TemplateId);

                    if (result.IsSuccess)
                    {
                        // 发送PDF预览消息
                        _sharedDataService.BroadcastDataChange("PdfPreview", result.Value);
                    }
                    else
                    {
                        SetError(result.ErrorMessage);
                        ShowErrorMessage("预览PDF失败", result.ErrorMessage);
                    }
                }
                else
                {
                    SetError("没有可预览的模板");
                    ShowErrorMessage("预览PDF失败", "请先选择一个模板");
                }
            }
            catch (Exception ex)
            {
                SetError($"预览PDF失败：{ex.Message}");
                ShowErrorMessage("预览PDF失败", $"预览PDF时发生错误：{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task PrintPdf()
        {
            IsLoading = true;
            ClearError();

            try
            {
                // 实现打印PDF逻辑
                if (_sharedDataService.CurrentTemplate != null)
                {
                    var generateResult = await _pdfService.GeneratePdfAsync(_sharedDataService.UserData, _sharedDataService.CurrentTemplate.TemplateId);

                    if (generateResult.IsSuccess)
                    {
                        var printResult = await _pdfService.PrintPdfAsync(generateResult.Value);

                        if (printResult.IsSuccess)
                        {
                            // 发送PDF打印完成消息
                            _sharedDataService.BroadcastDataChange("PdfPrinted", generateResult.Value);
                            ShowSuccessMessage("打印成功", "PDF文件已成功发送到打印机");
                        }
                        else
                        {
                            SetError(printResult.ErrorMessage);
                            ShowErrorMessage("打印PDF失败", printResult.ErrorMessage);
                        }
                    }
                    else
                    {
                        SetError(generateResult.ErrorMessage);
                        ShowErrorMessage("生成PDF失败", generateResult.ErrorMessage);
                    }
                }
                else
                {
                    SetError("没有可打印的模板");
                    ShowErrorMessage("打印PDF失败", "请先选择一个模板");
                }
            }
            catch (Exception ex)
            {
                SetError($"打印PDF失败：{ex.Message}");
                ShowErrorMessage("打印PDF失败", $"打印PDF时发生错误：{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// 显示错误消息对话框
        /// </summary>
        private void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 显示成功消息对话框
        /// </summary>
        private void ShowSuccessMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
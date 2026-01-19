using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportTemplateEditor.Core.Models;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.App.Services;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;
using WpfFrameworkElement = System.Windows.FrameworkElement;
using WpfPanel = System.Windows.Controls.Panel;
using WpfBinding = System.Windows.Data.Binding;
using WpfBindingMode = System.Windows.Data.BindingMode;
using WpfUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger;

namespace ReportTemplateEditor.App.ViewModels
{
    public partial class ControlPanelViewModel : ViewModelBase
    {
        private readonly IControlGeneratorService _controlGeneratorService;
        private readonly Dictionary<string, WpfFrameworkElement> _controlMap = new Dictionary<string, WpfFrameworkElement>();
        private INotifyPropertyChanged? _previousBoundDataNotifier;

        [ObservableProperty]
        private ReportTemplateDefinition? _currentTemplate;

        [ObservableProperty]
        private WpfPanel? _controlContainer;

        [ObservableProperty]
        private object? _boundData;

        [ObservableProperty]
        private string _statusMessage = "未加载模板";

        [ObservableProperty]
        private bool _isLoading;

        public ICommand LoadTemplateCommand { get; }

        public ICommand RefreshControlsCommand { get; }

        public ICommand UpdateDataCommand { get; }

        public ICommand ElementChangedCommand { get; }

        public ControlPanelViewModel(IControlGeneratorService controlGeneratorService)
        {
            _controlGeneratorService = controlGeneratorService;

            LoadTemplateCommand = new RelayCommand<ReportTemplateDefinition>(LoadTemplate);
            RefreshControlsCommand = new RelayCommand(RefreshControls);
            UpdateDataCommand = new RelayCommand<object>(UpdateData);
            ElementChangedCommand = new RelayCommand<ElementBase>(OnElementChanged);
        }

        private void LoadTemplate(ReportTemplateDefinition? template)
        {
            if (template == null)
            {
                StatusMessage = "模板为空";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "正在加载控件...";

                CurrentTemplate = template;
                GenerateControls();
                CreateSampleData();
                StatusMessage = $"已加载 {template.Elements.Count} 个控件";
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

        private void RefreshControls()
        {
            if (CurrentTemplate != null)
            {
                GenerateControls();
                StatusMessage = "控件已刷新";
            }
        }

        private void UpdateData(object? data)
        {
            BoundData = data;
            UpdateControlBindings();
            StatusMessage = "数据已更新";
        }

        private void OnElementChanged(ElementBase? element)
        {
            if (element == null || !_controlMap.TryGetValue(element.Id, out var control))
            {
                return;
            }

            _controlGeneratorService.UpdateControlFromElement(control, element);
        }

        private void GenerateControls()
        {
            if (CurrentTemplate == null || ControlContainer == null)
            {
                return;
            }

            ControlContainer.Children.Clear();
            _controlMap.Clear();

            foreach (var element in CurrentTemplate.Elements)
            {
                if (!element.IsVisible)
                {
                    continue;
                }

                try
                {
                    var control = _controlGeneratorService.GenerateControl(element);
                    if (control != null)
                    {
                        ControlContainer.Children.Add(control);
                        _controlMap[element.Id] = control;
                        SetupDataBinding(control, element);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"生成控件失败: {element.Type}, 错误: {ex.Message}");
                }
            }
        }

        private void CreateSampleData()
        {
            if (CurrentTemplate == null)
            {
                return;
            }

            var data = new System.Dynamic.ExpandoObject();
            var dataDict = (System.Collections.Generic.IDictionary<string, object?>)data;

            foreach (var element in CurrentTemplate.Elements)
            {
                if (element is TextElement textElement && !string.IsNullOrEmpty(textElement.DataBindingPath))
                {
                    if (!dataDict.ContainsKey(textElement.DataBindingPath))
                    {
                        dataDict[textElement.DataBindingPath] = textElement.Text;
                    }
                }
                else if (element is TableElement tableElement)
                {
                    foreach (var cell in tableElement.Cells)
                    {
                        if (!string.IsNullOrEmpty(cell.DataBindingPath) && !dataDict.ContainsKey(cell.DataBindingPath))
                        {
                            dataDict[cell.DataBindingPath] = cell.Content;
                        }
                    }
                }
            }

            BoundData = data;
        }

        private void SetupDataBinding(WpfFrameworkElement control, ElementBase element)
        {
            if (BoundData == null)
            {
                return;
            }

            var dataDict = BoundData as System.Collections.Generic.IDictionary<string, object?>;

            if (element is TextElement textElement && !string.IsNullOrEmpty(textElement.DataBindingPath))
            {
                if (control is Border border && border.Child is StackPanel panel)
                {
                    foreach (var child in panel.Children)
                    {
                        if (child is System.Windows.Controls.TextBox textBox)
                        {
                            var binding = new WpfBinding(textElement.DataBindingPath)
                            {
                                Source = BoundData,
                                Mode = WpfBindingMode.TwoWay,
                                UpdateSourceTrigger = WpfUpdateSourceTrigger.PropertyChanged
                            };
                            textBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
                        }
                    }
                }
            }
            else if (element is TableElement tableElement)
            {
                if (control is Border border && border.Child is StackPanel panel)
                {
                    foreach (var child in panel.Children)
                    {
                        if (child is Grid grid)
                        {
                            foreach (var gridChild in grid.Children)
                            {
                                if (gridChild is System.Windows.Controls.TextBox textBox)
                                {
                                    var cell = tableElement.Cells.FirstOrDefault(c => 
                                        Grid.GetRow(textBox) == c.RowIndex && 
                                        Grid.GetColumn(textBox) == c.ColumnIndex);
                                    
                                    if (cell != null && !string.IsNullOrEmpty(cell.DataBindingPath))
                                    {
                                        var binding = new WpfBinding(cell.DataBindingPath)
                                        {
                                            Source = BoundData,
                                            Mode = WpfBindingMode.TwoWay,
                                            UpdateSourceTrigger = WpfUpdateSourceTrigger.PropertyChanged
                                        };
                                        textBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateControlBindings()
        {
            if (ControlContainer == null || CurrentTemplate == null || BoundData == null)
            {
                return;
            }

            foreach (var element in CurrentTemplate.Elements)
            {
                if (!element.IsVisible)
                {
                    continue;
                }

                try
                {
                    if (_controlMap.TryGetValue(element.Id, out var control))
                    {
                        SetupDataBinding(control, element);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"更新控件绑定失败: {element.Type}, 错误: {ex.Message}");
                }
            }
        }

        partial void OnBoundDataChanged(object? value)
        {
            if (_previousBoundDataNotifier != null)
            {
                _previousBoundDataNotifier.PropertyChanged -= OnBoundDataPropertyChanged;
            }

            if (value is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged += OnBoundDataPropertyChanged;
                _previousBoundDataNotifier = notifier;
            }
        }

        private void OnBoundDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null)
            {
                System.Diagnostics.Debug.WriteLine($"数据已更改: {e.PropertyName}");
            }
        }
    }
}

using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Controls;
using Xinglin.Core.Models;
using Xinglin.Core.Elements;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using System.Windows.Data;
using WpfFrameworkElement = System.Windows.FrameworkElement;
using WpfPanel = System.Windows.Controls.StackPanel;
using WpfBinding = System.Windows.Data.Binding;
using WpfBindingMode = System.Windows.Data.BindingMode;
using WpfUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    public class ControlPanelViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<string, WpfFrameworkElement> _controlMap = new Dictionary<string, WpfFrameworkElement>();
        private INotifyPropertyChanged? _previousBoundDataNotifier;

        private ReportTemplateDefinition? _currentTemplate;
        public ReportTemplateDefinition? CurrentTemplate
        {
            get => _currentTemplate;
            set
            {
                _currentTemplate = value;
                OnPropertyChanged(nameof(CurrentTemplate));
                if (value != null)
                {
                    GenerateControls();
                    CreateSampleData();
                }
            }
        }

        private WpfPanel? _controlContainer;
        public WpfPanel? ControlContainer
        {
            get => _controlContainer;
            set
            {
                _controlContainer = value;
                OnPropertyChanged(nameof(ControlContainer));
                if (value != null && CurrentTemplate != null)
                {
                    GenerateControls();
                }
            }
        }

        private object? _boundData;
        public object? BoundData
        {
            get => _boundData;
            set
            {
                _boundData = value;
                OnPropertyChanged(nameof(BoundData));
                UpdateControlBindings();
            }
        }

        private string _statusMessage = "未加载模板";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ICommand LoadTemplateCommand { get; }
        public ICommand RefreshControlsCommand { get; }
        public ICommand GenerateSampleDataCommand { get; }
        public ICommand UpdateDataCommand { get; }

        public ControlPanelViewModel()
        {
            LoadTemplateCommand = new RelayCommand<ReportTemplateDefinition>(LoadTemplate);
            RefreshControlsCommand = new RelayCommand(RefreshControls);
            GenerateSampleDataCommand = new RelayCommand(CreateSampleData);
            UpdateDataCommand = new RelayCommand<object>(UpdateData);
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

        private void GenerateControls()
        {
            if (CurrentTemplate == null || ControlContainer == null)
            {
                return;
            }

            ControlContainer.Children.Clear();
            _controlMap.Clear();

            var textElements = new List<ElementBase>();
            var tableElements = new List<ElementBase>();
            var otherElements = new List<ElementBase>();

            foreach (var element in CurrentTemplate.Elements)
            {
                if (!element.IsVisible)
                {
                    continue;
                }

                if (element is TextElement)
                {
                    textElements.Add(element);
                }
                else
                {
                    otherElements.Add(element);
                }
            }

            foreach (var element in textElements.Concat(otherElements))
            {
                try
                {
                    var control = GenerateControl(element);
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

            if (ControlContainer.Children.Count == 0)
            {
                var noElementsText = new System.Windows.Controls.TextBlock
                {
                    Text = "模板中没有可见控件",
                    Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(153, 153, 153)),
                    Margin = new System.Windows.Thickness(20),
                    TextAlignment = System.Windows.TextAlignment.Center
                };
                ControlContainer.Children.Add(noElementsText);
            }
        }

        private WpfFrameworkElement GenerateControl(ElementBase element)
        {
            var border = new System.Windows.Controls.Border
            {
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(224, 224, 224)),
                BorderThickness = new System.Windows.Thickness(1),
                CornerRadius = new System.Windows.CornerRadius(4),
                Margin = new System.Windows.Thickness(0, 0, 0, 10),
                Padding = new System.Windows.Thickness(10)
            };

            var panel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Vertical
            };

            var label = new System.Windows.Controls.TextBlock
            {
                Text = element.Type,
                FontWeight = System.Windows.FontWeights.Medium,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 51, 51)),
                Margin = new System.Windows.Thickness(0, 0, 0, 5)
            };

            panel.Children.Add(label);

            if (element is TextElement textElement)
            {
                var textBox = new System.Windows.Controls.TextBox
                {
                    Text = textElement.Text,
                    Margin = new System.Windows.Thickness(0, 0, 0, 5),
                    Padding = new System.Windows.Thickness(8)
                };
                panel.Children.Add(textBox);
            }


            border.Child = panel;
            return border;
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
                if (control is System.Windows.Controls.Border border && border.Child is System.Windows.Controls.StackPanel panel)
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

        private void OnBoundDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null)
            {
                System.Diagnostics.Debug.WriteLine($"数据已更改: {e.PropertyName}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
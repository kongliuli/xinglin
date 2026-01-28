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

        private object? _currentTemplate;
        public object? CurrentTemplate
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

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                GenerateControls();
            }
        }

        private List<string> _elementCategories;
        public List<string> ElementCategories
        {
            get => _elementCategories;
            set
            {
                _elementCategories = value;
                OnPropertyChanged(nameof(ElementCategories));
            }
        }

        private string _selectedCategory = "全部";
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                GenerateControls();
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
            
            // 初始化元素分类列表
            ElementCategories = new List<string> { "全部", "文本输入", "下拉选择", "表格", "其他" };
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

            var elements = new List<object>();

            // 处理ReportTemplateDefinition类型
            if (CurrentTemplate is Xinglin.Core.Models.ReportTemplateDefinition reportTemplate)
            {
                foreach (var element in reportTemplate.Elements)
                {
                    if (element is Xinglin.Core.Elements.ElementBase coreElement && coreElement.IsVisible)
                    {
                        elements.Add(coreElement);
                    }
                }
            }
            // 处理TemplateDefinition类型
            else if (CurrentTemplate is Xinglin.ReportTemplateEditor.WPF.Models.TemplateDefinition templateDef)
            {
                // 添加全局元素
                if (templateDef.ElementCollection?.GlobalElements != null)
                {
                    foreach (var element in templateDef.ElementCollection.GlobalElements)
                    {
                        elements.Add(element);
                    }
                }
                // 添加区域内元素
                if (templateDef.ElementCollection?.Zones != null)
                {
                    foreach (var zone in templateDef.ElementCollection.Zones)
                    {
                        if (zone.Elements != null)
                        {
                            foreach (var element in zone.Elements)
                            {
                                elements.Add(element);
                            }
                        }
                    }
                }
            }

            // 过滤元素
            var filteredElements = FilterElements(elements);

            foreach (var element in filteredElements)
            {
                try
                {
                    if (element is Xinglin.Core.Elements.ElementBase coreElement)
                    {
                        var control = GenerateControl(coreElement);
                        if (control != null)
                        {
                            ControlContainer.Children.Add(control);
                            _controlMap[coreElement.Id] = control;
                            SetupDataBinding(control, coreElement);
                        }
                    }
                    else if (element is Xinglin.ReportTemplateEditor.WPF.Models.TemplateElement templateElement)
                    {
                        // 为TemplateElement创建控件
                        var control = GenerateTemplateElementControl(templateElement);
                        if (control != null)
                        {
                            ControlContainer.Children.Add(control);
                            _controlMap[templateElement.ElementId] = control;
                            SetupTemplateElementDataBinding(control, templateElement);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"生成控件失败: {ex.Message}");
                }
            }

            if (ControlContainer.Children.Count == 0)
            {
                var noElementsText = new System.Windows.Controls.TextBlock
                {
                    Text = filteredElements.Count == 0 ? "没有找到匹配的控件" : "模板中没有可见控件",
                    Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(153, 153, 153)),
                    Margin = new System.Windows.Thickness(20),
                    TextAlignment = System.Windows.TextAlignment.Center
                };
                ControlContainer.Children.Add(noElementsText);
            }
        }

        /// <summary>
        /// 过滤元素
        /// </summary>
        /// <param name="elements">原始元素列表</param>
        /// <returns>过滤后的元素列表</returns>
        private List<object> FilterElements(List<object> elements)
        {
            return elements.Where(element => 
            {
                // 搜索过滤
                bool matchesSearch = true;
                if (!string.IsNullOrEmpty(SearchText))
                {
                    string searchText = SearchText.ToLower();
                    if (element is Xinglin.Core.Elements.ElementBase coreElement)
                    {
                        matchesSearch = coreElement.Type?.ToLower().Contains(searchText) ?? false;
                    }
                    else if (element is Xinglin.ReportTemplateEditor.WPF.Models.TemplateElement templateElement)
                    {
                        matchesSearch = (templateElement.Label?.ToLower().Contains(searchText) ?? false) || 
                                       (templateElement.ElementType?.ToLower().Contains(searchText) ?? false);
                    }
                }

                // 分类过滤
                bool matchesCategory = true;
                if (SelectedCategory != "全部")
                {
                    if (element is Xinglin.Core.Elements.ElementBase coreElement)
                    {
                        matchesCategory = GetElementCategory(coreElement.Type) == SelectedCategory;
                    }
                    else if (element is Xinglin.ReportTemplateEditor.WPF.Models.TemplateElement templateElement)
                    {
                        matchesCategory = GetElementCategory(templateElement.ElementType) == SelectedCategory;
                    }
                }

                return matchesSearch && matchesCategory;
            }).ToList();
        }

        /// <summary>
        /// 获取元素分类
        /// </summary>
        /// <param name="elementType">元素类型</param>
        /// <returns>元素分类</returns>
        private string GetElementCategory(string elementType)
        {
            switch (elementType?.ToLower())
            {
                case "text":
                case "number":
                case "date":
                    return "文本输入";
                case "dropdown":
                    return "下拉选择";
                case "table":
                    return "表格";
                default:
                    return "其他";
            }
        }

        /// <summary>
        /// 为TemplateElement生成控件
        /// </summary>
        /// <param name="element">TemplateElement对象</param>
        /// <returns>生成的控件</returns>
        private WpfFrameworkElement GenerateTemplateElementControl(Xinglin.ReportTemplateEditor.WPF.Models.TemplateElement element)
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
                Text = element.Label ?? element.ElementType,
                FontWeight = System.Windows.FontWeights.Medium,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 51, 51)),
                Margin = new System.Windows.Thickness(0, 0, 0, 5)
            };

            panel.Children.Add(label);

            // 根据元素类型生成不同的控件
            switch (element.ElementType)
            {
                case "Text":
                case "Number":
                case "Date":
                    var textBox = new System.Windows.Controls.TextBox
                    {
                        Text = element.DefaultValue,
                        Margin = new System.Windows.Thickness(0, 0, 0, 5),
                        Padding = new System.Windows.Thickness(8)
                    };
                    panel.Children.Add(textBox);
                    break;
                case "Dropdown":
                    var comboBox = new System.Windows.Controls.ComboBox
                    {
                        Margin = new System.Windows.Thickness(0, 0, 0, 5),
                        Padding = new System.Windows.Thickness(8)
                    };
                    if (element.Options != null)
                    {
                        foreach (var option in element.Options)
                        {
                            comboBox.Items.Add(option);
                        }
                    }
                    panel.Children.Add(comboBox);
                    break;
                case "Table":
                    var tableLabel = new System.Windows.Controls.TextBlock
                    {
                        Text = "表格元素",
                        Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102)),
                        Margin = new System.Windows.Thickness(0, 0, 0, 5)
                    };
                    panel.Children.Add(tableLabel);
                    break;
                default:
                    var defaultLabel = new System.Windows.Controls.TextBlock
                    {
                        Text = $"元素类型: {element.ElementType}",
                        Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102)),
                        Margin = new System.Windows.Thickness(0, 0, 0, 5)
                    };
                    panel.Children.Add(defaultLabel);
                    break;
            }

            border.Child = panel;
            return border;
        }

        /// <summary>
        /// 为TemplateElement设置数据绑定
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="element">TemplateElement对象</param>
        private void SetupTemplateElementDataBinding(WpfFrameworkElement control, Xinglin.ReportTemplateEditor.WPF.Models.TemplateElement element)
        {
            // 这里可以根据需要实现数据绑定逻辑
            // 暂时留空，因为我们还没有实现完整的数据绑定系统
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

            // 处理ReportTemplateDefinition类型
            if (CurrentTemplate is Xinglin.Core.Models.ReportTemplateDefinition reportTemplate)
            {
                foreach (var element in reportTemplate.Elements)
                {
                    if (element is Xinglin.Core.Elements.TextElement textElement && !string.IsNullOrEmpty(textElement.DataBindingPath))
                    {
                        if (!dataDict.ContainsKey(textElement.DataBindingPath))
                        {
                            dataDict[textElement.DataBindingPath] = textElement.Text;
                        }
                    }
                }
            }
            // 处理TemplateDefinition类型
            else if (CurrentTemplate is Xinglin.ReportTemplateEditor.WPF.Models.TemplateDefinition templateDef)
            {
                // 处理全局元素
                if (templateDef.ElementCollection?.GlobalElements != null)
                {
                    foreach (var element in templateDef.ElementCollection.GlobalElements)
                    {
                        if (!string.IsNullOrEmpty(element.ElementId))
                        {
                            if (!dataDict.ContainsKey(element.ElementId))
                            {
                                dataDict[element.ElementId] = element.DefaultValue;
                            }
                        }
                    }
                }
                // 处理区域内元素
                if (templateDef.ElementCollection?.Zones != null)
                {
                    foreach (var zone in templateDef.ElementCollection.Zones)
                    {
                        if (zone.Elements != null)
                        {
                            foreach (var element in zone.Elements)
                            {
                                if (!string.IsNullOrEmpty(element.ElementId))
                                {
                                    if (!dataDict.ContainsKey(element.ElementId))
                                    {
                                        dataDict[element.ElementId] = element.DefaultValue;
                                    }
                                }
                            }
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

            // 处理ReportTemplateDefinition类型
            if (CurrentTemplate is Xinglin.Core.Models.ReportTemplateDefinition reportTemplate)
            {
                foreach (var element in reportTemplate.Elements)
                {
                    if (element is Xinglin.Core.Elements.ElementBase coreElement && coreElement.IsVisible)
                    {
                        try
                        {
                            if (_controlMap.TryGetValue(coreElement.Id, out var control))
                            {
                                SetupDataBinding(control, coreElement);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"更新控件绑定失败: {coreElement.Type}, 错误: {ex.Message}");
                        }
                    }
                }
            }
            // 处理TemplateDefinition类型
            else if (CurrentTemplate is Xinglin.ReportTemplateEditor.WPF.Models.TemplateDefinition templateDef)
            {
                // 处理全局元素
                if (templateDef.ElementCollection?.GlobalElements != null)
                {
                    foreach (var element in templateDef.ElementCollection.GlobalElements)
                    {
                        try
                        {
                            if (_controlMap.TryGetValue(element.ElementId, out var control))
                            {
                                SetupTemplateElementDataBinding(control, element);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"更新控件绑定失败: {element.ElementType}, 错误: {ex.Message}");
                        }
                    }
                }
                // 处理区域内元素
                if (templateDef.ElementCollection?.Zones != null)
                {
                    foreach (var zone in templateDef.ElementCollection.Zones)
                    {
                        if (zone.Elements != null)
                        {
                            foreach (var element in zone.Elements)
                            {
                                try
                                {
                                    if (_controlMap.TryGetValue(element.ElementId, out var control))
                                    {
                                        SetupTemplateElementDataBinding(control, element);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"更新控件绑定失败: {element.ElementType}, 错误: {ex.Message}");
                                }
                            }
                        }
                    }
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
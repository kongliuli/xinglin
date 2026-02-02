// �ο��ļ� - ��ע�͵�
/*
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Services.Data;
using Demo_ReportPrinter.Services.Shared;
using Demo_ReportPrinter.Services.UndoRedo;
using Demo_ReportPrinter.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Demo_ReportPrinter.ViewModels
{
    /// <summary>
    /// 模板编辑器ViewModel
    /// </summary>
    public partial class TemplateEditorViewModel : ViewModelBase
    {
        private readonly ISharedDataService _sharedDataService;

        [ObservableProperty]
        private TemplateData _currentTemplate;

        [ObservableProperty]
        private ControlElement _selectedElement;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private double _paperWidth;

        [ObservableProperty]
        private double _paperHeight;

        /// <summary>
        /// 命令历史记录
        /// </summary>
        private readonly CommandHistory _commandHistory;
        private readonly BidirectionalSyncCoordinator _syncCoordinator;

        /// <summary>
        /// 是否可以撤销
        /// </summary>
        public bool CanUndo => _commandHistory.CanUndo;

        /// <summary>
        /// 是否可以重做
        /// </summary>
        public bool CanRedo => _commandHistory.CanRedo;

        /// <summary>
        /// 纸张规格列表
        /// </summary>
        public List<PaperSizeInfo> PaperSizes => PaperSizeConstants.AllPaperSizes;

        /// <summary>
        /// 当前纸张类型
        /// </summary>
        [ObservableProperty]
        private PaperSizeType _currentPaperType;

        /// <summary>
        /// 是否横向
        /// </summary>
        [ObservableProperty]
        private bool _isLandscape;

        public TemplateEditorViewModel()
        {
            _sharedDataService = SharedDataService.Instance;
            _commandHistory = new CommandHistory();
            _syncCoordinator = new BidirectionalSyncCoordinator(_sharedDataService);

            // 监听模板变更
            RegisterMessageHandler<Services.Shared.TemplateSelectedMessage>((message) =>
            {
                LoadTemplateAsync(message.TemplateId);
            });

            // 监听数据变更
            RegisterMessageHandler<Services.Shared.DataChangedMessage>((message) =>
            {
                UpdateElementValue(message.Key, message.Value);
            });

            // 初始化当前模�?            CurrentTemplate = _sharedDataService.CurrentTemplate;
            if (CurrentTemplate != null)
            {
                PaperWidth = CurrentTemplate.Layout.PaperWidth;
                PaperHeight = CurrentTemplate.Layout.PaperHeight;
                CurrentPaperType = CurrentTemplate.Layout.PaperType;
                IsLandscape = CurrentTemplate.Layout.IsLandscape;
            }
        }

        [RelayCommand]
        private async Task LoadTemplateAsync(string templateId)
        {
            var templateService = new TemplateService();
            CurrentTemplate = await templateService.GetTemplateByIdAsync(templateId);

            PaperWidth = CurrentTemplate.Layout.PaperWidth;
            PaperHeight = CurrentTemplate.Layout.PaperHeight;
            CurrentPaperType = CurrentTemplate.Layout.PaperType;
            IsLandscape = CurrentTemplate.Layout.IsLandscape;

            // 通知DataEntry加载新模板字�?            _sharedDataService.SendMessage(new TemplateLoadedMessage(templateId));

            // 通知PDF预览刷新
            _sharedDataService.BroadcastDataChange("TemplateChanged", templateId);
        }

        [RelayCommand]
        private void AddElement(ControlType type)
        {
            var newElement = new ControlElement
            {
                Type = type,
                X = 50,
                Y = 50,
                Width = 100,
                Height = 30,
                EditState = EditableState.Editable,
                ZIndex = CurrentTemplate.Layout.EditableElements.Count
            };

            // 设置默认显示名称
            switch (type)
            {
                case ControlType.TextBox:
                    newElement.DisplayName = "文本�?;
                    break;
                case ControlType.ComboBox:
                    newElement.DisplayName = "下拉�?;
                    break;
                case ControlType.DatePicker:
                    newElement.DisplayName = "日期选择";
                    break;
                case ControlType.CheckBox:
                    newElement.DisplayName = "复选框";
                    break;
                case ControlType.Table:
                    newElement.DisplayName = "表格";
                    newElement.Width = 200;
                    newElement.Height = 100;
                    break;
                case ControlType.Image:
                    newElement.DisplayName = "图片";
                    newElement.Width = 150;
                    newElement.Height = 100;
                    break;
                case ControlType.Chart:
                    newElement.DisplayName = "图表";
                    newElement.Width = 200;
                    newElement.Height = 150;
                    break;
                default:
                    newElement.DisplayName = "控件";
                    break;
            }

            // 创建添加控件命令
            var command = new AddControlCommand(CurrentTemplate.Layout.EditableElements, newElement);
            _commandHistory.ExecuteCommand(command);
            
            SelectedElement = newElement;
        }

        [RelayCommand]
        private void DeleteElement(ControlElement element = null)
        {
            var targetElement = element ?? SelectedElement;
            if (targetElement != null)
            {
                // 创建删除控件命令
                var command = new RemoveControlCommand(CurrentTemplate.Layout.EditableElements, targetElement);
                _commandHistory.ExecuteCommand(command);
                
                if (SelectedElement == targetElement)
                {
                    SelectedElement = null;
                }
                // 更新其他元素的层�?                UpdateElementZIndices();
            }
        }

        [RelayCommand]
        private void EditProperties(ControlElement element)
        {
            SelectedElement = element;
            // 这里可以打开属性编辑窗口或焦点到属性面�?        }

        [RelayCommand]
        private void SetDefaultValue(ControlElement element)
        {
            // 创建WPF对话�?            var window = new Window();
            window.Title = "设置默认�?;
            window.Width = 300;
            window.Height = 150;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;
            
            var grid = new Grid();
            grid.Margin = new Thickness(10);
            
            // 添加行定�?            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            // 添加标签
            var label = new Label();
            label.Content = "默认�?";
            label.Margin = new Thickness(0, 0, 0, 5);
            Grid.SetRow(label, 0);
            grid.Children.Add(label);
            
            // 添加文本�?            var textBox = new System.Windows.Controls.TextBox();
            textBox.Margin = new Thickness(0, 0, 0, 10);
            textBox.Text = element.Value?.ToString() ?? "";
            Grid.SetRow(textBox, 1);
            grid.Children.Add(textBox);
            
            // 添加按钮容器
            var buttonPanel = new StackPanel();
            buttonPanel.Orientation = Orientation.Horizontal;
            buttonPanel.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);
            
            // 添加确定按钮
            var okButton = new System.Windows.Controls.Button();
            okButton.Content = "确定";
            okButton.Margin = new Thickness(0, 0, 10, 0);
            okButton.Width = 70;
            okButton.Click += (sender, e) => { window.DialogResult = true; };
            buttonPanel.Children.Add(okButton);
            
            // 添加取消按钮
            var cancelButton = new System.Windows.Controls.Button();
            cancelButton.Content = "取消";
            cancelButton.Width = 70;
            cancelButton.Click += (sender, e) => { window.DialogResult = false; };
            buttonPanel.Children.Add(cancelButton);
            
            window.Content = grid;
            
            if (window.ShowDialog() == true)
            {
                var oldValue = element.Value;
                var newValue = textBox.Text;
                element.Value = newValue;
                
                // 创建属性更改命�?                var command = new ChangeControlPropertyCommand(element, "Value", oldValue, newValue);
                _commandHistory.ExecuteCommand(command);
                
                // 通知DataEntry更新
                _sharedDataService.SendMessage(new ElementValueChangedMessage(element.ElementId, newValue, oldValue));
            }
        }

        [RelayCommand]
        private void EditOptions(ControlElement element)
        {
            if (element.Type == ControlType.ComboBox)
            {
                // 创建WPF对话�?                var window = new Window();
                window.Title = "编辑下拉选项";
                window.Width = 300;
                window.Height = 250;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.ResizeMode = ResizeMode.NoResize;
                
                var grid = new Grid();
                grid.Margin = new Thickness(10);
                
                // 添加行定�?                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                
                // 添加标签
                var label = new Label();
                label.Content = "选项（每行一个）:";
                label.Margin = new Thickness(0, 0, 0, 5);
                Grid.SetRow(label, 0);
                grid.Children.Add(label);
                
                // 添加文本�?                var textBox = new System.Windows.Controls.TextBox();
                textBox.Margin = new Thickness(0, 0, 0, 10);
                textBox.AcceptsReturn = true;
                textBox.TextWrapping = TextWrapping.Wrap;
                textBox.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
                
                // 获取现有选项
                var existingOptions = element.GetProperty<string>("Options", "选项1\n选项2\n选项3");
                textBox.Text = existingOptions;
                Grid.SetRow(textBox, 1);
                grid.Children.Add(textBox);
                
                // 添加按钮容器
                var buttonPanel = new StackPanel();
                buttonPanel.Orientation = Orientation.Horizontal;
                buttonPanel.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(buttonPanel, 2);
                grid.Children.Add(buttonPanel);
                
                // 添加确定按钮
                var okButton = new System.Windows.Controls.Button();
                okButton.Content = "确定";
                okButton.Margin = new Thickness(0, 0, 10, 0);
                okButton.Width = 70;
                okButton.Click += (sender, e) => { window.DialogResult = true; };
                buttonPanel.Children.Add(okButton);
                
                // 添加取消按钮
                var cancelButton = new System.Windows.Controls.Button();
                cancelButton.Content = "取消";
                cancelButton.Width = 70;
                cancelButton.Click += (sender, e) => { window.DialogResult = false; };
                buttonPanel.Children.Add(cancelButton);
                
                window.Content = grid;
                
                if (window.ShowDialog() == true)
                {
                    var oldOptions = existingOptions;
                    var newOptions = textBox.Text;
                    element.SetProperty("Options", newOptions);
                    
                    // 创建属性更改命�?                    var command = new ChangeControlPropertyCommand(element, "Options", oldOptions, newOptions);
                    _commandHistory.ExecuteCommand(command);
                }
            }
        }

        [RelayCommand]
        private async Task SaveTemplateAsync()
        {
            var templateService = new TemplateService();
            await templateService.SaveTemplateAsync(CurrentTemplate);
            
            // 通知PDF预览刷新
            _sharedDataService.BroadcastDataChange("TemplateChanged", CurrentTemplate.TemplateId);
        }

        [RelayCommand]
        private void ChangePaperSize(PaperSizeType paperType)
        {
            if (CurrentTemplate == null) return;

            CurrentPaperType = paperType;
            CurrentTemplate.Layout.SetPaperSize(paperType);
            PaperWidth = CurrentTemplate.Layout.PaperWidth;
            PaperHeight = CurrentTemplate.Layout.PaperHeight;
        }

        [RelayCommand]
        private void ToggleLandscape()
        {
            if (CurrentTemplate == null) return;

            IsLandscape = !IsLandscape;
            CurrentTemplate.Layout.IsLandscape = IsLandscape;
        }

        [RelayCommand]
        private void SetCustomPaperSize()
        {
            if (CurrentTemplate == null) return;

            CurrentPaperType = PaperSizeType.Custom;
            CurrentTemplate.Layout.SetCustomPaperSize(PaperWidth, PaperHeight);
        }

        [RelayCommand]
        private void BringToFront()
        {
            if (SelectedElement == null) return;

            var maxZIndex = CurrentTemplate.Layout.EditableElements.Max(e => e.ZIndex);
            SelectedElement.ZIndex = maxZIndex + 1;
        }

        [RelayCommand]
        private void SendToBack()
        {
            if (SelectedElement == null) return;

            var minZIndex = CurrentTemplate.Layout.EditableElements.Min(e => e.ZIndex);
            SelectedElement.ZIndex = minZIndex - 1;
        }

        [RelayCommand]
        private void CloneElement(ControlElement element = null)
        {
            var targetElement = element ?? SelectedElement;
            if (targetElement != null)
            {
                var clonedElement = targetElement.Clone();
                // 设置新的层级
                clonedElement.ZIndex = CurrentTemplate.Layout.EditableElements.Max(e => e.ZIndex) + 1;
                CurrentTemplate.Layout.EditableElements.Add(clonedElement);
                SelectedElement = clonedElement;
            }
        }

        private void UpdateElementValue(string key, object value)
        {
            // 查找对应的控件元素并更新其�?            var element = CurrentTemplate.Layout.EditableElements
                .FirstOrDefault(e => e.ElementId == key);

            if (element != null)
            {
                element.Value = value;
            }
        }

        private void UpdateElementZIndices()
        {
            // 重新计算所有元素的层级
            for (int i = 0; i < CurrentTemplate.Layout.EditableElements.Count; i++)
            {
                CurrentTemplate.Layout.EditableElements[i].ZIndex = i;
            }
        }

        [RelayCommand]
        private void Undo()
        {
            if (_commandHistory.CanUndo)
            {
                _commandHistory.Undo();
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }
        }

        [RelayCommand]
        private void Redo()
        {
            if (_commandHistory.CanRedo)
            {
                _commandHistory.Redo();
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }
        }
    }
}
*/

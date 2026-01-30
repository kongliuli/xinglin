# WPF MVVM模板编辑器项目架构设计

> 基于CommunityToolkit.Mvvm 8.2+的单窗口左右结构架构方案  
> 文档版本：v1.0  
> 创建日期：2026-01-30

---

## 一、架构概述

### 1.1 项目定位
- **应用类型**：模板编辑器系统
- **核心场景**：数据录入 + 可视化模板编辑 + PDF预览输出
- **技术栈**：WPF + .NET 8+ + CommunityToolkit.Mvvm + WebView2

### 1.2 架构分层

```
┌─────────────────────────────────────────────────────────────────┐
│                        表现层 (Presentation Layer)             │
│  ┌──────────────────────┐  ┌────────────────────────────────┐  │
│  │   Views (XAML)       │  │   ViewModels                  │  │
│  │   - MainWindow.xaml  │  │   - MainViewModel             │  │
│  │   - DataEntryPanel   │  │   - DataEntryViewModel        │  │
│  │   - TemplateEditor   │  │   - TemplateEditorViewModel   │  │
│  │   - PdfPreviewPanel  │  │   - PdfPreviewViewModel       │  │
│  └──────────────────────┘  └────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                        服务层 (Service Layer)                    │
│  ┌──────────────────────┐  ┌────────────────────────────────┐  │
│  │   数据服务           │  │   共享服务                    │  │
│  │   - TemplateService  │  │   - SharedDataService         │  │
│  │   - ConfigService    │  │   - MessageService            │  │
│  │   - PdfService       │  │   - ValidationService         │  │
│  └──────────────────────┘  └────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                        模型层 (Model Layer)                      │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │   CoreEntities       │   Configuration  │   Persistence  │   │
│  │   - TemplateData     │   - TemplateConfig │   - JSON    │   │
│  │   - LayoutMetadata   │   - ColumnConfig   │   - XML     │   │
│  │   - ControlElement   │   - ElementConfig  │             │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

### 1.3 项目目录结构

```
WpfMvvmTemplateEditor/
├── Models/                              # 数据模型层
│   ├── CoreEntities/                    # 核心实体
│   │   ├── TemplateData.cs
│   │   ├── LayoutMetadata.cs
│   │   ├── ControlElement.cs
│   │   └── TemplateConfig.cs
│   ├── Configuration/                  # 配置模型
│   │   ├── ColumnConfig.cs
│   │   ├── DropdownOptionConfig.cs
│   │   └── FixedRegionConfig.cs
│   └── Persistence/                     # 持久化模型
│       ├── TemplateFile.cs
│       └── ExportConfig.cs
│
├── ViewModels/                          # 视图模型层
│   ├── Base/
│   │   └── ViewModelBase.cs            # CommunityToolkit.Mvvm基类
│   ├── MainViewModel.cs
│   ├── DataEntryViewModel.cs
│   ├── TemplateEditorViewModel.cs
│   ├── PdfPreviewViewModel.cs
│   └── TemplateTreeViewModel.cs
│
├── Views/                               # 视图层
│   ├── MainWindow.xaml
│   ├── Panels/
│   │   ├── DataEntryPanel.xaml
│   │   ├── TemplateEditorPanel.xaml
│   │   ├── PdfPreviewPanel.xaml
│   │   └── TemplateTreePanel.xaml
│   └── Controls/
│       ├── DraggableCanvas.xaml
│       ├── EditableTextBox.xaml
│       └── CustomDataGrid.xaml
│
├── Services/                            # 服务层
│   ├── Data/
│   │   ├── ITemplateService.cs
│   │   ├── TemplateService.cs
│   │   └── ConfigService.cs
│   ├── Pdf/
│   │   ├── IPdfService.cs
│   │   └── PdfService.cs
│   ├── Shared/
│   │   ├── ISharedDataService.cs
│   │   ├── SharedDataService.cs
│   │   ├── IMessageService.cs
│   │   └── MessageService.cs
│   └── Validation/
│       ├── IValidationService.cs
│       └── ValidationService.cs
│
├── Converters/                          # 值转换器
│   ├── EditableStateConverter.cs
│   ├── ControlTypeTemplateSelector.cs
│   └── BoolToVisibilityConverter.cs
│
├── Behaviors/                           # 附加行为
│   ├── DragDropBehavior.cs
│   ├── ResizeBehavior.cs
│   └── SelectionBehavior.cs
│
├── Resources/                           # 资源文件
│   ├── Styles/
│   │   ├── App.xaml
│   │   ├── ControlStyles.xaml
│   │   └── DataGridStyles.xaml
│   ├── Templates/
│   │   ├── CellTemplates.xaml
│   │   └── HeaderTemplates.xaml
│   └── Icons/
│       ├── Folder.png
│       └── Document.png
│
├── Helpers/                             # 辅助类
│   ├── JsonHelper.cs
│   ├── FileHelper.cs
│   └── MathHelper.cs
│
├── Constants/                           # 常量定义
│   ├── Constants.cs
│   └── AppConstants.cs
│
├── App.xaml
├── App.xaml.cs
└── Program.cs
```

---

## 二、核心组件设计

### 2.1 数据共享机制

#### 2.1.1 共享数据服务接口

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace WpfMvvmTemplateEditor.Services.Shared
{
    /// <summary>
    /// 共享数据服务接口 - 管理跨ViewModel的数据共享
    /// </summary>
    public interface ISharedDataService
    {
        /// <summary>
        /// 当前选中的模板数据
        /// </summary>
        TemplateData CurrentTemplate { get; set; }

        /// <summary>
        /// 用户录入的数据字典
        /// </summary>
        ObservableDictionary<string, object> UserData { get; }

        /// <summary>
        /// 下拉框选项缓存
        /// </summary>
        ObservableDictionary<string, List<string>> DropdownOptions { get; }

        /// <summary>
        /// 更新用户数据
        /// </summary>
        void UpdateUserData(string key, object value);

        /// <summary>
        /// 广播数据变更消息
        /// </summary>
        void BroadcastDataChange(string key, object value);
    }
}
```

#### 2.1.2 共享数据服务实现

```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WpfMvvmTemplateEditor.Services.Shared
{
    /// <summary>
    /// 共享数据服务实现 - 单例模式
    /// </summary>
    public class SharedDataService : ObservableObject, ISharedDataService
    {
        private static readonly Lazy<SharedDataService> _instance =
            new(() => new SharedDataService());

        public static SharedDataService Instance => _instance.Value;

        private TemplateData _currentTemplate;
        private ObservableDictionary<string, object> _userData;
        private ObservableDictionary<string, List<string>> _dropdownOptions;

        public SharedDataService()
        {
            _userData = new ObservableDictionary<string, object>();
            _dropdownOptions = new ObservableDictionary<string, List<string>>();
        }

        public TemplateData CurrentTemplate
        {
            get => _currentTemplate;
            set => SetProperty(ref _currentTemplate, value);
        }

        public ObservableDictionary<string, object> UserData => _userData;

        public ObservableDictionary<string, List<string>> DropdownOptions => _dropdownOptions;

        public void UpdateUserData(string key, object value)
        {
            if (_userData.ContainsKey(key))
            {
                _userData[key] = value;
            }
            else
            {
                _userData.Add(key, value);
            }

            // 广播变更消息
            BroadcastDataChange(key, value);
        }

        public void BroadcastDataChange(string key, object value)
        {
            WeakReferenceMessenger.Default.Send(new DataChangedMessage(key, value));
        }
    }

    /// <summary>
    /// 数据变更消息
    /// </summary>
    public record DataChangedMessage(string Key, object Value);
}
```

### 2.2 消息通信服务

```csharp
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WpfMvvmTemplateEditor.Services.Shared
{
    /// <summary>
    /// 消息类型枚举
    /// </summary>
    public enum MessageType
    {
        DataChanged,
        TemplateChanged,
        SelectionChanged,
        ValidationFailed
    }

    /// <summary>
    /// 应用消息基类
    /// </summary>
    public record AppMessage(MessageType Type, object Payload);

    /// <summary>
    /// 模板选中消息
    /// </summary>
    public record TemplateSelectedMessage(string TemplateId) : AppMessage(MessageType.TemplateChanged, TemplateId);

    /// <summary>
    /// 控件选中消息
    /// </summary>
    public record ControlSelectedMessage(string ControlId) : AppMessage(MessageType.SelectionChanged, ControlId);
}
```

---

## 三、核心数据模型

### 3.1 模板数据模型

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfMvvmTemplateEditor.Models.CoreEntities
{
    /// <summary>
    /// 模板数据主类
    /// </summary>
    public partial class TemplateData : ObservableObject
    {
        [ObservableProperty]
        private string _templateId;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private LayoutMetadata _layout;

        [ObservableProperty]
        private TemplateConfig _config;

        [ObservableProperty]
        private DateTime _createdDate;

        [ObservableProperty]
        private DateTime _modifiedDate;

        public TemplateData()
        {
            _createdDate = DateTime.Now;
            _modifiedDate = DateTime.Now;
        }
    }
}
```

### 3.2 布局元数据模型

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace WpfMvvmTemplateEditor.Models.CoreEntities
{
    /// <summary>
    /// 布局元数据 - 包含所有控件的位置、大小等信息
    /// </summary>
    public partial class LayoutMetadata : ObservableObject
    {
        [ObservableProperty]
        private double _paperWidth = 210; // A4宽度(mm)

        [ObservableProperty]
        private double _paperHeight = 297; // A4高度(mm)

        [ObservableProperty]
        private string _paperType = "A4";

        /// <summary>
        /// 固定内容区域 - 不可编辑
        /// </summary>
        public ObservableCollection<ControlElement> FixedElements { get; set; }

        /// <summary>
        /// 可编辑内容区域 - 可拖拽、调整大小
        /// </summary>
        public ObservableCollection<ControlElement> EditableElements { get; set; }

        public LayoutMetadata()
        {
            FixedElements = new ObservableCollection<ControlElement>();
            EditableElements = new ObservableCollection<ControlElement>();
        }
    }
}
```

### 3.3 控件元素模型

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace WpfMvvmTemplateEditor.Models.CoreEntities
{
    /// <summary>
    /// 控件类型枚举
    /// </summary>
    public enum ControlType
    {
        TextBox,
        ComboBox,
        DatePicker,
        CheckBox,
        Table,
        Image,
        FixedText
    }

    /// <summary>
    /// 可编辑状态枚举
    /// </summary>
    public enum EditableState
    {
        ReadOnly,      // 只读（固定内容）
        Editable,      // 可编辑
        Locked          // 锁定（临时不可编辑）
    }

    /// <summary>
    /// 控件元素 - 表示画布上的一个控件
    /// </summary>
    public partial class ControlElement : ObservableObject
    {
        [ObservableProperty]
        private string _elementId;

        [ObservableProperty]
        private ControlType _type;

        [ObservableProperty]
        private string _displayName;

        [ObservableProperty]
        private double _x;

        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;

        [ObservableProperty]
        private object _value;

        [ObservableProperty]
        private EditableState _editState;

        /// <summary>
        /// 是否可移动
        /// </summary>
        public bool CanMove => EditState != EditableState.ReadOnly;

        /// <summary>
        /// 是否可调整大小
        /// </summary>
        public bool CanResize => EditState == EditableState.Editable;

        /// <summary>
        /// 扩展属性 - 用于存储特定控件的配置
        /// 如表格的列配置、下拉框的选项等
        /// </summary>
        public ObservableCollection<KeyValuePair<string, object>> Properties { get; set; }

        public ControlElement()
        {
            Properties = new ObservableCollection<KeyValuePair<string, object>>();
        }
    }
}
```

### 3.4 模板配置模型

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfMvvmTemplateEditor.Models.CoreEntities
{
    /// <summary>
    /// 模板配置
    /// </summary>
    public partial class TemplateConfig : ObservableObject
    {
        [ObservableProperty]
        private string _version;

        [ObservableProperty]
        private bool _isLocked;

        [ObservableProperty]
        private string _author;

        [ObservableProperty]
        private string _category;

        [ObservableProperty]
        private Dictionary<string, object> _metadata;
    }
}
```

### 3.5 表格列配置模型

```csharp
namespace WpfMvvmTemplateEditor.Models.Configuration
{
    /// <summary>
    /// 单元格控件类型
    /// </summary>
    public enum CellControlType
    {
        TextBox,
        ComboBox,
        DatePicker,
        CheckBox,
        Numeric
    }

    /// <summary>
    /// 表格列配置
    /// </summary>
    public class ColumnConfig
    {
        /// <summary>
        /// 列ID
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// 列头显示文本
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// 单元格控件类型
        /// </summary>
        public CellControlType ControlType { get; set; }

        /// <summary>
        /// 数据格式字符串
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// 下拉框选项（当ControlType为ComboBox时使用）
        /// </summary>
        public List<string> DropdownOptions { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 最小宽度
        /// </summary>
        public double MinWidth { get; set; }
    }

    /// <summary>
    /// 表格配置
    /// </summary>
    public class TableConfig
    {
        /// <summary>
        /// 表格ID
        /// </summary>
        public string TableId { get; set; }

        /// <summary>
        /// 列配置集合
        /// </summary>
        public List<ColumnConfig> Columns { get; set; }

        /// <summary>
        /// 默认行数
        /// </summary>
        public int DefaultRowCount { get; set; }

        /// <summary>
        /// 是否允许添加行
        /// </summary>
        public bool AllowAddRow { get; set; }

        /// <summary>
        /// 是否允许删除行
        /// </summary>
        public bool AllowDeleteRow { get; set; }
    }
}
```

---

## 四、核心ViewModel设计

### 4.1 主窗口ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfMvvmTemplateEditor.Services.Shared;

namespace WpfMvvmTemplateEditor.ViewModels
{
    /// <summary>
    /// 主窗口ViewModel
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly ISharedDataService _sharedDataService;

        [ObservableProperty]
        private string _windowTitle = "WPF模板编辑器";

        [ObservableProperty]
        private bool _isLoading;

        public MainViewModel(ISharedDataService sharedDataService)
        {
            _sharedDataService = sharedDataService;
        }

        [RelayCommand]
        private async Task InitializeAsync()
        {
            IsLoading = true;

            try
            {
                // 初始化应用数据
                await LoadInitialDataAsync();

                // 注册消息监听
                RegisterMessageHandlers();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadInitialDataAsync()
        {
            // 加载初始模板
            var templateService = new TemplateService();
            var defaultTemplate = await templateService.LoadDefaultTemplateAsync();

            _sharedDataService.CurrentTemplate = defaultTemplate;
        }

        private void RegisterMessageHandlers()
        {
            // 监听模板变更消息
            WeakReferenceMessenger.Default.Register<TemplateSelectedMessage>(this, (r, m) =>
            {
                // 处理模板选中逻辑
            });
        }

        [RelayCommand]
        private void ExitApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
```

### 4.2 数据录入面板ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WpfMvvmTemplateEditor.Services.Shared;

namespace WpfMvvmTemplateEditor.ViewModels
{
    /// <summary>
    /// 数据录入面板ViewModel
    /// </summary>
    public partial class DataEntryViewModel : ObservableObject
    {
        private readonly ISharedDataService _sharedDataService;

        [ObservableProperty]
        private string _userName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private DateTime _birthDate;

        [ObservableProperty]
        private string _department;

        public ObservableCollection<string> Departments { get; set; }

        public DataEntryViewModel(ISharedDataService sharedDataService)
        {
            _sharedDataService = sharedDataService;
            Departments = new ObservableCollection<string>();

            // 注册数据变更监听
            RegisterDataChangeHandlers();
        }

        partial void OnUserNameChanged(string value)
        {
            _sharedDataService.UpdateUserData("UserName", value);
        }

        partial void OnEmailChanged(string value)
        {
            _sharedDataService.UpdateUserData("Email", value);
        }

        partial void OnBirthDateChanged(DateTime value)
        {
            _sharedDataService.UpdateUserData("BirthDate", value);
        }

        partial void OnDepartmentChanged(string value)
        {
            _sharedDataService.UpdateUserData("Department", value);
        }

        private void RegisterDataChangeHandlers()
        {
            WeakReferenceMessenger.Default.Register<DataChangedMessage>(this, (r, m) =>
            {
                // 处理跨面板的数据变更
                HandleDataChange(m.Key, m.Value);
            });
        }

        private void HandleDataChange(string key, object value)
        {
            // 根据key更新对应的属性
            switch (key)
            {
                case "UserName":
                    UserName = value?.ToString();
                    break;
                case "Email":
                    Email = value?.ToString();
                    break;
                case "Department":
                    Department = value?.ToString();
                    break;
            }
        }

        [RelayCommand]
        private async Task SaveDataAsync()
        {
            // 保存数据逻辑
            await Task.CompletedTask;
        }

        [RelayCommand]
        private void ResetData()
        {
            // 重置数据逻辑
        }
    }
}
```

### 4.3 模板编辑器ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfMvvmTemplateEditor.Models.CoreEntities;
using WpfMvvmTemplateEditor.Services.Shared;

namespace WpfMvvmTemplateEditor.ViewModels
{
    /// <summary>
    /// 模板编辑器ViewModel
    /// </summary>
    public partial class TemplateEditorViewModel : ObservableObject
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

        public TemplateEditorViewModel(ISharedDataService sharedDataService)
        {
            _sharedDataService = sharedDataService;

            // 监听模板变更
            WeakReferenceMessenger.Default.Register<TemplateSelectedMessage>(this, (r, m) =>
            {
                LoadTemplateAsync(m.TemplateId);
            });

            // 监听数据变更
            WeakReferenceMessenger.Default.Register<DataChangedMessage>(this, (r, m) =>
            {
                UpdateElementValue(m.Key, m.Value);
            });
        }

        [RelayCommand]
        private async Task LoadTemplateAsync(string templateId)
        {
            var templateService = new TemplateService();
            CurrentTemplate = await templateService.GetTemplateByIdAsync(templateId);

            PaperWidth = CurrentTemplate.Layout.PaperWidth;
            PaperHeight = CurrentTemplate.Layout.PaperHeight;
        }

        [RelayCommand]
        private void AddElement(ControlType type)
        {
            var newElement = new ControlElement
            {
                ElementId = Guid.NewGuid().ToString(),
                Type = type,
                X = 50,
                Y = 50,
                Width = 100,
                Height = 30,
                EditState = EditableState.Editable
            };

            CurrentTemplate.Layout.EditableElements.Add(newElement);
            SelectedElement = newElement;
        }

        [RelayCommand]
        private void DeleteElement()
        {
            if (SelectedElement != null)
            {
                CurrentTemplate.Layout.EditableElements.Remove(SelectedElement);
                SelectedElement = null;
            }
        }

        [RelayCommand]
        private async Task SaveTemplateAsync()
        {
            var templateService = new TemplateService();
            await templateService.SaveTemplateAsync(CurrentTemplate);
        }

        private void UpdateElementValue(string key, object value)
        {
            // 查找对应的控件元素并更新其值
            var element = CurrentTemplate.Layout.EditableElements
                .FirstOrDefault(e => e.ElementId == key);

            if (element != null)
            {
                element.Value = value;
            }
        }
    }
}
```

### 4.4 PDF预览ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Wpf;

namespace WpfMvvmTemplateEditor.ViewModels
{
    /// <summary>
    /// PDF预览ViewModel
    /// </summary>
    public partial class PdfPreviewViewModel : ObservableObject
    {
        private WebView2 _webView;

        [ObservableProperty]
        private string _pdfFilePath;

        [ObservableProperty]
        private bool _isPdfLoaded;

        [ObservableProperty]
        private double _zoomLevel = 1.0;

        public PdfPreviewViewModel()
        {
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await Task.CompletedTask;
            // WebView初始化在View层完成
        }

        public void SetWebView(WebView2 webView)
        {
            _webView = webView;
        }

        [RelayCommand]
        private async Task LoadPdfAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("PDF文件不存在", filePath);
            }

            PdfFilePath = filePath;

            await _webView.EnsureCoreWebView2Async(null);

            var fileUrl = $"file:///{filePath.Replace('\\', '/')}";
            _webView.CoreWebView2.Navigate(fileUrl);

            IsPdfLoaded = true;
        }

        [RelayCommand]
        private async Task RefreshPdfAsync()
        {
            if (!string.IsNullOrEmpty(PdfFilePath))
            {
                await LoadPdfAsync(PdfFilePath);
            }
        }

        [RelayCommand]
        private void ZoomIn()
        {
            if (_webView?.CoreWebView2 != null)
            {
                ZoomLevel = Math.Min(ZoomLevel + 0.1, 3.0);
                _webView.CoreWebView2.ExecuteScriptAsync($"document.body.style.zoom = {ZoomLevel}");
            }
        }

        [RelayCommand]
        private void ZoomOut()
        {
            if (_webView?.CoreWebView2 != null)
            {
                ZoomLevel = Math.Max(ZoomLevel - 0.1, 0.5);
                _webView.CoreWebView2.ExecuteScriptAsync($"document.body.style.zoom = {ZoomLevel}");
            }
        }

        [RelayCommand]
        private async Task PrintPdfAsync()
        {
            if (_webView?.CoreWebView2 != null)
            {
                await _webView.CoreWebView2.ExecuteScriptAsync("window.print()");
            }
        }

        [RelayCommand]
        private async Task ExportPdfAsync(string outputPath)
        {
            // PDF导出逻辑
            await Task.CompletedTask;
        }
    }
}
```

---

## 五、关键XAML视图设计

### 5.1 主窗口布局

```xml
<Window x:Class="WpfMvvmTemplateEditor.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:views="clr-namespace:WpfMvvmTemplateEditor.Views"
        Title="WPF模板编辑器" Height="768" Width="1024"
        WindowStartupLocation="CenterScreen"
        WindowState="Maximized">

    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="200"/> <!-- 左侧模板树 -->
            <ColumnDefinition Width="*"/>   <!-- 主内容区 -->
        </Grid.ColumnDefinitions>

        <!-- 左侧模板管理树 -->
        <views:TemplateTreePanel Grid.Column="0" Margin="5"/>

        <!-- 右侧主内容区 -->
        <Grid Grid.Column="1" Margin="5">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/> <!-- 工具栏 -->
                <RowDefinition Height="*"/>    <!-- 内容区 -->
            </Grid.RowDefinitions>

            <!-- 工具栏 -->
            <ToolBar Grid.Row="0">
                <Button Content="新建模板"/>
                <Button Content="打开模板"/>
                <Button Content="保存模板"/>
                <Separator/>
                <Button Content="生成PDF"/>
                <Separator/>
                <Button Content="预览PDF"/>
            </ToolBar>

            <!-- 内容区 -->
            <TabControl Grid.Row="1">
                <TabItem Header="数据录入">
                    <views:DataEntryPanel/>
                </TabItem>
                <TabItem Header="模板编辑">
                    <views:TemplateEditorPanel/>
                </TabItem>
                <TabItem Header="PDF预览">
                    <views:PdfPreviewPanel/>
                </TabItem>
            </TabControl>
        </Grid>
    </Grid>
</Window>
```

### 5.2 模板编辑器面板

```xml
<UserControl x:Class="WpfMvvmTemplateEditor.Views.Panels.TemplateEditorPanel"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:WpfMvvmTemplateEditor.Behaviors"
             xmlns:converters="clr-namespace:WpfMvvmTemplateEditor.Converters">

    <UserControl.Resources>
        <converters:EditableStateConverter x:Key="EditableStateConverter"/>
    </UserControl.Resources>

    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto"/> <!-- 控件工具箱 -->
            <ColumnDefinition Width="*"/>    <!-- 画布区域 -->
            <ColumnDefinition Width="250"/>  <!-- 属性面板 -->
        </Grid.ColumnDefinitions>

        <!-- 控件工具箱 -->
        <StackPanel Grid.Column="0" Width="100" Margin="5">
            <TextBlock Text="控件工具箱" FontWeight="Bold" Margin="0,0,0,10"/>

            <Button Content="文本框" Command="{Binding AddElementCommand}"
                    CommandParameter="{x:Static local:ControlType.TextBox}" Margin="0,2"/>
            <Button Content="下拉框" Command="{Binding AddElementCommand}"
                    CommandParameter="{x:Static local:ControlType.ComboBox}" Margin="0,2"/>
            <Button Content="日期选择" Command="{Binding AddElementCommand}"
                    CommandParameter="{x:Static local:ControlType.DatePicker}" Margin="0,2"/>
            <Button Content="表格" Command="{Binding AddElementCommand}"
                    CommandParameter="{x:Static local:ControlType.Table}" Margin="0,2"/>

            <Separator Margin="0,10"/>
            <TextBlock Text="操作" FontWeight="Bold" Margin="0,0,0,10"/>
            <Button Content="删除选中" Command="{Binding DeleteElementCommand}" Margin="0,2"/>
            <Button Content="保存模板" Command="{Binding SaveTemplateCommand}" Margin="0,2"/>
        </StackPanel>

        <!-- 画布区域 -->
        <ScrollViewer Grid.Column="1" HorizontalScrollBarVisibility="Auto"
                      VerticalScrollBarVisibility="Auto">
            <Border BorderBrush="Gray" BorderThickness="1" Background="White"
                    Width="{Binding PaperWidth}" Height="{Binding PaperHeight}">
                <Canvas>
                    <!-- 固定内容区域 -->
                    <ItemsControl ItemsSource="{Binding CurrentTemplate.Layout.FixedElements}">
                        <ItemsControl.ItemsPanel>
                            <ItemsPanelTemplate>
                                <Canvas/>
                            </ItemsPanelTemplate>
                        </ItemsControl.ItemsPanel>
                        <ItemsControl.ItemContainerStyle>
                            <Style>
                                <Setter Property="Canvas.Left" Value="{Binding X}"/>
                                <Setter Property="Canvas.Top" Value="{Binding Y}"/>
                                <Setter Property="Canvas.ZIndex" Value="1"/>
                            </Style>
                        </ItemsControl.ItemContainerStyle>
                        <ItemsControl.ItemTemplate>
                            <DataTemplate>
                                <Border Width="{Binding Width}" Height="{Binding Height}"
                                        Background="LightGray" IsEnabled="False">
                                    <TextBlock Text="{Binding Value}" VerticalAlignment="Center"
                                               HorizontalAlignment="Center"/>
                                </Border>
                            </DataTemplate>
                        </ItemsControl.ItemTemplate>
                    </ItemsControl>

                    <!-- 可编辑内容区域 -->
                    <ItemsControl ItemsSource="{Binding CurrentTemplate.Layout.EditableElements}">
                        <ItemsControl.ItemsPanel>
                            <ItemsPanelTemplate>
                                <Canvas/>
                            </ItemsPanelTemplate>
                        </ItemsControl.ItemsPanel>
                        <ItemsControl.ItemContainerStyle>
                            <Style>
                                <Setter Property="Canvas.Left" Value="{Binding X}"/>
                                <Setter Property="Canvas.Top" Value="{Binding Y}"/>
                                <Setter Property="local:DragDropBehavior.IsDragEnabled" Value="{Binding CanMove}"/>
                            </Style>
                        </ItemsControl.ItemContainerStyle>
                        <ItemsControl.ItemTemplate>
                            <DataTemplate>
                                <Border Width="{Binding Width}" Height="{Binding Height}"
                                        BorderBrush="{Binding IsSelected, Converter={StaticResource EditableStateConverter}}"
                                        BorderThickness="2">
                                    <!-- 根据控件类型显示不同内容 -->
                                    <ContentControl Content="{Binding}">
                                        <ContentControl.ContentTemplateSelector>
                                            <local:ControlTemplateSelector>
                                                <local:ControlTemplateSelector.TextBoxTemplate>
                                                    <DataTemplate>
                                                        <TextBox Text="{Binding Value, Mode=TwoWay}"
                                                                 IsEnabled="{Binding CanMove}"/>
                                                    </DataTemplate>
                                                </local:ControlTemplateSelector.TextBoxTemplate>
                                                <local:ControlTemplateSelector.ComboBoxTemplate>
                                                    <DataTemplate>
                                                        <ComboBox ItemsSource="{Binding Properties[Options]}"
                                                                  SelectedItem="{Binding Value}"/>
                                                    </DataTemplate>
                                                </local:ControlTemplateSelector.ComboBoxTemplate>
                                            </local:ControlTemplateSelector>
                                        </ContentControl.ContentTemplateSelector>
                                    </ContentControl>
                                </Border>
                            </DataTemplate>
                        </ItemsControl.ItemTemplate>
                    </ItemsControl>
                </Canvas>
            </Border>
        </ScrollViewer>

        <!-- 属性面板 -->
        <StackPanel Grid.Column="2" Margin="5" DataContext="{Binding SelectedElement}">
            <TextBlock Text="属性面板" FontWeight="Bold" Margin="0,0,0,10"/>

            <TextBlock Text="X坐标"/>
            <TextBox Text="{Binding X, Mode=TwoWay}"/>

            <TextBlock Text="Y坐标"/>
            <TextBox Text="{Binding Y, Mode=TwoWorld}"/>

            <TextBlock Text="宽度"/>
            <TextBox Text="{Binding Width, Mode=TwoWorld}"/>

            <TextBlock Text="高度"/>
            <TextBox Text="{Binding Height, Mode=TwoWorld}"/>

            <TextBlock Text="可编辑状态"/>
            <ComboBox SelectedItem="{Binding EditState}">
                <ComboBox.Items>
                    <local:EditableState>ReadOnly</local:EditableState>
                    <local:EditableState>Editable</local:EditableState>
                    <local:EditableState>Locked</local:EditableState>
                </ComboBox.Items>
            </ComboBox>
        </StackPanel>
    </Grid>
</UserControl>
```

### 5.3 PDF预览面板

```xml
<UserControl x:Class="WpfMvvmTemplateEditor.Views.Panels.PdfPreviewPanel"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:wv2="clr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/> <!-- 工具栏 -->
            <RowDefinition Height="*"/>    <!-- WebView2 -->
        </Grid.RowDefinitions>

        <!-- PDF工具栏 -->
        <ToolBar Grid.Row="0">
            <Button Content="刷新" Command="{Binding RefreshPdfCommand}"/>
            <Separator/>
            <Button Content="放大" Command="{Binding ZoomInCommand}"/>
            <Button Content="缩小" Command="{Binding ZoomOutCommand}"/>
            <Separator/>
            <Button Content="打印" Command="{Binding PrintPdfCommand}"/>
            <Button Content="导出" Command="{Binding ExportPdfCommand}"/>
            <Separator/>
            <TextBlock Text="缩放:" VerticalAlignment="Center"/>
            <TextBlock Text="{Binding ZoomLevel, StringFormat={}{0:F1}x}"
                       VerticalAlignment="Center" Margin="5,0,0,0"/>
        </ToolBar>

        <!-- WebView2控件 -->
        <wv2:WebView2 Grid.Row="1" x:Name="PdfWebView"
                      HorizontalAlignment="Stretch" VerticalAlignment="Stretch"
                      DefaultBackgroundColor="White"
                      Loaded="PdfWebView_Loaded"/>
    </Grid>
</UserControl>
```

---

## 六、服务层设计

### 6.1 模板服务接口与实现

```csharp
using WpfMvvmTemplateEditor.Models.CoreEntities;

namespace WpfMvvmTemplateEditor.Services.Data
{
    /// <summary>
    /// 模板服务接口
    /// </summary>
    public interface ITemplateService
    {
        /// <summary>
        /// 获取所有模板
        /// </summary>
        Task<List<TemplateData>> GetAllTemplatesAsync();

        /// <summary>
        /// 根据ID获取模板
        /// </summary>
        Task<TemplateData> GetTemplateByIdAsync(string templateId);

        /// <summary>
        /// 保存模板
        /// </summary>
        Task SaveTemplateAsync(TemplateData template);

        /// <summary>
        /// 删除模板
        /// </summary>
        Task DeleteTemplateAsync(string templateId);

        /// <summary>
        /// 加载默认模板
        /// </summary>
        Task<TemplateData> LoadDefaultTemplateAsync();
    }
}
```

```csharp
using System.Text.Json;
using WpfMvvmTemplateEditor.Models.CoreEntities;

namespace WpfMvvmTemplateEditor.Services.Data
{
    /// <summary>
    /// 模板服务实现
    /// </summary>
    public class TemplateService : ITemplateService
    {
        private readonly string _templatesDirectory;
        private readonly string _templatesIndexFile;

        public TemplateService()
        {
            _templatesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            _templatesIndexFile = Path.Combine(_templatesDirectory, "templates.json");

            EnsureTemplatesDirectory();
        }

        private void EnsureTemplatesDirectory()
        {
            if (!Directory.Exists(_templatesDirectory))
            {
                Directory.CreateDirectory(_templatesDirectory);
            }
        }

        public async Task<List<TemplateData>> GetAllTemplatesAsync()
        {
            if (!File.Exists(_templatesIndexFile))
            {
                return new List<TemplateData>();
            }

            var json = await File.ReadAllTextAsync(_templatesIndexFile);
            return JsonSerializer.Deserialize<List<TemplateData>>(json) ?? new List<TemplateData>();
        }

        public async Task<TemplateData> GetTemplateByIdAsync(string templateId)
        {
            var templateFile = Path.Combine(_templatesDirectory, $"{templateId}.json");

            if (!File.Exists(templateFile))
            {
                throw new FileNotFoundException("模板文件不存在", templateFile);
            }

            var json = await File.ReadAllTextAsync(templateFile);
            return JsonSerializer.Deserialize<TemplateData>(json);
        }

        public async Task SaveTemplateAsync(TemplateData template)
        {
            template.ModifiedDate = DateTime.Now;

            var templateFile = Path.Combine(_templatesDirectory, $"{template.TemplateId}.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(template, options);

            await File.WriteAllTextAsync(templateFile, json);

            // 更新索引文件
            await UpdateTemplateIndexAsync(template);
        }

        public async Task DeleteTemplateAsync(string templateId)
        {
            var templateFile = Path.Combine(_templatesDirectory, $"{templateId}.json");

            if (File.Exists(templateFile))
            {
                File.Delete(templateFile);
            }

            await UpdateTemplateIndexAsync(templateId, isDelete: true);
        }

        public async Task<TemplateData> LoadDefaultTemplateAsync()
        {
            var defaultTemplate = new TemplateData
            {
                TemplateId = "default",
                Name = "默认模板",
                Layout = new LayoutMetadata
                {
                    PaperWidth = 210,
                    PaperHeight = 297,
                    PaperType = "A4"
                },
                Config = new TemplateConfig
                {
                    Version = "1.0",
                    Author = "系统",
                    Category = "通用"
                }
            };

            // 添加一些固定内容
            defaultTemplate.Layout.FixedElements.Add(new ControlElement
            {
                ElementId = "header_title",
                Type = ControlType.FixedText,
                DisplayName = "标题",
                X = 50,
                Y = 20,
                Width = 110,
                Height = 20,
                EditState = EditableState.ReadOnly,
                Value = "模板标题"
            });

            return await Task.FromResult(defaultTemplate);
        }

        private async Task UpdateTemplateIndexAsync(TemplateData template)
        {
            var templates = await GetAllTemplatesAsync();

            var existingIndex = templates.FindIndex(t => t.TemplateId == template.TemplateId);
            if (existingIndex >= 0)
            {
                templates[existingIndex] = template;
            }
            else
            {
                templates.Add(template);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(templates, options);
            await File.WriteAllTextAsync(_templatesIndexFile, json);
        }

        private async Task UpdateTemplateIndexAsync(string templateId, bool isDelete)
        {
            var templates = await GetAllTemplatesAsync();

            if (isDelete)
            {
                templates.RemoveAll(t => t.TemplateId == templateId);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(templates, options);
            await File.WriteAllTextAsync(_templatesIndexFile, json);
        }
    }
}
```

### 6.2 PDF服务接口与实现

```csharp
namespace WpfMvvmTemplateEditor.Services.Pdf
{
    /// <summary>
    /// PDF服务接口
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// 从模板生成PDF
        /// </summary>
        Task<string> GeneratePdfFromTemplateAsync(TemplateData template, Dictionary<string, object> userData);

        /// <summary>
        /// 合并多个PDF文件
        /// </summary>
        Task<string> MergePdfFilesAsync(List<string> pdfFiles);

        /// <summary>
        /// 验证PDF文件
        /// </summary>
        Task<bool> ValidatePdfAsync(string filePath);
    }
}
```

```csharp
using WpfMvvmTemplateEditor.Models.CoreEntities;

namespace WpfMvvmTemplateEditor.Services.Pdf
{
    /// <summary>
    /// PDF服务实现 - 使用WebView2生成PDF
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly string _outputDirectory;

        public PdfService()
        {
            _outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
        }

        public async Task<string> GeneratePdfFromTemplateAsync(TemplateData template, Dictionary<string, object> userData)
        {
            // 生成HTML模板
            var html = GenerateHtmlFromTemplate(template, userData);

            // 保存为临时HTML文件
            var tempHtmlFile = Path.Combine(_outputDirectory, $"temp_{Guid.NewGuid()}.html");
            await File.WriteAllTextAsync(tempHtmlFile, html);

            // 使用浏览器打印为PDF（需要配合WebView2）
            var pdfFile = Path.Combine(_outputDirectory, $"{template.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            // 这里需要实际的PDF生成逻辑
            // 可以使用Chrome DevTools Protocol或其他PDF生成库
            // 实现略...

            return pdfFile;
        }

        public async Task<string> MergePdfFilesAsync(List<string> pdfFiles)
        {
            // 使用PdfSharp或其他PDF合并库
            // 实现略...
            return await Task.FromResult(string.Empty);
        }

        public async Task<bool> ValidatePdfAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            // 验证PDF文件头部标识
            var header = new byte[4];
            using (var stream = File.OpenRead(filePath))
            {
                await stream.ReadAsync(header, 0, 4);
            }

            return header[0] == 0x25 && header[1] == 0x50 &&
                   header[2] == 0x44 && header[3] == 0x46; // %PDF
        }

        private string GenerateHtmlFromTemplate(TemplateData template, Dictionary<string, object> userData)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }");
            html.AppendLine(".fixed-element { position: absolute; }");
            html.AppendLine(".editable-element { position: absolute; border: 1px dashed #ccc; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            // 渲染固定元素
            foreach (var element in template.Layout.FixedElements)
            {
                html.AppendLine($"<div class='fixed-element' style='left:{element.X}px; top:{element.Y}px; width:{element.Width}px; height:{element.Height}px;'>");
                html.AppendLine(element.Value?.ToString() ?? "");
                html.AppendLine("</div>");
            }

            // 渲染可编辑元素
            foreach (var element in template.Layout.EditableElements)
            {
                var value = userData.ContainsKey(element.ElementId) ?
                    userData[element.ElementId]?.ToString() : element.Value?.ToString();

                html.AppendLine($"<div class='editable-element' style='left:{element.X}px; top:{element.Y}px; width:{element.Width}px; height:{element.Height}px;'>");
                html.AppendLine(value ?? "");
                html.AppendLine("</div>");
            }

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }
    }
}
```

---

## 七、附加行为与转换器

### 7.1 拖拽行为

```csharp
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfMvvmTemplateEditor.Behaviors
{
    /// <summary>
    /// Canvas拖拽附加行为
    /// </summary>
    public static class DragDropBehavior
    {
        public static readonly DependencyProperty IsDragEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsDragEnabled",
                typeof(bool),
                typeof(DragDropBehavior),
                new PropertyMetadata(false, OnIsDragEnabledChanged));

        public static bool GetIsDragEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragEnabledProperty);
        }

        public static void SetIsDragEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragEnabledProperty, value);
        }

        private static void OnIsDragEnabledChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
                    element.MouseMove += Element_MouseMove;
                    element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
                }
                else
                {
                    element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
                    element.MouseMove -= Element_MouseMove;
                    element.MouseLeftButtonUp -= Element_MouseLeftButtonUp;
                }
            }
        }

        private static bool _isDragging;
        private static Point _startPosition;

        private static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _startPosition = e.GetPosition(null);
            (sender as UIElement).CaptureMouse();
        }

        private static void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var element = sender as FrameworkElement;
            var position = e.GetPosition(null);
            var deltaX = position.X - _startPosition.X;
            var deltaY = position.Y - _startPosition.Y;

            if (element.Parent is Canvas canvas)
            {
                var currentX = Canvas.GetLeft(element);
                var currentY = Canvas.GetTop(element);

                Canvas.SetLeft(element, currentX + deltaX);
                Canvas.SetTop(element, currentY + deltaY);

                _startPosition = position;

                // 通知ViewModel更新位置
                if (element.DataContext is ControlElement controlElement)
                {
                    controlElement.X = currentX + deltaX;
                    controlElement.Y = currentY + deltaY;
                }
            }
        }

        private static void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            (sender as UIElement).ReleaseMouseCapture();
        }
    }
}
```

### 7.2 可编辑状态转换器

```csharp
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfMvvmTemplateEditor.Converters
{
    /// <summary>
    /// 可编辑状态转换为边框颜色
    /// </summary>
    public class EditableStateToBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditableState state)
            {
                return state switch
                {
                    EditableState.ReadOnly => new SolidColorBrush(Colors.Gray),
                    EditableState.Editable => new SolidColorBrush(Colors.Blue),
                    EditableState.Locked => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.Transparent)
                };
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 可编辑状态转换为可见性
    /// </summary>
    public class EditableStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditableState state)
            {
                return state == EditableState.Editable ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

### 7.3 控件模板选择器

```csharp
using System.Windows;
using System.Windows.Controls;

namespace WpfMvvmTemplateEditor.Converters
{
    /// <summary>
    /// 控件类型模板选择器
    /// </summary>
    public class ControlTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextBoxTemplate { get; set; }
        public DataTemplate ComboBoxTemplate { get; set; }
        public DataTemplate DatePickerTemplate { get; set; }
        public DataTemplate CheckBoxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ControlElement element)
            {
                return element.Type switch
                {
                    ControlType.TextBox => TextBoxTemplate,
                    ControlType.ComboBox => ComboBoxTemplate,
                    ControlType.DatePicker => DatePickerTemplate,
                    ControlType.CheckBox => CheckBoxTemplate,
                    _ => base.SelectTemplate(item, container)
                };
            }

            return base.SelectTemplate(item, container);
        }
    }
}
```

---

## 八、App.xaml配置

### 8.1 App.xaml资源定义

```xml
<Application x:Class="WpfMvvmTemplateEditor.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="Views/MainWindow.xaml">

    <Application.Resources>
        <!-- 全局样式 -->
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Default.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Blue.xaml" />
            </ResourceDictionary.MergedDictionaries>

            <!-- 控件样式 -->
            <Style TargetType="Button" BasedOn="{StaticResource MaterialDesignRaisedButton}">
                <Setter Property="Margin" Value="5"/>
                <Setter Property="Padding" Value="15,8"/>
            </Style>

            <Style TargetType="TextBox" BasedOn="{StaticResource MaterialDesignOutlinedTextBox}">
                <Setter Property="Margin" Value="5"/>
                <Setter Property="Padding" Value="5"/>
            </Style>

            <Style TargetType="ComboBox" BasedOn="{StaticResource MaterialDesignOutlinedComboBox}">
                <Setter Property="Margin" Value="5"/>
            </Style>

            <!-- DataGrid样式 -->
            <Style TargetType="DataGrid" BasedOn="{StaticResource MaterialDesignDataGrid}">
                <Setter Property="AutoGenerateColumns" Value="False"/>
                <Setter Property="CanUserAddRows" Value="False"/>
                <Setter Property="CanUserDeleteRows" Value="False"/>
                <Setter Property="SelectionMode" Value="Single"/>
                <Setter Property="GridLinesVisibility" Value="All"/>
                <Setter Property="HeadersVisibility" Value="Column"/>
            </Style>

            <!-- 控件模板 -->
            <DataTemplate x:Key="TextBoxCellTemplate">
                <TextBox Text="{Binding Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                         BorderThickness="0" Background="Transparent"/>
            </DataTemplate>

            <DataTemplate x:Key="ComboBoxCellTemplate">
                <ComboBox ItemsSource="{Binding Properties[DropdownOptions]}"
                          SelectedItem="{Binding Value}"
                          BorderThickness="0" Background="Transparent"/>
            </DataTemplate>

            <DataTemplate x:Key="DatePickerCellTemplate">
                <DatePicker SelectedDate="{Binding Value, Mode=TwoWay}"
                           BorderThickness="0" Background="Transparent"/>
            </DataTemplate>

            <DataTemplate x:Key="CheckBoxCellTemplate">
                <CheckBox IsChecked="{Binding Value, Mode=TwoWay}"
                          HorizontalAlignment="Center" VerticalAlignment="Center"/>
            </DataTemplate>

        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### 8.2 App.xaml.cs依赖注入配置

```csharp
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfMvvmTemplateEditor.Services.Shared;
using WpfMvvmTemplateEditor.ViewModels;
using WpfMvvmTemplateEditor.Views;

namespace WpfMvvmTemplateEditor
{
    public partial class App : Application
    {
        public App()
        {
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            // 使用CommunityToolkit.Mvvm的依赖注入
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddSingleton<ISharedDataService, SharedDataService>()
                    .AddTransient<MainViewModel>()
                    .AddTransient<DataEntryViewModel>()
                    .AddTransient<TemplateEditorViewModel>()
                    .AddTransient<PdfPreviewViewModel>()
                    .BuildServiceProvider());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 创建主窗口并注入ViewModel
            var mainWindow = new MainWindow
            {
                DataContext = Ioc.Default.GetRequiredService<MainViewModel>()
            };

            mainWindow.Show();
        }
    }
}
```

---

## 九、关键技术点说明

### 9.1 CommunityToolkit.Mvvm特性使用

1. **源生成器自动生成属性和命令**
   - `[ObservableProperty]` 自动生成属性通知
   - `[RelayCommand]` 自动生成ICommand实现

2. **弱引用消息传递**
   - 避免内存泄漏
   - 跨ViewModel通信

3. **ObservableValidator**
   - 数据验证
   - 错误提示

### 9.2 WebView2 PDF预览优化

1. **启用GPU加速**
   ```csharp
   await webView.EnsureCoreWebView2Async(new CoreWebView2EnvironmentOptions
   {
       AdditionalBrowserArguments = "--enable-gpu-rasterization"
   });
   ```

2. **预加载运行时**
   ```csharp
   await CoreWebView2Environment.CreateAsync(null, "WebView2Runtime");
   ```

3. **大文件处理**
   - 分块加载
   - 虚拟滚动

### 9.3 性能优化策略

1. **TreeView虚拟化**
   ```xml
   <TreeView VirtualizingStackPanel.IsVirtualizing="True"
             VirtualizingStackPanel.VirtualizationMode="Recycling"/>
   ```

2. **Canvas优化**
   - 使用轻量级控件
   - 避免过度渲染
   - 限制可见区域更新

3. **数据绑定优化**
   - 使用ObservableCollection
   - 合理使用UpdateSourceTrigger
   - 避免循环依赖

---

## 十、开发规范与最佳实践

### 10.1 命名规范

- **ViewModel类**: `XxxViewModel`
- **View类**: `XxxView` 或 `XxxPanel`
- **服务接口**: `IXxxService`
- **服务实现**: `XxxService`
- **模型类**: `XxxData` 或 `XxxMetadata`

### 10.2 代码组织

- 遵循MVVM严格分离原则
- ViewModel不直接引用View
- View通过DataContext绑定ViewModel
- 使用接口定义服务契约

### 10.3 异步处理

- 所有I/O操作使用async/await
- 避免阻塞UI线程
- 使用ConfigureAwait(false)优化后台任务

---

## 十一、部署与发布

### 11.1 依赖项

- .NET 8.0 Runtime或SDK
- WebView2 Runtime
- 可选: MaterialDesignThemes.Wpf

### 11.2 配置文件

```json
{
  "ApplicationSettings": {
    "TemplatesDirectory": "Templates",
    "OutputDirectory": "Output",
    "DefaultTemplateId": "default"
  },
  "WebView2Settings": {
    "EnableGpuAcceleration": true,
    "CacheDirectory": "WebView2Cache"
  }
}
```

### 11.3 打包发布

- 使用Visual Studio发布向导
- 支持单文件发布
- 自动包含WebView2 Runtime

---

## 十二、扩展点与未来规划

### 12.1 当前架构的扩展能力

1. **插件系统**: 支持第三方控件扩展
2. **多语言支持**: 通过资源文件实现国际化
3. **主题切换**: 支持浅色/深色主题
4. **云端同步**: 支持模板云端存储

### 12.2 性能优化方向

1. **虚拟化画布**: 大型模板的渲染优化
2. **增量保存**: 减少磁盘I/O
3. **内存管理**: 大型文档的内存优化

### 12.3 功能增强

1. **模板库**: 在线模板市场
2. **协作编辑**: 多人实时编辑
3. **版本控制**: 模板版本管理
4. **导出格式**: 支持更多输出格式

---

## 附录

### A. 参考资源

- [CommunityToolkit.Mvvm官方文档](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [WPF官方文档](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [WebView2官方文档](https://docs.microsoft.com/en-us/microsoft-edge/webview2/)
- [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)

### B. 常见问题 FAQ

**Q1: 为什么选择CommunityToolkit.Mvvm而不是Prism？**
A: CommunityToolkit.Mvvm是微软官方维护,支持源生成器,可以减少60%以上的样板代码,更适合中小型项目快速开发。

**Q2: 如何处理大型模板的性能问题？**
A: 采用虚拟化技术、延迟加载、分块渲染等多种策略,具体实现参见第九章。

**Q3: WebView2 PDF预览需要安装运行时吗？**
A: 是的,需要WebView2 Runtime。建议使用Evergreen Runtime,它会自动更新。

### C. 版本历史

| 版本 | 日期 | 说明 |
|------|------|------|
| v1.0 | 2026-01-30 | 初始版本,核心架构设计 |

---

**文档结束**

如有任何问题或建议,请联系开发团队。

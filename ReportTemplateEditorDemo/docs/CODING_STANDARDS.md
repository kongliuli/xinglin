# ReportTemplateEditor 代码风格规范

## 概述

本文档定义了ReportTemplateEditor项目的代码风格规范，确保代码的一致性、可读性和可维护性。所有开发人员都应遵循本规范。

---

## 1. 命名约定

### 1.1 类命名

- **规则**: 使用PascalCase（首字母大写，每个单词首字母大写）
- **示例**:
  ```csharp
  public class MainWindowViewModel : ViewModelBase
  public class PropertyPanelViewModel : ViewModelBase
  public class BooleanToVisibilityConverter : IValueConverter
  ```

### 1.2 接口命名

- **规则**: 使用PascalCase，以"I"开头
- **示例**:
  ```csharp
  public interface ITemplateRenderer
  public interface IAsyncRelayCommand : ICommand
  public interface IDataBindingEngine
  ```

### 1.3 方法命名

- **规则**: 使用PascalCase
- **动词开头**: 方法名应以动词开头，清晰表达方法的作用
- **示例**:
  ```csharp
  public void UpdatePropertyPanel(ElementBase? element)
  public bool CanSaveTemplate()
  public async Task ExecuteAsync(object? parameter)
  private void InitializeCommands()
  ```

### 1.4 属性命名

- **规则**: 使用PascalCase
- **布尔属性**: 使用"Is"或"Can"前缀
- **示例**:
  ```csharp
  public string WindowTitle { get; set; }
  public bool IsBusy { get; set; }
  public bool CanSaveTemplate { get; }
  public ObservableCollection<UIElementWrapper> ElementWrappers { get; }
  ```

### 1.5 私有字段命名

- **规则**: 使用camelCase（首字母小写，后续单词首字母大写）
- **前缀**: 使用下划线"_"前缀
- **示例**:
  ```csharp
  private ReportTemplateDefinition _currentTemplate;
  private string _windowTitle = "报告模板设计器";
  private bool _isBusy;
  private readonly TemplateSerializationService _serializationService;
  ```

### 1.6 常量命名

- **规则**: 使用PascalCase，全部大写
- **示例**:
  ```csharp
  public const int MaxZoomLevel = 5;
  public const double DefaultZoomLevel = 1.0;
  public const string DefaultBackgroundColor = "#FFFFFF";
  ```

### 1.7 参数命名

- **规则**: 使用camelCase
- **示例**:
  ```csharp
  public void AddElementToCanvas(UIElementWrapper wrapper)
  public bool CanExecute(object? parameter)
  private void HandlePropertyChange(object? parameter)
  ```

### 1.8 命名空间命名

- **规则**: 使用PascalCase，与项目结构保持一致
- **格式**: `项目名.子项目名.文件夹名`
- **示例**:
  ```csharp
  namespace ReportTemplateEditor.Designer.ViewModels
  namespace ReportTemplateEditor.Designer.Converters
  namespace ReportTemplateEditor.Designer.Services
  namespace ReportTemplateEditor.Core.Models.Elements
  ```

---

## 2. 代码缩进和格式化

### 2.1 缩进

- **规则**: 使用4个空格作为缩进单位
- **禁止**: 使用Tab字符
- **示例**:
  ```csharp
  public class MainWindowViewModel : ViewModelBase
  {
      private ReportTemplateDefinition _currentTemplate;
      
      public void NewTemplate()
      {
          try
          {
              IsBusy = true;
              StatusMessage = "正在创建新模板...";
              
              CurrentTemplate = new ReportTemplateDefinition
              {
                  Name = "新模板"
              };
          }
          catch (Exception ex)
          {
              StatusMessage = $"创建新模板失败: {ex.Message}";
          }
          finally
          {
              IsBusy = false;
          }
      }
  }
  ```

### 2.2 大括号

- **规则**: 左大括号不换行，右大括号换行
- **示例**:
  ```csharp
  public void NewTemplate()
  {
      try
      {
          IsBusy = true;
      }
      catch (Exception ex)
      {
          StatusMessage = $"创建新模板失败: {ex.Message}";
      }
      finally
      {
          IsBusy = false;
      }
  }
  ```

### 2.3 空行

- **规则**: 方法之间保留一个空行
- **示例**:
  ```csharp
  public void NewTemplate()
  {
  }
  
  public void OpenTemplate()
  {
  }
  ```

### 2.4 行长度

- **规则**: 单行代码不超过120个字符
- **建议**: 超过120字符时换行

### 2.5 方法长度

- **规则**: 单个方法不超过50行
- **建议**: 超过50行时拆分为多个小方法

---

## 3. 文件组织结构

### 3.1 项目结构

```
ReportTemplateEditorDemo/
├── ReportTemplateEditor.Core/          # 核心模型和业务逻辑
│   ├── Models/                      # 模型类
│   │   ├── Elements/             # 元素模型
│   │   ├── Commands/             # 命令模型
│   │   ├── Widgets/              # 控件模型
│   │   └── Enums/               # 枚举类型
│   ├── Services/                    # 服务类
│   └── Helpers/                    # 辅助类
├── ReportTemplateEditor.Engine/         # 模板渲染引擎
│   ├── ITemplateRenderer.cs        # 渲染器接口
│   └── TemplateRenderer.cs       # 渲染器实现
├── ReportTemplateEditor.Designer/       # 可视化设计器
│   ├── ViewModels/               # ViewModel层
│   ├── Views/                    # View层（XAML）
│   ├── Converters/               # 值转换器
│   ├── Styles/                   # 样式资源
│   ├── Services/                  # 服务类
│   └── Models/                   # UI模型类
└── ReportTemplateEditor.App/          # 应用程序入口
    ├── ViewModels/               # ViewModel层
    ├── Views/                    # View层（XAML）
    └── Services/                  # 服务类
```

### 3.2 文件命名

- **规则**: 使用PascalCase
- **ViewModel**: 以"ViewModel"结尾
- **Converter**: 以"Converter"结尾
- **Service**: 以"Service"结尾
- **Helper**: 以"Helper"结尾
- **示例**:
  ```
  MainWindowViewModel.cs
  PropertyPanelViewModel.cs
  BooleanToVisibilityConverter.cs
  TemplateSerializationService.cs
  ElementHelper.cs
  ```

### 3.3 文件夹命名

- **规则**: 使用PascalCase，复数形式
- **示例**:
  ```
  ViewModels/
  Converters/
  Services/
  Styles/
  Models/
  ```

---

## 4. 架构模式

### 4.1 MVVM模式

- **职责分离**:
  - **Model**: 数据模型，不包含任何UI逻辑
  - **View**: XAML界面，只负责显示
  - **ViewModel**: 业务逻辑，处理数据和命令

- **禁止行为**:
  - ❌ ViewModel中直接引用View或UI控件
  - ❌ XAML中使用事件处理器（应使用命令）
  - ❌ View中包含业务逻辑

- **正确示例**:
  ```csharp
  // ViewModel - 正确
  public partial class MainWindowViewModel : ViewModelBase
  {
      public ICommand NewTemplateCommand { get; private set; }
      
      private void NewTemplate()
      {
          // 纯业务逻辑
      }
  }
  ```

  ```xml
  <!-- View - 正确 -->
  <Button Content="新建模板" Command="{Binding NewTemplateCommand}"/>
  ```

- **错误示例**:
  ```csharp
  // ViewModel - 错误
  public partial class MainWindowViewModel : ViewModelBase
  {
      private MainWindow _mainWindow; // ❌ 直接引用View
      
      private void NewTemplate()
      {
          _mainWindow.Close(); // ❌ 直接操作UI
      }
  }
  ```

  ```xml
  <!-- View - 错误 -->
  <Button Content="新建模板" Click="Button_Click"/> <!-- ❌ 使用事件处理器 -->
  ```

### 4.2 依赖注入

- **规则**: 使用构造函数注入依赖
- **示例**:
  ```csharp
  public partial class MainWindowViewModel : ViewModelBase
  {
      private readonly TemplateSerializationService _serializationService;
      private readonly CommandManager _commandManager;
      
      public MainWindowViewModel(
          TemplateSerializationService serializationService,
          CommandManager commandManager)
      {
          _serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
          _commandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
      }
  }
  ```

### 4.3 命令模式

- **规则**: 使用ICommand或RelayCommand
- **示例**:
  ```csharp
  public ICommand NewTemplateCommand { get; private set; }
  
  public MainWindowViewModel()
  {
      NewTemplateCommand = new RelayCommand(NewTemplate, CanNewTemplate);
  }
  ```

---

## 5. 注释规范

### 5.1 XML注释

- **规则**: 所有公共类、方法、属性都必须有XML注释
- **必需标签**:
  - `<summary>`: 功能描述
  - `<param>`: 参数说明
  - `<returns>`: 返回值说明
  - `<example>`: 使用示例（可选）
  - `<exception>`: 异常说明（如适用）
  - `<remarks>`: 详细说明（可选）

- **示例**:
  ```csharp
  /// <summary>
  /// 新建模板
  /// </summary>
  /// <example>
  /// <code>
  /// NewTemplateCommand.Execute(null);
  /// </code>
  /// </example>
  public void NewTemplate()
  {
  }
  ```

### 5.2 行内注释

- **规则**: 使用"//"进行注释
- **位置**: 注释应放在代码上方
- **示例**:
  ```csharp
  // 初始化命令
  private void InitializeCommands()
  {
      NewTemplateCommand = new RelayCommand(NewTemplate);
      OpenTemplateCommand = new RelayCommand(OpenTemplate);
  }
  ```

### 5.3 注释语言

- **规则**: 使用中文编写注释
- **原因**: 项目团队使用中文，中文注释更易理解

---

## 6. 异常处理规范

### 6.1 异常捕获

- **规则**: 所有可能抛出异常的操作都必须有try-catch块
- **示例**:
  ```csharp
  public void NewTemplate()
  {
      try
      {
          IsBusy = true;
          StatusMessage = "正在创建新模板...";
          
          CurrentTemplate = new ReportTemplateDefinition();
      }
      catch (Exception ex)
      {
          StatusMessage = $"创建新模板失败: {ex.Message}";
          System.Diagnostics.Debug.WriteLine($"创建新模板失败: {ex}");
      }
      finally
      {
          IsBusy = false;
      }
  }
  ```

### 6.2 异常日志

- **规则**: 使用Debug.WriteLine记录异常详情
- **示例**:
  ```csharp
  catch (Exception ex)
  {
      StatusMessage = $"操作失败: {ex.Message}";
      System.Diagnostics.Debug.WriteLine($"操作失败: {ex}");
  }
  ```

### 6.3 用户友好提示

- **规则**: 向用户显示友好的错误提示
- **示例**:
  ```csharp
  StatusMessage = $"创建新模板失败: {ex.Message}";
  ```

---

## 7. 代码质量

### 7.1 SOLID原则

- **单一职责原则**: 每个类只负责一个功能
- **开闭原则**: 对扩展开放，对修改关闭
- **里氏替换原则**: 子类可以替换父类
- **接口隔离原则**: 使用小而专一的接口
- **依赖倒置原则**: 依赖抽象而非具体实现

### 7.2 DRY原则

- **规则**: 不要重复自己（Don't Repeat Yourself）
- **示例**:
  ```csharp
  // ❌ 错误 - 重复代码
  public void Method1()
  {
      try
      {
          IsBusy = true;
          // 操作代码
      }
      finally
      {
          IsBusy = false;
      }
  }
  
  public void Method2()
  {
      try
      {
          IsBusy = true;
          // 操作代码
      }
      finally
      {
          IsBusy = false;
      }
  }
  
  // ✅ 正确 - 提取重复代码
  private void ExecuteWithBusyIndicator(Action action)
  {
      try
      {
          IsBusy = true;
          action();
      }
      finally
      {
          IsBusy = false;
      }
  }
  
  public void Method1()
  {
      ExecuteWithBusyIndicator(() => {
          // 操作代码
      });
  }
  
  public void Method2()
  {
      ExecuteWithBusyIndicator(() => {
          // 操作代码
      });
  }
  ```

### 7.3 KISS原则

- **规则**: 保持简单（Keep It Simple, Stupid）
- **示例**:
  ```csharp
  // ❌ 错误 - 过于复杂
  public void ComplexMethod(object parameter)
  {
      if (parameter != null)
      {
          if (parameter is string str)
          {
              if (!string.IsNullOrEmpty(str))
              {
                  // 嵌套if
              }
          }
          else if (parameter is int num)
          {
              // 嵌套if
          }
      }
  }
  
  // ✅ 正确 - 简化逻辑
  public void SimpleMethod(object parameter)
  {
      if (parameter == null)
      {
          return;
      }
      
      if (parameter is string str && !string.IsNullOrEmpty(str))
      {
          // 处理字符串
      }
      else if (parameter is int num)
      {
          // 处理整数
      }
  }
  ```

---

## 8. XAML规范

### 8.1 命名约定

- **规则**: 使用PascalCase
- **示例**:
  ```xml
  <Button x:Name="NewTemplateButton"/>
  <TextBox x:Name="SearchTextBox"/>
  <ListBox x:Name="LayersListBox"/>
  ```

### 8.2 数据绑定

- **规则**: 使用Binding语法，避免直接设置值
- **示例**:
  ```xml
  <!-- ✅ 正确 -->
  <Button Content="新建模板" Command="{Binding NewTemplateCommand}"/>
  <TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"/>
  
  <!-- ❌ 错误 -->
  <Button x:Name="NewTemplateButton" Content="新建模板" Click="Button_Click"/>
  <TextBox x:Name="SearchTextBox"/>
  ```

### 8.3 资源引用

- **规则**: 使用StaticResource引用样式和资源
- **示例**:
  ```xml
  <Button Style="{StaticResource PrimaryButtonStyle}" Content="新建模板"/>
  <TextBlock Foreground="{StaticResource TextBrush}" Text="示例文本"/>
  ```

### 8.4 值转换器

- **规则**: 使用Converter进行值转换
- **示例**:
  ```xml
  <CheckBox IsChecked="{Binding IsVisible, Converter={StaticResource BooleanInverseConverter}}"/>
  <TextBlock Text="{Binding ZoomLevel, Converter={StaticResource ZoomLevelToTextConverter}}"/>
  ```

---

## 9. 测试规范

### 9.1 单元测试

- **规则**: 所有公共方法都应有单元测试
- **命名**: 测试类以"Tests"结尾
- **示例**:
  ```csharp
  [TestClass]
  public class MainWindowViewModelTests
  {
      [TestMethod]
      public void NewTemplate_ShouldCreateNewTemplate()
      {
          // 测试代码
      }
  }
  ```

### 9.2 测试命名

- **规则**: 使用"MethodName_Scenario"格式
- **示例**:
  ```csharp
  [TestMethod]
  public void NewTemplate_ShouldCreateNewTemplate()
  [TestMethod]
  public void NewTemplate_WhenBusy_ShouldNotExecute()
  [TestMethod]
  public void NewTemplate_WhenException_ShouldShowErrorMessage()
  ```

---

## 10. 版本控制

### 10.1 Git提交信息

- **规则**: 使用清晰的提交信息
- **格式**: `[类型] 简短描述`
- **示例**:
  ```
  [feat] 添加MainWindowViewModel
  [fix] 修复属性面板更新问题
  [refactor] 重构UIElementFactory类
  [docs] 更新代码风格规范文档
  ```

### 10.2 分支命名

- **规则**: 使用描述性的分支名
- **示例**:
  ```
  feature/add-viewmodel-layer
  bugfix/property-panel-update
  refactor/extract-common-methods
  ```

---

## 11. 性能优化

### 11.1 UI虚拟化

- **规则**: 对于长列表使用虚拟化
- **示例**:
  ```xml
  <ListBox VirtualizingPanel.IsVirtualizing="True">
  ```

### 11.2 延迟加载

- **规则**: 按需加载资源
- **示例**:
  ```csharp
  // 延迟加载ViewModel
  private PropertyPanelViewModel? _propertyPanelViewModel;
  
  public PropertyPanelViewModel PropertyPanelViewModel
  {
      get
      {
          if (_propertyPanelViewModel == null)
          {
              _propertyPanelViewModel = new PropertyPanelViewModel(_executeCommand, _showStatus);
          }
          return _propertyPanelViewModel;
      }
  }
  ```

### 11.3 内存管理

- **规则**: 及时释放资源，避免内存泄漏
- **示例**:
  ```csharp
  // 使用WeakReference防止内存泄漏
  private readonly List<WeakReference<EventHandler>> _eventHandlers = new List<WeakReference<EventHandler>>();
  ```

---

## 12. 安全规范

### 12.1 输入验证

- **规则**: 验证所有用户输入
- **示例**:
  ```csharp
  public void OpenTemplate()
  {
      if (string.IsNullOrEmpty(filePath))
      {
          StatusMessage = "文件路径不能为空";
          return;
      }
      
      if (!File.Exists(filePath))
      {
          StatusMessage = "文件不存在";
          return;
      }
  }
  ```

### 12.2 SQL注入防护

- **规则**: 使用参数化查询
- **示例**:
  ```csharp
  // ✅ 正确 - 使用参数化查询
  var query = "SELECT * FROM Templates WHERE Id = @Id";
  var parameters = new { Id = templateId };
  
  // ❌ 错误 - 字符串拼接
  var query = $"SELECT * FROM Templates WHERE Id = {templateId}";
  ```

---

## 13. 工具配置

### 13.1 .editorconfig

- **规则**: 使用.editorconfig统一代码格式化配置
- **示例**:
  ```ini
  [*.cs]
  indent_style = space
  indent_size = 4
  end_of_line = crlf
  charset = utf-8-bom
  trim_trailing_whitespace = true
  insert_final_newline = true
  ```

### 13.2 .gitignore

- **规则**: 忽略不需要版本控制的文件
- **示例**:
  ```
  bin/
  obj/
  *.user
  *.suo
  *.cache
  ```
`

---

## 14. 文档规范

### 14.1 README

- **规则**: 每个项目都应有README文件
- **内容**: 项目描述、安装说明、使用方法、贡献指南

### 14.2 CHANGELOG

- **规则**: 维护CHANGELOG记录版本变更
- **格式**: 遵循[Keep a Changelog](https://keepachangelog.com/)格式

---

## 15. 代码审查清单

在提交代码前，请检查以下项目：

- [ ] 所有公共类、方法、属性都有XML注释
- [ ] 命名符合规范
- [ ] 代码缩进正确（4个空格）
- [ ] 方法长度不超过50行
- [ ] 没有重复代码
- [ ] 异常处理完善
- [ ] 遵循MVVM模式
- [ ] 单元测试覆盖充分
- [ ] Git提交信息清晰

---

## 16. 常见问题

### 16.1 MVVM违规

- ❌ ViewModel中直接引用View
- ❌ XAML中使用事件处理器
- ❌ View中包含业务逻辑

### 16.2 命名违规

- ❌ 使用缩写或不清晰的名称
- ❌ 命名不一致
- ❌ 使用中文命名

### 16.3 代码重复

- ❌ 复制粘贴代码
- ❌ 未提取公共方法
- ❌ 未使用继承

### 16.4 异常处理不当

- ❌ 吞掉异常
- ❌ 只记录异常不处理
- ❌ 向用户显示技术错误信息

---

## 17. 最佳实践

### 17.1 优先使用依赖注入

- 通过构造函数注入依赖
- 避免在方法中直接创建依赖

### 17.2 优先使用命令而非事件

- 使用ICommand或RelayCommand
- 避免在XAML中使用事件处理器

### 17.3 优先使用数据绑定

- 使用Binding语法
- 避免在Code-behind中直接操作UI

### 17.4 优先使用异步操作

- 对于耗时操作使用async/await
- 避免阻塞UI线程

### 17.5 优先使用资源字典

- 将样式和资源放在ResourceDictionary中
- 避免在XAML中重复定义样式

---

## 18. 工具推荐

### 18.1 代码格式化工具

- Visual Studio 2022+: 内置代码格式化
- ReSharper: 强大的代码分析和重构工具
- XAML Styler: XAML代码格式化工具

### 18.2 代码分析工具

- SonarQube: 代码质量分析
- FxCop: C#代码规范检查
- StyleCop: C#代码风格检查

### 18.3 调试工具

- snoop: WPF运行时调试工具
- WPF Inspector: WPF可视化树检查工具

---

## 19. 更新记录

- **2026-01-21**: 初始版本，定义了完整的代码风格规范
  - 包括命名约定
  - 包括代码格式化规则
  - 包括架构模式规范
  - 包括注释规范
  - 包括异常处理规范
  - 包括XAML规范
  - 包括测试规范
  - 包括性能优化建议
  - 包括安全规范
  - 包括工具配置
  - 包括文档规范
  - 包括代码审查清单
  - 包括常见问题
  - 包括最佳实践
  - 包括工具推荐

---

## 20. 联系方式

如有疑问或建议，请联系：
- 项目负责人：[待填写]
- 技术负责人：[待填写]
- 代码审查人：[待填写]

---

**版本**: 1.0.0  
**最后更新**: 2026-01-21  
**维护者**: ReportTemplateEditor团队

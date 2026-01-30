# WPF MVVM模板编辑器系统实现计划

## 一、项目初始化与环境搭建

### 1.1 技术选型
- **开发框架**：WPF (.NET 8)
- **MVVM框架**：CommunityToolkit.Mvvm 8.2+
- **PDF预览**：WebView2
- **数据存储**：JSON文件存储
- **UI组件**：标准WPF控件 + 自定义控件
- **依赖注入**：内置DI容器
- **消息传递**：CommunityToolkit.Mvvm.Messaging

### 1.2 项目结构搭建
1. **创建WPF项目**：使用Visual Studio创建.NET 8 WPF应用
2. **安装核心依赖**：
   - CommunityToolkit.Mvvm
   - Microsoft.Web.WebView2
3. **搭建目录结构**：
   - Models/
   - ViewModels/
   - Views/
   - Services/
   - Converters/
   - Behaviors/
   - Resources/
   - Helpers/
   - Constants/

## 二、核心功能实现

### 2.1 模型层实现
1. **核心实体模型**：
   - TemplateData.cs
   - LayoutMetadata.cs
   - ControlElement.cs
   - TemplateConfig.cs
2. **配置模型**：
   - ColumnConfig.cs
   - DropdownOptionConfig.cs
   - FixedRegionConfig.cs
3. **持久化模型**：
   - TemplateFile.cs
   - ExportConfig.cs

### 2.2 服务层实现
1. **数据服务**：
   - ITemplateService.cs / TemplateService.cs
   - ConfigService.cs
   - PdfService.cs
2. **共享服务**：
   - ISharedDataService.cs / SharedDataService.cs
   - IMessageService.cs / MessageService.cs
   - IValidationService.cs / ValidationService.cs

### 2.3 视图模型层实现
1. **基础ViewModel**：
   - ViewModelBase.cs
2. **核心ViewModel**：
   - MainViewModel.cs
   - DataEntryViewModel.cs
   - TemplateEditorViewModel.cs
   - PdfPreviewViewModel.cs
   - TemplateTreeViewModel.cs

### 2.4 视图层实现
1. **主窗口**：
   - MainWindow.xaml
2. **面板控件**：
   - DataEntryPanel.xaml
   - TemplateEditorPanel.xaml
   - PdfPreviewPanel.xaml
   - TemplateTreePanel.xaml
3. **自定义控件**：
   - DraggableCanvas.xaml
   - EditableTextBox.xaml
   - CustomDataGrid.xaml

### 2.5 辅助功能实现
1. **值转换器**：
   - EditableStateConverter.cs
   - ControlTypeTemplateSelector.cs
   - BoolToVisibilityConverter.cs
2. **附加行为**：
   - DragDropBehavior.cs
   - ResizeBehavior.cs
   - SelectionBehavior.cs
3. **辅助类**：
   - JsonHelper.cs
   - FileHelper.cs
   - MathHelper.cs

## 三、核心功能特性

### 3.1 数据录入功能
- 用户数据表单录入
- 数据验证
- 数据绑定与同步

### 3.2 模板编辑功能
- 画布控件拖拽
- 控件属性编辑
- 模板保存与加载
- 模板预览

### 3.3 PDF预览功能
- WebView2集成
- PDF文件加载
- 缩放控制
- 打印功能

### 3.4 模板管理功能
- 模板列表展示
- 模板创建与删除
- 模板配置管理

## 四、实现步骤

### 4.1 阶段一：基础架构搭建
1. 创建项目并安装依赖
2. 搭建目录结构
3. 实现核心数据模型

### 4.2 阶段二：服务层实现
1. 实现数据服务接口与实现
2. 实现共享服务
3. 实现PDF服务

### 4.3 阶段三：ViewModel实现
1. 实现基础ViewModel
2. 实现各功能模块ViewModel
3. 实现消息传递机制

### 4.4 阶段四：视图层实现
1. 实现主窗口布局
2. 实现各功能面板
3. 实现自定义控件

### 4.5 阶段五：功能集成与测试
1. 集成各功能模块
2. 测试核心功能
3. 优化用户体验

## 五、技术实现要点

### 5.1 MVVM模式实现
- 使用CommunityToolkit.Mvvm的ObservableObject和RelayCommand
- 实现双向数据绑定
- 使用Messenger进行组件间通信

### 5.2 画布编辑功能
- 使用Canvas作为编辑区域
- 实现控件拖拽与调整大小
- 使用ItemsControl展示控件集合

### 5.3 PDF预览集成
- WebView2控件配置
- PDF文件加载与渲染
- 缩放与打印功能实现

### 5.4 数据持久化
- JSON序列化与反序列化
- 模板文件管理
- 配置文件处理

## 六、预期成果

1. **完整的WPF MVVM模板编辑器系统**
2. **支持数据录入、模板编辑、PDF预览功能**
3. **符合参考文档的架构设计**
4. **代码结构清晰，易于维护**
5. **用户体验良好的界面设计**
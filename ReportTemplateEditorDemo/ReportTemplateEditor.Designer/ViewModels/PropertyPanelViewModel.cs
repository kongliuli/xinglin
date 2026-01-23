using System;
using System.Windows.Input;
using ReportTemplateEditor.Core.Models.Elements;
using ReportTemplateEditor.Core.Models.Commands;
using ReportTemplateEditor.Designer.Services;

namespace ReportTemplateEditor.Designer.ViewModels
{
    /// <summary>
    /// 属性面板ViewModel，管理元素属性编辑状态
    /// </summary>
    /// <remarks>
    /// 职责包括：
    /// 1. 管理当前选中元素
    /// 2. 管理属性编辑状态
    /// 3. 提供属性变更命令
    /// 4. 提供数据绑定路径选择功能
    /// </remarks>
    public partial class PropertyPanelViewModel : ViewModelBase
    {
        #region 私有字段

        private ElementBase? _selectedElement;
        private string _selectedElementInfo = "未选择元素";
        private bool _hasSelection;
        private bool _isTextElement;
        private bool _isLabelElement;
        private bool _isImageElement;
        private bool _isTableElement;
        private bool _isBarcodeElement;
        private bool _isSignatureElement;
        private bool _isAutoNumberElement;
        private bool _isLabelInputBoxElement;

        #endregion

        #region 服务依赖

        private readonly Action<CommandBase> _executeCommand;
        private readonly Action<string> _showStatus;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化PropertyPanelViewModel实例
        /// </summary>
        /// <param name="executeCommand">命令执行委托</param>
        /// <param name="showStatus">状态显示委托</param>
        /// <exception cref="ArgumentNullException">当任何参数为null时抛出</exception>
        public PropertyPanelViewModel(
            Action<CommandBase> executeCommand,
            Action<string> showStatus)
        {
            _executeCommand = executeCommand ?? throw new ArgumentNullException(nameof(executeCommand));
            _showStatus = showStatus ?? throw new ArgumentNullException(nameof(showStatus));
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 当前选中的元素
        /// </summary>
        public ElementBase? SelectedElement
        {
            get => _selectedElement;
            set
            {
                if (SetProperty(ref _selectedElement, value))
                {
                    UpdateElementInfo();
                    UpdateElementFlags();
                }
            }
        }

        /// <summary>
        /// 选中元素信息
        /// </summary>
        public string SelectedElementInfo
        {
            get => _selectedElementInfo;
            set => SetProperty(ref _selectedElementInfo, value);
        }

        /// <summary>
        /// 是否有选中的元素
        /// </summary>
        public bool HasSelection
        {
            get => _hasSelection;
            set => SetProperty(ref _hasSelection, value);
        }

        /// <summary>
        /// 是否为文本元素
        /// </summary>
        public bool IsTextElement
        {
            get => _isTextElement;
            set => SetProperty(ref _isTextElement, value);
        }

        /// <summary>
        /// 是否为标签元素
        /// </summary>
        public bool IsLabelElement
        {
            get => _isLabelElement;
            set => SetProperty(ref _isLabelElement, value);
        }

        /// <summary>
        /// 是否为图片元素
        /// </summary>
        public bool IsImageElement
        {
            get => _isImageElement;
            set => SetProperty(ref _isImageElement, value);
        }

        /// <summary>
        /// 是否为表格元素
        /// </summary>
        public bool IsTableElement
        {
            get => _isTableElement;
            set => SetProperty(ref _isTableElement, value);
        }

        /// <summary>
        /// 是否为条形码元素
        /// </summary>
        public bool IsBarcodeElement
        {
            get => _isBarcodeElement;
            set => SetProperty(ref _isBarcodeElement, value);
        }

        /// <summary>
        /// 是否为签名元素
        /// </summary>
        public bool IsSignatureElement
        {
            get => _isSignatureElement;
            set => SetProperty(ref _isSignatureElement, value);
        }

        /// <summary>
        /// 是否为自动编号元素
        /// </summary>
        public bool IsAutoNumberElement
        {
            get => _isAutoNumberElement;
            set => SetProperty(ref _isAutoNumberElement, value);
        }

        /// <summary>
        /// 是否为标签输入框元素
        /// </summary>
        public bool IsLabelInputBoxElement
        {
            get => _isLabelInputBoxElement;
            set => SetProperty(ref _isLabelInputBoxElement, value);
        }

        #endregion

        #region 命令属性

        /// <summary>
        /// 更新属性命令
        /// </summary>
        public ICommand? UpdatePropertyCommand { get; private set; }

        #endregion

        #region 初始化方法

        /// <summary>
        /// 初始化命令
        /// </summary>
        private void InitializeCommands()
        {
            UpdatePropertyCommand = new RelayCommand<object>(HandlePropertyChange);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 更新属性面板
        /// </summary>
        /// <param name="element">要显示的元素</param>
        /// <example>
        /// <code>
        /// propertyPanelViewModel.UpdatePropertyPanel(textElement);
        /// </code>
        /// </example>
        public void UpdatePropertyPanel(ElementBase? element)
        {
            SelectedElement = element;
            HasSelection = element != null;

            if (element != null)
            {
                SelectedElementInfo = $"{element.Type} 元素";
            }
            else
            {
                SelectedElementInfo = "未选择元素";
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 更新元素信息
        /// </summary>
        private void UpdateElementInfo()
        {
            if (SelectedElement != null)
            {
                SelectedElementInfo = $"{SelectedElement.Type} 元素 (ID: {SelectedElement.Id})";
            }
            else
            {
                SelectedElementInfo = "未选择元素";
            }
        }

        /// <summary>
        /// 更新元素类型标志
        /// </summary>
        private void UpdateElementFlags()
        {
            if (SelectedElement == null)
            {
                IsTextElement = false;
                IsLabelElement = false;
                IsImageElement = false;
                IsTableElement = false;
                IsBarcodeElement = false;
                IsSignatureElement = false;
                IsAutoNumberElement = false;
                IsLabelInputBoxElement = false;
                return;
            }

            IsTextElement = SelectedElement is TextElement;
            IsLabelElement = SelectedElement is LabelElement;
            IsImageElement = SelectedElement is ImageElement;
            IsTableElement = SelectedElement is TableElement;
            IsBarcodeElement = SelectedElement is BarcodeElement;
            IsSignatureElement = SelectedElement is SignatureElement;
            IsAutoNumberElement = SelectedElement is AutoNumberElement;
            IsLabelInputBoxElement = SelectedElement is LabelInputBoxElement;
        }

        /// <summary>
        /// 处理属性变更
        /// </summary>
        /// <param name="parameter">属性变更参数</param>
        /// <example>
        /// <code>
        /// UpdatePropertyCommand.Execute(new { PropertyName = "Text", Value = "新文本" });
        /// </code>
        /// </example>
        /// <summary>
        /// 处理属性变更
        /// </summary>
        /// <param name="parameter">属性变更参数</param>
        /// <example>
        /// <code>
        /// UpdatePropertyCommand.Execute(new { PropertyName = "Text", Value = "新文本" });
        /// </code>
        /// </example>
        private void HandlePropertyChange(object? parameter)
        {
            if (parameter == null || SelectedElement == null)
            {
                return;
            }

            ExceptionHandler.TryExecute(() =>
            {
                var propertyInfo = parameter as dynamic;
                string propertyName = propertyInfo?.PropertyName as string ?? string.Empty;
                object? value = propertyInfo?.Value;

                if (string.IsNullOrEmpty(propertyName))
                {
                    _showStatus?.Invoke("属性名称不能为空");
                    return;
                }

                var command = new ModifyElementPropertyCommand(SelectedElement, propertyName, value);
                _executeCommand?.Invoke(command);
                ExceptionHandler.LogInfo($"属性修改成功: {propertyName} = {value}", "Property");
            },
            "修改属性",
            errorMessage => _showStatus?.Invoke(errorMessage));
        }

        #endregion
    }
}

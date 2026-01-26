using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.Core.Elements;
using Xinglin.Core.Models;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 模板树节点视图模型
    /// </summary>
    public class TemplateTreeNodeViewModel : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private bool _isSelected;
        
        /// <summary>
        /// 节点显示名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 节点类型
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// 关联的元素对象
        /// </summary>
        public object Data { get; set; }
        
        /// <summary>
        /// 子节点
        /// </summary>
        public List<TemplateTreeNodeViewModel> Children { get; set; }
        
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateTreeNodeViewModel()
        {
            Children = new List<TemplateTreeNodeViewModel>();
        }
    }
    
    /// <summary>
    /// 模板树视图模型
    /// </summary>
    public class TemplateTreeViewModel : INotifyPropertyChanged
    {
        private List<TemplateTreeNodeViewModel> _treeNodes;
        private TemplateTreeNodeViewModel _selectedNode;
        
        /// <summary>
        /// 树节点集合
        /// </summary>
        public List<TemplateTreeNodeViewModel> TreeNodes
        {
            get => _treeNodes;
            set
            {
                if (_treeNodes != value)
                {
                    _treeNodes = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// 选中的节点
        /// </summary>
        public TemplateTreeNodeViewModel SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (_selectedNode != value)
                {
                    _selectedNode = value;
                    OnPropertyChanged();
                    OnNodeSelected?.Invoke(this, value);
                }
            }
        }
        
        /// <summary>
        /// 节点选中事件
        /// </summary>
        public event System.EventHandler<TemplateTreeNodeViewModel> OnNodeSelected;
        
        /// <summary>
        /// 编辑命令
        /// </summary>
        public ICommand EditCommand { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateTreeViewModel()
        {
            TreeNodes = new List<TemplateTreeNodeViewModel>();
            
            // 初始化编辑命令
            EditCommand = new RelayCommand<object>(EditNode, CanEditNode);
        }
        
        /// <summary>
        /// 从模板定义构建树结构
        /// </summary>
        /// <param name="template">模板定义</param>
        public void BuildTreeFromTemplate(ReportTemplateDefinition template)
        {
            if (template == null)
                return;
            
            // 清空现有节点
            TreeNodes.Clear();
            
            // 创建根节点
            var rootNode = new TemplateTreeNodeViewModel
            {
                Name = template.Name,
                Type = "Template",
                Data = template,
                IsExpanded = true
            };
            
            // 添加元素节点
            foreach (var element in template.Elements)
            {
                var elementNode = CreateElementNode(element);
                rootNode.Children.Add(elementNode);
            }
            
            // 添加到树节点集合
            TreeNodes.Add(rootNode);
        }
        
        /// <summary>
        /// 创建元素节点
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <returns>元素节点</returns>
        private TemplateTreeNodeViewModel CreateElementNode(ElementBase element)
        {
            var node = new TemplateTreeNodeViewModel
            {
                Name = GetElementDisplayName(element),
                Type = element.Type,
                Data = element,
                IsExpanded = true
            };
            
            // 检查是否有子元素（如果支持的话）
            // 目前ElementBase没有子元素属性，这里可以根据实际需求扩展
            
            return node;
        }
        
        /// <summary>
        /// 获取元素显示名称
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <returns>显示名称</returns>
        private string GetElementDisplayName(ElementBase element)
        {
            switch (element.Type)
            {
                case "Text":
                    var textElement = (TextElement)element;
                    return $"文本 [{textElement.Text}]";
                case "Image":
                    return "图片";
                case "Line":
                    return "线条";
                default:
                    return element.Type;
            }
        }
        
        /// <summary>
        /// 编辑节点
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void EditNode(object parameter)
        {
            if (parameter is TemplateTreeNodeViewModel node)
            {
                SelectedNode = node;
                // 触发节点选中事件，由外部处理编辑逻辑
                OnNodeSelected?.Invoke(this, node);
            }
        }
        
        /// <summary>
        /// 是否可以编辑节点
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以编辑</returns>
        private bool CanEditNode(object parameter)
        {
            return parameter is TemplateTreeNodeViewModel;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
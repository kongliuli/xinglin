using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.Core.Elements;
using Xinglin.Core.Models;
using Xinglin.ReportTemplateEditor.WPF.Models;

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
        public void BuildTreeFromTemplate(object template)
        {
            if (template == null)
                return;
            
            // 清空现有节点
            TreeNodes.Clear();
            
            // 处理TemplateDefinition类型
            if (template is TemplateDefinition templateDef)
            {
                // 创建根节点
                var rootNode = new TemplateTreeNodeViewModel
                {
                    Name = templateDef.TemplateName,
                    Type = "Template",
                    Data = templateDef,
                    IsExpanded = true
                };
                
                // 添加全局元素节点
                if (templateDef.ElementCollection?.GlobalElements != null)
                {
                    foreach (var element in templateDef.ElementCollection.GlobalElements)
                    {
                        var elementNode = CreateElementNode(element);
                        rootNode.Children.Add(elementNode);
                    }
                }
                
                // 添加区域节点
                if (templateDef.ElementCollection?.Zones != null)
                {
                    foreach (var zone in templateDef.ElementCollection.Zones)
                    {
                        var zoneNode = new TemplateTreeNodeViewModel
                        {
                            Name = zone.ZoneId,
                            Type = "Zone",
                            Data = zone,
                            IsExpanded = true
                        };
                        
                        // 添加区域内元素节点
                        if (zone.Elements != null)
                        {
                            foreach (var element in zone.Elements)
                            {
                                var elementNode = CreateElementNode(element);
                                zoneNode.Children.Add(elementNode);
                            }
                        }
                        
                        rootNode.Children.Add(zoneNode);
                    }
                }
                
                // 添加到树节点集合
                TreeNodes.Add(rootNode);
            }
            // 处理ReportTemplateDefinition类型
            else if (template is ReportTemplateDefinition reportTemplate)
            {
                // 创建根节点
                var rootNode = new TemplateTreeNodeViewModel
                {
                    Name = reportTemplate.Name,
                    Type = "Template",
                    Data = reportTemplate,
                    IsExpanded = true
                };
                
                // 添加元素节点
                foreach (var element in reportTemplate.Elements)
                {
                    var elementNode = CreateElementNode(element);
                    rootNode.Children.Add(elementNode);
                }
                
                // 添加到树节点集合
                TreeNodes.Add(rootNode);
            }
        }
        
        /// <summary>
        /// 为元素创建树节点
        /// </summary>
        /// <param name="element">元素对象</param>
        /// <returns>创建的树节点</returns>
        private TemplateTreeNodeViewModel CreateElementNode(object element)
        {
            var node = new TemplateTreeNodeViewModel
            {
                Type = "Element",
                Data = element
            };
            
            // 设置节点名称
            if (element is TemplateElement templateElement)
            {
                node.Name = templateElement.Label ?? templateElement.ElementId;
            }
            else if (element is Xinglin.Core.Elements.ElementBase coreElement)
            {
                node.Name = coreElement.Label ?? coreElement.Id;
            }
            else if (element is Xinglin.ReportTemplateEditor.WPF.Core.Elements.ElementBase localElement)
            {
                node.Name = localElement.Label ?? localElement.ElementId;
            }
            
            return node;
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
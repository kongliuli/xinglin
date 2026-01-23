using System.Collections.ObjectModel;
using System.ComponentModel;
using ReportTemplateEditor.Core.Models.Nodes;

namespace ReportTemplateEditor.Core.Models.Nodes
{
    /// <summary>
    /// 节点视图模型
    /// </summary>
    public class NodeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 节点列表
        /// </summary>
        private readonly ObservableCollection<ReportNode> _nodes;

        /// <summary>
        /// 选中的节点列表
        /// </summary>
        private readonly ObservableCollection<ReportNode> _selectedNodes;

        /// <summary>
        /// 主选中节点
        /// </summary>
        private ReportNode _primarySelectedNode;

        /// <summary>
        /// 节点选中事件
        /// </summary>
        public event Action<ReportNode> NodeSelected;

        /// <summary>
        /// 选择清除事件
        /// </summary>
        public event Action SelectionCleared;

        /// <summary>
        /// 选择变更事件
        /// </summary>
        public event Action SelectionChanged;

        public NodeViewModel()
        {
            _nodes = new ObservableCollection<ReportNode>();
            _selectedNodes = new ObservableCollection<ReportNode>();
        }

        /// <summary>
        /// 节点列表
        /// </summary>
        public ObservableCollection<ReportNode> Nodes => _nodes;

        /// <summary>
        /// 选中的节点列表
        /// </summary>
        public ObservableCollection<ReportNode> SelectedNodes => _selectedNodes;

        /// <summary>
        /// 主选中节点
        /// </summary>
        public ReportNode PrimarySelectedNode
        {
            get => _primarySelectedNode;
            set
            {
                if (_primarySelectedNode != value)
                {
                    _primarySelectedNode = value;
                    OnPropertyChanged(nameof(PrimarySelectedNode));
                    NodeSelected?.Invoke(value);
                }
            }
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        public void AddNode(ReportNode node)
        {
            _nodes.Add(node);
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        public void RemoveNode(ReportNode node)
        {
            _nodes.Remove(node);
            _selectedNodes.Remove(node);
            if (_primarySelectedNode == node)
            {
                PrimarySelectedNode = null;
            }
        }

        /// <summary>
        /// 选择节点
        /// </summary>
        public void SelectNode(ReportNode node, bool clearPrevious = true)
        {
            if (!node.IsVisible)
            {
                return;
            }

            if (clearPrevious)
            {
                foreach (var n in _selectedNodes)
                {
                    n.IsSelected = false;
                }
                _selectedNodes.Clear();
            }

            node.IsSelected = true;
            _selectedNodes.Add(node);
            PrimarySelectedNode = node;
            OnPropertyChanged(nameof(SelectedNodes));
            NodeSelected?.Invoke(node);
            SelectionChanged?.Invoke();
        }

        /// <summary>
        /// 清除所有选择
        /// </summary>
        public void ClearAll()
        {
            foreach (var node in _selectedNodes)
            {
                node.IsSelected = false;
            }
            _selectedNodes.Clear();
            PrimarySelectedNode = null;
            OnPropertyChanged(nameof(SelectedNodes));
            SelectionCleared?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

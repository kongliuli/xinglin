using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ReportTemplateEditor.Designer.Controls
{
    /// <summary>
    /// DataPathSelector.xaml 的交互逻辑
    /// </summary>
    public partial class DataPathSelector : UserControl
    {
        /// <summary>
        /// 数据路径选择事件
        /// </summary>
        public event EventHandler<string> PathSelected;

        /// <summary>
        /// 当前选中的数据路径
        /// </summary>
        public string SelectedPath { get; private set; }

        /// <summary>
        /// 数据源对象
        /// </summary>
        public object DataSource { get; set; }

        /// <summary>
        /// 数据路径节点集合
        /// </summary>
        private List<DataPathNode> _dataPathNodes;

        public DataPathSelector()
        {
            InitializeComponent();
            this._dataPathNodes = new List<DataPathNode>();
        }

        /// <summary>
        /// 初始化数据路径选择器
        /// </summary>
        /// <param name="dataSource">数据源对象</param>
        public void Initialize(object dataSource)
        {
            this.DataSource = dataSource;
            this._dataPathNodes = GenerateDataPathNodes(dataSource, "");
            this.pathTreeView.ItemsSource = this._dataPathNodes;
        }

        /// <summary>
        /// 生成数据路径节点
        /// </summary>
        private List<DataPathNode> GenerateDataPathNodes(object data, string parentPath)
        {
            List<DataPathNode> nodes = new List<DataPathNode>();

            if (data == null)
            {
                return nodes;
            }

            Type type = data.GetType();
            string displayName = parentPath == "" ? type.Name : parentPath.Split('.').Last();

            // 如果是集合类型
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type != typeof(string))
            {
                DataPathNode collectionNode = new DataPathNode
                {
                    DisplayName = displayName,
                    FullPath = parentPath,
                    IsCollection = true,
                    Children = new List<DataPathNode>()
                };

                // 尝试获取集合中的第一个元素类型
                var enumerable = data as System.Collections.IEnumerable;
                if (enumerable != null)
                {
                    var firstItem = enumerable.Cast<object>().FirstOrDefault();
                    if (firstItem != null)
                    {
                        // 添加集合索引节点
                        DataPathNode indexNode = new DataPathNode
                        {
                            DisplayName = $"[{0}]",
                            FullPath = $"{parentPath}[0]",
                            IsCollectionItem = true,
                            Children = GenerateDataPathNodes(firstItem, $"{parentPath}[0]")
                        };
                        collectionNode.Children.Add(indexNode);
                    }
                }

                nodes.Add(collectionNode);
            }
            // 如果是对象类型
            else if (type.IsClass || type.IsValueType)
            {
                // 只处理非基本类型
                if (type.Namespace != null && !type.Namespace.StartsWith("System"))
                {
                    DataPathNode objectNode = new DataPathNode
                    {
                        DisplayName = displayName,
                        FullPath = parentPath,
                        IsObject = true,
                        Children = new List<DataPathNode>()
                    };

                    // 获取所有公共属性
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo property in properties)
                    {
                        string propertyPath = parentPath == "" ? property.Name : $"{parentPath}.{property.Name}";
                        object propertyValue = property.GetValue(data);

                        var propertyNodes = GenerateDataPathNodes(propertyValue, propertyPath);
                        if (propertyNodes.Count > 0)
                        {
                            objectNode.Children.AddRange(propertyNodes);
                        }
                        else
                        {
                            // 基本类型属性
                            DataPathNode propertyNode = new DataPathNode
                            {
                                DisplayName = property.Name,
                                FullPath = propertyPath,
                                IsProperty = true,
                                PropertyType = property.PropertyType,
                                Children = new List<DataPathNode>()
                            };
                            objectNode.Children.Add(propertyNode);
                        }
                    }

                    nodes.Add(objectNode);
                }
                else
                {
                    // 基本类型属性
                    DataPathNode propertyNode = new DataPathNode
                    {
                        DisplayName = displayName,
                        FullPath = parentPath,
                        IsProperty = true,
                        PropertyType = type,
                        Children = new List<DataPathNode>()
                    };
                    nodes.Add(propertyNode);
                }
            }

            return nodes;
        }

        /// <summary>
        /// 搜索按钮点击事件
        /// </summary>
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = this.searchTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                this.pathTreeView.ItemsSource = this._dataPathNodes;
                return;
            }

            // 搜索匹配的数据路径
            List<DataPathNode> searchResults = new List<DataPathNode>();
            SearchDataPathNodes(this._dataPathNodes, searchText, searchResults);
            this.pathTreeView.ItemsSource = searchResults;
        }

        /// <summary>
        /// 搜索数据路径节点
        /// </summary>
        private void SearchDataPathNodes(List<DataPathNode> nodes, string searchText, List<DataPathNode> results)
        {
            foreach (var node in nodes)
            {
                if (node.DisplayName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    node.FullPath.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(node);
                }

                if (node.Children != null && node.Children.Count > 0)
                {
                    SearchDataPathNodes(node.Children, searchText, results);
                }
            }
        }

        /// <summary>
        /// 数据路径选中事件
        /// </summary>
        private void pathTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is DataPathNode selectedNode)
            {
                this.SelectedPath = selectedNode.FullPath;
            }
        }

        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.PathSelected != null && !string.IsNullOrEmpty(this.SelectedPath))
            {
                this.PathSelected.Invoke(this, this.SelectedPath);
            }

            // 关闭对话框（如果是在对话框中使用）
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.DialogResult = true;
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // 关闭对话框（如果是在对话框中使用）
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.DialogResult = false;
            }
        }
    }

    /// <summary>
    /// 数据路径节点类
    /// </summary>
    public class DataPathNode : INotifyPropertyChanged
    {
        private string _displayName;
        private string _fullPath;
        private bool _isCollection;
        private bool _isObject;
        private bool _isProperty;
        private bool _isCollectionItem;
        private Type _propertyType;
        private List<DataPathNode> _children;
        private FontWeight _fontWeight = FontWeights.Normal;
        private Brush _foreground = Brushes.Black;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; OnPropertyChanged("DisplayName"); }
        }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; OnPropertyChanged("FullPath"); }
        }

        /// <summary>
        /// 是否为集合类型
        /// </summary>
        public bool IsCollection
        {
            get { return _isCollection; }
            set { _isCollection = value; OnPropertyChanged("IsCollection"); }
        }

        /// <summary>
        /// 是否为对象类型
        /// </summary>
        public bool IsObject
        {
            get { return _isObject; }
            set { _isObject = value; OnPropertyChanged("IsObject"); }
        }

        /// <summary>
        /// 是否为属性类型
        /// </summary>
        public bool IsProperty
        {
            get { return _isProperty; }
            set { _isProperty = value; OnPropertyChanged("IsProperty"); }
        }

        /// <summary>
        /// 是否为集合项
        /// </summary>
        public bool IsCollectionItem
        {
            get { return _isCollectionItem; }
            set { _isCollectionItem = value; OnPropertyChanged("IsCollectionItem"); }
        }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType
        {
            get { return _propertyType; }
            set { _propertyType = value; OnPropertyChanged("PropertyType"); }
        }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<DataPathNode> Children
        {
            get { return _children; }
            set { _children = value; OnPropertyChanged("Children"); }
        }

        /// <summary>
        /// 字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return _fontWeight; }
            set { _fontWeight = value; OnPropertyChanged("FontWeight"); }
        }

        /// <summary>
        /// 前景色
        /// </summary>
        public Brush Foreground
        {
            get { return _foreground; }
            set { _foreground = value; OnPropertyChanged("Foreground"); }
        }

        /// <summary>
        /// 属性改变通知
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

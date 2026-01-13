using System.Windows;

namespace ReportTemplateEditor.Designer
{
    /// <summary>
    /// DataPathSelectorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataPathSelectorWindow : Window
    {
        /// <summary>
        /// 选中的数据路径
        /// </summary>
        public string SelectedPath { get; private set; }

        /// <summary>
        /// 数据源对象
        /// </summary>
        public object DataSource { get; set; }

        public DataPathSelectorWindow()
        {
            InitializeComponent();
            this.dataPathSelector.PathSelected += DataPathSelector_PathSelected;
        }

        /// <summary>
        /// 初始化数据路径选择器
        /// </summary>
        public void Initialize()
        {
            if (this.DataSource != null)
            {
                this.dataPathSelector.Initialize(this.DataSource);
            }
        }

        /// <summary>
        /// 数据路径选择事件处理
        /// </summary>
        private void DataPathSelector_PathSelected(object sender, string e)
        {
            this.SelectedPath = e;
            this.DialogResult = true;
        }
    }
}
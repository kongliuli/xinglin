using System.Windows;
using System.Windows.Controls;

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

        public DataPathSelectorWindow()
        {
            InitializeComponent();
            InitializeDataTree();
        }

        /// <summary>
        /// 初始化数据树
        /// </summary>
        private void InitializeDataTree()
        {
            var rootItem = new TreeViewItem
            {
                Header = "数据对象",
                Tag = ""
            };

            var patientItem = new TreeViewItem
            {
                Header = "Patient (患者信息)",
                Tag = "Patient"
            };

            patientItem.Items.Add(new TreeViewItem { Header = "Name (姓名)", Tag = "Patient.Name" });
            patientItem.Items.Add(new TreeViewItem { Header = "Age (年龄)", Tag = "Patient.Age" });
            patientItem.Items.Add(new TreeViewItem { Header = "Gender (性别)", Tag = "Patient.Gender" });
            patientItem.Items.Add(new TreeViewItem { Header = "IdCard (身份证号)", Tag = "Patient.IdCard" });
            patientItem.Items.Add(new TreeViewItem { Header = "Phone (电话)", Tag = "Patient.Phone" });

            var reportItem = new TreeViewItem
            {
                Header = "Report (报告信息)",
                Tag = "Report"
            };

            reportItem.Items.Add(new TreeViewItem { Header = "ReportNo (报告编号)", Tag = "Report.ReportNo" });
            reportItem.Items.Add(new TreeViewItem { Header = "ReportDate (报告日期)", Tag = "Report.ReportDate" });
            reportItem.Items.Add(new TreeViewItem { Header = "Doctor (医生)", Tag = "Report.Doctor" });
            reportItem.Items.Add(new TreeViewItem { Header = "Department (科室)", Tag = "Report.Department" });

            var testItemsItem = new TreeViewItem
            {
                Header = "TestItems (检验项目)",
                Tag = "TestItems"
            };

            testItemsItem.Items.Add(new TreeViewItem { Header = "ItemName (项目名称)", Tag = "TestItems.ItemName" });
            testItemsItem.Items.Add(new TreeViewItem { Header = "Result (结果)", Tag = "TestItems.Result" });
            testItemsItem.Items.Add(new TreeViewItem { Header = "Unit (单位)", Tag = "TestItems.Unit" });
            testItemsItem.Items.Add(new TreeViewItem { Header = "Reference (参考值)", Tag = "TestItems.Reference" });

            rootItem.Items.Add(patientItem);
            rootItem.Items.Add(reportItem);
            rootItem.Items.Add(testItemsItem);

            dataPathTree.Items.Add(rootItem);
        }

        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataPathTree.SelectedItem is TreeViewItem selectedItem && selectedItem.Tag != null)
            {
                SelectedPath = selectedItem.Tag.ToString();
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("请选择一个数据路径", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xinglin.Core.Models;

namespace Xinglin.Client.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// 当前报告对象
    /// </summary>
    private Report currentReport = null!;

    /// <summary>
    /// 报告项目集合
    /// </summary>
    private ObservableCollection<ReportItem> reportItems = null!;

    public MainWindow()
    {
        InitializeComponent();
        InitializeReport();
    }

    /// <summary>
    /// 初始化报告数据
    /// </summary>
    private void InitializeReport()
    {
        // 创建新报告
        currentReport = new Report
        {
            ReportNumber = string.Empty,
            Patient = new Patient
            {
                VisitDate = DateTime.Now
            },
            ReportItems = new List<ReportItem>(),
            EntryTime = DateTime.Now,
            UpdateTime = DateTime.Now,
            Status = "草稿",
            ReportType = "病理检验报告",
            Operator = "当前用户"
        };

        // 初始化报告项目集合
        reportItems = new ObservableCollection<ReportItem>();
        dgReportItems.ItemsSource = reportItems;

        // 设置就诊日期为当前日期
        dpVisitDate.SelectedDate = DateTime.Now;
    }

    /// <summary>
    /// 新建报告按钮点击事件
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="e">事件参数</param>
    private void btnNewReport_Click(object sender, RoutedEventArgs e)
    {
        InitializeReport();
        ClearPatientInfo();
        lblStatus.Text = "已创建新报告";
    }

    /// <summary>
    /// 保存草稿按钮点击事件
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="e">事件参数</param>
    private void btnSaveDraft_Click(object sender, RoutedEventArgs e)
    {
        // 保存报告草稿逻辑
        SaveReportToDraft();
        lblStatus.Text = "报告已保存为草稿";
    }

    /// <summary>
    /// 预览按钮点击事件
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="e">事件参数</param>
    private void btnPreview_Click(object sender, RoutedEventArgs e)
    {
        // 预览报告逻辑
        PreviewReport();
        lblStatus.Text = "正在预览报告";
    }

    /// <summary>
    /// 打印按钮点击事件
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="e">事件参数</param>
    private void btnPrint_Click(object sender, RoutedEventArgs e)
    {
        // 打印报告逻辑
        PrintReport();
        lblStatus.Text = "正在打印报告";
    }

    /// <summary>
    /// 清空病人信息
    /// </summary>
    private void ClearPatientInfo()
    {
        txtPatientName.Text = string.Empty;
        cmbGender.SelectedIndex = -1;
        txtAge.Text = string.Empty;
        txtIdCardNumber.Text = string.Empty;
        txtContactNumber.Text = string.Empty;
        txtDepartment.Text = string.Empty;
        txtDoctor.Text = string.Empty;
        dpVisitDate.SelectedDate = DateTime.Now;
        txtHospitalizationNumber.Text = string.Empty;
        txtBedNumber.Text = string.Empty;
        reportItems.Clear();
    }

    /// <summary>
    /// 保存报告为草稿
    /// </summary>
    private void SaveReportToDraft()
    {
        // 更新报告数据
        UpdateReportData();
        
        // 这里可以添加保存到本地文件或数据库的逻辑
        // 例如：FileHelper.WriteJsonFile("draft_report.json", currentReport);
    }

    /// <summary>
    /// 预览报告
    /// </summary>
    private void PreviewReport()
    {
        // 更新报告数据
        UpdateReportData();
        
        // 这里可以添加报告预览逻辑
        MessageBox.Show("报告预览功能待实现");
    }

    /// <summary>
    /// 打印报告
    /// </summary>
    private void PrintReport()
    {
        // 更新报告数据
        UpdateReportData();
        
        // 这里可以添加报告打印逻辑
        MessageBox.Show("报告打印功能待实现");
    }

    /// <summary>
    /// 更新报告数据
    /// </summary>
    private void UpdateReportData()
    {
        // 更新病人信息
        currentReport.Patient.Name = txtPatientName.Text;
        currentReport.Patient.Gender = cmbGender.SelectedItem != null ? (cmbGender.SelectedItem as ComboBoxItem)?.Content?.ToString() : string.Empty;
        currentReport.Patient.Age = int.TryParse(txtAge.Text, out int age) ? age : 0;
        currentReport.Patient.IdCardNumber = txtIdCardNumber.Text;
        currentReport.Patient.ContactNumber = txtContactNumber.Text;
        currentReport.Patient.Department = txtDepartment.Text;
        currentReport.Patient.Doctor = txtDoctor.Text;
        currentReport.Patient.VisitDate = dpVisitDate.SelectedDate ?? DateTime.Now;
        currentReport.Patient.HospitalizationNumber = txtHospitalizationNumber.Text;
        currentReport.Patient.BedNumber = txtBedNumber.Text;

        // 更新报告项目
        currentReport.ReportItems = reportItems.ToList();
        currentReport.UpdateTime = DateTime.Now;
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xinglin.ReportTemplateEditor.WPF.Commands;
using Xinglin.Core.Models;

namespace Xinglin.ReportTemplateEditor.WPF.ViewModels
{
    /// <summary>
    /// 设置视图模型
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged
    {
        // 页面设置
        private double _defaultPageWidth = 210;
        private double _defaultPageHeight = 297;
        private double _defaultMarginLeft = 10;
        private double _defaultMarginRight = 10;
        private double _defaultMarginTop = 10;
        private double _defaultMarginBottom = 10;
        
        // 字体设置
        private string _defaultFontFamily = "微软雅黑";
        private double _defaultFontSize = 12;
        private string _defaultFontColor = "#000000";
        
        // PDF渲染设置
        private int _pdfDpi = 300;
        private int _pdfCompressionQuality = 80;
        
        // 界面设置
        private bool _isLightTheme = true;
        private bool _isDarkTheme;
        private string _selectedLanguage = "中文";
        
        /// <summary>
        /// 默认页面宽度（毫米）
        /// </summary>
        public double DefaultPageWidth
        {
            get => _defaultPageWidth;
            set => SetProperty(ref _defaultPageWidth, value);
        }
        
        /// <summary>
        /// 默认页面高度（毫米）
        /// </summary>
        public double DefaultPageHeight
        {
            get => _defaultPageHeight;
            set => SetProperty(ref _defaultPageHeight, value);
        }
        
        /// <summary>
        /// 默认左边距（毫米）
        /// </summary>
        public double DefaultMarginLeft
        {
            get => _defaultMarginLeft;
            set => SetProperty(ref _defaultMarginLeft, value);
        }
        
        /// <summary>
        /// 默认右边距（毫米）
        /// </summary>
        public double DefaultMarginRight
        {
            get => _defaultMarginRight;
            set => SetProperty(ref _defaultMarginRight, value);
        }
        
        /// <summary>
        /// 默认上边距（毫米）
        /// </summary>
        public double DefaultMarginTop
        {
            get => _defaultMarginTop;
            set => SetProperty(ref _defaultMarginTop, value);
        }
        
        /// <summary>
        /// 默认下边距（毫米）
        /// </summary>
        public double DefaultMarginBottom
        {
            get => _defaultMarginBottom;
            set => SetProperty(ref _defaultMarginBottom, value);
        }
        
        /// <summary>
        /// 默认字体名称
        /// </summary>
        public string DefaultFontFamily
        {
            get => _defaultFontFamily;
            set => SetProperty(ref _defaultFontFamily, value);
        }
        
        /// <summary>
        /// 默认字体大小
        /// </summary>
        public double DefaultFontSize
        {
            get => _defaultFontSize;
            set => SetProperty(ref _defaultFontSize, value);
        }
        
        /// <summary>
        /// 默认字体颜色
        /// </summary>
        public string DefaultFontColor
        {
            get => _defaultFontColor;
            set => SetProperty(ref _defaultFontColor, value);
        }
        
        /// <summary>
        /// PDF DPI
        /// </summary>
        public int PdfDpi
        {
            get => _pdfDpi;
            set => SetProperty(ref _pdfDpi, value);
        }
        
        /// <summary>
        /// PDF压缩质量
        /// </summary>
        public int PdfCompressionQuality
        {
            get => _pdfCompressionQuality;
            set => SetProperty(ref _pdfCompressionQuality, value);
        }
        
        /// <summary>
        /// 是否为浅色主题
        /// </summary>
        public bool IsLightTheme
        {
            get => _isLightTheme;
            set 
            {
                if (SetProperty(ref _isLightTheme, value) && value)
                {
                    IsDarkTheme = false;
                }
            }
        }
        
        /// <summary>
        /// 是否为深色主题
        /// </summary>
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set 
            {
                if (SetProperty(ref _isDarkTheme, value) && value)
                {
                    IsLightTheme = false;
                }
            }
        }
        
        /// <summary>
        /// 选中的语言
        /// </summary>
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }
        
        /// <summary>
        /// 可用字体列表
        /// </summary>
        public List<string> AvailableFonts
        {
            get
            {
                return new List<string>
                {
                    "微软雅黑",
                    "宋体",
                    "黑体",
                    "Arial",
                    "Times New Roman",
                    "Courier New"
                };
            }
        }
        
        /// <summary>
        /// 可用语言列表
        /// </summary>
        public List<string> AvailableLanguages
        {
            get
            {
                return new List<string>
                {
                    "中文",
                    "English"
                };
            }
        }
        
        /// <summary>
        /// 应用命令
        /// </summary>
        public ICommand ApplyCommand { get; set; }
        
        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand { get; set; }
        
        /// <summary>
        /// 取消命令
        /// </summary>
        public ICommand CancelCommand { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public SettingsViewModel()
        {
            // 初始化命令
            ApplyCommand = new RelayCommand<object>(Apply, CanApply);
            SaveCommand = new RelayCommand<object>(Save, CanSave);
            CancelCommand = new RelayCommand<object>(Cancel, CanCancel);
        }
        
        /// <summary>
        /// 应用设置
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void Apply(object parameter)
        {
            // 应用设置的逻辑
            OnSettingsApplied?.Invoke(this, null);
        }
        
        /// <summary>
        /// 是否可以应用设置
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以应用设置</returns>
        private bool CanApply(object parameter)
        {
            return true;
        }
        
        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void Save(object parameter)
        {
            // 保存设置的逻辑
            OnSettingsSaved?.Invoke(this, null);
        }
        
        /// <summary>
        /// 是否可以保存设置
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以保存设置</returns>
        private bool CanSave(object parameter)
        {
            return true;
        }
        
        /// <summary>
        /// 取消设置
        /// </summary>
        /// <param name="parameter">命令参数</param>
        private void Cancel(object parameter)
        {
            // 取消设置的逻辑
            OnSettingsCancelled?.Invoke(this, null);
        }
        
        /// <summary>
        /// 是否可以取消设置
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以取消设置</returns>
        private bool CanCancel(object parameter)
        {
            return true;
        }
        
        /// <summary>
        /// 设置应用事件
        /// </summary>
        public event System.EventHandler OnSettingsApplied;
        
        /// <summary>
        /// 设置保存事件
        /// </summary>
        public event System.EventHandler OnSettingsSaved;
        
        /// <summary>
        /// 设置取消事件
        /// </summary>
        public event System.EventHandler OnSettingsCancelled;
        
        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 设置属性值并通知更改
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="field">字段引用</param>
        /// <param name="value">新值</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>是否更改</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        
        /// <summary>
        /// 通知属性已更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
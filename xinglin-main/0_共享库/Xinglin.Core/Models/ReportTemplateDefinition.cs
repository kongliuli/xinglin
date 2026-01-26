using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Xinglin.Core.Elements;

namespace Xinglin.Core.Models
{
    /// <summary>
    /// 报告模板定义
    /// </summary>
    public class ReportTemplateDefinition : INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString();
        private string _name = "未命名模板";
        private string _filePath = string.Empty;
        private string _version = "1.0";
        private string _type = "病理报告";
        private string _hospitalId = string.Empty;
        private double _pageWidth = 210;
        private double _pageHeight = 297;
        private double _marginLeft = 10;
        private double _marginRight = 10;
        private double _marginTop = 10;
        private double _marginBottom = 10;
        private string _orientation = "Portrait";
        private string _backgroundColor = "#FFFFFF";
        private bool _isDefault = false;
        private bool _isForceUpdate = false;
        private double _globalFontSize = 12;
        private bool _enableGlobalFontSize = false;
        private DateTime _createTime = DateTime.Now;
        private DateTime _updateTime = DateTime.Now;

        /// <summary>
        /// 模板ID
        /// </summary>
        public string Id 
        { 
            get => _id; 
            set => SetProperty(ref _id, value); 
        }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name 
        { 
            get => _name; 
            set => SetProperty(ref _name, value); 
        }

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string FilePath 
        { 
            get => _filePath; 
            set => SetProperty(ref _filePath, value); 
        }

        /// <summary>
        /// 模板版本
        /// </summary>
        public string Version 
        { 
            get => _version; 
            set => SetProperty(ref _version, value); 
        }

        /// <summary>
        /// 模板类型
        /// </summary>
        public string Type 
        { 
            get => _type; 
            set => SetProperty(ref _type, value); 
        }

        /// <summary>
        /// 医院ID
        /// </summary>
        public string HospitalId 
        { 
            get => _hospitalId; 
            set => SetProperty(ref _hospitalId, value); 
        }

        /// <summary>
        /// 页面宽度（毫米）
        /// </summary>
        public double PageWidth 
        { 
            get => _pageWidth; 
            set => SetProperty(ref _pageWidth, value); 
        }

        /// <summary>
        /// 页面高度（毫米）
        /// </summary>
        public double PageHeight 
        { 
            get => _pageHeight; 
            set => SetProperty(ref _pageHeight, value); 
        }

        /// <summary>
        /// 左边距（毫米）
        /// </summary>
        public double MarginLeft 
        { 
            get => _marginLeft; 
            set => SetProperty(ref _marginLeft, value); 
        }

        /// <summary>
        /// 右边距（毫米）
        /// </summary>
        public double MarginRight 
        { 
            get => _marginRight; 
            set => SetProperty(ref _marginRight, value); 
        }

        /// <summary>
        /// 上边距（毫米）
        /// </summary>
        public double MarginTop 
        { 
            get => _marginTop; 
            set => SetProperty(ref _marginTop, value); 
        }

        /// <summary>
        /// 下边距（毫米）
        /// </summary>
        public double MarginBottom 
        { 
            get => _marginBottom; 
            set => SetProperty(ref _marginBottom, value); 
        }

        /// <summary>
        /// 页面方向
        /// </summary>
        public string Orientation 
        { 
            get => _orientation; 
            set => SetProperty(ref _orientation, value); 
        }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor 
        { 
            get => _backgroundColor; 
            set => SetProperty(ref _backgroundColor, value); 
        }

        /// <summary>
        /// 模板元素集合
        /// </summary>
        public List<ElementBase> Elements { get; set; } = new List<ElementBase>();

        /// <summary>
        /// 数据绑定集合
        /// </summary>
        public List<TemplateDataBinding> DataBindings { get; set; } = new List<TemplateDataBinding>();

        /// <summary>
        /// 是否为默认模板
        /// </summary>
        public bool IsDefault 
        { 
            get => _isDefault; 
            set => SetProperty(ref _isDefault, value); 
        }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool IsForceUpdate 
        { 
            get => _isForceUpdate; 
            set => SetProperty(ref _isForceUpdate, value); 
        }

        /// <summary>
        /// 全局字体大小
        /// </summary>
        public double GlobalFontSize 
        { 
            get => _globalFontSize; 
            set => SetProperty(ref _globalFontSize, value); 
        }

        /// <summary>
        /// 是否启用全局字体设置
        /// </summary>
        public bool EnableGlobalFontSize 
        { 
            get => _enableGlobalFontSize; 
            set => SetProperty(ref _enableGlobalFontSize, value); 
        }
    
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime 
        { 
            get => _createTime; 
            set => SetProperty(ref _createTime, value); 
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime 
        { 
            get => _updateTime; 
            set => SetProperty(ref _updateTime, value); 
        }

        /// <summary>
        /// 转换为现有系统的ReportTemplate格式
        /// </summary>
        /// <returns></returns>
        public string ToExistingReportTemplateContent()
        {
            // 这里可以实现与现有系统ReportTemplate的转换逻辑
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        
        /// <summary>
        /// 设置属性值并通知更改
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="field">字段引用</param>
        /// <param name="value">新值</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>是否更改</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
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
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 数据绑定
    /// </summary>
    public class TemplateDataBinding
    {
        /// <summary>
        /// 绑定唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 元素ID
        /// </summary>
        public string ElementId { get; set; } = string.Empty;

        /// <summary>
        /// 数据路径
        /// </summary>
        public string DataPath { get; set; } = string.Empty;

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string FormatString { get; set; } = string.Empty;
    }
}
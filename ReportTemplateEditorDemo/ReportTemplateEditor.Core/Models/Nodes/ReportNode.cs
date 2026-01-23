using System;
using System.ComponentModel;
using ReportTemplateEditor.Core.Models.Controls;

namespace ReportTemplateEditor.Core.Models.Nodes
{
    /// <summary>
    /// 节点基类
    /// </summary>
    public abstract class ReportNode : INotifyPropertyChanged
    {
        /// <summary>
        /// 节点唯一标识符
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// X坐标（毫米）
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标（毫米）
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 宽度（毫米）
        /// </summary>
        public double Width { get; set; } = 100;

        /// <summary>
        /// 高度（毫米）
        /// </summary>
        public double Height { get; set; } = 30;

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// 是否选中
        /// </summary>
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

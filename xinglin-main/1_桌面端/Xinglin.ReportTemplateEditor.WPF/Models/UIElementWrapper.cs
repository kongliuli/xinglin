using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Xinglin.ReportTemplateEditor.WPF.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Models
{
    /// <summary>
    /// UI元素包装器，用于统一管理画布上的元素
    /// </summary>
    public class UIElementWrapper : INotifyPropertyChanged
    {
        #region 私有字段

        private ElementBase _modelElement;
        private bool _isSelected;
        private bool _isDragging;
        private bool _isResizing;

        #endregion

        #region 公共属性

        /// <summary>
        /// 模型元素
        /// </summary>
        public ElementBase ModelElement
        {
            get => _modelElement;
            set
            {
                if (_modelElement != value)
                {
                    _modelElement = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Type));
                    OnPropertyChanged(nameof(X));
                    OnPropertyChanged(nameof(Y));
                    OnPropertyChanged(nameof(Width));
                    OnPropertyChanged(nameof(Height));
                    OnPropertyChanged(nameof(ZIndex));
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        /// <summary>
        /// 元素类型
        /// </summary>
        public string Type => _modelElement?.ElementType ?? string.Empty;

        /// <summary>
        /// X坐标
        /// </summary>
        public double X
        {
            get => _modelElement?.X ?? 0;
            set
            {
                if (_modelElement != null && _modelElement.X != value)
                {
                    _modelElement.X = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y
        {
            get => _modelElement?.Y ?? 0;
            set
            {
                if (_modelElement != null && _modelElement.Y != value)
                {
                    _modelElement.Y = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width
        {
            get => _modelElement?.Width ?? 0;
            set
            {
                if (_modelElement != null && _modelElement.Width != value)
                {
                    _modelElement.Width = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height
        {
            get => _modelElement?.Height ?? 0;
            set
            {
                if (_modelElement != null && _modelElement.Height != value)
                {
                    _modelElement.Height = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Z轴顺序
        /// </summary>
        public int ZIndex
        {
            get => _modelElement?.ZIndex ?? 0;
            set
            {
                if (_modelElement != null && _modelElement.ZIndex != value)
                {
                    _modelElement.ZIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible
        {
            get => _modelElement?.IsVisible ?? true;
            set
            {
                if (_modelElement != null && _modelElement.IsVisible != value)
                {
                    _modelElement.IsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否被选中
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

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        public bool IsDragging
        {
            get => _isDragging;
            set
            {
                if (_isDragging != value)
                {
                    _isDragging = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否正在调整大小
        /// </summary>
        public bool IsResizing
        {
            get => _isResizing;
            set
            {
                if (_isResizing != value)
                {
                    _isResizing = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化UIElementWrapper实例
        /// </summary>
        /// <param name="modelElement">模型元素</param>
        public UIElementWrapper(ElementBase modelElement)
        {
            _modelElement = modelElement ?? throw new ArgumentNullException(nameof(modelElement));
            _isSelected = false;
            _isDragging = false;
            _isResizing = false;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 更新元素位置
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        public void UpdatePosition(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 更新元素大小
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void UpdateSize(double width, double height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 移动元素
        /// </summary>
        /// <param name="deltaX">X方向增量</param>
        /// <param name="deltaY">Y方向增量</param>
        public void Move(double deltaX, double deltaY)
        {
            X += deltaX;
            Y += deltaY;
        }

        /// <summary>
        /// 选择元素
        /// </summary>
        public void Select()
        {
            IsSelected = true;
        }

        /// <summary>
        /// 取消选择元素
        /// </summary>
        public void Unselect()
        {
            IsSelected = false;
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        public void StartDrag()
        {
            IsDragging = true;
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        public void EndDrag()
        {
            IsDragging = false;
        }

        /// <summary>
        /// 开始调整大小
        /// </summary>
        public void StartResize()
        {
            IsResizing = true;
        }

        /// <summary>
        /// 结束调整大小
        /// </summary>
        public void EndResize()
        {
            IsResizing = false;
        }

        /// <summary>
        /// 复制元素
        /// </summary>
        /// <returns>复制的元素包装器</returns>
        public UIElementWrapper Copy()
        {
            var copiedElement = _modelElement.Clone();
            return new UIElementWrapper(copiedElement);
        }

        /// <summary>
        /// 检查点是否在元素内
        /// </summary>
        /// <param name="point">点坐标</param>
        /// <returns>是否在元素内</returns>
        public bool ContainsPoint(Point point)
        {
            return point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
        }

        #endregion

        #region 事件

        /// <summary>
        /// 属性更改事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region 私有方法

        /// <summary>
        /// 通知属性更改
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

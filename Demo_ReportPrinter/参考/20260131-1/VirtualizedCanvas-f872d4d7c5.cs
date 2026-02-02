// �ο��ļ� - ��ע�͵�
/*
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Demo_ReportPrinter.Models.CoreEntities;
using Demo_ReportPrinter.Helpers;

namespace Demo_ReportPrinter.Controls
{
    /// <summary>
    /// 虚拟化画布控�?- 用于优化大量元素的渲染性能
    /// </summary>
    public class VirtualizedCanvas : Canvas
    {
        #region 依赖属�?
        /// <summary>
        /// 数据源依赖属�?        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable<ControlElement>),
                typeof(VirtualizedCanvas),
                new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// 数据�?        /// </summary>
        public IEnumerable<ControlElement> ItemsSource
        {
            get => (IEnumerable<ControlElement>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// 可见区域依赖属�?        /// </summary>
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register(
                "Viewport",
                typeof(Rect),
                typeof(VirtualizedCanvas),
                new PropertyMetadata(Rect.Empty, OnViewportChanged));

        /// <summary>
        /// 可见区域
        /// </summary>
        public Rect Viewport
        {
            get => (Rect)GetValue(ViewportProperty);
            set => SetValue(ViewportProperty, value);
        }

        /// <summary>
        /// 缓冲区大小依赖属性（像素�?        /// </summary>
        public static readonly DependencyProperty BufferSizeProperty =
            DependencyProperty.Register(
                "BufferSize",
                typeof(double),
                typeof(VirtualizedCanvas),
                new PropertyMetadata(100.0));

        /// <summary>
        /// 缓冲区大�?        /// </summary>
        public double BufferSize
        {
            get => (double)GetValue(BufferSizeProperty);
            set => SetValue(BufferSizeProperty, value);
        }

        /// <summary>
        /// 是否启用虚拟化依赖属�?        /// </summary>
        public static readonly DependencyProperty IsVirtualizationEnabledProperty =
            DependencyProperty.Register(
                "IsVirtualizationEnabled",
                typeof(bool),
                typeof(VirtualizedCanvas),
                new PropertyMetadata(true));

        /// <summary>
        /// 是否启用虚拟�?        /// </summary>
        public bool IsVirtualizationEnabled
        {
            get => (bool)GetValue(IsVirtualizationEnabledProperty);
            set => SetValue(IsVirtualizationEnabledProperty, value);
        }

        #endregion

        #region 私有字段

        private readonly Dictionary<string, FrameworkElement> _realizedElements = new Dictionary<string, FrameworkElement>();
        private bool _isUpdating;
        private DispatcherTimer _updateTimer;

        #endregion

        #region 构造函�?
        public VirtualizedCanvas()
        {
            // 初始化更新计时器
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // �?0FPS
            };
            _updateTimer.Tick += UpdateTimer_Tick;

            // 启用位图缓存
            CacheMode = new BitmapCache();

            // 加载时触发更�?            Loaded += VirtualizedCanvas_Loaded;
        }

        #endregion

        #region 事件处理

        private void VirtualizedCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateVisibleElements();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            _updateTimer.Stop();
            UpdateVisibleElements();
        }

        #endregion

        #region 属性变更处�?
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VirtualizedCanvas)d).OnItemsSourceChanged(e.OldValue as IEnumerable<ControlElement>, e.NewValue as IEnumerable<ControlElement>);
        }

        private static void OnViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VirtualizedCanvas)d).OnViewportChanged(e.OldValue as Rect, e.NewValue as Rect);
        }

        private void OnItemsSourceChanged(IEnumerable<ControlElement> oldValue, IEnumerable<ControlElement> newValue)
        {
            ClearRealizedElements();
            ScheduleUpdate();
        }

        private void OnViewportChanged(Rect oldValue, Rect newValue)
        {
            ScheduleUpdate();
        }

        #endregion

        #region 更新逻辑

        /// <summary>
        /// 调度更新
        /// </summary>
        private void ScheduleUpdate()
        {
            if (IsVirtualizationEnabled)
            {
                // 使用计时器延迟更新，避免频繁操作
                _updateTimer.Stop();
                _updateTimer.Start();
            }
            else
            {
                // 禁用虚拟化时，直接更新所有元�?                UpdateAllElements();
            }
        }

        /// <summary>
        /// 更新可见元素
        /// </summary>
        private void UpdateVisibleElements()
        {
            if (_isUpdating || ItemsSource == null)
                return;

            try
            {
                _isUpdating = true;

                if (IsVirtualizationEnabled)
                {
                    UpdateVirtualizedElements();
                }
                else
                {
                    UpdateAllElements();
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        /// <summary>
        /// 虚拟化更�?        /// </summary>
        private void UpdateVirtualizedElements()
        {
            if (Viewport.IsEmpty)
                return;

            // 扩展视口以包含缓冲区
            var extendedViewport = new Rect(
                Viewport.X - BufferSize,
                Viewport.Y - BufferSize,
                Viewport.Width + BufferSize * 2,
                Viewport.Height + BufferSize * 2);

            // 获取可见元素
            var visibleItems = GetVisibleItems(extendedViewport);
            var visibleIds = new HashSet<string>();
            foreach (var item in visibleItems)
            {
                visibleIds.Add(item.ElementId);
            }

            // 移除不在视口中的元素
            var toRemove = new List<string>();
            foreach (var pair in _realizedElements)
            {
                if (!visibleIds.Contains(pair.Key))
                {
                    toRemove.Add(pair.Key);
                }
            }

            foreach (var id in toRemove)
            {
                if (_realizedElements.TryGetValue(id, out var element))
                {
                    Children.Remove(element);
                    _realizedElements.Remove(id);
                }
            }

            // 添加或更新可见元�?            foreach (var item in visibleItems)
            {
                if (!_realizedElements.ContainsKey(item.ElementId))
                {
                    // 创建新元�?                    var element = CreateElement(item);
                    if (element != null)
                    {
                        _realizedElements[item.ElementId] = element;
                        Children.Add(element);
                    }
                }
                else
                {
                    // 更新现有元素
                    UpdateElement(_realizedElements[item.ElementId], item);
                }
            }
        }

        /// <summary>
        /// 更新所有元素（非虚拟化模式�?        /// </summary>
        private void UpdateAllElements()
        {
            var allIds = new HashSet<string>();
            foreach (var item in ItemsSource)
            {
                allIds.Add(item.ElementId);

                if (!_realizedElements.ContainsKey(item.ElementId))
                {
                    var element = CreateElement(item);
                    if (element != null)
                    {
                        _realizedElements[item.ElementId] = element;
                        Children.Add(element);
                    }
                }
                else
                {
                    UpdateElement(_realizedElements[item.ElementId], item);
                }
            }

            // 移除不存在的元素
            var toRemove = new List<string>();
            foreach (var pair in _realizedElements)
            {
                if (!allIds.Contains(pair.Key))
                {
                    toRemove.Add(pair.Key);
                }
            }

            foreach (var id in toRemove)
            {
                if (_realizedElements.TryGetValue(id, out var element))
                {
                    Children.Remove(element);
                    _realizedElements.Remove(id);
                }
            }
        }

        #endregion

        #region 元素创建和更�?
        /// <summary>
        /// 获取可见的元�?        /// </summary>
        private List<ControlElement> GetVisibleItems(Rect viewport)
        {
            var visibleItems = new List<ControlElement>();

            foreach (var item in ItemsSource)
            {
                if (IsItemVisible(item, viewport))
                {
                    visibleItems.Add(item);
                }
            }

            return visibleItems;
        }

        /// <summary>
        /// 检查元素是否可�?        /// </summary>
        private bool IsItemVisible(ControlElement item, Rect viewport)
        {
            var itemRect = new Rect(item.X, item.Y, item.Width, item.Height);
            return viewport.IntersectsWith(itemRect);
        }

        /// <summary>
        /// 创建UI元素
        /// </summary>
        private FrameworkElement CreateElement(ControlElement item)
        {
            // 根据元素类型创建对应的UI元素
            FrameworkElement element = null;

            switch (item.Type)
            {
                case ControlType.TextBlock:
                    element = new System.Windows.Controls.TextBlock
                    {
                        Text = item.DisplayName,
                        Width = item.Width,
                        Height = item.Height,
                        Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                        Padding = new Thickness(4),
                        TextAlignment = System.Windows.TextAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    break;

                case ControlType.TextBox:
                    element = new System.Windows.Controls.TextBox
                    {
                        Text = item.GetDefaultValue(),
                        Width = item.Width,
                        Height = item.Height,
                        Padding = new Thickness(4),
                        IsReadOnly = item.EditState != EditableState.Editable
                    };
                    break;

                case ControlType.Image:
                    element = new System.Windows.Controls.Image
                    {
                        Width = item.Width,
                        Height = item.Height,
                        Stretch = Stretch.UniformToFill
                    };
                    break;

                case ControlType.GroupBox:
                    element = new System.Windows.Controls.GroupBox
                    {
                        Header = item.DisplayName,
                        Width = item.Width,
                        Height = item.Height,
                        Background = new SolidColorBrush(Color.FromRgb(248, 248, 248)),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(4)
                    };
                    break;

                case ControlType.Label:
                    element = new System.Windows.Controls.Label
                    {
                        Content = item.DisplayName,
                        Width = item.Width,
                        Height = item.Height,
                        Padding = new Thickness(4)
                    };
                    break;

                default:
                    element = new System.Windows.Controls.Border
                    {
                        Width = item.Width,
                        Height = item.Height,
                        Background = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                        BorderThickness = new Thickness(1),
                        Child = new System.Windows.Controls.TextBlock
                        {
                            Text = item.DisplayName,
                            TextAlignment = System.Windows.TextAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    };
                    break;
            }

            if (element != null)
            {
                element.Tag = item;
                SetLeft(element, item.X);
                SetTop(element, item.Y);
            }

            return element;
        }

        /// <summary>
        /// 更新元素属�?        /// </summary>
        private void UpdateElement(FrameworkElement element, ControlElement item)
        {
            if (element == null)
                return;

            // 更新位置和大�?            SetLeft(element, item.X);
            SetTop(element, item.Y);
            element.Width = item.Width;
            element.Height = item.Height;

            // 根据元素类型更新内容
            switch (item.Type)
            {
                case ControlType.TextBlock:
                    if (element is System.Windows.Controls.TextBlock textBlock)
                    {
                        textBlock.Text = item.DisplayName;
                    }
                    break;

                case ControlType.TextBox:
                    if (element is System.Windows.Controls.TextBox textBox)
                    {
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            textBox.Text = item.GetDefaultValue();
                        }
                    }
                    break;
            }
        }

        #endregion

        #region 清理

        /// <summary>
        /// 清除所有已实现的元�?        /// </summary>
        private void ClearRealizedElements()
        {
            Children.Clear();
            _realizedElements.Clear();
        }

        #endregion

        #region 性能优化方法

        /// <summary>
        /// 获取当前实现的元素数�?        /// </summary>
        public int GetRealizedElementCount()
        {
            return _realizedElements.Count;
        }

        /// <summary>
        /// 获取总元素数�?        /// </summary>
        public int GetTotalElementCount()
        {
            if (ItemsSource == null)
                return 0;

            int count = 0;
            foreach (var _ in ItemsSource)
            {
                count++;
            }
            return count;
        }

        /// <summary>
        /// 获取虚拟化效率（已实现元素数 / 总元素数�?        /// </summary>
        public double GetVirtualizationEfficiency()
        {
            int total = GetTotalElementCount();
            if (total == 0)
                return 1.0;

            int realized = GetRealizedElementCount();
            return 1.0 - (double)realized / total;
        }

        /// <summary>
        /// 强制更新可见元素
        /// </summary>
        public void ForceUpdate()
        {
            UpdateVisibleElements();
        }

        #endregion

        #region 资源清理

        protected override void OnRender(DrawingContext dc)
        {
            // 可以在这里实现自定义渲染优化
            base.OnRender(dc);
        }

        #endregion
    }
}

*/

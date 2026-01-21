using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ReportTemplateEditor.Designer.Models;
using TemplateElements = ReportTemplateEditor.Core.Models.Elements;

namespace ReportTemplateEditor.Designer.Services
{
    /// <summary>
    /// 元素选择管理器，负责管理画布元素的选择状态
    /// </summary>
    /// <remarks>
    /// 功能特性：
    /// - 管理单个或多个元素的选择
    /// - 维护主选中元素（PrimarySelectedElement）
    /// - 提供选择事件通知
    /// - 自动更新选中元素的边框样式
    /// </remarks>
    public class ElementSelectionManager
    {
        /// <summary>
        /// 元素包装器列表
        /// </summary>
        private readonly List<UIElementWrapper> _elementWrappers;

        /// <summary>
        /// 选中的元素列表
        /// </summary>
        private readonly List<UIElementWrapper> _selectedElements;

        /// <summary>
        /// 主选中元素
        /// </summary>
        private UIElementWrapper _primarySelectedElement;

        /// <summary>
        /// 元素选中事件
        /// </summary>
        public event Action<UIElementWrapper> ElementSelected;

        /// <summary>
        /// 选择清除事件
        /// </summary>
        public event Action SelectionCleared;

        /// <summary>
        /// 选择变更事件
        /// </summary>
        public event Action SelectionChanged;

        /// <summary>
        /// 初始化ElementSelectionManager实例
        /// </summary>
        /// <param name="elementWrappers">元素包装器列表</param>
        /// <exception cref="ArgumentNullException">当elementWrappers参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// var wrappers = new List&lt;UIElementWrapper&gt;();
        /// var manager = new ElementSelectionManager(wrappers);
        /// </code>
        /// </example>
        public ElementSelectionManager(List<UIElementWrapper> elementWrappers)
        {
            _elementWrappers = elementWrappers ?? throw new ArgumentNullException(nameof(elementWrappers));
            _selectedElements = new List<UIElementWrapper>();
        }

        /// <summary>
        /// 获取选中的元素列表
        /// </summary>
        public List<UIElementWrapper> SelectedElements => _selectedElements;

        /// <summary>
        /// 获取主选中元素
        /// </summary>
        public UIElementWrapper PrimarySelectedElement => _primarySelectedElement;

        /// <summary>
        /// 选择元素
        /// </summary>
        /// <param name="wrapper">要选择的元素包装器</param>
        /// <param name="clearPrevious">是否清除之前的选择</param>
        /// <exception cref="ArgumentNullException">当wrapper参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// manager.SelectElement(elementWrapper, true);
        /// </code>
        /// </example>
        public void SelectElement(UIElementWrapper wrapper, bool clearPrevious = true)
        {
            if (wrapper == null)
                throw new ArgumentNullException(nameof(wrapper));

            if (!wrapper.ModelElement.IsVisible)
            {
                return;
            }

            if (clearPrevious)
            {
                ClearSelection();
            }

            if (!_selectedElements.Contains(wrapper))
            {
                _selectedElements.Add(wrapper);
                wrapper.SetSelection(true);
                _primarySelectedElement = wrapper;
                UpdateSelectedElementsBorderThickness();
                ElementSelected?.Invoke(wrapper);
                SelectionChanged?.Invoke();
            }
        }

        /// <summary>
        /// 清除所有选择
        /// </summary>
        /// <example>
        /// <code>
        /// manager.ClearSelection();
        /// </code>
        /// </example>
        public void ClearSelection()
        {
            foreach (var wrapper in _selectedElements)
            {
                wrapper.SetSelection(false);
            }

            _selectedElements.Clear();
            _primarySelectedElement = null;
            SelectionCleared?.Invoke();
            SelectionChanged?.Invoke();
        }

        /// <summary>
        /// 添加元素到选择
        /// </summary>
        /// <param name="wrapper">要添加的元素包装器</param>
        /// <exception cref="ArgumentNullException">当wrapper参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// manager.AddToSelection(elementWrapper);
        /// </code>
        /// </example>
        public void AddToSelection(UIElementWrapper wrapper)
        {
            if (wrapper == null)
                throw new ArgumentNullException(nameof(wrapper));

            if (!_selectedElements.Contains(wrapper))
            {
                _selectedElements.Add(wrapper);
                wrapper.SetSelection(true);
                _primarySelectedElement = wrapper;
                UpdateSelectedElementsBorderThickness();
                SelectionChanged?.Invoke();
            }
        }

        /// <summary>
        /// 从选择中移除元素
        /// </summary>
        /// <param name="wrapper">要移除的元素包装器</param>
        /// <exception cref="ArgumentNullException">当wrapper参数为null时抛出</exception>
        /// <example>
        /// <code>
        /// manager.RemoveFromSelection(elementWrapper);
        /// </code>
        /// </example>
        public void RemoveFromSelection(UIElementWrapper wrapper)
        {
            if (wrapper == null)
                throw new ArgumentNullException(nameof(wrapper));

            if (_selectedElements.Remove(wrapper))
            {
                wrapper.SetSelection(false);

                if (_primarySelectedElement == wrapper)
                {
                    _primarySelectedElement = _selectedElements.Count > 0 ? _selectedElements[_selectedElements.Count - 1] : null;
                }

                UpdateSelectedElementsBorderThickness();
                SelectionChanged?.Invoke();
            }
        }

        /// <summary>
        /// 更新选中元素的边框样式
        /// </summary>
        /// <remarks>
        /// 主选中元素使用蓝色粗边框
        /// 其他选中元素使用浅蓝色细边框
        /// </remarks>
        private void UpdateSelectedElementsBorderThickness()
        {
            for (int i = 0; i < _selectedElements.Count; i++)
            {
                var wrapper = _selectedElements[i];
                if (wrapper.SelectionBorder != null)
                {
                    if (wrapper == _primarySelectedElement)
                    {
                        wrapper.SelectionBorder.BorderThickness = new Thickness(2);
                        wrapper.SelectionBorder.BorderBrush = Brushes.Blue;
                    }
                    else
                    {
                        wrapper.SelectionBorder.BorderThickness = new Thickness(1);
                        wrapper.SelectionBorder.BorderBrush = Brushes.LightBlue;
                    }
                }
            }
        }

        /// <summary>
        /// 根据视觉对象查找元素包装器
        /// </summary>
        /// <param name="visual">视觉对象</param>
        /// <returns>找到的元素包装器，未找到返回null</returns>
        /// <example>
        /// <code>
        /// var wrapper = manager.FindElementWrapper(uiElement);
        /// </code>
        /// </example>
        public UIElementWrapper FindElementWrapper(DependencyObject visual)
        {
            if (visual == null)
                return null;

            var uiElement = visual as UIElement;
            if (uiElement == null)
                return null;

            return _elementWrappers.FirstOrDefault(w => w.UiElement == uiElement || w.UiElement.IsAncestorOf(uiElement));
        }

        /// <summary>
        /// 判断元素是否被选中
        /// </summary>
        /// <param name="wrapper">元素包装器</param>
        /// <returns>如果元素被选中返回true，否则返回false</returns>
        /// <example>
        /// <code>
        /// var isSelected = manager.IsElementSelected(elementWrapper);
        /// </code>
        /// </example>
        public bool IsElementSelected(UIElementWrapper wrapper)
        {
            return wrapper != null && _selectedElements.Contains(wrapper);
        }

        /// <summary>
        /// 获取选中元素的数量
        /// </summary>
        public int SelectedCount => _selectedElements.Count;

        /// <summary>
        /// 判断是否有选中的元素
        /// </summary>
        public bool HasSelection => _selectedElements.Count > 0;
    }
}

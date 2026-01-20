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
    public class ElementSelectionManager
    {
        private readonly List<UIElementWrapper> _elementWrappers;
        private readonly List<UIElementWrapper> _selectedElements;
        private UIElementWrapper _primarySelectedElement;

        public event Action<UIElementWrapper> ElementSelected;
        public event Action SelectionCleared;
        public event Action SelectionChanged;

        public ElementSelectionManager(List<UIElementWrapper> elementWrappers)
        {
            _elementWrappers = elementWrappers ?? throw new ArgumentNullException(nameof(elementWrappers));
            _selectedElements = new List<UIElementWrapper>();
        }

        public List<UIElementWrapper> SelectedElements => _selectedElements;
        public UIElementWrapper PrimarySelectedElement => _primarySelectedElement;

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

        public UIElementWrapper FindElementWrapper(DependencyObject visual)
        {
            if (visual == null)
                return null;

            var uiElement = visual as UIElement;
            if (uiElement == null)
                return null;

            return _elementWrappers.FirstOrDefault(w => w.UiElement == uiElement || w.UiElement.IsAncestorOf(uiElement));
        }

        public bool IsElementSelected(UIElementWrapper wrapper)
        {
            return wrapper != null && _selectedElements.Contains(wrapper);
        }

        public int SelectedCount => _selectedElements.Count;

        public bool HasSelection => _selectedElements.Count > 0;
    }
}

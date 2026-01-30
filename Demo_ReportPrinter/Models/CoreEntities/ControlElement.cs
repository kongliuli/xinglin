using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace Demo_ReportPrinter.Models.CoreEntities
{
    /// <summary>
    /// 控件类型枚举
    /// </summary>
    public enum ControlType
    {
        TextBox,
        ComboBox,
        DatePicker,
        CheckBox,
        Table,
        Image,
        FixedText,
        Chart,
        Line,
        Rectangle,
        Circle
    }

    /// <summary>
    /// 可编辑状态枚举
    /// </summary>
    public enum EditableState
    {
        ReadOnly,      // 只读（固定内容）
        Editable,      // 可编辑
        Locked          // 锁定（临时不可编辑）
    }

    /// <summary>
    /// 控件元素 - 表示画布上的一个控件
    /// </summary>
    public partial class ControlElement : ObservableObject
    {
        [ObservableProperty]
        private string _elementId;

        [ObservableProperty]
        private ControlType _type;

        [ObservableProperty]
        private string _displayName;

        [ObservableProperty]
        private double _x;

        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;

        [ObservableProperty]
        private object _value;

        [ObservableProperty]
        private EditableState _editState;

        /// <summary>
        /// 层级顺序
        /// </summary>
        [ObservableProperty]
        private int _zIndex;

        /// <summary>
        /// 是否选中
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;

        /// <summary>
        /// 是否可移动
        /// </summary>
        public bool CanMove => EditState != EditableState.ReadOnly;

        /// <summary>
        /// 是否可调整大小
        /// </summary>
        public bool CanResize => EditState == EditableState.Editable;

        /// <summary>
        /// 扩展属性 - 用于存储特定控件的配置
        /// 如表格的列配置、下拉框的选项等
        /// </summary>
        public ObservableCollection<KeyValuePair<string, object>> Properties { get; set; }

        public ControlElement()
        {
            ElementId = Guid.NewGuid().ToString();
            Properties = new ObservableCollection<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// 克隆控件
        /// </summary>
        /// <returns>克隆后的控件</returns>
        public ControlElement Clone()
        {
            var cloned = new ControlElement
            {
                ElementId = Guid.NewGuid().ToString(),
                Type = Type,
                DisplayName = $"{DisplayName} (副本)",
                X = X + 10,
                Y = Y + 10,
                Width = Width,
                Height = Height,
                Value = Value,
                EditState = EditState,
                ZIndex = ZIndex
            };

            // 克隆属性
            foreach (var prop in Properties)
            {
                cloned.Properties.Add(new KeyValuePair<string, object>(prop.Key, prop.Value));
            }

            return cloned;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="key">属性键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>属性值</returns>
        public T GetProperty<T>(string key, T defaultValue = default)
        {
            var prop = Properties.FirstOrDefault(p => p.Key == key);
            if (prop.Value is T value)
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="key">属性键</param>
        /// <param name="value">属性值</param>
        public void SetProperty(string key, object value)
        {
            var existing = Properties.FirstOrDefault(p => p.Key == key);
            if (existing.Key != null)
            {
                Properties.Remove(existing);
            }
            Properties.Add(new KeyValuePair<string, object>(key, value));
        }
    }
}
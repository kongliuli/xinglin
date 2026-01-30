using Demo_ReportPrinter.Models.CoreEntities;
using System.Collections.ObjectModel;

namespace Demo_ReportPrinter.Services.UndoRedo
{
    /// <summary>
    /// 添加控件命令
    /// </summary>
    public class AddControlCommand : ICommand
    {
        private readonly ObservableCollection<ControlElement> _elements;
        private readonly ControlElement _element;

        public AddControlCommand(ObservableCollection<ControlElement> elements, ControlElement element)
        {
            _elements = elements;
            _element = element;
        }

        public void Execute()
        {
            _elements.Add(_element);
        }

        public void Undo()
        {
            _elements.Remove(_element);
        }

        public void Redo()
        {
            Execute();
        }

        public string Description => "添加控件";
    }

    /// <summary>
    /// 删除控件命令
    /// </summary>
    public class RemoveControlCommand : ICommand
    {
        private readonly ObservableCollection<ControlElement> _elements;
        private readonly ControlElement _element;
        private readonly int _index;

        public RemoveControlCommand(ObservableCollection<ControlElement> elements, ControlElement element)
        {
            _elements = elements;
            _element = element;
            _index = elements.IndexOf(element);
        }

        public void Execute()
        {
            _elements.Remove(_element);
        }

        public void Undo()
        {
            if (_index >= 0 && _index < _elements.Count)
            {
                _elements.Insert(_index, _element);
            }
            else
            {
                _elements.Add(_element);
            }
        }

        public void Redo()
        {
            Execute();
        }

        public string Description => "删除控件";
    }

    /// <summary>
    /// 移动控件命令
    /// </summary>
    public class MoveControlCommand : ICommand
    {
        private readonly ControlElement _element;
        private readonly double _oldX;
        private readonly double _oldY;
        private readonly double _newX;
        private readonly double _newY;

        public MoveControlCommand(ControlElement element, double oldX, double oldY, double newX, double newY)
        {
            _element = element;
            _oldX = oldX;
            _oldY = oldY;
            _newX = newX;
            _newY = newY;
        }

        public void Execute()
        {
            _element.X = _newX;
            _element.Y = _newY;
        }

        public void Undo()
        {
            _element.X = _oldX;
            _element.Y = _oldY;
        }

        public void Redo()
        {
            Execute();
        }

        public string Description => "移动控件";
    }

    /// <summary>
    /// 调整控件大小命令
    /// </summary>
    public class ResizeControlCommand : ICommand
    {
        private readonly ControlElement _element;
        private readonly double _oldWidth;
        private readonly double _oldHeight;
        private readonly double _newWidth;
        private readonly double _newHeight;

        public ResizeControlCommand(ControlElement element, double oldWidth, double oldHeight, double newWidth, double newHeight)
        {
            _element = element;
            _oldWidth = oldWidth;
            _oldHeight = oldHeight;
            _newWidth = newWidth;
            _newHeight = newHeight;
        }

        public void Execute()
        {
            _element.Width = _newWidth;
            _element.Height = _newHeight;
        }

        public void Undo()
        {
            _element.Width = _oldWidth;
            _element.Height = _oldHeight;
        }

        public void Redo()
        {
            Execute();
        }

        public string Description => "调整控件大小";
    }

    /// <summary>
    /// 更改控件属性命令
    /// </summary>
    public class ChangeControlPropertyCommand : ICommand
    {
        private readonly ControlElement _element;
        private readonly string _propertyName;
        private readonly object _oldValue;
        private readonly object _newValue;

        public ChangeControlPropertyCommand(ControlElement element, string propertyName, object oldValue, object newValue)
        {
            _element = element;
            _propertyName = propertyName;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public void Execute()
        {
            SetProperty(_newValue);
        }

        public void Undo()
        {
            SetProperty(_oldValue);
        }

        public void Redo()
        {
            Execute();
        }

        public string Description => "更改控件属性";

        private void SetProperty(object value)
        {
            switch (_propertyName)
            {
                case "DisplayName":
                    _element.DisplayName = value as string;
                    break;
                case "EditState":
                    if (value is EditableState state)
                    {
                        _element.EditState = state;
                    }
                    break;
                case "ZIndex":
                    if (value is int zIndex)
                    {
                        _element.ZIndex = zIndex;
                    }
                    break;
                case "Value":
                    _element.Value = value;
                    break;
                case "Options":
                    _element.SetProperty("Options", value);
                    break;
                // 可以添加更多属性
            }
        }
    }
}

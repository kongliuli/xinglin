using System.Windows;using System.Windows.Controls;using System.Windows.Media;using System.Windows.Shapes;using Xinglin.Core.Elements;

namespace Xinglin.ReportTemplateEditor.WPF.Controls
{
    /// <summary>
    /// 线条控件，用于显示和编辑LineElement
    /// </summary>
    public class LineControl : ElementControlBase
    {
        private Line _line;
        
        public LineControl()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            _line = new Line
            {
                StrokeThickness = 1,
                Stroke = Brushes.Black
            };
            
            Content = _line;
        }
        
        /// <summary>
        /// 从元素更新控件属性
        /// </summary>
        public override void UpdateFromElement()
        {
            if (Element is not LineElement lineElement)
                return;
            
            ApplyElementProperties();
            
            // 设置线条属性
            try
            {
                _line.Stroke = (Brush)new BrushConverter().ConvertFromString(lineElement.LineColor);
            }
            catch (System.Exception)
            {
                _line.Stroke = Brushes.Black;
            }
            
            _line.StrokeThickness = lineElement.LineWidth;
            
            // 设置线条样式
            switch (lineElement.LineStyle?.ToLower())
            {
                case "dash":
                    _line.StrokeDashArray = new DoubleCollection { 4, 2 };
                    break;
                case "dot":
                    _line.StrokeDashArray = new DoubleCollection { 1, 1 };
                    break;
                case "dashdot":
                    _line.StrokeDashArray = new DoubleCollection { 4, 2, 1, 2 };
                    break;
                case "dashdotdot":
                    _line.StrokeDashArray = new DoubleCollection { 4, 2, 1, 2, 1, 2 };
                    break;
                default:
                    _line.StrokeDashArray = null;
                    break;
            }
            
            // 设置线条坐标
            _line.X1 = lineElement.StartX;
            _line.Y1 = lineElement.StartY;
            _line.X2 = lineElement.EndX;
            _line.Y2 = lineElement.EndY;
        }
        
        /// <summary>
        /// 从控件更新元素属性
        /// </summary>
        public override void UpdateToElement()
        {
            if (Element is not LineElement lineElement)
                return;
            
            GetControlProperties();
            
            // 更新线条属性
            lineElement.LineWidth = _line.StrokeThickness;
            lineElement.StartX = _line.X1;
            lineElement.StartY = _line.Y1;
            lineElement.EndX = _line.X2;
            lineElement.EndY = _line.Y2;
            
            // 更新线条颜色
            lineElement.LineColor = _line.Stroke.ToString();
            
            // 更新线条样式
            if (_line.StrokeDashArray == null || _line.StrokeDashArray.Count == 0)
            {
                lineElement.LineStyle = "Solid";
            }
            else if (_line.StrokeDashArray.Count == 2 && _line.StrokeDashArray[0] == 4 && _line.StrokeDashArray[1] == 2)
            {
                lineElement.LineStyle = "Dash";
            }
            else if (_line.StrokeDashArray.Count == 2 && _line.StrokeDashArray[0] == 1 && _line.StrokeDashArray[1] == 1)
            {
                lineElement.LineStyle = "Dot";
            }
            else if (_line.StrokeDashArray.Count == 4 && _line.StrokeDashArray[0] == 4 && _line.StrokeDashArray[1] == 2 && _line.StrokeDashArray[2] == 1 && _line.StrokeDashArray[3] == 2)
            {
                lineElement.LineStyle = "DashDot";
            }
            else if (_line.StrokeDashArray.Count == 6)
            {
                lineElement.LineStyle = "DashDotDot";
            }
        }
    }
}
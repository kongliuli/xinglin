using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ReportTemplateEditor.Designer.Services
{
    public class GridHelper
    {
        private Canvas _designCanvas;
        private bool _showGrid;
        private bool _snapToGrid;
        private double _gridSize;
        private Rectangle _gridBackground;

        public event Action<double> GridSizeChanged;

        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                if (_showGrid != value)
                {
                    _showGrid = value;
                    DrawGrid();
                }
            }
        }

        public bool IsSnapToGridEnabled
        {
            get => _snapToGrid;
            set => _snapToGrid = value;
        }

        public double GridSize
        {
            get => _gridSize;
            set
            {
                if (_gridSize != value)
                {
                    _gridSize = value;
                    GridSizeChanged?.Invoke(_gridSize);
                    DrawGrid();
                }
            }
        }

        public GridHelper(Canvas designCanvas)
        {
            _designCanvas = designCanvas ?? throw new ArgumentNullException(nameof(designCanvas));
            _showGrid = true;
            _snapToGrid = false;
            _gridSize = 10;
        }

        public GridHelper(Canvas designCanvas, bool showGrid, bool snapToGrid, double gridSize)
        {
            _designCanvas = designCanvas ?? throw new ArgumentNullException(nameof(designCanvas));
            _showGrid = showGrid;
            _snapToGrid = snapToGrid;
            _gridSize = gridSize;
        }

        public void DrawGrid()
        {
            if (_designCanvas == null)
                return;

            ClearGrid();

            if (!_showGrid)
                return;

            double canvasWidth = _designCanvas.ActualWidth;
            double canvasHeight = _designCanvas.ActualHeight;

            if (canvasWidth <= 0 || canvasHeight <= 0)
                return;

            var gridBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));

            var drawingGroup = new DrawingGroup();
            
            for (double x = 0; x <= canvasWidth; x += _gridSize)
            {
                var verticalLine = new GeometryDrawing
                {
                    Brush = gridBrush,
                    Pen = new Pen(gridBrush, 0.5),
                    Geometry = new LineGeometry(new Point(x, 0), new Point(x, canvasHeight))
                };
                drawingGroup.Children.Add(verticalLine);
            }

            for (double y = 0; y <= canvasHeight; y += _gridSize)
            {
                var horizontalLine = new GeometryDrawing
                {
                    Brush = gridBrush,
                    Pen = new Pen(gridBrush, 0.5),
                    Geometry = new LineGeometry(new Point(0, y), new Point(canvasWidth, y))
                };
                drawingGroup.Children.Add(horizontalLine);
            }

            var drawingBrush = new DrawingBrush(drawingGroup)
            {
                TileMode = TileMode.None,
                Stretch = Stretch.None
            };

            _gridBackground = new Rectangle
            {
                Width = canvasWidth,
                Height = canvasHeight,
                Fill = drawingBrush,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(_gridBackground, 0);
            Canvas.SetTop(_gridBackground, 0);
            Canvas.SetZIndex(_gridBackground, -1000);
            
            _designCanvas.Children.Add(_gridBackground);
        }

        public void ClearGrid()
        {
            if (_gridBackground != null && _designCanvas.Children.Contains(_gridBackground))
            {
                _designCanvas.Children.Remove(_gridBackground);
                _gridBackground = null;
            }
        }

        public double SnapToGrid(double value)
        {
            if (!_snapToGrid)
                return value;

            return Math.Round(value / _gridSize) * _gridSize;
        }

        public Point SnapToGrid(Point point)
        {
            if (!_snapToGrid)
                return point;

            return new Point(SnapToGrid(point.X), SnapToGrid(point.Y));
        }

        public void CalculateAndSetGridSize(double templateWidth, double templateHeight, double zoomLevel = 1.0)
        {
            if (templateWidth <= 0 || templateHeight <= 0)
                return;

            double effectiveWidth = templateWidth * zoomLevel;
            double effectiveHeight = templateHeight * zoomLevel;

            double minDimension = Math.Min(effectiveWidth, effectiveHeight);

            if (minDimension < 200)
            {
                _gridSize = 5;
            }
            else if (minDimension < 400)
            {
                _gridSize = 10;
            }
            else if (minDimension < 800)
            {
                _gridSize = 15;
            }
            else if (minDimension < 1200)
            {
                _gridSize = 20;
            }
            else
            {
                _gridSize = 25;
            }

            DrawGrid();
        }

        public void UpdateGrid(double canvasWidth, double canvasHeight)
        {
            if (_showGrid)
            {
                DrawGrid();
            }
        }

        public void SetGridVisibility(bool showGrid)
        {
            ShowGrid = showGrid;
        }

        public void SetSnapToGrid(bool snapToGrid)
        {
            IsSnapToGridEnabled = snapToGrid;
        }

        public void SetGridSize(double gridSize)
        {
            GridSize = gridSize;
        }
    }
}

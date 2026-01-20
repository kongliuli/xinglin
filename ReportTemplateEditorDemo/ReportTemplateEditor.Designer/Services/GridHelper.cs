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
        private List<Line> _gridLines;

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
            _gridLines = new List<Line>();
        }

        public GridHelper(Canvas designCanvas, bool showGrid, bool snapToGrid, double gridSize)
        {
            _designCanvas = designCanvas ?? throw new ArgumentNullException(nameof(designCanvas));
            _showGrid = showGrid;
            _snapToGrid = snapToGrid;
            _gridSize = gridSize;
            _gridLines = new List<Line>();
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

            for (double x = 0; x <= canvasWidth; x += _gridSize)
            {
                var verticalLine = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = canvasHeight,
                    Stroke = gridBrush,
                    StrokeThickness = 0.5,
                    IsHitTestVisible = false
                };
                _designCanvas.Children.Insert(0, verticalLine);
                _gridLines.Add(verticalLine);
            }

            for (double y = 0; y <= canvasHeight; y += _gridSize)
            {
                var horizontalLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = canvasWidth,
                    Y2 = y,
                    Stroke = gridBrush,
                    StrokeThickness = 0.5,
                    IsHitTestVisible = false
                };
                _designCanvas.Children.Insert(0, horizontalLine);
                _gridLines.Add(horizontalLine);
            }
        }

        public void ClearGrid()
        {
            foreach (var line in _gridLines)
            {
                if (_designCanvas.Children.Contains(line))
                {
                    _designCanvas.Children.Remove(line);
                }
            }
            _gridLines.Clear();
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

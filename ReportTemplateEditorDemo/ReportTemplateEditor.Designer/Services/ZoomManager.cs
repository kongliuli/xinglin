using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ReportTemplateEditor.Designer.Services
{
    public class ZoomManager
    {
        private Canvas _designCanvas;
        private Slider _zoomSlider;
        private TextBlock _zoomText;
        private ScrollViewer _scrollViewer;
        private double _currentZoom;
        private TransformGroup _transformGroup;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;

        public double CurrentZoom => _currentZoom;
        public TransformGroup TransformGroup => _transformGroup;

        public event Action<double> ZoomChanged;

        public ZoomManager(Canvas designCanvas, Slider zoomSlider, TextBlock zoomText, ScrollViewer scrollViewer = null)
        {
            _designCanvas = designCanvas ?? throw new ArgumentNullException(nameof(designCanvas));
            _zoomSlider = zoomSlider ?? throw new ArgumentNullException(nameof(zoomSlider));
            _zoomText = zoomText ?? throw new ArgumentNullException(nameof(zoomText));
            _scrollViewer = scrollViewer;

            _transformGroup = new TransformGroup();
            _scaleTransform = new ScaleTransform(1.0, 1.0);
            _translateTransform = new TranslateTransform(0, 0);
            
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_translateTransform);
            
            _designCanvas.RenderTransform = _transformGroup;
            _currentZoom = 1.0;

            _zoomSlider.ValueChanged += OnZoomSliderValueChanged;
        }

        public void SetZoom(double zoomLevel)
        {
            if (zoomLevel < 0.1)
            {
                zoomLevel = 0.1;
            }
            else if (zoomLevel > 5.0)
            {
                zoomLevel = 5.0;
            }

            _currentZoom = zoomLevel;
            ApplyZoom();
            UpdateZoomUI();
            ZoomChanged?.Invoke(_currentZoom);
        }

        public void ZoomIn()
        {
            SetZoom(Math.Min(_currentZoom + 0.1, 3.0));
        }

        public void ZoomOut()
        {
            SetZoom(Math.Max(_currentZoom - 0.1, 0.1));
        }

        public void ZoomTo50()
        {
            SetZoom(0.5);
        }

        public void ZoomTo75()
        {
            SetZoom(0.75);
        }

        public void ZoomTo100()
        {
            SetZoom(1.0);
        }

        public void ZoomTo150()
        {
            SetZoom(1.5);
        }

        public void ZoomTo200()
        {
            SetZoom(2.0);
        }

        public void FitWidth()
        {
            if (_scrollViewer != null && _designCanvas.ActualWidth > 0)
            {
                double containerWidth = _scrollViewer.ViewportWidth - 40;
                double canvasWidth = _designCanvas.ActualWidth;
                if (canvasWidth > 0)
                {
                    double zoom = containerWidth / canvasWidth;
                    SetZoom(zoom);
                }
            }
        }

        public void FitPage()
        {
            if (_scrollViewer != null && _designCanvas.ActualWidth > 0 && _designCanvas.ActualHeight > 0)
            {
                double containerWidth = _scrollViewer.ViewportWidth - 40;
                double containerHeight = _scrollViewer.ViewportHeight - 100;
                double canvasWidth = _designCanvas.ActualWidth;
                double canvasHeight = _designCanvas.ActualHeight;
                
                if (canvasWidth > 0 && canvasHeight > 0)
                {
                    double zoom = Math.Min(containerWidth / canvasWidth, containerHeight / canvasHeight);
                    SetZoom(zoom);
                }
            }
        }

        public void ResetZoom()
        {
            SetZoom(1.0);
            ResetPan();
        }

        public void Pan(double deltaX, double deltaY)
        {
            _translateTransform.X += deltaX;
            _translateTransform.Y += deltaY;
        }

        public void ResetPan()
        {
            _translateTransform.X = 0;
            _translateTransform.Y = 0;
        }

        private void ApplyZoom()
        {
            _scaleTransform.ScaleX = _currentZoom;
            _scaleTransform.ScaleY = _currentZoom;
        }

        private void UpdateZoomUI()
        {
            _zoomSlider.Value = _currentZoom;
            _zoomText.Text = $"{Math.Round(_currentZoom * 100)}%";
        }

        private void OnZoomSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetZoom(e.NewValue);
        }
    }
}

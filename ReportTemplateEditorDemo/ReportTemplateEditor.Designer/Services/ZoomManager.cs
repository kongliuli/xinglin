using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ReportTemplateEditor.Designer.Services
{
    public class ZoomManager
    {
        private Canvas _designCanvas;
        private ScaleTransform _scaleTransform;
        private Slider _zoomSlider;
        private TextBlock _zoomText;
        private double _currentZoom;

        public double CurrentZoom => _currentZoom;

        public event Action<double> ZoomChanged;

        public ZoomManager(Canvas designCanvas, Slider zoomSlider, TextBlock zoomText)
        {
            _designCanvas = designCanvas ?? throw new ArgumentNullException(nameof(designCanvas));
            _zoomSlider = zoomSlider ?? throw new ArgumentNullException(nameof(zoomSlider));
            _zoomText = zoomText ?? throw new ArgumentNullException(nameof(zoomText));

            _scaleTransform = new ScaleTransform(1.0, 1.0);
            _designCanvas.LayoutTransform = _scaleTransform;
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

        public void ResetZoom()
        {
            SetZoom(1.0);
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

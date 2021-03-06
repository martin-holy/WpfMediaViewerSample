﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfMediaViewerSample {
  public sealed class ZoomImageBox : Border {
    private Point _origin;
    private Point _start;
    private bool _isDecoded;
    private readonly ScaleTransform _scaleTransform;
    private readonly TranslateTransform _translateTransform;
    private string _filePath;

    public ZoomImageBox() {
      _isDecoded = true;

      _scaleTransform = new ScaleTransform();
      _translateTransform = new TranslateTransform();
      var group = new TransformGroup();
      group.Children.Add(_scaleTransform);
      group.Children.Add(_translateTransform);
      Image = new Image {
        RenderTransform = group,
        RenderTransformOrigin = new Point(0, 0)
      };
      Child = Image;

      MouseMove += OnMouseMove;
      MouseWheel += OnMouseWheel;
      MouseLeftButtonUp += OnMouseLeftButtonUp;
      MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    public Image Image;
    public string FilePath { get => _filePath; set { _filePath = value; SetSource(); }}

    public void SetSource() {
      Reset();
      var src = new BitmapImage();
      src.BeginInit();
      src.UriSource = new Uri(FilePath);
      src.CacheOption = BitmapCacheOption.OnLoad;
      if (_isDecoded)
        src.DecodePixelWidth = 1920;
      src.EndInit();
      Image.Source = src;
      GC.Collect();
    }

    private void Reset() {
      // reset zoom
      _scaleTransform.ScaleX = 1.0;
      _scaleTransform.ScaleY = 1.0;
      // reset pan
      _translateTransform.X = 0.0;
      _translateTransform.Y = 0.0;
    }

    private void OnMouseMove(object o, MouseEventArgs e) {
      if (!Image.IsMouseCaptured) return;
      var v = _start - e.GetPosition(this);
      _translateTransform.X = _origin.X - v.X;
      _translateTransform.Y = _origin.Y - v.Y;
    }

    private void OnMouseWheel(object o, MouseWheelEventArgs e) {
      if (_isDecoded) {
        _isDecoded = false;
        SetSource();
      }

      var zoom = e.Delta > 0 ? .2 : -.2;
      if (!(e.Delta > 0) && (_scaleTransform.ScaleX < .4 || _scaleTransform.ScaleY < .4))
        return;

      var relative = e.GetPosition(Image);
      var abosuluteX = relative.X * _scaleTransform.ScaleX + _translateTransform.X;
      var abosuluteY = relative.Y * _scaleTransform.ScaleY + _translateTransform.Y;

      _scaleTransform.ScaleX += zoom;
      _scaleTransform.ScaleY += zoom;

      _translateTransform.X = abosuluteX - relative.X * _scaleTransform.ScaleX;
      _translateTransform.Y = abosuluteY - relative.Y * _scaleTransform.ScaleY;
    }

    private void OnMouseLeftButtonUp(object o, MouseButtonEventArgs e) {
      Image.ReleaseMouseCapture();
      Cursor = Cursors.Arrow;
    }

    private void OnMouseLeftButtonDown(object o, MouseButtonEventArgs e) {
      _start = e.GetPosition(this);
      _origin = new Point(_translateTransform.X, _translateTransform.Y);
      Cursor = Cursors.Hand;
      Image.CaptureMouse();
    }
  }
}

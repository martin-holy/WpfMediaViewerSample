using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace WpfMediaViewerSample {
  public partial class MainWindow: INotifyPropertyChanged {
    private int _mediaItemIndex;
    private int _mediaItemsCount;
    private BaseMediaItem _currentMediaItem;

    public MainWindow() {
      InitializeComponent();

      DataContext = this;
      MediaItems = new List<BaseMediaItem>();
      SplitedMediaItems = new List<List<BaseMediaItem>>();

      MoveForward.InputGestures.Add(new KeyGesture(Key.Right));
      MoveBack.InputGestures.Add(new KeyGesture(Key.Left));
    }

    public static RoutedCommand MoveForward = new RoutedCommand();
    public static RoutedCommand MoveBack = new RoutedCommand();
    public List<BaseMediaItem> MediaItems { get; set; }
    public List<List<BaseMediaItem>> SplitedMediaItems { get; set; }
    public int MediaItemIndex {
      get => _mediaItemIndex;
      set {
        _mediaItemIndex = value;
        CurrentMediaItem = _mediaItemIndex == -1 ? null : MediaItems[_mediaItemIndex];
      }
    }

    public BaseMediaItem CurrentMediaItem {
      get => _currentMediaItem;
      set {
        _currentMediaItem = value;
        
        switch (_currentMediaItem?.MediaType) {
          case MediaTypes.Image: {
            FullImage.FilePath = _currentMediaItem.FilePath;
            FullImage.Visibility = Visibility.Visible;
            FullMedia.Visibility = Visibility.Hidden;
            FullMedia.Source = null;
            break;
          }
          case MediaTypes.Video: {
            FullImage.Visibility = Visibility.Hidden;
            FullMedia.Visibility = Visibility.Visible;
            FullMedia.Source = _currentMediaItem.FilePathUri;
            break;
          }
        }

        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string name = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
      Thumbs.ItemsSource = null;
      LoadMediaItems();
      SplitMediaItems();
      Thumbs.ItemsSource = SplitedMediaItems;
      MediaItemIndex = _mediaItemsCount == 0 ? -1 : 0;
    }

    public void LoadMediaItems() {
      var sizes = new List<Point> {new Point(320, 180), new Point(320, 240), new Point(180, 320), new Point(240, 320)};
      MediaItems.Clear();
      var random = new Random();

      foreach (var filePath in Directory.EnumerateFiles(@"d:\Pictures\01 Digital_Foto\-=Hotovo\2016\", "*.*", SearchOption.AllDirectories)) {
        var size = sizes[random.Next(sizes.Count)];
        MediaItems.Add(new BaseMediaItem(filePath) {
          ThumbWidth = (int)size.X,
          ThumbHeight = (int)size.Y
        });
      }

      /*for (var i = 0; i < 5000; i++) {
        var size = sizes[random.Next(sizes.Count)];
        MediaItems.Add(new BaseMediaItem($"Item {i}") {
          ThumbWidth = (int)size.X,
          ThumbHeight = (int)size.Y
        });
      }*/
      
      _mediaItemsCount = MediaItems.Count;
    }

    public void SplitMediaItems() {
      foreach (var itemsGroup in SplitedMediaItems) {
        itemsGroup.Clear();
      }
      SplitedMediaItems.Clear();

      var groupMaxWidth = Thumbs.ActualWidth;
      var groupWidth = 0;
      const int itemOffset = 6; //border, margin, padding, ...
      var row = new List<BaseMediaItem>();
      foreach (var item in MediaItems) {
        if (item.ThumbWidth + itemOffset <= groupMaxWidth - groupWidth) {
          row.Add(item);
          groupWidth += item.ThumbWidth + itemOffset;
        }
        else {
          SplitedMediaItems.Add(row);
          row = new List<BaseMediaItem>();
          groupWidth = 0;
        }
      }
    }

    private void MoveForwardExecuted(object sender, ExecutedRoutedEventArgs e) {
      if (_mediaItemIndex >= _mediaItemsCount) return;
      MediaItemIndex++;
    }

    private void MoveBackExecuted(object sender, ExecutedRoutedEventArgs e) {
      if (_mediaItemIndex <= 0) return;
      MediaItemIndex--;
    }

    protected bool IsFullyOrPartiallyVisible(FrameworkElement child, FrameworkElement scrollViewer) {
      var childTransform = child.TransformToAncestor(scrollViewer);
      var childRectangle = childTransform.TransformBounds(
        new Rect(new Point(0, 0), child.RenderSize));
      var ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);
      return ownerRectangle.IntersectsWith(childRectangle);
    }
  }
}

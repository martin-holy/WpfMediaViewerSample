using System;
using System.Collections.Generic;

namespace WpfMediaViewerSample {
  public enum MediaTypes { Image = 0, Video = 1, Unknown = 2 }

  public class BaseMediaItem {
    public BaseMediaItem(string filePath) {
      BoxInfoItems = new List<string> { System.IO.Path.GetFileName(filePath) };

      FilePath = filePath;
      if (filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
        MediaType = MediaTypes.Image;
      else if (filePath.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
        MediaType = MediaTypes.Video;
      else
        MediaType = MediaTypes.Unknown;
    }
    
    public string FilePath;
    public Uri FilePathUri => new Uri(FilePath);
    public string FilePathCache => FilePath.Replace(":\\", @":\Temp\PictureManagerCache\");
    public Uri FilePathCacheUri => new Uri(FilePathCache);
    public Uri FakeSource => new Uri("FakeThumbSource.jpg", UriKind.Relative);
    public MediaTypes MediaType;
    public List<string> BoxInfoItems { get; set; }
    public int ThumbWidth { get; set; }
    public int ThumbHeight { get; set; }
  }
}
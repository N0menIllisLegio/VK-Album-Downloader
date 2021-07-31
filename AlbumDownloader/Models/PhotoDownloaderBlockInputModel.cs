using System.Collections.Concurrent;
using AlbumDownloader.Models.ApiSchema;

namespace AlbumDownloader.Models
{
  internal class PhotoDownloaderBlockInputModel
  {
    public PhotoModel Photo { get; set; }
    public string DownloadPathWithoutExtension { get; set; }
    public DownloadProgressModel DownloadProgress { get; set; }
    public ConcurrentBag<string> LogLines { get; set; }
  }
}

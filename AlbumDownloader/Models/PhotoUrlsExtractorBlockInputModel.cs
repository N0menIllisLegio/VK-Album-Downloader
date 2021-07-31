using System.Collections.Concurrent;
using AlbumDownloader.Models.ApiSchema;

namespace AlbumDownloader.Models
{
  internal class PhotoUrlsExtractorBlockInputModel
  {
    public AlbumModel Album { get; set; }
    public string DownloadDirectory { get; set; }
    public DownloadProgressModel DownloadProgress { get; set; }
    public ConcurrentBag<string> LogLines { get; set; }
  }
}

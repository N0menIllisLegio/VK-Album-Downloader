namespace AlbumDownloader.Models
{
  internal class DownloadedPhotoModel
  {
    public byte[] Bytes { get; set; }
    public string Path { get; set; }
    public DownloadProgressModel DownloadProgress { get; set; }
  }
}

using System;
using AlbumDownloader.Models.ApiSchema;

namespace AlbumDownloader
{
  internal class Settings
  {
    public const string MainWindowRegion = "ContentRegion";
    public const string MainPageRegion = "AlbumnPageContentRegion";
    public const int SendRequestCancellationTimeoutSeconds = 10;

    public static string WebViewUserDataFolder { get; set; }

    public int ImagesBatchSize { get; set; }
    public string AppID { get; set; }
    public string ApiVersion { get; set; }
    public string Token { get; set; }
    public DateTimeOffset TokenExpirationTime { get; set; }
    public ProfileInfoModel ProfileInfo { get; set; }
  }
}

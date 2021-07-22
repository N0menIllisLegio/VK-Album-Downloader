using System;

namespace AlbumDownloader
{
  internal static class Settings
  {
    public const string MainRegion = "ContentRegion";

    public static string AppID { get; set; }
    public static string ApiVersion { get; set; }
    public static string Token { get; set; }
    public static DateTimeOffset TokenExpirationTime { get; set; }
  }
}

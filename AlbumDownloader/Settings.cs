using System;

namespace AlbumDownloader
{
  internal class Settings
  {
    public const string MainRegion = "ContentRegion";

    public string AppID { get; set; }
    public string ApiVersion { get; set; }
    public string Token { get; set; }
    public DateTimeOffset TokenExpirationTime { get; set; }
    public int ClientID { get; set; }
  }
}

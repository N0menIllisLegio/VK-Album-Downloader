using System;
using AlbumDownloader.Models.ApiSchema;

namespace AlbumDownloader
{
  internal class Settings
  {
    public const string MainRegion = "ContentRegion";

    public string AppID { get; set; }
    public string ApiVersion { get; set; }
    public string Token { get; set; }
    public DateTimeOffset TokenExpirationTime { get; set; }
    public ProfileInfoModel ProfileInfo { get; set; }
  }
}

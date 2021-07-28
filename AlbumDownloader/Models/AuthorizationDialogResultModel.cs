using System;

namespace AlbumDownloader.Models
{
  internal class AuthorizationDialogResultModel
  {
    public bool Success { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
    public DateTimeOffset TokenExpirationTime { get; set; }
  }
}

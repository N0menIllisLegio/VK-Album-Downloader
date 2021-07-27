using System.Text.Json.Serialization;

namespace AlbumDownloader.Models.ApiSchema
{
  internal class ResponseWrapper<TResponse>
    where TResponse: class
  {
    [JsonPropertyName("response")]
    public TResponse Response { get; set; }
  }
}

using System.Text.Json.Serialization;

namespace AlbumDownloader.Models.ApiSchema
{
  internal class PhotoSizeModel
  {

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("type")]
    public PhotoSizeType Type { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
  }
}

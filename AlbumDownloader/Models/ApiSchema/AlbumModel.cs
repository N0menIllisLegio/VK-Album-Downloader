using System.Text.Json.Serialization;

namespace AlbumDownloader.Models.ApiSchema
{
  public class AlbumModel
  {
    [JsonPropertyName("id")]
    public int ID { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("thumb_src")]
    public string ThumbnailUrl { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }
  }
}

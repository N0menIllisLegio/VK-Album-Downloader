using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlbumDownloader.Models.ApiSchema
{
  internal class PhotoModel
  {

    [JsonPropertyName("id")]
    public long ID { get; set; }

    [JsonPropertyName("date")]
    public long Date { get; set; }

    [JsonPropertyName("sizes")]
    public List<PhotoSizeModel> Sizes { get; set; }
  }
}

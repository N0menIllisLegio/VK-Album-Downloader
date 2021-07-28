using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlbumDownloader.Models.ApiSchema
{
  internal class AlbumsListModel
  {
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("items")]
    public List<AlbumModel> Albums { get; set; }
  }
}

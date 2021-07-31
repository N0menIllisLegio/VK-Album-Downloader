using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlbumDownloader.Models.ApiSchema
{
  internal class PhotosListModel
  {

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("items")]
    public List<PhotoModel> Photos { get; set; }
  }
}

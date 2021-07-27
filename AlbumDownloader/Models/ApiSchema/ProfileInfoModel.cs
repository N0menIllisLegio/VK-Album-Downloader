using System.Text.Json.Serialization;

namespace AlbumDownloader.Models.ApiSchema
{
  internal class ProfileInfoModel
  {
    [JsonPropertyName("id")]
    public int ID { get; set; }
  }
}

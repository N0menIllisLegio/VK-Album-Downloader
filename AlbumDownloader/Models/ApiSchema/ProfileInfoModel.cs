using System.Text.Json.Serialization;

namespace AlbumDownloader.Models.ApiSchema
{
  internal class ProfileInfoModel
  {
    [JsonPropertyName("id")]
    public int ID { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
  }
}

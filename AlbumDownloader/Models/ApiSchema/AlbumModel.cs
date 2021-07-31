using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace AlbumDownloader.Models.ApiSchema
{
  public class AlbumModel : INotifyPropertyChanged
  {
    private bool _selected;

    [JsonPropertyName("id")]
    public int ID { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("thumb_src")]
    public string ThumbnailUrl { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    public bool Selected
    {
      get => _selected;
      set
      {
        if (_selected != value)
        {
          _selected = value;
          OnPropertyChanged(nameof(Selected));
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}

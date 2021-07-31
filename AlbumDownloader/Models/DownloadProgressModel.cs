using System.ComponentModel;
using System.Runtime.CompilerServices;
using AlbumDownloader.Models.ApiSchema;

namespace AlbumDownloader.Models
{
  internal class DownloadProgressModel : INotifyPropertyChanged
  {
    private int _completed;
    private int _total;
    private string _title;

    public DownloadProgressModel()
    { }

    public DownloadProgressModel(AlbumModel albumModel)
    {
      Title = albumModel.Title;
      Total = albumModel.Size;
    }

    public string Title
    {
      get => _title;
      set
      {
        if (_title != value)
        {
          _title = value;
          OnPropertyChanged(nameof(Title));
        }
      }
    }

    public int Total
    {
      get => _total;
      set
      {
        if (_total != value)
        {
          _total = value;
          OnPropertyChanged(nameof(Total));
          OnPropertyChanged(nameof(Percent));
        }
      }
    }

    public int Completed
    {
      get => _completed;
      set
      {
        if (_completed != value)
        {
          _completed = value;
          OnPropertyChanged(nameof(Completed));
          OnPropertyChanged(nameof(Percent));
        }
      }
    }

    public int Percent => (int)((double)Completed / Total * 100);

    public event PropertyChangedEventHandler PropertyChanged;

    public void IncrementCompleted()
    {
      Completed++;
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}

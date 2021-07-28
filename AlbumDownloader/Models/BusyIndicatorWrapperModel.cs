using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AlbumDownloader.Models
{
  internal class BusyIndicatorWrapperModel : INotifyPropertyChanged
  {
    private bool _busy;

    public bool Busy
    {
      get => _busy;
      set
      {
        if (_busy != value)
        {
          _busy = value;
          OnPropertyChanged(nameof(Busy));
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

using System.Collections.ObjectModel;
using AlbumDownloader.Models;
using Prism.Mvvm;

namespace AlbumDownloader.ViewModels
{
  internal class AlbumnsPageViewModel : BindableBase
  {
    public AlbumnsPageViewModel()
    {
      Albumns = new ObservableCollection<AlbumModel>();
    }

    public ObservableCollection<AlbumModel> Albumns { get; set; }
  }
}

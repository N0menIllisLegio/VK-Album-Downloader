using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AlbumDownloader.Models;
using AlbumDownloader.Models.ApiSchema;
using AlbumDownloader.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AlbumDownloader.ViewModels
{
  internal class AlbumsPageViewModel : BindableBase, INavigationAware
  {
    private readonly VKApiRequestProvider _vkApiRequestProvider;

    private DelegateCommand _downloadAlbums;
    private BusyIndicatorWrapperModel _busyIndicatorWrapper;
    private ObservableCollection<AlbumModel> _albums;

    public AlbumsPageViewModel(VKApiRequestProvider vkApiRequestProvider)
    {
      Albums = new ObservableCollection<AlbumModel>();

      _vkApiRequestProvider = vkApiRequestProvider ?? throw new ArgumentNullException(nameof(vkApiRequestProvider));
    }

    public ObservableCollection<AlbumModel> Albums
    {
      get => _albums;
      set => SetProperty(ref _albums, value);
    }

    public DelegateCommand DownloadAlbums =>
        _downloadAlbums ?? (_downloadAlbums = new DelegateCommand(ExecuteDownloadAlbums, CanExecuteDownloadAlbums));

    void ExecuteDownloadAlbums()
    {
      _busyIndicatorWrapper.Busy = !_busyIndicatorWrapper.Busy;
    }

    bool CanExecuteDownloadAlbums()
    {
      return true; // _busyIndicatorWrapper.Busy;
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      _busyIndicatorWrapper = navigationContext.Parameters
        .GetValue<BusyIndicatorWrapperModel>(nameof(BusyIndicatorWrapperModel))
          ?? throw new ArgumentException(nameof(_busyIndicatorWrapper));

      _ = RefreshAlbums();
    }

    private async Task RefreshAlbums()
    {
      var previousBusyValue = _busyIndicatorWrapper.Busy;
      _busyIndicatorWrapper.Busy = true;

      var albums = await _vkApiRequestProvider.GetAlbums();

      Albums = new ObservableCollection<AlbumModel>(albums);

      _busyIndicatorWrapper.Busy = previousBusyValue;
    }
  }
}

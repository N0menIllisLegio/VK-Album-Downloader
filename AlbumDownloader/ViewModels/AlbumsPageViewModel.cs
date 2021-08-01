using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AlbumDownloader.Models;
using AlbumDownloader.Models.ApiSchema;
using AlbumDownloader.Services;
using AlbumDownloader.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AlbumDownloader.ViewModels
{
  internal class AlbumsPageViewModel : BindableBase, INavigationAware
  {
    private readonly VKApiRequestProvider _vkApiRequestProvider;
    private readonly IRegionManager _regionManager;

    private DelegateCommand _downloadAlbums;
    private DelegateCommand _refreshAlbums;
    private BusyIndicatorWrapperModel _busyIndicatorWrapper;
    private TrulyObservableCollection<AlbumModel> _albums;

    public AlbumsPageViewModel(VKApiRequestProvider vkApiRequestProvider, IRegionManager regionManager)
    {
      Albums = new TrulyObservableCollection<AlbumModel>();

      _vkApiRequestProvider = vkApiRequestProvider ?? throw new ArgumentNullException(nameof(vkApiRequestProvider));
      _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
    }

    public TrulyObservableCollection<AlbumModel> Albums
    {
      get => _albums;
      set => SetProperty(ref _albums, value);
    }

    public DelegateCommand DownloadAlbumsCommand =>
      _downloadAlbums ??= new DelegateCommand(ExecuteDownloadAlbumsCommand, CanExecuteDownloadAlbumsCommand);

    public DelegateCommand RefreshAlbumsCommand =>
      _refreshAlbums ??= new DelegateCommand(ExecuteRefreshAlbumsCommand, CanExecuteRefreshAlbumsCommand);

    public string DownloadButtonCaption => $"Max amount of photos for download at once: {MaxDownloadPhotos}";

    public int MaxDownloadPhotos => 10000;

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    { }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      _busyIndicatorWrapper = navigationContext.Parameters
        .GetValue<BusyIndicatorWrapperModel>(nameof(BusyIndicatorWrapperModel))
          ?? throw new ArgumentException(nameof(_busyIndicatorWrapper));

      _busyIndicatorWrapper.PropertyChanged += BusyIndicatorWrapper_PropertyChanged;

      _ = RefreshAlbums();
    }

    private void ExecuteDownloadAlbumsCommand()
    {
      var selectedAlbums = Albums.Where(album => album.Selected).ToList();

      _regionManager.RequestNavigate(Settings.MainPageRegion, nameof(DownloadPage), new NavigationParameters
      {
        { nameof(BusyIndicatorWrapperModel), _busyIndicatorWrapper },
        { nameof(Albums), selectedAlbums }
      });
    }

    private bool CanExecuteDownloadAlbumsCommand() =>
      (!_busyIndicatorWrapper?.Busy ?? false)
        && Albums.Count(album => album.Selected) > 0
        && Albums.Where(album => album.Selected).Sum(album => album.Size) <= MaxDownloadPhotos;

    private void ExecuteRefreshAlbumsCommand()
    {
      _ = RefreshAlbums();
    }

    private bool CanExecuteRefreshAlbumsCommand() => !_busyIndicatorWrapper?.Busy ?? false;

    private async Task RefreshAlbums()
    {
      var previousBusyValue = _busyIndicatorWrapper.Busy;
      _busyIndicatorWrapper.Busy = true;

      if (Albums is not null)
      {
        Albums.CollectionChanged -= Albums_CollectionChanged;
      }

      var albums = await _vkApiRequestProvider.GetAlbums();

      _busyIndicatorWrapper.Busy = previousBusyValue;

      if (albums is not null)
      {
        Albums = new(albums);
        Albums.CollectionChanged += Albums_CollectionChanged;
      }
      else
      {
        _regionManager.RequestNavigate(Settings.MainWindowRegion, nameof(LoginPage));
      }
    }

    private void BusyIndicatorWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      RefreshAlbumsCommand.RaiseCanExecuteChanged();
      DownloadAlbumsCommand.RaiseCanExecuteChanged();
    }

    private void Albums_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      DownloadAlbumsCommand.RaiseCanExecuteChanged();
    }
  }
}

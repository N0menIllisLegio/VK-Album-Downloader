using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AlbumDownloader.Models;
using AlbumDownloader.Models.ApiSchema;
using AlbumDownloader.Services;
using AlbumDownloader.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AlbumDownloader.ViewModels
{
  internal class DownloadPageViewModel : BindableBase, INavigationAware
  {
    private readonly VKApiRequestProvider _vkApiRequestProvider;
    private readonly DialogService _dialogService;
    private readonly IRegionManager _regionManager;
    private readonly AlbumsDownloadService _imageDownloadService;

    private BusyIndicatorWrapperModel _busyIndicatorWrapper;
    private List<AlbumModel> _downloadingAlbums;
    private DownloadProgressModel _totalPhotosProgress;
    private DelegateCommand _cancelCommand;
    private CancellationTokenSource _downloadCancellationTokenSource;

    public DownloadPageViewModel(VKApiRequestProvider vkApiRequestProvider, DialogService dialogService, IRegionManager regionManager,
      AlbumsDownloadService imageDownloadService)
    {
      _vkApiRequestProvider = vkApiRequestProvider ?? throw new ArgumentNullException(nameof(vkApiRequestProvider));
      _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
      _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
      _imageDownloadService = imageDownloadService ?? throw new ArgumentNullException(nameof(imageDownloadService));
    }

    public DownloadProgressModel TotalPhotosProgress
    {
      get => _totalPhotosProgress;
      set => SetProperty(ref _totalPhotosProgress, value);
    }

    public DelegateCommand CancelCommand => _cancelCommand ??= new DelegateCommand(ExecuteCancelCommand);

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

      _downloadingAlbums = navigationContext.Parameters
        .GetValue<List<AlbumModel>>(nameof(AlbumsPageViewModel.Albums))
          ?? throw new ArgumentException(nameof(AlbumsPageViewModel.Albums));

      TotalPhotosProgress = new DownloadProgressModel
      {
        Title = AppResources.TotalPhotosDownloadProgressString,
        Total = _downloadingAlbums.Sum(album => album.Size)
      };

      DownloadAlbums();
    }

    private void ExecuteCancelCommand()
    {
      _downloadCancellationTokenSource.Cancel();
    }

    private async void DownloadAlbums()
    {
      var previousBusy = _busyIndicatorWrapper.Busy;
      _busyIndicatorWrapper.Busy = true;

      _downloadCancellationTokenSource = new ();

      var downloadResult = await _imageDownloadService.DownloadAlbums(_downloadingAlbums, _downloadCancellationTokenSource.Token, TotalPhotosProgress);

      if (downloadResult.Success)
      {
        await _dialogService.ShowOkDialog(AppResources.SuccessTitleString, downloadResult.Result);
      }
      else
      {
        await _dialogService.ShowOkDialog(AppResources.ErrorTitleString, downloadResult.Message);
      }

      _busyIndicatorWrapper.Busy = previousBusy;

      _regionManager.RequestNavigate(Settings.MainPageRegion, nameof(AlbumsPage));
    }
  }
}

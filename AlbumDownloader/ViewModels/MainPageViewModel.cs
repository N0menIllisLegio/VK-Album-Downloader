using System.ComponentModel;
using AlbumDownloader.Models;
using AlbumDownloader.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AlbumDownloader.ViewModels
{
  internal class MainPageViewModel : BindableBase, INavigationAware
  {
    private readonly Settings _settings;
    private readonly IRegionManager _regionManager;

    private string _fullName;
    private DelegateCommand _logoutCommand;

    public MainPageViewModel(Settings settings, IRegionManager regionManager)
    {
      _settings = settings ?? throw new System.ArgumentNullException(nameof(settings));
      _regionManager = regionManager ?? throw new System.ArgumentNullException(nameof(regionManager));

      _logoutCommand = new DelegateCommand(ExecuteLogoutCommand, CanExecuteLogoutCommand);

      BusyIndicatorWrapper = new BusyIndicatorWrapperModel();
      BusyIndicatorWrapper.PropertyChanged += BusyIndicatorWrapper_PropertyChanged;

      FullName = $"{_settings.ProfileInfo.FirstName} {_settings.ProfileInfo.LastName}";
    }

    public string FullName
    {
      get => _fullName;
      set => SetProperty(ref _fullName, value);
    }

    public DelegateCommand LogoutCommand => _logoutCommand;

    public BusyIndicatorWrapperModel BusyIndicatorWrapper { get; }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      _regionManager.RequestNavigate(Settings.MainPageRegion, nameof(AlbumsPage), new NavigationParameters
      {
        { nameof(BusyIndicatorWrapperModel), BusyIndicatorWrapper }
      });
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    { }

    private void BusyIndicatorWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      LogoutCommand.RaiseCanExecuteChanged();
    }

    private void ExecuteLogoutCommand()
    {
      _regionManager.RequestNavigate(Settings.MainWindowRegion, nameof(LoginPage));
    }

    private bool CanExecuteLogoutCommand()
    {
      return !BusyIndicatorWrapper.Busy;
    }
  }
}

using System;
using AlbumDownloader.Services;
using AlbumDownloader.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace AlbumDownloader.ViewModels
{
  internal class LoginPageViewModel : BindableBase
  {
    private readonly IDialogService _dialogService;
    private readonly IRegionManager _regionManager;
    private readonly Settings _settings;
    private readonly VKApiRequestProvider _vkApiRequestProvider;
    private DelegateCommand _loginCommand;
    private string _message;

    public LoginPageViewModel(IDialogService dialogService, IRegionManager regionManager, Settings settings,
      VKApiRequestProvider vkApiRequestProvider)
    {
      _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
      _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
      _settings = settings ?? throw new ArgumentNullException(nameof(settings));
      _vkApiRequestProvider = vkApiRequestProvider ?? throw new ArgumentNullException(nameof(vkApiRequestProvider));

      LoginCommand = new DelegateCommand(Login);
    }

    public DelegateCommand LoginCommand
    {
      get => _loginCommand;
      set => SetProperty(ref _loginCommand, value);
    }

    public string Message
    {
      get => _message;
      set => SetProperty(ref _message, value);
    }

    private void Login()
    {
      Message = string.Empty;

      var parameters = new DialogParameters
      {
        { nameof(Settings.AppID), _settings.AppID },
        { nameof(Settings.ApiVersion), _settings.ApiVersion }
      };

      _dialogService.ShowDialog(nameof(VKAuthDialog), parameters, async (result) =>
      {
        if (result.Result == ButtonResult.Abort)
        {
          Message = result.Parameters.GetValue<string>("Error");
        }
        else
        {
          _settings.Token = result.Parameters.GetValue<string>("Token");
          _settings.TokenExpirationTime = result.Parameters.GetValue<DateTimeOffset>("TokenExpirationTime");
          _settings.ClientID = await _vkApiRequestProvider.GetProfileID();

          _regionManager.RequestNavigate(Settings.MainRegion, nameof(AlbumnsPage));
        }
      });
    }
  }
}

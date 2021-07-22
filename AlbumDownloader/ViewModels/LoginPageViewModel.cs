using System;
using AlbumDownloader.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace AlbumDownloader.ViewModels
{
  public class LoginPageViewModel : BindableBase
  {
    private readonly IDialogService _dialogService;
    private readonly IRegionManager _regionManager;
    private DelegateCommand _loginCommand;
    private string _message;

    public LoginPageViewModel(IDialogService dialogService, IRegionManager regionManager)
    {
      _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
      _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));

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

      _dialogService.ShowDialog(nameof(VKAuthDialog), null, (result) =>
      {
        if (result.Result == ButtonResult.Abort)
        {
          Message = result.Parameters.GetValue<string>("Error");
        }
        else
        {
          Settings.Token = result.Parameters.GetValue<string>("Token");
          Settings.TokenExpirationTime = result.Parameters.GetValue<DateTimeOffset>("TokenExpirationTime");

          _regionManager.RequestNavigate(Settings.MainRegion, nameof(AlbumnsPage));
        }
      });
    }
  }
}

using System;
using AlbumDownloader.Services;
using AlbumDownloader.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AlbumDownloader.ViewModels
{
  internal class LoginPageViewModel : BindableBase
  {
    private readonly IRegionManager _regionManager;
    private readonly AccountService _accountService;
    private DelegateCommand _loginCommand;
    private string _message;

    public LoginPageViewModel(IRegionManager regionManager, AccountService accountService)
    {
      _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
      _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
      
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

    private async void Login()
    {
      Message = string.Empty;

      var authorizationResult = await _accountService.Authorize();

      if (authorizationResult.Success)
      {
        _regionManager.RequestNavigate(Settings.MainRegion, nameof(MainPage));
      }
      else
      {
        Message = authorizationResult.Message;
      }
    }
  }
}

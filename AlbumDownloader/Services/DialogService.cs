using System;
using System.Threading.Tasks;
using AlbumDownloader.Models;
using AlbumDownloader.ViewModels;
using AlbumDownloader.Views;
using Prism.Services.Dialogs;

namespace AlbumDownloader.Services
{
  internal class DialogService
  {
    private readonly IDialogService _dialogService;
    private readonly Settings _settings;

    public DialogService(IDialogService dialogService, Settings settings)
    {
      _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
      _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public Task<AuthorizationDialogResultModel> ShowAuthorizationDialog()
    {
      var authorizationDialogClosed = new TaskCompletionSource<AuthorizationDialogResultModel>();

      var parameters = new DialogParameters
      {
        { nameof(Settings.AppID), _settings.AppID },
        { nameof(Settings.ApiVersion), _settings.ApiVersion }
      };

      _dialogService.ShowDialog(nameof(VKAuthDialog), parameters, result =>
      {
        if (result.Result == ButtonResult.Abort)
        {
          authorizationDialogClosed.SetResult(new AuthorizationDialogResultModel
          {
            Success = false,
            Message = result.Parameters.GetValue<string>("Error")
          });
        }
        else
        {
          authorizationDialogClosed.SetResult(new AuthorizationDialogResultModel
          {
            Success = true,
            Token = result.Parameters.GetValue<string>("Token"),
            TokenExpirationTime = result.Parameters.GetValue<DateTimeOffset>("TokenExpirationTime")
          });
        }
      });

      return authorizationDialogClosed.Task;
    }

    public Task ShowOkDialog(string title, string message)
    {
      if (String.IsNullOrEmpty(title))
      {
        throw new ArgumentException(nameof(title));
      }

      if (String.IsNullOrEmpty(message))
      {
        throw new ArgumentException(nameof(message));
      }

      var okDialogClosed = new TaskCompletionSource<bool>();

      var parameters = new DialogParameters
      {
        { nameof(OKDialogViewModel.Title), title },
        { nameof(OKDialogViewModel.Message), message }
      };

      _dialogService.ShowDialog(nameof(OKDialog), parameters, result =>
      {
        okDialogClosed.SetResult(true);
      });

      return okDialogClosed.Task;
    }
  }
}

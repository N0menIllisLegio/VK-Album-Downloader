using System.Threading.Tasks;
using AlbumDownloader.Models;

namespace AlbumDownloader.Services
{
  internal class AccountService
  {
    private readonly DialogService _dialogService;
    private readonly Settings _settings;
    private readonly VKApiRequestProvider _vkApiRequestProvider;

    public AccountService(DialogService dialogService, Settings settings, VKApiRequestProvider vkApiRequestProvider)
    {
      _dialogService = dialogService ?? throw new System.ArgumentNullException(nameof(dialogService));
      _settings = settings ?? throw new System.ArgumentNullException(nameof(settings));
      _vkApiRequestProvider = vkApiRequestProvider ?? throw new System.ArgumentNullException(nameof(vkApiRequestProvider));
    }

    public async Task<ServiceOperationResultModel<string>> Authorize()
    {
      var dialogResult = await _dialogService.ShowAuthorizationDialog();

      if (dialogResult.Success)
      {
        _settings.Token = dialogResult.Token;
        _settings.TokenExpirationTime = dialogResult.TokenExpirationTime;
        _settings.ProfileInfo = await _vkApiRequestProvider.GetProfileInfo();

        if (_settings.ProfileInfo is null)
        {
          return ServiceOperationResultModel<string>.Failure(AppResources.ReceivingProfileInfoFailedErrorString);
        }

        return ServiceOperationResultModel<string>.CompletedSuccessfully(null);
      }

      return ServiceOperationResultModel<string>.Failure(dialogResult.Message);
    }
  }
}

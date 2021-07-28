using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using AlbumDownloader.Models.ApiSchema;

namespace AlbumDownloader.Services
{
  internal class VKApiRequestProvider: IDisposable
  {
    private readonly HttpClient _httpClient;
    private readonly Settings _settings;
    private readonly DialogService _dialogService;
    private bool _disposed = false;

    public VKApiRequestProvider(Settings settings, DialogService dialogService)
    {
      _settings = settings ?? throw new ArgumentNullException(nameof(settings));
      _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

      _httpClient = new HttpClient
      {
        BaseAddress = new Uri("https://api.vk.com/method/")
      };
    }

    ~VKApiRequestProvider()
    {
      Dispose(disposing: false);
    }

    public async Task<ProfileInfoModel> GetProfileInfo()
    {
      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = BuildRelativeUri("account.getProfileInfo")
      };

      var profileInfo = await SendRequest<ProfileInfoModel>(request);

      return profileInfo;
    }

    public void Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          _httpClient.Dispose();
        }

        _disposed = true;
      }
    }

    private async Task<TResult> SendRequest<TResult>(HttpRequestMessage httpRequest)
      where TResult: class
    {
      if (_settings.TokenExpirationTime.AddSeconds(-10) <= DateTimeOffset.Now)
      {
        await _dialogService.ShowOkDialog(AppResources.ErrorTitleString, AppResources.TokenExpiredErrorString);

        return null;
      }

      var response = await _httpClient.SendAsync(httpRequest);

      try
      {
        response.EnsureSuccessStatusCode();
      }
      catch (HttpRequestException exception)
      {
        await _dialogService.ShowOkDialog(AppResources.ErrorTitleString,
          String.Format(AppResources.RequestFailedErrorString, exception.Message));

        return null;
      }

      string content = await response.Content.ReadAsStringAsync();

      try
      {
        return JsonSerializer.Deserialize<ResponseWrapper<TResult>>(content).Response;
      }
      catch (JsonException)
      {
        await _dialogService.ShowOkDialog(AppResources.ErrorTitleString,
          AppResources.ResponseDeserializationFailedErrorString);

        return null;
      }
    }

    private Uri BuildRelativeUri(string methodName, Dictionary<string, string> queryParameters = null)
    {
      if (queryParameters is null)
      {
        queryParameters = new Dictionary<string, string>();
      }

      queryParameters.Add("access_token", _settings.Token);
      queryParameters.Add("v", _settings.ApiVersion);

      string uri = QueryHelpers.AddQueryString(methodName, queryParameters);

      return new Uri(uri, UriKind.Relative);
    }
  }
}

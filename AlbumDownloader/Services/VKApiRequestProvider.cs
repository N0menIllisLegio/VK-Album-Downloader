using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using AlbumDownloader.Models.ApiSchema;
using System.Threading;
using System.Text.Json.Serialization;

namespace AlbumDownloader.Services
{
  // TODO handle null results eg errors
  internal class VKApiRequestProvider: IDisposable
  {
    private readonly HttpClient _httpClient;
    private readonly Settings _settings;
    private readonly DialogService _dialogService;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private bool _disposed = false;

    public VKApiRequestProvider(Settings settings, DialogService dialogService)
    {
      _settings = settings ?? throw new ArgumentNullException(nameof(settings));
      _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

      _jsonSerializerOptions = new JsonSerializerOptions();
      _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

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

    public async Task<List<AlbumModel>> GetAlbums()
    {
      var requestUri = BuildRelativeUri("photos.getAlbums", new Dictionary<string, string>
      {
        { "owner_id", _settings.ProfileInfo.ID.ToString() },
        { "need_system", "1" },
        { "need_covers", "1" }
      });

      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = requestUri
      };

      var albumsWrapper = await SendRequest<AlbumsListModel>(request);

      return albumsWrapper?.Albums;
    }

    public async Task<List<PhotoModel>> GetAlbumPhotosBatch(int albumID, int offset)
    {
      var requestUri = BuildRelativeUri("photos.get", new Dictionary<string, string>
      {
        { "owner_id", _settings.ProfileInfo.ID.ToString() },
        { "album_id", albumID.ToString() },
        { "offset", offset.ToString() },
        { "count", _settings.ImagesBatchSize.ToString() },
        { "photo_sizes", "1" }
      });

      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = requestUri
      };

      var result = await SendRequest<PhotosListModel>(request, silent: true);

      return result?.Photos;
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

    private async Task<TResult> SendRequest<TResult>(HttpRequestMessage httpRequest, bool silent = false)
      where TResult: class
    {
      if (_settings.TokenExpirationTime.AddSeconds(-10) <= DateTimeOffset.Now)
      {
        await _dialogService.ShowOkDialog(AppResources.ErrorTitleString, AppResources.TokenExpiredErrorString);

        return null;
      }

      var cancellationTokenSource = new CancellationTokenSource(Settings.SendRequestCancellationTimeoutSeconds * 1000);
      HttpResponseMessage response;

      try
      {
        response = await _httpClient.SendAsync(httpRequest, cancellationTokenSource.Token);
      }
      catch(OperationCanceledException)
      {
        if (!silent)
        {
          await _dialogService.ShowOkDialog(AppResources.ErrorTitleString,
            String.Format(AppResources.RequestCanceledErrorString, Settings.SendRequestCancellationTimeoutSeconds));
        }

        return null;
      }

      try
      {
        response.EnsureSuccessStatusCode();
      }
      catch (HttpRequestException exception)
      {
        if (!silent)
        {
          await _dialogService.ShowOkDialog(AppResources.ErrorTitleString,
            String.Format(AppResources.RequestFailedErrorString, exception.Message));
        }

        return null;
      }

      string content = await response.Content.ReadAsStringAsync();

      try
      {
        return JsonSerializer.Deserialize<ResponseWrapper<TResult>>(content, _jsonSerializerOptions).Response;
      }
      catch (JsonException)
      {
        if (!silent)
        {
          await _dialogService.ShowOkDialog(AppResources.ErrorTitleString,
            AppResources.ResponseDeserializationFailedErrorString);
        }

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

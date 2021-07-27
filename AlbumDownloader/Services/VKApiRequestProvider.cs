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
    private bool _disposed = false;

    public VKApiRequestProvider(Settings settings)
    {
      _settings = settings ?? throw new ArgumentNullException(nameof(settings));

      _httpClient = new HttpClient
      {
        BaseAddress = new Uri("https://api.vk.com/method/")
      };
    }

    ~VKApiRequestProvider()
    {
      Dispose(disposing: false);
    }

    public async Task<int> GetProfileID()
    {
      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = BuildRelativeUri("account.getProfileInfo")
      };

      var result = await SendRequest<ProfileInfoModel>(request);

      return result.ID;
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
      var response = await _httpClient.SendAsync(httpRequest);

      response.EnsureSuccessStatusCode();

      string content = await response.Content.ReadAsStringAsync();

      return JsonSerializer.Deserialize<ResponseWrapper<TResult>>(content).Response;
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

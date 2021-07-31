using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AlbumDownloader.Models;
using AlbumDownloader.Models.ApiSchema;

namespace AlbumDownloader.Services
{
  internal class AlbumsDownloadService: IDisposable
  {
    private readonly Regex _extensionRegex = new (@"\.(\w+)\?", RegexOptions.Compiled);
    private readonly object locker = new object();
    private readonly VKApiRequestProvider _vkApiRequestProvider;
    private readonly HttpClient _httpClient;

    private bool _disposed = false;

    public AlbumsDownloadService(VKApiRequestProvider vkApiRequestProvider)
    {
      _vkApiRequestProvider = vkApiRequestProvider ?? throw new ArgumentNullException(nameof(vkApiRequestProvider));

      _httpClient = new HttpClient();
    }

    public async Task<ServiceOperationResultModel<string>> DownloadAlbums(List<AlbumModel> albums,
      CancellationToken cancellationToken, DownloadProgressModel photosProgress = null)
    {
      string downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        $"VK_Albums_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}");

      Directory.CreateDirectory(downloadDirectory);

      var blockOptions = new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken };
      var photoUrlsExtractorBlock = new TransformManyBlock<PhotoUrlsExtractorBlockInputModel, PhotoDownloaderBlockInputModel>(ExtractPhotoUrls, blockOptions);
      var photoDownloaderBlock = new TransformBlock<PhotoDownloaderBlockInputModel, DownloadedPhotoModel>(DownloadPhoto, blockOptions);
      var saveDownloadedPhotoBlock = new ActionBlock<DownloadedPhotoModel>(SaveDownloadedPhoto, blockOptions);

      var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
      photoUrlsExtractorBlock.LinkTo(photoDownloaderBlock, linkOptions);
      photoDownloaderBlock.LinkTo(saveDownloadedPhotoBlock, linkOptions);

      var logLines = new ConcurrentBag<string>();

      foreach (var album in albums)
      {
        photoUrlsExtractorBlock.Post(new PhotoUrlsExtractorBlockInputModel
        {
          Album = album,
          DownloadProgress = photosProgress,
          DownloadDirectory = downloadDirectory,
          LogLines = logLines
        });
      }

      photoUrlsExtractorBlock.Complete();

      var result = ServiceOperationResultModel<string>.CompletedSuccessfully(null);

      try
      {
        await saveDownloadedPhotoBlock.Completion;
      }
      catch (Exception exception)
      {
        logLines.Add(exception.Message);
        logLines.Add(exception.ToString());

        result = ServiceOperationResultModel<string>.Failure(exception.Message);
      }

      if (logLines.Count > 0)
      {
        await File.AppendAllLinesAsync(Path.Combine(downloadDirectory, "log.txt"), logLines);
      }

      return result;
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

    private async Task<IEnumerable<PhotoDownloaderBlockInputModel>> ExtractPhotoUrls(PhotoUrlsExtractorBlockInputModel input)
    {
      var albumPhotos = new List<PhotoModel>();
      int extractedPhotosCount = 0;

      while (extractedPhotosCount < input.Album.Size)
      {
        var extractedPhotos = await _vkApiRequestProvider.GetAlbumPhotosBatch(input.Album.ID, extractedPhotosCount);

        if (extractedPhotos is null)
        {
          input.LogLines.Add($"Album {input.Album.ID} received no photos for {extractedPhotosCount} offset");

          break;
        }

        albumPhotos.AddRange(extractedPhotos);
        extractedPhotosCount += extractedPhotos.Count;
      }

      int photoIncrementor = 1;
      string albumPhotosDirectory = Path.Combine(input.DownloadDirectory, $"{input.Album.Title}_{input.Album.ID}");
      Directory.CreateDirectory(albumPhotosDirectory);

      return new List<PhotoDownloaderBlockInputModel>(albumPhotos.Select(albumPhoto => new PhotoDownloaderBlockInputModel
      {
        Photo = albumPhoto,
        DownloadPathWithoutExtension = Path.Combine(albumPhotosDirectory, $"Photo_{photoIncrementor++}"),
        DownloadProgress = input.DownloadProgress,
        LogLines = input.LogLines
      }));
    }

    private async Task<DownloadedPhotoModel> DownloadPhoto(PhotoDownloaderBlockInputModel input)
    {
      var bestPhotoSize = input.Photo.Sizes.OrderByDescending(size => size.Type).First();

      HttpResponseMessage response;

      try
      {
        var cancellationTokenSource = new CancellationTokenSource(2000);
        response = await _httpClient.GetAsync(bestPhotoSize.Url, cancellationTokenSource.Token);
      }
      catch (Exception)
      {
        input.LogLines.Add($"Failed to successfully execute GET request for photo {input.Photo.ID} from {bestPhotoSize.Url}");

        return null;
      }

      if (!response.IsSuccessStatusCode)
      {
        input.LogLines.Add($"Failed to successfully execute GET request for photo {input.Photo.ID} from {bestPhotoSize.Url}");

        return null;
      }

      var match = _extensionRegex.Match(bestPhotoSize.Url);

      if (match.Success)
      {
        return new DownloadedPhotoModel
        {
          Path = $"{input.DownloadPathWithoutExtension}.{match.Groups[1].Value}",
          Bytes = await response.Content.ReadAsByteArrayAsync(),
          DownloadProgress = input.DownloadProgress
        };
      }

      input.LogLines.Add($"Failed to extract {input.Photo.ID} photo's extension. {bestPhotoSize.Url}");

      return null;
    }

    private async Task SaveDownloadedPhoto(DownloadedPhotoModel downloadedPhoto)
    {
      if (downloadedPhoto is not null)
      {
        await File.WriteAllBytesAsync(downloadedPhoto.Path, downloadedPhoto.Bytes);

        lock (locker)
        {
          downloadedPhoto.DownloadProgress?.IncrementCompleted();
        }
      }
    }
  }
}

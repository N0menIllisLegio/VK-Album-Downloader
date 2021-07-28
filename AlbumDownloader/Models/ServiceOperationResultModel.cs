namespace AlbumDownloader.Models
{
  internal class ServiceOperationResultModel<TResult>
    where TResult : class
  {
    private ServiceOperationResultModel()
    { }

    public bool Success { get; private set; }
    public string Message { get; private set; }
    public TResult Result { get; private set; }

    public static ServiceOperationResultModel<TResult> Failure(string message)
    {
      return new ServiceOperationResultModel<TResult>
      {
        Success = false,
        Message = message,
        Result = null
      };
    }

    public static ServiceOperationResultModel<TResult> CompletedSuccessfully(TResult result)
    {
      return new ServiceOperationResultModel<TResult>
      {
        Success = true,
        Message = null,
        Result = result
      };
    }
  }
}

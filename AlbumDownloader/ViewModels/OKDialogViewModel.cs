using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace AlbumDownloader.ViewModels
{
  public class OKDialogViewModel : BindableBase, IDialogAware
  {
    private string _title;
    private string _message;
    private bool _canCloseDialog;
    private DelegateCommand _okCommmand;

    public string Title
    {
      get => _title;
      set => SetProperty(ref _title, value);
    }

    public string Message
    {
      get => _message;
      set => SetProperty(ref _message, value);
    }

    public DelegateCommand OKCommand => _okCommmand ?? (_okCommmand = new DelegateCommand(ExecuteOKCommand));

    public event Action<IDialogResult> RequestClose;

    public bool CanCloseDialog() => _canCloseDialog;

    public void OnDialogClosed()
    { }

    public void OnDialogOpened(IDialogParameters parameters)
    {
      Title = parameters.GetValue<string>(nameof(Title));
      Message = parameters.GetValue<string>(nameof(Message));
    }

    private void ExecuteOKCommand()
    {
      _canCloseDialog = true;
      RequestClose.Invoke(new DialogResult(ButtonResult.OK));
    }
  }
}

﻿using System;
using System.Text.RegularExpressions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace AlbumDownloader.ViewModels
{
  internal class VKAuthDialogViewModel : BindableBase, IDialogAware
  {
    private static Regex _authSuccessRegex = new (@"access_token=(.+)&(?:.*?)expires_in=(.+)&", RegexOptions.Compiled);
    private static Regex _authFailureRegex = new (@"error=(.+)&(?:.*?)error_description=(.+)&", RegexOptions.Compiled);

    private const string _redirectUrl = "https://oauth.vk.com/blank.html";

    private string _authUrl;

    public event Action<IDialogResult> RequestClose;
    public event Action OnLeavingAuthDialog;

    public string Title => "VK Auth";

    public DelegateCommand CloseDialog { get; }

    public string AuthUrl
    {
      get => _authUrl;
      set
      {
        SetProperty(ref _authUrl, value);

        if (value.StartsWith(_redirectUrl))
        {
          var match = _authSuccessRegex.Match(AuthUrl);
          DialogResult dialogResult;

          if (match.Success)
          {
            string token = match.Groups[1].Value;
            int tokenLifetimeSeconds = Int32.Parse(match.Groups[2].Value);

            dialogResult = new DialogResult(ButtonResult.OK, new DialogParameters
            {
              { "Token", token },
              { "TokenExpirationTime", DateTimeOffset.Now.AddSeconds(tokenLifetimeSeconds) }
            });
          }
          else
          {
            match = _authFailureRegex.Match(AuthUrl);

            dialogResult = match.Success
              ? new DialogResult(ButtonResult.Abort, new DialogParameters
              {
                { "Error", match.Groups[2].Value }
              })
              : new DialogResult(ButtonResult.Abort, new DialogParameters
              {
                { "Error", "Unknown error!" }
              });
          }

          RequestClose(dialogResult);
        }
      }
    }

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
      OnLeavingAuthDialog?.Invoke();
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
      string appId = parameters.GetValue<string>(nameof(Settings.AppID));
      string vkApiVersion = parameters.GetValue<string>(nameof(Settings.ApiVersion));

      AuthUrl = $"https://oauth.vk.com/authorize?client_id={appId}&display=page&redirect_uri={_redirectUrl}&scope=photos&response_type=token&v={vkApiVersion}";
    }
  }
}

using System;
using System.Configuration;
using System.IO;
using System.Windows;
using AlbumDownloader.Services;
using AlbumDownloader.ViewModels;
using AlbumDownloader.Views;
using Prism.Ioc;
using Prism.Regions;

namespace AlbumDownloader
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    protected override Window CreateShell()
    {
      return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      containerRegistry.RegisterSingleton<Settings>();
      containerRegistry.RegisterSingleton<VKApiRequestProvider>();
      containerRegistry.RegisterSingleton<AlbumsDownloadService>();
      containerRegistry.Register<DialogService>();
      containerRegistry.Register<AccountService>();

      containerRegistry.RegisterDialog<VKAuthDialog, VKAuthDialogViewModel>(nameof(VKAuthDialog));
      containerRegistry.RegisterDialog<OKDialog, OKDialogViewModel>(nameof(OKDialog));

      containerRegistry.RegisterForNavigation<LoginPage>(nameof(LoginPage));
      containerRegistry.RegisterForNavigation<MainPage>(nameof(MainPage));
      containerRegistry.RegisterForNavigation<AlbumsPage>(nameof(AlbumsPage));
      containerRegistry.RegisterForNavigation<DownloadPage>(nameof(DownloadPage));
    }

    protected override void OnInitialized()
    {
      base.OnInitialized();

      var settings = Container.Resolve<Settings>();

      settings.ApiVersion = ConfigurationManager.AppSettings.Get("VKApiVersion");
      settings.AppID = ConfigurationManager.AppSettings.Get("AppID");
      settings.ImagesBatchSize = Int32.Parse(ConfigurationManager.AppSettings.Get("ImagesBatchSize"));

      Settings.WebViewUserDataFolder = Path.Combine(Directory.GetCurrentDirectory(), "BrowserData");

      if (!Directory.Exists(Settings.WebViewUserDataFolder))
      {
        Directory.CreateDirectory(Settings.WebViewUserDataFolder);
      }

      Container.Resolve<IRegionManager>().RequestNavigate(Settings.MainWindowRegion, nameof(LoginPage));
    }
  }
}

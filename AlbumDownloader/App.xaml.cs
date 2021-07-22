using System.Configuration;
using System.Windows;
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
      containerRegistry.RegisterDialog<VKAuthDialog, VKAuthDialogViewModel>(nameof(VKAuthDialog));

      containerRegistry.RegisterForNavigation<LoginPage>(nameof(LoginPage));
      containerRegistry.RegisterForNavigation<AlbumnsPage>(nameof(AlbumnsPage));
    }

    protected override void OnInitialized()
    {
      base.OnInitialized();

      Settings.ApiVersion = ConfigurationManager.AppSettings.Get("VKApiVersion");
      Settings.AppID = ConfigurationManager.AppSettings.Get("AppID");

      Container.Resolve<IRegionManager>().RequestNavigate(Settings.MainRegion, nameof(LoginPage));
    }
  }
}

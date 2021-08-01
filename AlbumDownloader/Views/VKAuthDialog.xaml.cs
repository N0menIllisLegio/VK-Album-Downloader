using System.Windows.Controls;
using AlbumDownloader.ViewModels;
using Microsoft.Web.WebView2.Core;

namespace AlbumDownloader.Views
{
  /// <summary>
  /// Interaction logic for VKAuthDialog
  /// </summary>
  public partial class VKAuthDialog : UserControl
  {
    private bool coreWebView2InitializationCompleted = false;

    public VKAuthDialog()
    {
      InitializeComponent();

      (DataContext as VKAuthDialogViewModel).OnLeavingAuthDialog += OnLeavingAuthDialog;

      webView2.CoreWebView2InitializationCompleted += WebView2_CoreWebView2InitializationCompleted;
    }

    private void OnLeavingAuthDialog()
    {
      if (coreWebView2InitializationCompleted)
      {
        webView2.CoreWebView2.CookieManager.DeleteAllCookies();
      }
    }

    private void WebView2_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
    {
      coreWebView2InitializationCompleted = true;
    }
  }
}

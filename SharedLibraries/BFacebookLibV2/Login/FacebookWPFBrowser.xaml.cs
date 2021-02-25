namespace Sobees.Library.BFacebookLibV2.Login
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;
  using System.Web;
  using System.Windows;
  using System.Windows.Navigation;

  #endregion

  /// <summary>
  ///   Interaction logic for FacebookWPFBrowser.xaml
  /// </summary>
  public partial class FacebookWpfBrowser : Window
  {
    private readonly string _url;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="url">The url to navigate to when loaded.</param>
    public FacebookWpfBrowser(string url)
    {
      InitializeComponent();
      Loaded += FacebookWpfBrowser_Loaded;
      _url = url;
    }

    /// <summary>
    ///   Key value pairs of the session information returned by facebook includes session_key, secret, expires and uid
    /// </summary>
    public Dictionary<string, string> SessionProperties { get; set; }

    //The access token retrieved from facebook's authentication
    public string AccessToken { get; set; }

    private void FacebookWpfBrowser_Loaded(object sender, RoutedEventArgs e)
    {
      //Add the message hook in the code behind since I got a weird bug when trying to do it in the XAML
      webBrowser.MessageHook += webBrowser_MessageHook;

      //Delete the cookies since the last authentication
      //DeleteFacebookCookie();
      SetFacebookCookie();

      webBrowser.Navigate(_url);
    }

    private void DeleteFacebookCookie()
    {
      //Set the current user cookie to have expired yesterday
      var cookie = $"c_user=; expires={DateTime.UtcNow.AddDays(-1).ToString("R"):R}; path=/; domain=.facebook.com";
      Application.SetCookie(new Uri("https://www.facebook.com"), cookie);
    }
    private void SetFacebookCookie()
    {
      //Set the current user cookie to have expired yesterday
      var cookie = $"c_user=; expires={DateTime.UtcNow.AddDays(1).ToString("R"):R}; path=/; domain=.facebook.com";
      Application.SetCookie(new Uri("https://www.facebook.com"), cookie);
    }

    private void WebBrowserNavigated(object sender, NavigationEventArgs e)
    {
      //If the URL has an access_token, grab it and walk away...
      var url = e.Uri.Fragment;
      if (url.Contains("access_token") && url.Contains("#"))
      {
        url = (new Regex("#")).Replace(url, "?", 1);
        AccessToken = HttpUtility.ParseQueryString(url).Get("access_token");
        DialogResult = true;
        Close();
      }
    }

    private IntPtr webBrowser_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      //msg = 130 is the last call for when the window gets closed on a window.close() in javascript
      if (msg == 130)
      {
        Close();
      }
      return IntPtr.Zero;
    }

    private void WebBrowserNavigating(object sender, NavigatingCancelEventArgs e)
    {
      if (e.Uri.LocalPath == "/r.php")
      {
        MessageBox.Show("To create a new account go to www.facebook.com", "Could Not Create Account", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Cancel = true;
      }
    }
  }
}
#region

using System.Linq;
using System.Windows;
using FacebookConsoleTest.Properties;
using FacebookConsoleTest.ViewModel;
using Sobees.Library.BFacebookLibV2;
using Sobees.Library.BFacebookLibV2.Login;
using Sobees.Library.BFacebookLibV2.Objects.Token;
using Sobees.Library.BFacebookLibV2.Scope;

#endregion

namespace FacebookConsoleTest
{
  #region

  

  #endregion

  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public static string FACEBOOK_WPF_API = "95695045581";
    public static string FACEBOOK_WPF_SECRET = "d0dc7a9c8e64cfef216a08eab22f3df3";
    private DesktopSession _session;

    public MainWindow()
    {
      InitializeComponent();
    }

    public DesktopSession CurrentSession
    {
      get
      {
        if (_session != null) return _session;
        var permissions = FacebookScopeCollection.GetPermissionList();
        _session = new DesktopSession(FACEBOOK_WPF_API, permissions);
        return _session;
      }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      LoginSession();
    }

    private void LoginSession()
    {
      var model = DataContext as MainViewModel;

      var accessToken = Settings.Default.AccessToken;
      if (!string.IsNullOrEmpty(accessToken))
      {
        var res = FacebookContext.Instance.GetDebugToken(accessToken, FACEBOOK_WPF_API);
        var debugToken = DebugToken.Parse(res?.ToString());
        accessToken = debugToken == null ? null : (debugToken.Is_Valid ? accessToken : null);
        if (!string.IsNullOrEmpty(accessToken))
          FacebookContext.Instance.RenewToken(accessToken);
      }

      FacebookContext.Instance.Init(FACEBOOK_WPF_API, FacebookScopeCollection.GetPermissionList(), accessToken);

      if (FacebookContext.Instance.DesktopSessionInstance.AccessToken == null) return;
      var me = FacebookContext.Instance.GetMe();
      lblToken.Content = $"{me.Id}|{me.Name}";

      var user = FacebookContext.Instance.GetUserInfo(me.Id);
      if (user != null)
      {
        LblUser.Content = $"{user.Id}|{user.Name}|{user.Cover}{user.Link}|\n\r{user.About}|\n\r{user.Bio}";
      }

      if (model != null) model.CurrentFacebookUser = me;

      Settings.Default.Name = me.Name;
      Settings.Default.AccessToken = FacebookContext.Instance.DesktopSessionInstance.AccessToken;
      Settings.Default.Save();

      //dynamic res2 = FacebookContext.Instance.GetDebugToken(FacebookContext.Instance.DesktopSessionInstance.AccessToken, FACEBOOK_WPF_API);
      //var valid = res2.data.expires_at;
      //var validdate = SocialUtils.GetDateTimeFromUnixTime(valid);
      //var v2 = DateTime.Parse(validdate.ToString());

      var feeds = FacebookContext.Instance.GetFeed(me.Id);
      foreach (var feed in feeds)
      {
        MyItemsControl.Items.Add(feed);
      }

      var perms = FacebookContext.Instance.GetPermissions(me.Id);

      //var result = FacebookContext.Instance.PostAsync("Beez Beez ....").Result;
      //MyItemsControl.Items.Add($"Post:{result?.Values.FirstOrDefault()}");
    }
  }
}
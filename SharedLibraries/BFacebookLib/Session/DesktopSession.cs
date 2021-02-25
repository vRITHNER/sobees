namespace Sobees.Library.BFacebookLibV1.Session
{
  #region

  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using System.Windows.Forms;
  using Sobees.Library.BFacebookLibV1.Schema;
  using Sobees.Library.BFacebookLibV1.Session.DesktopPopup;
  using Sobees.Library.BFacebookLibV1.Utility;

  #endregion

  /// <summary>
  ///   Represents session object for desktop apps
  /// </summary>
  public class DesktopSession : FacebookSession
  {
    //const string _loginUrl = "http://www.facebook.com/login.php?api_key={0}&connect_display=popup&v=1.0&next=http://www.facebook.com/connect/login_success.html&cancel_url=http://www.facebook.com/connect/login_failure.html&fbconnect=true&return_session=true";
    private const string GRAPH_LOGIN_URL =
      @"https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri=https://www.facebook.com/connect/login_success.html&type=user_agent&display=popup";

    private readonly bool _isWpf;

    #region Public Methods

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="appKey">Application Key</param>
    /// <param name="isWpf">true for WPF, false for winform</param>
    public DesktopSession(string appKey, bool isWpf)
      : this(appKey, null, null, isWpf)
    {
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="appKey">Application Key</param>
    /// <param name="isWpf">true for WPF, false for winform</param>
    /// <param name="permissions">list of extended permissions to prompt for upon login</param>
    public DesktopSession(string appKey, bool isWpf, List<Enums.ExtendedPermissions> permissions)
      : this(appKey, null, null, isWpf, permissions)
    {
    }

    /// <summary>
    ///   Constructor - You should not need login function when using this constructor as this is a previously cached session
    /// </summary>
    /// <param name="appKey">Application Key</param>
    /// <param name="sessionSecret">Session secret - If previously cached</param>
    /// <param name="sessionKey">Session key - If previously cached</param>
    public DesktopSession(string appKey, string sessionSecret, string sessionKey)
      : this(appKey, sessionSecret, sessionKey, false)
    {
    }

    /// <summary>
    ///   Constructor - You should not need login function when using this constructor as this is a previously cached session
    /// </summary>
    /// <param name="appKey">Application Key</param>
    /// <param name="sessionSecret">Session secret - If previously cached</param>
    /// <param name="sessionKey">Session key - If previously cached</param>
    /// <param name="isWPF">true for WPF, false for winform</param>
    public DesktopSession(string appKey, string sessionSecret, string sessionKey, bool isWPF)
      : this(appKey, sessionSecret, sessionKey, isWPF, null)
    {
    }

    /// <summary>
    ///   Constructor - You should not need login function when using this constructor as this is a previously cached session
    /// </summary>
    /// <param name="appKey">Application Key</param>
    /// <param name="sessionSecret">Session secret - If previously cached</param>
    /// <param name="sessionKey">Session key - If previously cached</param>
    /// <param name="isWPF">true for WPF, false for winform</param>
    /// <param name="permissions">list of extended permissions to prompt for upon login</param>
    public DesktopSession(string appKey, string sessionSecret, string sessionKey, bool isWPF, List<Enums.ExtendedPermissions> permissions)
    {
      ApplicationKey = appKey;
      SessionSecret = sessionSecret;
      SessionKey = sessionKey;
      _isWpf = true;
      //_isWPF = isWPF;
      RequiredPermissions = permissions;
      UseGraphAuth = true;
    }

    /// <summary>
    ///   Logs into session
    /// </summary>
    public override void Login()
    {
      if (_isWpf)
      {
        var formLogin = new FacebookWPFBrowser(GetLoginUrl())
        {
          Title = "Facebook: Login",
          WindowStartupLocation = WindowStartupLocation.CenterScreen,
          Width = 626,
          Height = 431,
          WindowStyle = WindowStyle.ToolWindow
        };
        var dialogResult = formLogin.ShowDialog();
        if (dialogResult.HasValue && dialogResult.Value)
          CompleteLogin(formLogin.SessionProperties);
        else
          OnLoggedIn(new FacebookException("Login attempt failed"));
      }
      else
      {
        using (var formLogin = new FacebookWinformBrowser(GetLoginUrl()))
        {
          var result = formLogin.ShowDialog();
          if (result == DialogResult.OK)
            CompleteLogin(formLogin.SessionProperties);
          else
            OnLoggedIn(new FacebookException("Login attempt failed"));
        }
      }
    }

    private void CompleteLogin(Dictionary<string, string> sessionProperties)
    {
      if (!UseGraphAuth)
      {
        SessionExpires = sessionProperties["expires"] != "0";
        ExpiryTime = DateHelper.ConvertUnixTimeToDateTime(long.Parse(sessionProperties["expires"]));
        SessionKey = sessionProperties["session_key"];
        SessionSecret = sessionProperties["secret"];

        if (sessionProperties["uid"] != null)
          UserId = long.Parse(sessionProperties["uid"]);
      }
      else
      {
        AccessToken = sessionProperties["access_token"];
      }
      OnLoggedIn(null);
    }

    /// <summary>
    ///   logs out of session
    /// </summary>
    public override void Logout()
    {
      OnLoggedOut(null);
    }


    /// <summary>
    ///   Gets login url which can be used to login to facebook server
    /// </summary>
    /// <returns>This method returns the Facebook Login URL.</returns>
    public string GetLoginUrl()
    {
      string loginUrl = null;
      if (UseGraphAuth)
      {
        loginUrl = string.Format(GRAPH_LOGIN_URL, ApplicationKey);
      }
      if (RequiredPermissions == null) return loginUrl;
      if (RequiredPermissions != null && RequiredPermissions.Any())
        loginUrl += $"&scope={PermissionsToString(RequiredPermissions)}";
      loginUrl += $"&req_perms={PermissionsToString(RequiredPermissions)}";
      return loginUrl;
    }

    #endregion
  }
}
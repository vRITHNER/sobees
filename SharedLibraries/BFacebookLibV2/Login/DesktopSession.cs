namespace Sobees.Library.BFacebookLibV2.Login
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using Facebook;
  using Sobees.Library.BFacebookLibV2.Scope;

  #endregion

  /// <summary>
  ///   Represents session object for desktop apps
  /// </summary>
  public class DesktopSession : FacebookSession
  {
    #region Public Methods

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="appKey">Application Key</param>
    public DesktopSession(string appKey)
      : this(appKey, null)
    {
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="appKey">Application Key</param>
    /// <param name="permissions">list of extended permissions to prompt for upon login</param>
    public DesktopSession(string appKey, FacebookScopeCollection permissions)
      : this(appKey, null, null,  permissions)
    {
    }

    /// <summary>
    ///   Constructor - You should not need login function when using this constructor as this is a previously cached session
    /// </summary>
    /// <param name="appKey">Application Key</param>
    /// <param name="sessionSecret">Session secret - If previously cached</param>
    /// <param name="sessionKey">Session key - If previously cached</param>
    /// <param name="permissions">list of extended permissions to prompt for upon login</param>
    public DesktopSession(string appKey, string sessionSecret, string sessionKey, FacebookScopeCollection permissions)
    {
      ApplicationKey = appKey;
      SessionSecret = sessionSecret;
      SessionKey = sessionKey;
      RequiredPermissions = permissions;
      UseGraphAuth = true;
    }


    public string AppId { get; set; }

    public string RedirectUri { get; set; }

    /// <summary>
    ///   Generates the authorization URL using the specified state and scope.
    /// </summary>
    /// <param name="state">The state to send to Facebook's OAuth login page.</param>
    /// <param name="scope">The scope of the application.</param>
    public string GetAuthorizationUrl(string state, FacebookScopeCollection scope)
    {
      return $"https://www.facebook.com/dialog/oauth?client_id={AppId}&redirect_uri={RedirectUri}&state={state}&scope={string.Join(",", scope)}";
    }

    public override void Login()
    {
      var args = "";
      if (UseGraphAuth)
      {
        if (RequiredPermissions != null && RequiredPermissions.Any())
          args += PermissionsToString(RequiredPermissions);
          //args += "email,user_birthday";
        //args += $"&req_perms={PermissionsToString(RequiredPermissions)}";
      }
      var instance = FacebookContext.Instance;

      var loginArg = new
      {
        client_id = ApplicationKey,
        redirect_uri = "https://www.facebook.com/connect/login_success.html",
        response_type = "token",
        display = "popup",
        type = "user_agent",
        scope = args // Add other permissions as needed
      };
      
      var loginUrl = instance.FacebookClient.GetLoginUrl(loginArg);
      var formLogin = new FacebookWpfBrowser(loginUrl.ToString())
      {
        Title = "Facebook: Login",
        WindowStartupLocation = WindowStartupLocation.CenterScreen,
        WindowStyle = WindowStyle.ToolWindow
      };
      var dialogResult = formLogin.ShowDialog();
      if (dialogResult.HasValue && dialogResult.Value)
      {
        AccessToken = formLogin.AccessToken;

        instance.FacebookClient.AccessToken = formLogin.AccessToken;
        var user = instance.GetMe();
        instance.DesktopSessionInstance.UserId = Convert.ToInt64(user.Id);
        OnLoggedIn(null);
      }
      else
        OnLoggedIn(new FacebookApiException("Login attempt failed"));
    }

    #endregion
  }
}
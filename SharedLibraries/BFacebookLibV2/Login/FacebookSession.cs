#region



#endregion

namespace Sobees.Library.BFacebookLibV2.Login
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Text;
  using Facebook;
  using Sobees.Library.BFacebookLibV2.Scope;

  /// <summary>
  ///   Base class for session object
  /// </summary>
  public class FacebookSession
  {
    private const string PROMPT_PERMISSIONS_URL =
      "http://www.facebook.com/connect/prompt_permissions.php?api_key={0}&next={1}&display=popup&ext_perm={2}&enable_profile_selector=1&profile_selector_ids={3}";

    private const string PROMPT_PERMISSIONS_NEXT_URL =
      "http://www.facebook.com/connect/login_success.html?xxRESULTTOKENxx";

    private long _userId;

    /// <summary>
    ///   List of extended permissions required by this application
    /// </summary>
    public FacebookScopeCollection RequiredPermissions { get; set; }

    public bool UseGraphAuth { get; set; }

    /// <summary>
    ///   Access Token from Graph Auth
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    ///   Code from Graph Auth
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    ///   Application key
    /// </summary>
    public string ApplicationKey { get; set; }

    /// <summary>
    ///   Application secret
    /// </summary>
    public string ApplicationSecret { get; set; }

    /// <summary>
    ///   Session key
    /// </summary>
    public string SessionKey { get; set; }

    /// <summary>
    ///   Session secret
    /// </summary>
    public string SessionSecret { get; set; }

    /// <summary>
    ///   Expiry time
    /// </summary>
    public DateTime ExpiryTime { get; protected set; }

    /// <summary>
    ///   Whether or not the session expires
    /// </summary>
    public bool SessionExpires { get; set; }

    /// <summary>
    ///   User Id
    /// </summary>
    public long UserId
    {
      get
      {
        return _userId > 0 ? _userId : FacebookContext.Instance.DesktopSessionInstance.UserId;
      }
      set { _userId = value; }
    }

    /// <summary>
    ///   Whether the Http Post and Response should be compressed
    /// </summary>
    public bool CompressHttp { get; set; }

    /// <summary>
    ///   Secret key that needs to be used to encrypt requests
    /// </summary>
    public virtual string Secret => SessionSecret ?? ApplicationSecret;

    /// <summary>
    ///   Login completed event
    /// </summary>
    public event EventHandler<AsyncCompletedEventArgs> LoginCompleted;

    /// <summary>
    ///   Logout completed event
    /// </summary>
    public event EventHandler<AsyncCompletedEventArgs> LogoutCompleted;

    /// <summary>
    ///   Called when log in completed
    /// </summary>
    /// <param name="e"></param>
    protected internal void OnLoggedIn(Exception e)
    {
      LoginCompleted?.Invoke(this, new AsyncCompletedEventArgs(e, false, null));
    }

    /// <summary>
    ///   Called when log out completes
    /// </summary>
    /// <param name="e"></param>
    protected internal void OnLoggedOut(Exception e)
    {
      LogoutCompleted?.Invoke(this, new AsyncCompletedEventArgs(e, false, null));
    }

    /// <summary>
    ///   Check if user has the proper permissions for this app
    /// </summary>
    public string CheckPermissions()
    {
      if (RequiredPermissions == null) return null;
      //var query = $"select {PermissionsToString(RequiredPermissions)} from permissions where uid = {UserId}";

      //var fql = new Api(this).Fql;

      //var permission = fql.Query<permissions_response>(query);

      //var permissionsToApprove = (from p in RequiredPermissions let f = permission.permissions.GetType().GetField(p.ToString()) where f != null let hasPermission = (bool)f.GetValue(permission.permissions) where !hasPermission select p).ToList();

      dynamic result = FacebookContext.Instance.FacebookClient.Get(UserId + "/permissions");
      return result.ToString();
    }

    /// <summary>
    ///   Gets login url which can be used to login to facebook server
    /// </summary>
    /// <returns>This method returns the Facebook Login URL.</returns>
    public string GetPermissionUrl(string permissionString)
    {
      return string.Format(PROMPT_PERMISSIONS_URL, ApplicationKey, PROMPT_PERMISSIONS_NEXT_URL, permissionString, UserId);
    }

    /// <summary>
    ///   Gets login url which can be used to login to facebook server
    /// </summary>
    /// <returns>This method returns the Facebook Login URL.</returns>
    public string GetPermissionUrl(string permissionString, string nextUrl)
    {
      return string.Format(PROMPT_PERMISSIONS_URL, ApplicationKey, nextUrl, permissionString, UserId);
    }

    /// <summary>
    ///   Convert permission list to "read_stream, status_update, photo_upload, publish_stream" format
    /// </summary>
    /// <param name="permissions"></param>
    /// <returns>This method returns a string of permissions.</returns>
    protected string PermissionsToString(FacebookScopeCollection permissions)
    {
      var sb = new StringBuilder();

      var i = 0;
      foreach (var permission in permissions)
      {
        sb.Append(permission);
        i++;
        if (i < permissions.Count)
          sb.Append(",");
      }

      return sb.ToString();
    }

    public virtual void Login()
    {
      throw new NotImplementedException();
    }

    public virtual void Logout()
    {
      throw new NotImplementedException();
    }
  }
}
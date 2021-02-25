#region

using Facebook;
using System;
using System.Threading.Tasks;

#endregion

namespace Sobees.Library.BFacebookLibV2
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using Sobees.Library.BFacebookLibV2.Extensions;
  using Sobees.Library.BFacebookLibV2.Json;
  using Sobees.Library.BFacebookLibV2.Login;
  using Sobees.Library.BFacebookLibV2.Objects.Feed;
  using Sobees.Library.BFacebookLibV2.Objects.Users;
  using Sobees.Library.BFacebookLibV2.Scope;

  public class FacebookContext : SingletonBase<FacebookContext>
  {
    private static FacebookClient _facebookClient;

    public DesktopSession DesktopSessionInstance { get; set; }

    public FacebookContext()
    {
      //Init(BFacebookLibGlobal.APP_ID, FacebookScopeCollection.GetPermissionList());
    }

    public void Init(string appId)
    {
      Init(appId, FacebookScopeCollection.GetPermissionList());
    }

    public void Init(string appId, FacebookScopeCollection permissions, string accessToken = null)
    {
      DesktopSessionInstance = new DesktopSession(appId, FacebookScopeCollection.GetPermissionList());
      Init(DesktopSessionInstance, appId, permissions, accessToken);
    }
    public void Init(DesktopSession dession, string appId, FacebookScopeCollection permissions, string accessToken = null)
    {
      DesktopSessionInstance = dession;
      //get the session from FB ViewModel
      if (!string.IsNullOrEmpty(accessToken))
      {
        DesktopSessionInstance.AccessToken = accessToken;
        FacebookClient = new FacebookClient(accessToken);
       
      }
      if (DesktopSessionInstance.AccessToken != null) return;
      FacebookClient = new FacebookClient();
      DesktopSessionInstance.Login();
    }
    
   public FacebookClient FacebookClient { get; private set; }
   
    public string GetAccessToken()
    {
      //_facebookClient = new FacebookClient();
      dynamic result = FacebookClient.Get("oauth/access_token",
        new
        {
          client_id = BFacebookLibGlobal.APP_ID,
          client_secret = BFacebookLibGlobal.APP_SECRET,
          grant_type = "client_credentials",
        });
      var token = result.access_token;
      _facebookClient.AccessToken = token;

      return token;
    }

    public string RenewToken(string existingToken)
    {
      _facebookClient = new FacebookClient(existingToken);
      dynamic result = _facebookClient.Get("oauth/access_token",
        new
        {
          client_id = BFacebookLibGlobal.APP_ID,
          client_secret = BFacebookLibGlobal.APP_SECRET,
          grant_type = "fb_exchange_token",
          fb_exchange_token = existingToken
        });
      return result.access_token;
    }

    public FacebookUser GetMe()
    {
      dynamic result = FacebookClient.Get("me", new[] { "id", "name", "last_name", "first_name", "picture" });
      var user = FacebookUser.Parse(result.ToString());
      return user;
    }

    public FacebookUser GetUserInfo(string userId)
    {
      dynamic result = FacebookClient.Get(userId, new[] { "id", "name", "first_name", "last_name", "picture" });
      var user = FacebookUser.Parse(result.ToString());
      return user;
    }

    public dynamic GetDebugToken(string token, string accesstoken)
    {
      var client = new FacebookClient(token);
      try
      {
        dynamic result = client.Get($"debug_token?input_token={token}&access_token={accesstoken}");
        return result;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return null;
    }

    public ObservableCollection<FacebookFeedEntry> GetFeed(string userId, int nbToGet=25)
    {
      var result = FacebookClient.Get($"{userId}/Feed?limit={nbToGet}").ToJsonObjectEx();
      var feed = FacebookFeedCollection.Parse(result);
      return feed?.Data?.ToObservableCollection(); 
    }

    public ObservableCollection<FacebookFeedEntry> GetHome(string userId, int nbToGet = 25)
    {
      var result = FacebookClient.Get($"{userId}/Home/?fields=id,from,message,description,story,picture,link,name,caption,icon,type,status_type,application,created_time,update_type,shares,likes,comments,attachments,objectid&limit={nbToGet}").ToJsonObjectEx();
      var feed = FacebookFeedCollection.Parse(result);
      return feed?.Data?.ToObservableCollection();
    }

    public async Task<IDictionary<string, object>> PostAsync(string message)
    {
      var fb = FacebookClient;
      IDictionary<string, object> result = null;
      fb.PostCompleted += (o, e) => {
        if (e.Error == null)
        {
          result = (IDictionary<string, object>)e.GetResultData();
        }
      };

      var parameters = new Dictionary<string, object> {["message"] = message};
      var resobject = await FacebookClient.PostTaskAsync("me/feed", parameters);
      return await Task.FromResult(result);
    }

    public JsonObjectEx GetPermissions(string userId)
    {
      var result = FacebookClient.Get($"{userId}/permissions").ToJsonObjectEx();
      return result;

    }

    /// <summary>
    ///   logs out of session
    /// </summary>
    public void Logout()
    {
      OnLoggedOut(null);
    }

    private void OnLoggedOut(object o)
    {
      throw new NotImplementedException();
    }
  }
}
#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Navigation;
using System.Xml.Linq;
using BUtility;
using LinqToTwitter;
using Sobees.Library.BTwitterLib.Extensions;
using Sobees.Library.BTwitterLib.Helpers;
using Sobees.Library.BTwitterLib.Response;
using Sobees.Tools.Extensions;
using Sobees.Tools.Logging;
using Utilities = LinqToTwitter.Utilities;

#endregion

namespace Sobees.Library.BTwitterLib
{
  public class TwitterLibV11 : OAuth
  {
    #region Method enum

    public enum Method
    {
      GET,
      POST
    }

    #endregion

    private const string APPNAME = "TwitterLibV11";
    private const string photoStartTag = "<Photo>|";
    private const string photoEndTag = "|</Photo>";
    public static readonly string AccessToken = TwitterResources.ACCESS_TOKEN;
    public static readonly string Authorize = TwitterResources.AUTHORIZE;
    public static readonly string RequestToken = TwitterResources.REQUEST_TOKEN;
    private static string _callBackUrl = "oob";

    private static string _consumerKey = "";
    private static string _consumerSecret = "";
    private static string _oauthVerifier = "";
    private static string _token = "";
    private static string _tokenSecret = "";

    private static PinAuthorizer _pinAuthorizer;
    public static TwitterContext _twitterContext;
   

    #region Properties

    public static string OAuthVerifier
    {
      get { return _oauthVerifier; }
      set { _oauthVerifier = value; }
    }

    public static string CallBackUrl
    {
      get { return _callBackUrl; }
      set { _callBackUrl = value; }
    }

    #endregion

    /// <summary>
    ///   Get the link to Twitter's authorization page for this application.
    /// </summary>
    /// <returns>The url with a valid request token, or a null string.</returns>
    public static string AuthorizationLinkGet(string consumerKey, string consumerSecret, string callbackUrl,
      WebProxy proxy)
    {
      TraceHelper.Trace("TwitterLib AuthorizationLinkGet", "Started");
      CallBackUrl = callbackUrl;

      OAuthVerifier = "";
      string ret = null;
      var response = OAuthWebRequest(Method.GET, RequestToken, String.Empty, consumerKey, consumerSecret,
        string.Empty, string.Empty, proxy);

      //TraceHelper.Trace("TwitterLib AuthorizationLinkGet", response);
      if (response.Length > 0)
      {
        //response contains token and token secret.  We only need the token.
        var qs = HttpUtility.ParseQueryString(response);

        if (qs["oauth_callback_confirmed"] != null)
        {
          if (!(qs["oauth_callback_confirmed"] == "true"))
          {
            throw new Exception("OAuth callback not confirmed.");
          }
        }

        if (qs["oauth_token"] != null)
        {
          ret = Authorize + "?oauth_token=" + qs["oauth_token"];
        }
      }
      TraceHelper.Trace("TwitterLib AuthorizationLinkGet", ret);
      return ret;
    }

    public static string ConnectXAuth(string consumerKey, string consumerSecret, string username, string token,
      string secret, WebProxy proxy)
    {
      if (string.IsNullOrEmpty(token))
        throw new Exception("token is null");

      // configure the OAuth object
      using (var twitterCtx = Context.CreateTwitterContext(token, secret))
      {
        var users =
          (from tweet in twitterCtx.User
            where tweet.Type == UserType.Search &&
                  tweet.ScreenName == username
            select tweet)
            .ToList();
        if (users.Any())
          return token;
      }
      return null;
    }

    /// <summary>
    ///   Submit a web request using oAuth.
    /// </summary>
    /// <param name="method">GET or POST</param>
    /// <param name="url">The full url, including the querystring.</param>
    /// <param name="postData">Data to post (querystring format)</param>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="tokenSecret"></param>
    /// <returns>The web server response.</returns>
    public static string OAuthWebRequest(Method method, string url, string postData, string consumerKey,
      string consumerSecret, string token, string tokenSecret, WebProxy proxy)
    {
      string outUrl;
      string querystring;

      //Setup postData for signing.
      //Add the postData to the querystring.
      if (method == Method.POST)
      {
        if (postData.Length > 0)
        {
          //Decode the parameters and re-encode using the oAuth UrlEncode method.
          var qs = HttpUtility.ParseQueryString(postData);
          postData = "";
          foreach (var key in qs.AllKeys)
          {
            if (postData.Length > 0)
              postData += "&";

            //qs[key] = HttpUtility.UrlDecode(qs[key]);
            //qs[key] = Uri.EscapeDataString(qs[key]);
            //qs[key] = UrlEncode(qs[key]);

            //if (!key.Equals("x_auth_password"))

            if (!key.Equals("x_auth_password"))
            {
              //qs[key] = HttpUtility.UrlDecode(qs[key]);
              qs[key] = UrlEncode(qs[key]);
            }
            else
            {
              qs[key] = UrlEncode(qs[key]);
            }

            postData += string.Format("{0}={1}", key, qs[key]);
          }
          url += url.IndexOf("?") > 0 ? "&" : "?";
          url += postData;
        }
      }

      var uri = new Uri(url);

      var nonce = GenerateNonce();
      var timeStamp = GenerateTimeStamp();

      //Generate Signature
      var sig = GenerateSignature(uri,
        consumerKey,
        consumerSecret,
        token,
        tokenSecret,
        CallBackUrl,
        OAuthVerifier,
        method.ToString(),
        timeStamp,
        nonce,
        out outUrl,
        out querystring);

      querystring += string.Format("&oauth_signature={0}", HttpUtility.UrlEncode(sig));

      //Convert the querystring to postData
      if (method == Method.POST)
      {
        postData = querystring;
        querystring = "";
      }

      if (querystring.Length > 0)
        outUrl += "?";

      var ret = WebRequest(method, outUrl + querystring, postData, proxy);
      BLogManager.LogEntry(APPNAME, "OAuthWebRequest", string.Format("\n\r{0}{1}", outUrl, querystring), true);
      return ret;
    }

    /// <summary>
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="screenname"></param>
    /// <param name="page"></param>
    /// <param name="errorMsg"></param>
    /// <param name="nextCursor"></param>
    /// <returns></returns>
    public static List<TwitterUser> GetFriendsCursor(string consumerKey, string consumerSecret,
      string token, string secret, string screenname, string page,
      out string errorMsg, out string nextCursor, WebProxy proxy)
    {
      errorMsg = null;
      var friendsResult = new List<TwitterUser>();
      const int pageNumber = 1;

      var cursor = "-1";
      BLogManager.LogEntry(string.Format("GetFriendsCursor::START"), true);
      //if (!Debugger.IsAttached) Debugger.Launch();
      try
      {
        //using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        //{
        //    do
        //    {
        //        var friendship = (from friend in twitterCtx.Friendship
        //                          where friend.Type == FriendshipType.FollowersList &&
        //                              friend.ScreenName == screenname &&
        //                              friend.Cursor == cursor
        //                          select friend)
        //             .SingleOrDefault();

        //        if (friendship != null)
        //        {
        //            cursor = friendship.CursorMovement.Next;
        //            friendsResult.AddRange(friendship.Users.Select(LinqToTwitterHelper.ConvertUserToTwitterUser).ToList());
        //        }

        //        BLogManager.LogEntry(string.Format("friendsList has {0} users for now and counting...", friendsResult.Count()), true);
        //        Task.Delay(TimeSpan.FromSeconds(5)).Wait();
        //    } while (cursor != "0");
        //}
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        BLogManager.LogEntry(APPNAME + "::" + "GetFriendsCursor", ex);
      }
      nextCursor = null;
      BLogManager.LogEntry(string.Format("GetFriendsCursor::END"), true);
      return friendsResult;
    }

    public static TwitterUser GetUserInfo(string consumerKey, string consumerSecret, string sessionKey,
      string secret, string userToGet,
      out string errorMsg, WebProxy proxy)
    {
      return GetUserInfo(consumerKey, consumerSecret, sessionKey, secret, userToGet,
        false,
        out errorMsg, proxy);
    }

    /// <summary>
    ///   GetReplies
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="count"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterEntry> GetReplies(string consumerKey, string consumerSecret, string token, string secret,
      int count, out string errorMsg, WebProxy proxy)
    {
      List<TwitterEntry> entries = null;
      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var myMentions =
            from mention in twitterCtx.Status
            where mention.Type == StatusType.Mentions && mention.Count == count
            //  && mention.IncludeEntities //&& mention.IncludeContributorDetails
            select mention;

          entries = myMentions.Select(myMention => LinqToTwitterHelper.ConvertStatusToTwitterEntry(myMention)).ToList();
          return entries;
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetReplies", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return entries;
    }

    /// <summary>
    ///   GetFavorites
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userToGet"></param>
    /// <param name="count"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterEntry> GetFavorites(string consumerKey, string consumerSecret, string token, string secret,
      string userToGet,
      int count,
      out string errorMsg, WebProxy proxy)
    {
      List<TwitterEntry> entries = null;
      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var favorites =
            (from fav in twitterCtx.Favorites
              where fav.Type == FavoritesType.Favorites
              select fav)
              .ToList();

          entries = favorites.Select(LinqToTwitterHelper.ConvertStatusToTwitterEntry).ToList();
          return entries;
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetFavorites", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return entries;
    }

    /// <summary>
    ///   GetFriendsTimeline
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="login"></param>
    /// <param name="count"></param>
    /// <param name="pageNb"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterEntry> GetFriendsTimeline(string consumerKey, string consumerSecret, string token,
      string secret,
      string login,
      int count,
      int pageNb,
      out string errorMsg, WebProxy proxy)
    {
      errorMsg = null;
      var friendsResult = new List<TwitterEntry>();

      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var friendTweets =
            from tweet in twitterCtx.Status
            where tweet.Type == StatusType.User && tweet.Count == count
            select tweet;

          friendsResult = friendTweets.Select(LinqToTwitterHelper.ConvertStatusToTwitterEntry).ToList();
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetFriendsTimeline", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return friendsResult;
    }

    /// <summary>
    ///   GetUserInfo
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userToGet"></param>
    /// <param name="bigImage"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static TwitterUser GetUserInfo(string consumerKey, string consumerSecret, string token, string secret,
      string userToGet,
      bool bigImage,
      out string errorMsg, WebProxy proxy)
    {
      errorMsg = null;

      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var twuser =
            (from tw in twitterCtx.User
              where tw.Type == UserType.Show &&
                    tw.ScreenName == userToGet
              select tw).FirstOrDefault();

          if (bigImage && !string.IsNullOrEmpty(twuser.ProfileImageUrl) && twuser.ProfileImageUrl.Contains("_normal"))
            twuser.ProfileImageUrl = twuser.ProfileImageUrl.Replace("_normal", string.Empty);

          var user = LinqToTwitterHelper.ConvertUserToTwitterUser(twuser);
          return user;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::GetUserInfo", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return null;
    }

    /// <summary>
    ///   UploadPhoto
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="status"></param>
    /// <param name="imagepath"></param>
    /// <returns></returns>
    public static async Task<BTwitterResponseResult<Status>> AddTweetWithPhoto(string consumerKey, string consumerSecret, string token,
      string secret, string status, string imagepath)
    {
      var result = BTwitterResponseResult<Status>.CreateInstance();

      try
      {

        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var imageBytes = File.ReadAllBytes(imagepath);
          var imageUploadTasks =
            new List<Task<Media>>
            {
                    twitterCtx.UploadMediaAsync(imageBytes, "image/png"),
            };

          await Task.WhenAll(imageUploadTasks);
          var mediaIds =
              (from tsk in imageUploadTasks
               select tsk.Result.MediaID)
              .ToList();

          var tweet =
            await
              twitterCtx.TweetAsync(status, mediaIds);

          result.DataResult = tweet;
          result.BisSuccess = true;
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "AddTweetWithPhoto", ex);
        result.ErrorMessage = ex.Message;
      }
      return result;
    }

    /// <summary>
    ///   FriendshipExists
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userA"></param>
    /// <param name="userB"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static bool FriendshipExists(string consumerKey, string consumerSecret, string token, string secret,
      string userA,
      string userB,
      out string errorMsg, WebProxy proxy)
    {
      var bFound = false;
      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var friendship =
            (from friend in twitterCtx.Friendship
              where friend.Type == FriendshipType.Show &&
                    friend.SourceScreenName == userA &&
                    friend.TargetScreenName == userB
              select friend)
              .First();
          bFound = friendship != null;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::FriendshipExists()", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return bFound;
    }

    /// <summary>
    ///   Follow
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="sessionKey"></param>
    /// <param name="secret"></param>
    /// <param name="userIdToFollow"></param>
    /// <param name="destroy"></param>
    /// <returns></returns>
    public static async Task<TwitterUser> Follow(string consumerKey, string consumerSecret, string sessionKey, string secret,
      string userIdToFollow,
      bool destroy)
    {
      return await Follow(consumerKey, consumerSecret, sessionKey, secret, userIdToFollow, destroy, null);
    }

    /// <summary>
    ///   Follow
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userIdToFollow"></param>
    /// <param name="destroy"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static async Task<TwitterUser> Follow(string consumerKey, string consumerSecret, string token, string secret,
      string userIdToFollow,
      bool destroy, WebProxy proxy)
    {
      TwitterUser user = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var bFollow = !destroy;
          var usr = await twitterCtx.CreateFriendshipAsync(userIdToFollow, bFollow);
          user = LinqToTwitterHelper.ConvertUserToTwitterUser(usr);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::Follow()", ex);
        var errorMsg = ex.ToCompleteExceptionMessage();
      }
      return user;
    }

    /// <summary>
    ///   AddFavorite
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="tweetId"></param>
    /// <param name="destroy"></param>
    /// <returns></returns>
    public static async Task<BTwitterResponseResult<TwitterEntry>> AddFavorite(string consumerKey, string consumerSecret, string token, string secret,
      string tweetId,bool destroy)
    {
      var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          Status status = null;
          status = !destroy
            ? await twitterCtx.CreateFavoriteAsync(tweetId.ToUlongValue())
            : await twitterCtx.DestroyFavoriteAsync(tweetId.ToUlongValue());

          result.DataResult = LinqToTwitterHelper.ConvertStatusToTwitterEntry(status);
          result.BisSuccess = true;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("AddFavorite", ex);
        result.ErrorMessage = ex.ToCompleteExceptionMessage();
      }
      return result;
    }

    /// <summary>
    ///   DeleteStatus
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="tweetId"></param>
    /// <returns></returns>
    public static async Task<BTwitterResponseResult<TwitterEntry>> DeleteStatus(string consumerKey, string consumerSecret, string token, string secret,
      string tweetId)
    {
      var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var status = await twitterCtx.DeleteTweetAsync(tweetId.ToUlongValue());
          result.DataResult = LinqToTwitterHelper.ConvertStatusToTwitterEntry(status);
          result.BisSuccess = true;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::DeleteStatus()", ex);
        result.ErrorMessage = ex.ToCompleteExceptionMessage();
      }
      return result;
    }

    /// <summary>
    ///   RemainingApi
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="sessionKey"></param>
    /// <param name="secret"></param>
    /// <param name="errorMsg"></param>
    /// <returns></returns>
    public static TwitterRateLimit RemainingApi(string consumerKey, string consumerSecret, string sessionKey,
      string secret,
      out string errorMsg)
    {
      return RemainingApi(consumerKey, consumerSecret, sessionKey, secret, out errorMsg,
        null);
    }

    /// <summary>
    ///   RemainingApi
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static TwitterRateLimit RemainingApi(string consumerKey, string consumerSecret, string token,
      string secret,
      out string errorMsg,
      WebProxy proxy)
    {
      TwitterRateLimit rateLimit = null;
      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var helpResult =
            (from help in twitterCtx.Help
              where help.Type == HelpType.RateLimits
              where help.Resources == "application"
              select help)
              .SingleOrDefault();

          if (helpResult != null)
            foreach (var category in helpResult.RateLimits)
            {
              if (category.Key.ToLower() != "application") continue;
              foreach (var limit in category.Value)
              {
                rateLimit = new TwitterRateLimit
                {
                  HourlyLimit = limit.Limit,
                  RemainingHits = limit.Remaining,
                  ResetTime = limit.Reset.ToConvertFromUniversalTimeToDate().ToLocalTime()
                };
              }
            }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::DeleteStatus()", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return rateLimit;
    }

    /// <summary>
    ///   AddTweet
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="message"></param>
    /// <param name="convertUrlsToTinyUrls"></param>
    /// <param name="replyID"></param>
    /// <returns></returns>
    public static async Task<BTwitterResponseResult<TwitterEntry>> AddTweet(string consumerKey, string consumerSecret, string token, string secret,string message,bool convertUrlsToTinyUrls,string replyID)
    {
      var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();

      try
      {
        var bisDirectMessage = (message.StartsWith("d ", StringComparison.CurrentCultureIgnoreCase));

        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var bContainsPhotoToUpload = message.Contains(photoStartTag);
          if (bContainsPhotoToUpload)
          {
            var statusMessage = GetMessageOnly(message);
            var imagePath = GetPhotoPathOnly(message);

            var resultStatus = await AddTweetWithPhoto(consumerKey, consumerSecret, token, secret, statusMessage, imagePath);
            if (!resultStatus.BisSuccess)
              throw new Exception(resultStatus.ErrorMessage);
            
            var status = resultStatus.DataResult;
            result.DataResult = LinqToTwitterHelper.ConvertStatusToTwitterEntry(status);
            result.BisSuccess = true;
            

          }
          else if (bisDirectMessage)
          {
            message = HttpUtility.UrlEncode(message);
            var users = message.Split('+');
            var user = users[1];
            message = "";
            var i = 0;
            foreach (var s in users)
            {
              if (i > 1)
                message += s;
              i++;
            }

            var directMessage = await twitterCtx.NewDirectMessageAsync(user, message);
            result.DataResult = LinqToTwitterHelper.ConvertDirectMessageToTwitterEntry(directMessage);
            result.BisSuccess = true;
          }
          else
          {
            var status = !string.IsNullOrEmpty(replyID)
              ? await twitterCtx.ReplyAsync(replyID.ToUlongValue(), message)
              : await twitterCtx.TweetAsync(message);
            result.DataResult = LinqToTwitterHelper.ConvertStatusToTwitterEntry(status);
            result.BisSuccess = true;
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::AddTweet()", ex);
        result.ErrorMessage = ex.ToCompleteExceptionMessage();
      }
      return result;
    }

    /// <summary>
    /// GetMessageOnly
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private static string GetMessageOnly(string message)
    {
      var startoffset = message.IndexOf(photoStartTag);
      var endoffset = message.IndexOf(photoEndTag);

      var tempMsg1 = message.Substring(0, startoffset);
      var tempMsg2 = message.Substring(endoffset + photoEndTag.Length);

      var resultMessage = tempMsg1 + tempMsg2;
      return resultMessage;
    }

    /// <summary>
    /// GetPhotoPathOnly
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private static string GetPhotoPathOnly(string message)
    {
      var startoffset = message.IndexOf(photoStartTag);
      var endoffset = message.IndexOf(photoEndTag);

      var tempMsg = message.Substring(startoffset + photoStartTag.Length,
        endoffset - (startoffset + photoStartTag.Length));
      var resultMessage = tempMsg;
      return resultMessage;
    }

    /// <summary>
    ///   GetUser
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userToGet"></param>
    /// <param name="count"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterEntry> GetUser(string consumerKey, string consumerSecret, string token, string secret,
      string userToGet,
      int count,
      out string errorMsg, WebProxy proxy)
    {
      List<TwitterEntry> entries = null;
      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var tweets =
            (from status in twitterCtx.Status
              where status.Type == StatusType.User
                    && status.ScreenName == userToGet
                    && status.Count == count
              select status).ToList();

          entries = tweets.Select(LinqToTwitterHelper.ConvertStatusToTwitterEntry).ToList();
          return entries;
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetUser", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return entries;
    }

    /// <summary>
    ///   GetDirectMessages
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="count"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterEntry> GetDirectMessages(string consumerKey, string consumerSecret, string token,
      string secret, int count, out string errorMsg, WebProxy proxy)
    {
      List<TwitterEntry> entries = null;
      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var directMsgs =
            (from dm in twitterCtx.DirectMessage
              where
                dm.Type == DirectMessageType.SentBy && dm.Count == count
              select dm).ToList();

          entries = directMsgs.Select(LinqToTwitterHelper.ConvertDirectMessageToTwitterEntry).ToList();

          var directMsgs2 =
            (from dm in twitterCtx.DirectMessage
              where
                dm.Type == DirectMessageType.SentTo && dm.Count == count
              select dm).ToList();

          entries.AddRange(directMsgs2.Select(LinqToTwitterHelper.ConvertDirectMessageToTwitterEntry).ToList());

          return entries;
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetDirectMessages", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return entries;
    }

    /// <summary>
    ///   verifies that account credentials are correct
    /// </summary>
    /// <param name="twitterCtx">TwitterContext</param>
    public static void VerifyAccountCredentials(TwitterContext twitterCtx)
    {
      var accounts =
        from acct in twitterCtx.Account
        where acct.Type == AccountType.VerifyCredentials
        select acct;

      try
      {
        var account = accounts.SingleOrDefault();
        var user = account.User;
        var tweet = user.Status ?? new Status();
        Console.WriteLine("User (#{0}): {1}\nTweet: {2}\nTweet ID: {3}\n", user.UserID, user.ScreenName, tweet.Text, tweet.StatusID);
        Console.WriteLine("Account credentials are verified.");
      }
      catch (WebException wex)
      {
        Console.WriteLine("Twitter did not recognize the credentials. Response from Twitter: {0}", wex.Message);
      }
    }

    public static List<TwitterEntry> GetFeedFromUrl(string consumerKey, string consumerSecret, string sessionKey,
      string secret, string uri,
      int count,
      int pageNb,
      bool isDirectMessages,
      out string errorMsg, WebProxy proxy)
    {
      errorMsg = string.Empty;
      var entries = new List<TwitterEntry>();

      try
      {
        var feedXML = OAuthWebRequest(Method.GET, string.Format("{0}?count={1}&pageNb={2}.xml", uri, count, pageNb),
          String.Empty, consumerKey, consumerSecret, sessionKey, secret, proxy);

        var feedXML2 = XDocument.Parse(feedXML);

        if (!isDirectMessages)
        {
          entries = TwitterLib.LoadFeed(feedXML2);
        }
        else
        {
          entries = TwitterLib.LoadDirectMessage(feedXML2);
        }

        return entries;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib -> GetFeedFromUrl() -> ",
          ex);
      }
      return entries;
    }

    public static List<TwitterEntry> GetFeedFromUrl(string uri,
      int count,
      int pageNb,
      bool isDirectMessages,
      out string errorMsg, string consumerKey, string consumerSecret,
      string token, string tokenSecret, WebProxy proxy)
    {
      errorMsg = string.Empty;
      var entries = new List<TwitterEntry>();

      try
      {
        var feedXML = OAuthWebRequest(Method.GET, string.Format("{0}?count={1}&pageNb={2}.xml", uri, count, pageNb),
          String.Empty, consumerKey, consumerSecret, token, tokenSecret, proxy);

        var feedXML2 = XDocument.Parse(feedXML);

        if (!isDirectMessages)
          entries = TwitterLib.LoadFeed(feedXML2);
        else
          entries = TwitterLib.LoadDirectMessage(feedXML2);

        return entries;
      }
      catch (Exception ex)
      {
        errorMsg = ex.Message;
        TraceHelper.Trace("TwitterLib -> GetFeedFromUrl() -> ",
          ex);
      }
      return entries;
    }

    /// <summary>
    ///   Block
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userToBlock"></param>
    public static async Task<BTwitterResponseResult<User>> Block(string consumerKey, string consumerSecret, string token, string secret,string userToBlock)
    {
      var result = BTwitterResponseResult<User>.CreateInstance();
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          result.DataResult = await twitterCtx.CreateBlockAsync(0, userToBlock, true);
          result.BisSuccess = true;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::Block()", ex);
        result.ErrorMessage = ex.ToCompleteExceptionMessage();
      }
      return result;
    }

    /// <summary>
    ///   ReportSpam
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userToBlock"></param>
    public static async Task<BTwitterResponseResult<User>> ReportSpam(string consumerKey, string consumerSecret, string token, string secret,string userToBlock)
    {
      var result = BTwitterResponseResult<User>.CreateInstance();
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          result.DataResult = await twitterCtx.ReportSpam(null, userToBlock);
          result.BisSuccess = true;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::ReportSpam()", ex);
        result.ErrorMessage = ex.ToCompleteExceptionMessage();
      }
      return result;
    }

    /// <summary>
    ///   Retweet
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="retweetId"></param>
    /// <returns></returns>
    public static async Task<BTwitterResponseResult<TwitterEntry>> Retweet(string consumerKey,
      string consumerSecret,
      string token,
      string secret,
      string retweetId)
    {
      var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var status = await twitterCtx.RetweetAsync(retweetId.ToUlongValue());
          var entry = LinqToTwitterHelper.ConvertStatusToTwitterEntry(status);
          result.DataResult = entry;
          result.BisSuccess = true;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib::DeleteStatus()", ex);
        result.ErrorMessage = ex.ToCompleteExceptionMessage();
      }
      return result;
    }

    ////public static async Task<TwitterEntry> RetweetOld(string consumerKey,
    ////  string consumerSecret,
    ////  string token,
    ////  string secret,
    ////  string retweetId,
    ////  WebProxy proxy)
    ////{
    ////  var entry = new TwitterEntry();
    ////  try
    ////  {
    ////    using (var twitterCtx = Context.CreateTwitterContext(token, secret))
    ////    {
    ////      var status = await twitterCtx.RetweetAsync(retweetId.ToUlongValue());
    ////      entry = LinqToTwitterHelper.ConvertStatusToTwitterEntry(status);
    ////    }
    ////  }
    ////  catch (Exception ex)
    ////  {
    ////    TraceHelper.Trace("TwitterLib::DeleteStatus()", ex);
    ////    entry.ErrorMsg = ex.ToCompleteExceptionMessage();
    ////  }
    ////  return entry;
    ////}

    /// <summary>
    ///   GetTweetInfo
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<BTwitterResponseResult<List<TwitterEntry>>> GetTweetInfo(string consumerKey, string consumerSecret, string token,
      string secret,
      string id
      )
    {
      var result = BTwitterResponseResult<List<TwitterEntry>>.CreateInstance();
      var friendsResult = new List<TwitterEntry>();

      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          //var friendTweets =
          //  from tweet in twitterCtx.Status
          //  where tweet.Type == StatusType.Conversation &&
          //        tweet.ID == id.ToUlongValue()
          //  select tweet;

          var friendTweets = await
             (from tweet in twitterCtx.Status
              where tweet.Type == StatusType.Lookup &&
                    tweet.TweetIDs == id
              select tweet)
             .ToListAsync();

          friendsResult = friendTweets.Select(LinqToTwitterHelper.ConvertStatusToTwitterEntry).ToList();
          result.BisSuccess = true;
          result.DataResult = friendsResult;
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetTweetInfo", ex);
        result.ErrorMessage = ex.ToCompleteExceptionMessage();
      }
      return result;
    }

    /// <summary>
    ///   GetHomeTimeline
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="count"></param>
    /// <param name="maxId"></param>
    /// <param name="sinceId"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterEntry> GetHomeTimeline(string consumerKey, string consumerSecret, string token,string secret,int count,long maxId,long sinceId,out string errorMsg, WebProxy proxy)
    {
      List<TwitterEntry> entries = null;
      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var maxid = decimal.Parse(maxId.ToString());
          var sinceid = decimal.Parse(sinceId.ToString());

          List<Status> tweets = null;
          if (maxid > 0 && sinceid > 0)
          {
            tweets =
              (from status in twitterCtx.Status
                where status.Type == StatusType.Home
                      && status.SinceID == sinceid && status.MaxID == maxid
                  //&& status.IncludeEntities && status.IncludeContributorDetails
                      && status.Count == count
                select status).ToList();
          }
          else if (maxid > 0 && sinceid <= 0)
          {
            tweets =
              (from status in twitterCtx.Status
                where status.Type == StatusType.Home
                  //&& status.IncludeEntities && status.IncludeContributorDetails
                      && status.MaxID == maxid
                      && status.Count == count
                select status).ToList();
          }
          else if (maxid <= 0 && sinceid > 0)
          {
            tweets =
              (from status in twitterCtx.Status
                where status.Type == StatusType.Home
                  //&& status.IncludeEntities && status.IncludeContributorDetails
                      && status.SinceID == sinceid
                      && status.Count == count
                select status).ToList();
          }
          else if (maxid <= 0 && sinceid <= 0)
          {
            //if (!Debugger.IsAttached) Debugger.Launch();

            tweets =
              (from status in twitterCtx.Status
                where status.Type == StatusType.Home
                  // && status.IncludeEntities //&& status.IncludeContributorDetails
                      && status.Count == count
                select status).ToList();
          }
          entries = tweets.Select(LinqToTwitterHelper.ConvertStatusToTwitterEntry).ToList();
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetHomeTimeline", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return entries;
    }

    /// <summary>
    ///   Web Request Wrapper
    /// </summary>
    /// <param name="method">Http Method</param>
    /// <param name="url">Full url to the web resource</param>
    /// <param name="postData">Data to post in querystring format</param>
    /// <returns>The web server response.</returns>
    public static string WebRequest(Method method, string url, string postData, WebProxy proxy)
    {
      StreamWriter requestWriter;

      var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
      if (proxy != null)
      {
        webRequest.Proxy = proxy;
      }
      if (webRequest != null)
      {
        webRequest.Method = method.ToString();
        webRequest.ServicePoint.Expect100Continue = false;

        if (method == Method.POST)
        {
          webRequest.ContentType = "application/x-www-form-urlencoded";
          webRequest.ContentLength = postData.Length;

          //POST the data.
          requestWriter = new StreamWriter(webRequest.GetRequestStream());
          try
          {
            requestWriter.Write(postData);
          }
          finally
          {
            requestWriter.Close();
          }
        }
      }
      return WebResponseGet(webRequest);
    }

    /// <summary>
    ///   Process the web response.
    /// </summary>
    /// <param name="webRequest">The request object.</param>
    /// <returns>The response data.</returns>
    public static string WebResponseGet(HttpWebRequest webRequest)
    {
      TraceHelper.Trace("TwitterLib", webRequest.RequestUri.ToString());
      StreamReader responseReader = null;
      var responseData = "";

      try
      {
        responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
        responseData = responseReader.ReadToEnd();

        //TraceHelper.Trace("TwitterLib", webRequest.RequestUri.ToString());
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("TwitterLib", ex.Message);
      }
      finally
      {
        webRequest.GetResponse().GetResponseStream().Close();
        if (responseReader != null) responseReader.Close();
      }

      return responseData;
    }

    public static TwitterContext GetTwitterContext(TwitterCredentials twitterCredentials)
    {
      if (_twitterContext == null)
      {
        // configure the OAuth object
        var auth = new SingleUserAuthorizer
        {
          CredentialStore = new InMemoryCredentialStore()
          {
            ConsumerKey = twitterCredentials.ConsumerKey,
            ConsumerSecret = twitterCredentials.ConsumerSecret,
            OAuthToken = twitterCredentials.OAuthToken,
            OAuthTokenSecret = twitterCredentials.OAuthSecret
          }
        };
        _twitterContext = new TwitterContext(auth);
      }
      return _twitterContext;
    }

    /// <summary>
    ///   PostTweetWithMedia
    /// </summary>
    /// <param name="twitterContext"></param>
    /// <param name="status"></param>
    /// <param name="imageFilePath"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static async Task<BTwitterResponseResult<TwitterEntry>> PostTweetWithMedia(TwitterContext twitterContext, string status, string imageFilePath,string fileName)
    {
      const bool possiblySensitive = false;
      var latitude = TwitterContext.NoCoordinate; //37.78215m;
      var longitude = TwitterContext.NoCoordinate; // -122.40060m;
      const bool displayCoordinates = false;
      
      var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();

      try
      {
        var imageBytes = File.ReadAllBytes(imageFilePath);
        var imageUploadTasks =
          new List<Task<Media>>
          {
                    twitterContext.UploadMediaAsync(imageBytes, "image/png"),
          };

        await Task.WhenAll(imageUploadTasks);
        var mediaIds =
            (from tsk in imageUploadTasks
             select tsk.Result.MediaID)
            .ToList();

        var tweet =
          await
            twitterContext.TweetAsync(status, mediaIds);

        result.DataResult = LinqToTwitterHelper.ConvertStatusToTwitterEntry(tweet);
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "PostTweetWithMedia", ex);
      }
      return result;
    }

    #region Lists

    /// <summary>
    ///   CreateNewList
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="login"></param>
    /// <param name="mode"></param>
    /// <param name="name"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static async Task<string> CreateNewList(string consumerKey, string consumerSecret, string token, string secret,
      string login,
      string mode, string name,
      WebProxy proxy)
    {
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var list = await twitterCtx.CreateListAsync(name, mode, name);
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "CreateNewList", ex);
      }
      return null;
    }

    //public static string ModifyList(string consumerKey, string consumerSecret, string token, string secret,
    //                                string login, string pwdHash, string hashKey, string mode, string name,
    //                                string oldName, WebProxy proxy)
    //{

    //  var listResults = new List<TwitterList>();
    //  try
    //  {
    //    using (var twitterCtx = Context.CreateTwitterContext(token, secret))
    //    {
    //      var list = twitterCtx.UpdateList(null, "test", null, "Test List", "Linq2Tweeter", "public", "This is a test2");        
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    BLogManager.LogEntry(APPNAME, "GetList", ex);
    //  }
    //  return null;
    //}

    public static List<TwitterList> GetListOwn(string consumerKey, string consumerSecret, string token,
      string secret, string login,
      out string errorMsg,
      WebProxy proxy)
    {
      var listResults = new List<TwitterList>();

      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var lists =
            (from list in twitterCtx.List
              where list.Type == ListType.List && list.ScreenName == login
              select list)
              .ToList();

          listResults = lists.Select(LinqToTwitterHelper.ConvertListToTwitterList).ToList();
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetListOwn", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return listResults;
    }

    /// <summary>
    ///   GetList
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="login"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterList> GetList(string consumerKey, string consumerSecret, string token, string secret,
      string login, out string errorMsg,
      WebProxy proxy)
    {
      var listResults = new List<TwitterList>();

      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var lists =
            (from list in twitterCtx.List
              where list.Type == ListType.Subscriptions &&
                    list.ScreenName == login && list.IncludeEntities
              select list)
              .ToList();

          listResults = lists.Select(LinqToTwitterHelper.ConvertListToTwitterList).ToList();
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetList", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return listResults;
    }

    /// <summary>
    ///   GetListMembership
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="login"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterList> GetListMembership(string consumerKey, string consumerSecret, string token,
      string secret, string login,
      out string errorMsg, WebProxy proxy)
    {
      var listResults = new List<TwitterList>();

      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var lists =
            (from list in twitterCtx.List
              where list.Type == ListType.Memberships &&
                    list.ScreenName == login
              select list)
              .ToList();

          listResults = lists.Select(LinqToTwitterHelper.ConvertListToTwitterList).ToList();
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetListMembership", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return listResults;
    }

    /// <summary>
    ///   GetListStatuse
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userList"></param>
    /// <param name="listId"></param>
    /// <param name="nbGet"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterEntry> GetListStatuse(string consumerKey, string consumerSecret, string token,
      string secret,
      string userList,
      string listId, int nbGet, out string errorMsg, WebProxy proxy)
    {
      
      var listEntries = new List<TwitterEntry>();
      
      errorMsg = null;

      if (listId == "0") return listEntries;
      
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var listResponse =
            (from list in twitterCtx.List
              where list.Type == ListType.Statuses &&
                    list.OwnerScreenName == userList &&
                    list.Count == nbGet && list.ListID == listId.ToUlongValue()
              select list)
              .First();

          var newStatuses = listResponse.Statuses;

          listEntries = newStatuses.Select(LinqToTwitterHelper.ConvertStatusToTwitterEntry).ToList();
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetListStatuse", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return listEntries;
    }

    public static bool AddToList(string consumerKey, string consumerSecret, string sessionKey, string secret,
      string userList, string listSlug,
      string userID, WebProxy proxy)
    {
      // Create the web request
      var url = string.Format("http://api.twitter.com/1.1/{0}/{1}/members.xml", userList, listSlug);

      // Set values for the request back
      //request.ContentType = "application/x-www-form-urlencoded";
      var sourceParam = string.Format("id={0}&list_id={1}", HttpUtility.UrlEncode(userID), listSlug);

      var feedXML =
        XDocument.Parse(OAuthWebRequest(Method.POST, url, sourceParam, consumerKey, consumerSecret, sessionKey, secret,
          proxy));

      // Get the response stream
      return feedXML != null;
    }

    /// <summary>
    ///   GetListMembers
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userList"></param>
    /// <param name="listSlug"></param>
    /// <param name="errorMsg"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static List<TwitterUser> GetListMembers(string consumerKey, string consumerSecret, string token,
      string secret,
      string userList,
      string listSlug, out string errorMsg, WebProxy proxy)
    {
      var listResults = new List<TwitterUser>();

      errorMsg = null;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var lists =
            (from list in twitterCtx.List
              where list.Type == ListType.Members &&
                    list.OwnerScreenName == userList
              select list)
              .First();

          listResults = lists.Users.Select(LinqToTwitterHelper.ConvertUserToTwitterUser).ToList();
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "GetListMembers", ex);
        errorMsg = ex.ToCompleteExceptionMessage();
      }
      return listResults;
    }

    /// <summary>
    ///   DeleteListMembers
    /// </summary>
    /// <param name="consumerKey"></param>
    /// <param name="consumerSecret"></param>
    /// <param name="token"></param>
    /// <param name="secret"></param>
    /// <param name="userList"></param>
    /// <param name="listSlug"></param>
    /// <param name="userId"></param>
    /// <param name="proxy"></param>
    /// <returns></returns>
    public static bool DeleteListMembers(string consumerKey, string consumerSecret, string token, string secret,
      string userList, string listSlug,
      string userId, WebProxy proxy)
    {
      var bResult = false;
      try
      {
        using (var twitterCtx = Context.CreateTwitterContext(token, secret))
        {
          var list = twitterCtx.DeleteMemberFromListAsync(userList.ToUlongValue(), null, 0, listSlug, userId.ToUlongValue(),
            null);
          bResult = list != null;
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "DeleteListMembers", ex);
      }
      return bResult;
    }

    #endregion
  }
}
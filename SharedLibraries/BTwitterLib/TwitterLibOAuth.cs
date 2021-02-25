//#region

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Xml.Linq;
//using Sobees.Library.BGenericLib;
//using Sobees.Library.BTwitterLib.Helpers;
//using Sobees.Tools.Logging;
//using Media = LinqToTwitter.Media;
//using User = LinqToTwitter.User;
//#if !SILVERLIGHT
//using BUtility;
//using LinqToTwitter;
//#endif

//#endregion

//namespace Sobees.Library.BTwitterLib
//{
//  public class TwitterLibOAuth : OAuth
//  {
//    #region Method enum

//    public enum Method
//    {
//      GET,
//      POST
//    }

//    #endregion

//    public static readonly string AccessToken = TwitterResources.ACCESS_TOKEN;
//    public static readonly string Authorize = TwitterResources.AUTHORIZE;
//    public static readonly string RequestToken = TwitterResources.REQUEST_TOKEN;
//    private static string _callBackUrl = "oob";

//    private static string _consumerKey = "";
//    private static string _consumerSecret = "";
//    private static string _oauthVerifier = "";
//    private static string _token = "";
//    private static string _tokenSecret = "";

//    #region Properties

//    public static string OAuthVerifier
//    {
//      get { return _oauthVerifier; }
//      set { _oauthVerifier = value; }
//    }

//    public static string CallBackUrl
//    {
//      get { return _callBackUrl; }
//      set { _callBackUrl = value; }
//    }

//    #endregion

//    ///// <summary>
//    /////   Get the link to Twitter's authorization page for this application.
//    ///// </summary>
//    ///// <returns>The url with a valid request token, or a null string.</returns>
//    //public static string AuthorizationLinkGet(string consumerKey, string consumerSecret, string callbackUrl,
//    //                                          WebProxy proxy)
//    //{
//    //  TraceHelper.Trace("TwitterLib AuthorizationLinkGet", "Started");
//    //  CallBackUrl = callbackUrl;

//    //  OAuthVerifier = "";
//    //  string ret = null;
//    //  var response = OAuthWebRequest(Method.GET, RequestToken, String.Empty, consumerKey, consumerSecret,
//    //                                 string.Empty, string.Empty, proxy);
//    //  //TraceHelper.Trace("TwitterLib AuthorizationLinkGet", response);
//    //  if (response.Length > 0)
//    //  {
//    //    //response contains token and token secret.  We only need the token.
//    //    var qs = HttpUtility.ParseQueryString(response);

//    //    if (qs["oauth_callback_confirmed"] != null)
//    //    {
//    //      if (qs["oauth_callback_confirmed"] != "true")
//    //      {
//    //        throw new Exception("OAuth callback not confirmed.");
//    //      }
//    //    }

//    //    if (qs["oauth_token"] != null)
//    //    {
//    //      ret = Authorize + "?oauth_token=" + qs["oauth_token"];
//    //    }
//    //  }
//    //  TraceHelper.Trace("TwitterLib AuthorizationLinkGet", ret);
//    //  return ret;
//    //}

//    ///// <summary>
//    /////   Exchange the request token for an access token.
//    ///// </summary>
//    ///// <param name="consumerSecret"></param>
//    ///// <param name="authToken">The oauth_token is supplied by Twitter's authorization page following the callback.</param>
//    ///// <param name="oauthVerifier"></param>
//    ///// <param name="token"></param>
//    ///// <param name="tokenSecret"></param>
//    ///// <param name="consumerKey"></param>
//    //public static string AccessTokenGet(string consumerKey, string consumerSecret, string authToken,
//    //                                    string oauthVerifier,
//    //                                    string token, out string tokenSecret, WebProxy proxy)
//    //{
//    //  tokenSecret = "";
//    //  OAuthVerifier = oauthVerifier;

//    //  var response = OAuthWebRequest(Method.GET, AccessToken, String.Empty, consumerKey, consumerSecret, string.Empty,
//    //                                 string.Empty, proxy);
//    //  //TraceHelper.Trace("Twitterlib", response);
//    //  string Token = null;
//    //  if (response.Length > 0)
//    //  {
//    //    //Store the Token and Token Secret
//    //    var qs = HttpUtility.ParseQueryString(response);
//    //    if (qs["oauth_token"] != null)
//    //    {
//    //      //TraceHelper.Trace("Token", qs["oauth_token"]);
//    //      Token = qs["oauth_token"];
//    //    }
//    //    if (qs["oauth_token_secret"] != null)
//    //    {
//    //      //TraceHelper.Trace("TokenSecret", qs["oauth_token_secret"]);
//    //      tokenSecret = string.Format("{0}name={1}", qs["oauth_token_secret"], qs["screen_name"]);
//    //    }
//    //  }
//    //  return Token;
//    //}

//#if !SILVERLIGHT
//    /// <summary>
//    /// </summary>
//    /// <param name="consumerKey"></param>
//    /// <param name="consumerSecret"></param>
//    /// <param name="username"></param>
//    /// <param name="pincode"></param>
//    /// <param name="tokenSecret"></param>
//    /// <param name="proxy"></param>
//    /// <returns></returns>
//    private static PinAuthorizer _pinAuthorizer;
//#endif

//    private const string APPNAME = "TwitterLibOAuth";

////    public static string ConnectXAuth(string consumerKey, string consumerSecret, string username, string token,
////                                      string secret,
////                                      out string tokenSecret, WebProxy proxy)
////    {
////      tokenSecret = string.Empty;
////      OAuthVerifier = string.Empty;

////#if SILVERLIGHT
////      string response = OAuthWebRequest(Method.POST, "https://api.twitter.com/oauth/access_token",
////                                        string.Format(
////                                          "x_auth_username={0}&x_auth_password={1}&x_auth_mode=client_auth", username,
////                                         UrlEncodePassword(userpassword)), consumerKey, consumerSecret, string.Empty, string.Empty, proxy);
////#endif
////#if !SILVERLIGHT
////      var response = string.Empty;
////      if (string.IsNullOrEmpty(token))
////      {
////      }
////      else
////      {
////        // configure the OAuth object
////        var auth = new SingleUserAuthorizer
////                     {
////                       CredentialStore = new InMemoryCredentialStore()
////                                       {
////                                         ConsumerKey = consumerKey,
////                                         ConsumerSecret = consumerSecret,
////                                         OAuthToken = token,
////                                         AccessToken = secret
////                                       }
////                     };

////        using (var twitterCtx = new TwitterContext(auth, "https://api.twitter.com/1.1/", "https://search.twitter.com/"))
////        {
////          var users =
////            (from tweet in twitterCtx.User
////             where tweet.Type == UserType.Search &&
////                   tweet.ScreenName == username
////             select tweet)
////              .ToList();
////          if (users.Count > 0)
////            return token;
////        }
////      }
////#endif

////      //TraceHelper.Trace("Twitterlib", response);
////      string Token = null;
////      if (response.Length > 0)
////      {
////        //Store the Token and Token Secret
////        var qs = HttpUtility.ParseQueryString(response);
////        if (qs["oauth_token"] != null)
////        {
////          //TraceHelper.Trace("Token", qs["oauth_token"]);
////          Token = qs["oauth_token"];
////        }
////        if (qs["oauth_token_secret"] != null)
////        {
////          //TraceHelper.Trace("TokenSecret", qs["oauth_token_secret"]);
////          tokenSecret = qs["oauth_token_secret"];
////        }
////      }
////      return Token;

////      return null;
////    }


//    /// <summary>
//    ///   Submit a web request using oAuth.
//    /// </summary>
//    /// <param name="method">GET or POST</param>
//    /// <param name="url">The full url, including the querystring.</param>
//    /// <param name="postData">Data to post (querystring format)</param>
//    /// <param name="consumerKey"></param>
//    /// <param name="consumerSecret"></param>
//    /// <param name="token"></param>
//    /// <param name="tokenSecret"></param>
//    /// <returns>The web server response.</returns>
//    public static string OAuthWebRequest(Method method, string url, string postData, string consumerKey,
//                                         string consumerSecret, string token, string tokenSecret, WebProxy proxy)
//    {
//      string outUrl;
//      string querystring;


//      //Setup postData for signing.
//      //Add the postData to the querystring.
//      if (method == Method.POST)
//      {
//        if (postData.Length > 0)
//        {
//          //Decode the parameters and re-encode using the oAuth UrlEncode method.
//          var qs = HttpUtility.ParseQueryString(postData);
//          postData = "";
//          foreach (var key in qs.AllKeys)
//          {
//            if (postData.Length > 0)
//              postData += "&";

//            //qs[key] = HttpUtility.UrlDecode(qs[key]);
//            //qs[key] = Uri.EscapeDataString(qs[key]);
//            //qs[key] = UrlEncode(qs[key]);

//            //if (!key.Equals("x_auth_password"))

//            if (!key.Equals("x_auth_password"))
//            {
//              //qs[key] = HttpUtility.UrlDecode(qs[key]);
//              qs[key] = UrlEncode(qs[key]);
//            }
//            else
//            {
//              qs[key] = UrlEncode(qs[key]);
//            }

//            postData += string.Format("{0}={1}", key, qs[key]);
//          }
//          url += url.IndexOf("?") > 0 ? "&" : "?";
//          url += postData;
//        }
//      }

//      var uri = new Uri(url);

//      var nonce = GenerateNonce();
//      var timeStamp = GenerateTimeStamp();

//      //Generate Signature
//      var sig = GenerateSignature(uri,
//                                  consumerKey,
//                                  consumerSecret,
//                                  token,
//                                  tokenSecret,
//                                  CallBackUrl,
//                                  OAuthVerifier,
//                                  method.ToString(),
//                                  timeStamp,
//                                  nonce,
//                                  out outUrl,
//                                  out querystring);

//      querystring += string.Format("&oauth_signature={0}", HttpUtility.UrlEncode(sig));

//      //Convert the querystring to postData
//      if (method == Method.POST)
//      {
//        postData = querystring;
//        querystring = "";
//      }

//      if (querystring.Length > 0)
//        outUrl += "?";


//      var ret = WebRequest(method, outUrl + querystring, postData, proxy);
//      BLogManager.LogEntry(APPNAME, "OAuthWebRequest", string.Format("\n\r{0}{1}", outUrl, querystring),true);
//      return ret;
//    }

//    ///// <summary>
//    ///// </summary>
//    ///// <param name="consumerKey"></param>
//    ///// <param name="consumerSecret"></param>
//    ///// <param name="sessionKey"></param>
//    ///// <param name="secret"></param>
//    ///// <param name="page"></param>
//    ///// <param name="errorMsg"></param>
//    ///// <param name="nextCursor"></param>
//    ///// <returns></returns>
//    //public static List<TwitterUser> GetFriendsCursor(string consumerKey, string consumerSecret,
//    //                                                 string sessionKey, string secret, string screenname, string page,
//    //                                                 out string errorMsg, out string nextCursor, WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  var friendsResult = new List<TwitterUser>();

//    //  try
//    //  {
//    //    var m_pinAuth = new SingleUserAuthorizer
//    //                      {
//    //                        CredentialStore = new InMemoryCredentialStore()
//    //                                        {
//    //                                          ConsumerKey = consumerKey,
//    //                                          ConsumerSecret = consumerSecret,
//    //                                          OAuthToken = sessionKey,
//    //                                          AccessToken = secret
//    //                                        }
//    //                      };

//    //    var twitterCtx = new TwitterContext(m_pinAuth, "https://api.twitter.com/1.1/", "https://search.twitter.com/");
//    //    //var tweets = from tweet in m_twitterCtx.Status where tweet.Type == StatusType.Friends select tweet;

//    //    const int pageNumber = 1;
//    //    nextCursor = "-1";


//    //    var friendsList = new List<User>();

//    //    var friendList =
//    //      (from friend in twitterCtx.SocialGraph
//    //       where friend.Type == SocialGraphType.Friends &&
//    //             friend.ScreenName == screenname
//    //       select friend)
//    //        .SingleOrDefault();


//    //    BLogManager.LogEntry(string.Format("Page #{0} has {1} users.", pageNumber, friendsList.Count()), true);

//    //    for (var i = 0; i < friendList.IDs.Count(); i += 100)
//    //    {
//    //      var nbIdsToTake = 100;
//    //      var usernames = String.Join(",", friendList.IDs.Skip(i).Take(nbIdsToTake).ToArray());

//    //      var users =
//    //        (from user in twitterCtx.User
//    //         where user.Type == UserType.Lookup &&
//    //               user.UserID == usernames
//    //         select user)
//    //          .ToList();

//    //      friendsList.AddRange(users);
//    //    }

//    //    friendsResult = friendsList.Select(LinqToTwitterHelper.ConvertUserToTwitterUser).ToList();

//    //    //var feedXML = OAuthWebRequest(Method.GET, string.Format("{0}?cursor={1}", TwitterResources.friends, page), String.Empty, consumerKey, consumerSecret, sessionKey, secret,proxy);
//    //    //var f = TwitterLib.LoadUser(XDocument.Parse(feedXML));
//    //    //if (f.Count > 0)
//    //    //{
//    //    //  friends.AddRange(f);
//    //    //}
//    //    //var tempStartCursor = feedXML.ToLower().IndexOf("<next_cursor>");
//    //    //var tempNextCursor = feedXML.Substring(tempStartCursor + 13);
//    //    //var tempEndCursor = tempNextCursor.ToLower().IndexOf("</next_cursor>");
//    //    //nextCursor = tempNextCursor.Substring(0, tempEndCursor);
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    errorMsg = ex.Message;
//    //    TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
//    //                      ex);
//    //    nextCursor = "-1";
//    //  }

//    //  nextCursor = null;
//    //  return friendsResult;
//    //}

//    //public static List<TwitterUser> GetFriends(string consumerKey, string consumerSecret, string login,
//    //                                           string sessionKey, string secret, int page,
//    //                                           out string errorMsg, WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  var friends = new List<TwitterUser>();

//    //  try
//    //  {
//    //    var feedXML = OAuthWebRequest(Method.GET, string.Format("{0}?page={1}", TwitterResources.friends, page),
//    //                                  String.Empty, consumerKey, consumerSecret, secret, sessionKey, proxy);

//    //    var f = TwitterLib.LoadUser(XDocument.Parse(feedXML));

//    //    if (f.Count > 0)
//    //    {
//    //      friends.AddRange(f);
//    //    }
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    errorMsg = ex.Message;
//    //    TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
//    //                      ex);
//    //  }
//    //  return friends;
//    //}

//    //public static TwitterUser GetUserInfo(string consumerKey, string consumerSecret, string sessionKey,
//    //                                      string secret, string userToGet,
//    //                                      out string errorMsg, WebProxy proxy)
//    //{
//    //  return GetUserInfo(consumerKey, consumerSecret, sessionKey, secret, userToGet, false, out errorMsg, proxy);
//    //}

//    //public static List<TwitterEntry> GetReplies(string consumerKey, string consumerSecret, string sessionKey,
//    //                                            string secret,
//    //                                            int count,
//    //                                            int pageNb,
//    //                                            out string errorMsg, WebProxy proxy)
//    //{
//    //  return GetFeedFromUrl(TwitterResources.replies,
//    //                        count,
//    //                        pageNb,
//    //                        false,
//    //                        out errorMsg, consumerKey, consumerSecret, sessionKey, secret, proxy);
//    //}

//    //public static List<TwitterEntry> GetFavorites(string consumerKey, string consumerSecret, string sessionKey,
//    //                                              string secret,
//    //                                              string userToGet,
//    //                                              int count,
//    //                                              int pageNb,
//    //                                              out string errorMsg, WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  if (string.IsNullOrEmpty(userToGet))
//    //    return new List<TwitterEntry>();

//    //  return GetFeedFromUrl(string.Format("{0}{1}.xml",
//    //                                      TwitterResources.favorites,
//    //                                      userToGet),
//    //                        count,
//    //                        pageNb,
//    //                        false,
//    //                        out errorMsg, consumerKey, consumerSecret, sessionKey, secret, proxy);
//    //}

//    //public static List<TwitterEntry> GetFriendsTimeline(string consumerKey, string consumerSecret, string sessionKey,
//    //                                                    string secret, string login,
//    //                                                    int count,
//    //                                                    int pageNb,
//    //                                                    out string errorMsg, WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  var friendsResult = new List<TwitterEntry>();

//    //  try
//    //  {
//    //    var m_pinAuth = new SingleUserAuthorizer
//    //                      {
//    //                        CredentialStore = new InMemoryCredentialStore()
//    //                                        {
//    //                                          ConsumerKey = consumerKey,
//    //                                          ConsumerSecret = consumerSecret,
//    //                                          OAuthToken = sessionKey,
//    //                                          AccessToken = secret
//    //                                        }
//    //                      };

//    //    var twitterCtx = new TwitterContext(m_pinAuth, "https://api.twitter.com/1.1/", "https://search.twitter.com/");
//    //    //var tweets = from tweet in m_twitterCtx.Status where tweet.Type == StatusType.Friends select tweet;

//    //    var friendTweets =
//    //      from tweet in twitterCtx.Status
//    //      where tweet.Type == StatusType.User && tweet.Count == count
//    //      select tweet;

//    //    friendsResult = friendTweets.Select(LinqToTwitterHelper.ConvertStatusToTwitterEntry).ToList();
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    BLogManager.LogEntry(APPNAME, "GetFriendsTimeline", ex);
//    //    errorMsg = ex.Message;
//    //  }


//    //  return friendsResult;
//    //}

//    //public static TwitterUser GetUserInfo(string consumerKey, string consumerSecret, string sessionKey,
//    //                                      string secret, string userToGet,
//    //                                      bool bigImage,
//    //                                      out string errorMsg, WebProxy proxy)
//    //{
//    //  errorMsg = null;

//    //  try
//    //  {
//    //    var feedXML = OAuthWebRequest(Method.GET, string.Format("{0}{1}.xml",
//    //                                                            TwitterResources.extendedUserInfo,
//    //                                                            userToGet), String.Empty, consumerKey, consumerSecret,
//    //                                  sessionKey, secret, proxy);

//    //    var users = TwitterLib.LoadUser(XDocument.Parse(feedXML));
//    //    if (users == null ||
//    //        users.Count < 1) return null;
//    //    var user = users.First();
//    //    if (user == null) return null;

//    //    if (bigImage && !string.IsNullOrEmpty(user.ProfileImgUrl) && user.ProfileImgUrl.Contains("_normal"))
//    //      user.ProfileImgUrl = user.ProfileImgUrl.Replace("_normal",
//    //                                                      string.Empty);

//    //    return user;
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
//    //                      ex);
//    //  }
//    //  return null;
//    //}

//    ///// <summary>
//    /////   UploadPhoto
//    ///// </summary>
//    ///// <param name="consumerKey"></param>
//    ///// <param name="consumerSecret"></param>
//    ///// <param name="sessionKey"></param>
//    ///// <param name="secret"></param>
//    ///// <param name="status"></param>
//    ///// <param name="imagepath"></param>
//    ///// <param name="errorMsg"></param>
//    ///// <param name="proxy"></param>
//    ///// <returns></returns>
//    //public static Status AddTweetWithPhoto(string consumerKey, string consumerSecret, string sessionKey,
//    //                                       string secret, string status, string imagepath,
//    //                                       out string errorMsg, WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  Status tweet = null;

//    //  try
//    //  {
//    //    var m_pinAuth = new SingleUserAuthorizer
//    //                      {
//    //                        CredentialStore = new InMemoryCredentialStore()
//    //                                        {
//    //                                          ConsumerKey = consumerKey,
//    //                                          ConsumerSecret = consumerSecret,
//    //                                          OAuthToken = sessionKey,
//    //                                          AccessToken = secret
//    //                                        }
//    //                      };

//    //    var twitterCtx = new TwitterContext(m_pinAuth, "https://api.twitter.com/1.1/", "https://search.twitter.com/");

//    //    const bool possiblySensitive = false;
//    //    const decimal latitude = StatusExtensions.NoCoordinate; //37.78215m;
//    //    const decimal longitude = StatusExtensions.NoCoordinate; // -122.40060m;
//    //    const bool displayCoordinates = false;

//    //    var mediaItems =
//    //      new List<Media>
//    //        {
//    //          new Media
//    //            {
//    //              Data = Utilities.GetFileBytes(imagepath),
//    //              FileName = "200xColor_2.png",
//    //              ContentType = MediaContentType.Png
//    //            }
//    //        };

//    //    tweet = twitterCtx.TweetWithMedia(
//    //      status, possiblySensitive, latitude, longitude,
//    //      null, displayCoordinates, mediaItems, null);
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    BLogManager.LogEntry(APPNAME, "UploadPhoto", ex);
//    //    errorMsg = ex.Message;
//    //  }
//    //  return tweet;
//    //}


//    //public static bool FriendshipExists(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                    string userA,
//    //                                    string userB,
//    //                                    out string errorMsg, WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  try
//    //  {
//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.GET,
//    //                                      string.Format("{0}user_a={1}&user_b={2}", TwitterResources.friendshipExists,
//    //                                                    userA,
//    //                                                    userB), String.Empty, consumerKey, consumerSecret, sessionKey,
//    //                                      secret, proxy));
//    //    var result = feedXML.Element("friends").Value;
//    //    return bool.Parse(result);
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
//    //                      ex);
//    //  }
//    //  return false;
//    //}

//    //public static TwitterUser Follow(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                 string userIdToFollow,
//    //                                 bool destroy,
//    //                                 out string errorMsg)
//    //{
//    //  return Follow(consumerKey,
//    //                consumerSecret,
//    //                sessionKey,
//    //                secret,
//    //                userIdToFollow,
//    //                destroy,
//    //                out errorMsg,
//    //                null);
//    //}

//    //public static TwitterUser Follow(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                 string userIdToFollow,
//    //                                 bool destroy,
//    //                                 out string errorMsg,
//    //                                 WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  try
//    //  {
//    //    var url = TwitterResources.follow;
//    //    if (destroy)
//    //      url = TwitterResources.unfollow;
//    //    url += string.Format("{0}.xml",
//    //                         userIdToFollow);
//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.POST, url, String.Empty, consumerKey, consumerSecret, sessionKey,
//    //                                      secret, proxy));


//    //    return TwitterLib.LoadUser(feedXML).First();
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::Follow()",
//    //                      ex);
//    //    return null;
//    //  }
//    //}

//    //public static TwitterEntry AddFavorite(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                       string tweetId,
//    //                                       bool destroy,
//    //                                       out string errorMsg)
//    //{
//    //  return AddFavorite(consumerKey, consumerSecret, sessionKey, secret,
//    //                     tweetId,
//    //                     destroy,
//    //                     out errorMsg,
//    //                     null);
//    //}

//    //public static TwitterEntry AddFavorite(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                       string tweetId,
//    //                                       bool destroy,
//    //                                       out string errorMsg,
//    //                                       WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  try
//    //  {
//    //    var url = TwitterResources.favoritesCreate;
//    //    if (destroy)
//    //      url = TwitterResources.favoritesDestroy;
//    //    url += string.Format("{0}.xml",
//    //                         tweetId);
//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.POST, url, String.Empty, consumerKey, consumerSecret, sessionKey,
//    //                                      secret, proxy));


//    //    return TwitterLib.LoadFeed(feedXML).First();
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::AddFavorite()",
//    //                      ex);
//    //    return null;
//    //  }
//    //}

//    //public static TwitterEntry AddTweet(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                    string message,
//    //                                    bool convertUrlsToTinyUrls,
//    //                                    out string errorMsg,
//    //                                    string replyID)
//    //{
//    //  return AddTweet(consumerKey, consumerSecret, sessionKey, secret,
//    //                  message,
//    //                  convertUrlsToTinyUrls,
//    //                  out errorMsg,
//    //                  replyID,
//    //                  null);
//    //}

//    //public static TwitterEntry DeleteStatus(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                        string tweetId,
//    //                                        out string errorMsg)
//    //{
//    //  return DeleteStatus(consumerKey, consumerSecret, sessionKey, secret,
//    //                      tweetId,
//    //                      out errorMsg,
//    //                      null);
//    //}

//    //public static TwitterEntry DeleteStatus(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                        string tweetId,
//    //                                        out string errorMsg,
//    //                                        WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  try
//    //  {
//    //    var url = TwitterResources.statusDestroy;

//    //    url += string.Format("{0}.xml",
//    //                         tweetId);
//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.POST, url, String.Empty, consumerKey, consumerSecret, sessionKey,
//    //                                      secret, proxy));


//    //    return TwitterLib.LoadFeed(feedXML).First();
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::AddFavorite()",
//    //                      ex);
//    //    return null;
//    //  }
//    //}

//    //public static TwitterRateLimit RemainingApi(string consumerKey, string consumerSecret, string sessionKey,
//    //                                            string secret,
//    //                                            out string errorMsg)
//    //{
//    //  return RemainingApi(consumerKey, consumerSecret, sessionKey, secret, out errorMsg,
//    //                      null);
//    //}

//    //public static TwitterRateLimit RemainingApi(string consumerKey, string consumerSecret, string sessionKey,
//    //                                            string secret,
//    //                                            out string errorMsg,
//    //                                            WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  TwitterRateLimit trl = null;
//    //  try
//    //  {
//    //    var url = "http://api.twitter.com/1.1/application/rate_limit_status.xml";

//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.GET, url, String.Empty, consumerKey, consumerSecret, sessionKey, secret,
//    //                                      proxy));
//    //    if (feedXML != null)
//    //    {
//    //      var el = feedXML.Element("hash");
//    //      if (el != null)
//    //      {
//    //        trl = new TwitterRateLimit();
//    //        trl.RemainingHits = int.Parse(el.Element("X-Rate-Limit-Limit").Value);
//    //        trl.HourlyLimit = int.Parse(el.Element("X-Rate-Limit-Limit").Value);
//    //        trl.ResetTime = DateTime.Parse(el.Element("X-Rate-Limit-Reset").Value);
//    //        return trl;
//    //      }
//    //    }

//    //    return null;
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::RemainingApi()",
//    //                      ex);
//    //    return null;
//    //  }
//    //}

//    //public static TwitterEntry AddTweet(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                    string message,
//    //                                    bool convertUrlsToTinyUrls,
//    //                                    out string errorMsg,
//    //                                    string replyID,
//    //                                    WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  try
//    //  {
//    //    var plainTextMessage = message;
//    //    var isDirectMessage = (message.StartsWith("d ",
//    //                                              StringComparison.CurrentCultureIgnoreCase));
//    //    if (string.IsNullOrEmpty(message))
//    //      return null;
//    //    message = HttpUtility.UrlEncode(message);
//    //    string url;
//    //    string param;
//    //    string sourceParam;
//    //    if (isDirectMessage)
//    //    {
//    //      url = TwitterResources.directMessageNew;
//    //      var user = message.Split('+');
//    //      param = "user=" + user[1];
//    //      message = "";
//    //      var i = 0;
//    //      foreach (var s in user)
//    //      {
//    //        if (i > 1)
//    //        {
//    //          message += "+";
//    //          message += s;
//    //        }
//    //        i++;
//    //      }
//    //      sourceParam = "&text=" + message;
//    //    }
//    //    else
//    //    {
//    //      url = TwitterResources.postNewStatus;
//    //      param = string.Format("status={0}", message);

//    //      if (!string.IsNullOrEmpty(replyID))
//    //      {
//    //        sourceParam = string.Format("&source={0}&in_reply_to_status_id={1}", TwitterResources.clientName, replyID);
//    //      }
//    //      else
//    //      {
//    //        sourceParam = string.Format("&source={0}", TwitterResources.clientName);
//    //      }
//    //    }

//    //    var bContainsPhotoToUpload = plainTextMessage.Contains(photoStartTag);
//    //    if (bContainsPhotoToUpload)
//    //    {
//    //      var statusMessage = GetMessageOnly(plainTextMessage);
//    //      var imagePath = GetPhotoPathOnly(plainTextMessage);
//    //      var status = AddTweetWithPhoto(consumerKey, consumerSecret, sessionKey, secret, statusMessage, imagePath,
//    //                                     out errorMsg, proxy);
//    //      var twitterEntry = LinqToTwitterHelper.ConvertSearchToTwitterEntry(status);
//    //      return twitterEntry;
//    //    }

//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.POST, url, param + sourceParam, consumerKey, consumerSecret, sessionKey,
//    //                                      secret, proxy));
//    //    if (isDirectMessage)
//    //      return TwitterLib.LoadDirectMessage(feedXML).First();

//    //    return TwitterLib.LoadFeed(feedXML).First();
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib",
//    //                      ex);
//    //    errorMsg = ex.Message;
//    //  }
//    //  return null;
//    //}

//    private const string photoStartTag = "<Photo>|";
//    private const string photoEndTag = "|</Photo>";

//    private static string GetMessageOnly(string message)
//    {
//      var startoffset = message.IndexOf(photoStartTag);
//      var endoffset = message.IndexOf(photoEndTag);

//      var tempMsg1 = message.Substring(0, startoffset);
//      var tempMsg2 = message.Substring(endoffset + photoEndTag.Length);

//      var resultMessage = tempMsg1 + tempMsg2;
//      return resultMessage;
//    }

//    private static string GetPhotoPathOnly(string message)
//    {
//      var startoffset = message.IndexOf(photoStartTag);
//      var endoffset = message.IndexOf(photoEndTag);

//      var tempMsg = message.Substring(startoffset + photoStartTag.Length,
//                                      endoffset - (startoffset + photoStartTag.Length));
//      var resultMessage = tempMsg;
//      return resultMessage;
//    }

//    //public static List<TwitterEntry> GetUser(string consumerKey, string consumerSecret, string sessionKey, string secret, string userToGet,
//    //                                         int count, int pageNb,
//    //                                         out string errorMsg, WebProxy proxy)
//    //{
//    //  errorMsg = null;
//    //  if (string.IsNullOrEmpty(userToGet))
//    //    return new List<TwitterEntry>();

//    //  return GetFeedFromUrl(
//    //    string.Format("{0}{1}.xml",
//    //                  TwitterResources.userTimeline,
//    //                  userToGet),
//    //    count,
//    //    pageNb,
//    //    false,
//    //    out errorMsg, consumerKey, consumerSecret, sessionKey, secret, proxy);
//    //}

//    //public static List<TwitterEntry> GetDirectMessages(string consumerKey, string consumerSecret, string sessionKey,
//    //                                                   string secret,
//    //                                                   int count,
//    //                                                   int pageNb,
//    //                                                   out string errorMsg, WebProxy proxy)
//    //{
//    //  var dmsReceived = GetFeedFromUrl(TwitterResources.directMessages,
//    //                                   count,
//    //                                   pageNb,
//    //                                   true,
//    //                                   out errorMsg, consumerKey, consumerSecret, sessionKey, secret,
//    //                                   proxy);
//    //  var dmsSent = GetFeedFromUrl(TwitterResources.directMessagesSent,
//    //                               count,
//    //                               pageNb,
//    //                               true,
//    //                               out errorMsg, consumerKey, consumerSecret, sessionKey, secret, proxy);
//    //  dmsReceived.AddRange(dmsSent);
//    //  return dmsReceived;
//    //}


//    public static List<TwitterEntry> GetFeedFromUrl(string consumerKey, string consumerSecret, string sessionKey,
//                                                    string secret, string uri,
//                                                    int count,
//                                                    int pageNb,
//                                                    bool isDirectMessages,
//                                                    out string errorMsg, WebProxy proxy)
//    {
//      errorMsg = string.Empty;
//      var entries = new List<TwitterEntry>();

//      try
//      {
//        var feedXML = OAuthWebRequest(Method.GET, string.Format("{0}?count={1}&pageNb={2}.xml", uri, count, pageNb),
//                                      String.Empty, consumerKey, consumerSecret, sessionKey, secret, proxy);

//        var feedXML2 = XDocument.Parse(feedXML);


//        if (!isDirectMessages)
//        {
//          entries = TwitterLib.LoadFeed(feedXML2);
//        }
//        else
//        {
//          entries = TwitterLib.LoadDirectMessage(feedXML2);
//        }


//        return entries;
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("TwitterLib -> GetFeedFromUrl() -> ",
//                          ex);
//      }
//      return entries;
//    }


//    public static List<TwitterEntry> GetFeedFromUrl(string uri,
//                                                    int count,
//                                                    int pageNb,
//                                                    bool isDirectMessages,
//                                                    out string errorMsg, string consumerKey, string consumerSecret,
//                                                    string token, string tokenSecret, WebProxy proxy)
//    {
//      errorMsg = string.Empty;
//      var entries = new List<TwitterEntry>();

//      try
//      {
//        var feedXML = OAuthWebRequest(Method.GET, string.Format("{0}?count={1}&pageNb={2}.xml", uri, count, pageNb),
//                                      String.Empty, consumerKey, consumerSecret, token, tokenSecret, proxy);

//        var feedXML2 = XDocument.Parse(feedXML);

//        if (!isDirectMessages)
//          entries = TwitterLib.LoadFeed(feedXML2);
//        else
//          entries = TwitterLib.LoadDirectMessage(feedXML2);

//        return entries;
//      }
//      catch (Exception ex)
//      {
//        errorMsg = ex.Message;
//        TraceHelper.Trace("TwitterLib -> GetFeedFromUrl() -> ",
//                          ex);
//      }
//      return entries;
//    }

//    //public static void Block(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                         string userToBlock, out string errorMsg,
//    //                         WebProxy proxy)
//    //{

//    //  // Create the web request  
//    //  var url = string.Format("http://api.twitter.com/1.1/blocks/create/{0}.xml", userToBlock);

//    //  // Set values for the request back
//    //  //request.ContentType = "application/x-www-form-urlencoded";
//    //  var sourceParam = string.Format("id={0}", HttpUtility.UrlEncode(userToBlock));

//    //  var feedXML =
//    //    XDocument.Parse(OAuthWebRequest(Method.POST, url, sourceParam, consumerKey, consumerSecret, sessionKey, secret,
//    //                                    proxy));

//    //  errorMsg = "";
//    //}

//    public static void ReportSpam(string consumerKey, string consumerSecret, string sessionKey, string secret,
//                                  string userToBlock, out string errorMsg,
//                                  WebProxy proxy)
//    {
//      // Create the web request  
//      var url = "http://api.twitter.com/1.1/report_spam.xml";


//      // Set values for the request back
//      //request.ContentType = "application/x-www-form-urlencoded";
//      var sourceParam = string.Format("id={0}", HttpUtility.UrlEncode(userToBlock));

//      var feedXML =
//        XDocument.Parse(OAuthWebRequest(Method.POST, url, sourceParam, consumerKey, consumerSecret, sessionKey, secret,
//                                        proxy));
//      errorMsg = "";
//    }


//    //public static TwitterEntry Retweet(string consumerKey,
//    //                                   string consumerSecret,
//    //                                   string sessionKey,
//    //                                   string secret,
//    //                                   bool convertUrlsToTinyUrls,
//    //                                   out string errorMsg,
//    //                                   string retweetId,
//    //                                   WebProxy proxy)
//    //{
//    //  try
//    //  {
//    //    // Create the web request  
//    //    var url = TwitterResources.retweet + retweetId + ".xml?source=" + TwitterResources.clientName;

//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.POST, url, String.Empty, consumerKey, consumerSecret, sessionKey,
//    //                                      secret, proxy));

//    //    errorMsg = "";
//    //    return TwitterLib.LoadFeed(feedXML).First();
//    //  }
//    //  catch (Exception e)
//    //  {
//    //    errorMsg = e.Message;
//    //    Console.WriteLine(e);
//    //  }

//    //  return null;
//    //}

//    //public static List<TwitterEntry> GetTweetInfo(string consumerKey, string consumerSecret, string sessionKey,
//    //                                              string secret,
//    //                                              string id,
//    //                                              out string errorMsg, WebProxy proxy)
//    //{
//    //  var lists = new List<TwitterEntry>();

//    //  try
//    //  {
//    //    var path = string.Format("{0}{1}.xml",
//    //                             TwitterResources.tweetShow,
//    //                             id);
//    //    var feedXml =
//    //      XDocument.Parse(
//    //        OAuthWebRequest(Method.GET, path, string.Empty, consumerKey, consumerSecret, sessionKey, secret, proxy).
//    //          Replace(
//    //            "georss:point", "georss_point"));

//    //    var f = TwitterLib.LoadFeed(feedXml);

//    //    if (f.Count > 0)
//    //    {
//    //      lists.AddRange(f);
//    //    }
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::GetTwitterHomeTimeline()",
//    //                      ex);
//    //  }
//    //  errorMsg = "";
//    //  return lists;
//    //}

//    //public static List<TwitterEntry> GetHomeTimeline(string consumerKey, string consumerSecret, string sessionKey,
//    //                                                 string secret,
//    //                                                 int count,
//    //                                                 int pageNb,
//    //                                                 long maxId, 
//    //                                                 long SinceId,
//    //                                                 out string errorMsg, WebProxy proxy)
//    //{
//    //  var lists = new List<TwitterEntry>();

//    //  try
//    //  {
//    //    var txt = string.Empty;
//    //    if (maxId != -1)
//    //    {
//    //      txt = "&max_id=" + maxId;
//    //    }
//    //    if (SinceId != -1)
//    //    {
//    //      txt += "&since_id=" + SinceId;
//    //    }
//    //    var path = string.Format("{0}?count={1}&pageNb={2}{3}.xml",
//    //                             TwitterResources.home_timeline,
//    //                             count,
//    //                             pageNb, txt);
//    //    var feedXML =
//    //      XDocument.Parse(
//    //        OAuthWebRequest(Method.GET, path, string.Empty, consumerKey, consumerSecret, sessionKey, secret, proxy).
//    //          Replace(
//    //            "georss:point", "georss_point"));

//    //    var f = TwitterLib.LoadFeed(feedXML);

//    //    if (f.Count > 0)
//    //    {
//    //      lists.AddRange(f);
//    //    }
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::GetTwitterHomeTimeline()",
//    //                      ex);
//    //  }
//    //  errorMsg = "";
//    //  return lists;
//    //}

//    /// <summary>
//    ///   Web Request Wrapper
//    /// </summary>
//    /// <param name="method">Http Method</param>
//    /// <param name="url">Full url to the web resource</param>
//    /// <param name="postData">Data to post in querystring format</param>
//    /// <returns>The web server response.</returns>
//    public static string WebRequest(Method method, string url, string postData, WebProxy proxy)
//    {
//      StreamWriter requestWriter;

//      var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
//      if (proxy != null)
//      {
//        webRequest.Proxy = proxy;
//      }
//      if (webRequest != null)
//      {
//        webRequest.Method = method.ToString();
//        webRequest.ServicePoint.Expect100Continue = false;

//        if (method == Method.POST)
//        {
//          webRequest.ContentType = "application/x-www-form-urlencoded";
//          webRequest.ContentLength = postData.Length;
//          //POST the data.
//          requestWriter = new StreamWriter(webRequest.GetRequestStream());
//          try
//          {
//            requestWriter.Write(postData);
//          }
//          finally
//          {
//            requestWriter.Close();
//          }
//        }
//      }
//      return WebResponseGet(webRequest);
//    }

//    /// <summary>
//    ///   Process the web response.
//    /// </summary>
//    /// <param name="webRequest">The request object.</param>
//    /// <returns>The response data.</returns>
//    public static string WebResponseGet(HttpWebRequest webRequest)
//    {
//      TraceHelper.Trace("TwitterLib", webRequest.RequestUri.ToString());
//      StreamReader responseReader = null;
//      var responseData = "";

//      try
//      {
//        responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
//        responseData = responseReader.ReadToEnd();
//        //TraceHelper.Trace("TwitterLib", webRequest.RequestUri.ToString());
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("TwitterLib", ex.Message);
//      }
//      finally
//      {
//        webRequest.GetResponse().GetResponseStream().Close();
//        if (responseReader != null) responseReader.Close();
//      }

//      return responseData;
//    }

//    #region Lists

//    public static string CreateNewList(string consumerKey, string consumerSecret, string sessionKey, string secret,
//                                       string login,
//                                       string mode, string name,
//                                       WebProxy proxy)
//    {
//      // Create the web request  
//      var url = string.Format("http://api.twitter.com/1.1/{0}/lists.xml", login);

//      // Set values for the request back
//      //request.ContentType = "application/x-www-form-urlencoded";
//      var sourceParam = string.Format("name={0}&mode={1}", HttpUtility.UrlEncode(name), mode);

//      var feedXML =
//        XDocument.Parse(OAuthWebRequest(Method.POST, url, sourceParam, consumerKey, consumerSecret, sessionKey, secret,
//                                        proxy));

//      return null;
//    }


//    public static string ModifyList(string consumerKey, string consumerSecret, string sessionKey, string secret,
//                                    string login, string pwdHash, string hashKey, string mode, string name,
//                                    string oldName, WebProxy proxy)
//    {
//      // Create the web request  
//      var url = string.Format("http://api.twitter.com/1.1/{0}/lists/{1}.xml", login, oldName);

//      // Set values for the request back
//      //request.ContentType = "application/x-www-form-urlencoded";
//      var sourceParam = string.Format("name={0}&mode={1}", HttpUtility.UrlEncode(name), mode);

//      var feedXML =
//        XDocument.Parse(OAuthWebRequest(Method.POST, url, sourceParam, consumerKey, consumerSecret, sessionKey, secret,
//                                        proxy));

//      return null;
//    }


//    //public static List<TwitterList> GetListOwn(string consumerKey, string consumerSecret, string sessionKey,
//    //                                           string secret, string login,
//    //                                           out string errorMsg,
//    //                                           WebProxy proxy)
//    //{
//    //  var lists = new List<TwitterList>();

//    //  try
//    //  {
//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.GET, string.Format("http://api.twitter.com/1.1/{0}/lists.xml", login),
//    //                                      string.Empty, consumerKey, consumerSecret, sessionKey, secret, proxy));

//    //    var f = TwitterLib.LoadLists(feedXML);

//    //    if (f.Count > 0)
//    //    {
//    //      lists.AddRange(f);
//    //    }
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
//    //                      ex);
//    //  }
//    //  errorMsg = "";
//    //  return lists;
//    //}


//    //public static List<TwitterList> GetList(string consumerKey, string consumerSecret, string sessionKey, string secret,
//    //                                        string login, out string errorMsg,
//    //                                        WebProxy proxy)
//    //{
//    //  var lists = new List<TwitterList>();

//    //  try
//    //  {
//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.GET,
//    //                                      string.Format("http://api.twitter.com/1.1/{0}/lists/subscriptions.xml", login),
//    //                                      string.Empty, consumerKey, consumerSecret, sessionKey, secret, proxy));

//    //    var f = TwitterLib.LoadLists(feedXML);

//    //    if (f.Count > 0)
//    //    {
//    //      lists.AddRange(f);
//    //    }
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
//    //                      ex);
//    //  }
//    //  errorMsg = "";
//    //  return lists;
//    //}


//    //public static List<TwitterList> GetListMembership(string consumerKey, string consumerSecret, string sessionKey,
//    //                                                  string secret, string login,
//    //                                                  out string errorMsg, WebProxy proxy)
//    //{
//    //  var lists = new List<TwitterList>();

//    //  try
//    //  {
//    //    var feedXML =
//    //      XDocument.Parse(OAuthWebRequest(Method.GET,
//    //                                      string.Format("http://api.twitter.com/1.1/{0}/lists/memberships.xml", login),
//    //                                      string.Empty, consumerKey, consumerSecret, sessionKey, secret, proxy));

//    //    var f = TwitterLib.LoadLists(feedXML);

//    //    if (f.Count > 0)
//    //    {
//    //      lists.AddRange(f);
//    //    }
//    //  }
//    //  catch (Exception ex)
//    //  {
//    //    TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
//    //                      ex);
//    //  }
//    //  errorMsg = "";
//    //  return lists;
//    //}

//    public static List<TwitterEntry> GetListStatuse(string consumerKey, string consumerSecret, string sessionKey,
//                                                    string secret,
//                                                    string userList,
//                                                    string listSlug, int nbGet, out string errorMsg, WebProxy proxy)
//    {
//      var lists = new List<TwitterEntry>();

//      try
//      {
//        var feedXML =
//          XDocument.Parse(OAuthWebRequest(Method.GET,
//                                          string.Format(
//                                            "http://api.twitter.com/1.1/{0}/lists/{1}/statuses.xml?per_page={2}",
//                                            userList,
//                                            listSlug, nbGet), string.Empty, consumerKey, consumerSecret, sessionKey,
//                                          secret, proxy));

//        var f = TwitterLib.LoadFeed(feedXML);

//        if (f.Count > 0)
//        {
//          lists.AddRange(f);
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("TwitterLib::GetListStatuse()",
//                          ex);
//      }
//      errorMsg = "";
//      return lists;
//    }


//    //private static TwitterUser GetTwitterData(XElement status)
//    //{
//    //  try
//    //  {
//    //    if (status.Element("retweeted_status") != null)
//    //    {
//    //      return new TwitterUser
//    //               {
//    //                 Id = status.Element("user").Element("id").Value,
//    //                 Name = status.Element("user").Element("name").Value,
//    //                 NickName = status.Element("user").Element("screen_name").Value,
//    //                 ProfileUrl =
//    //                   TwitterResources.home +
//    //                   status.Element("user").Element("screen_name").Value,
//    //                 Location = status.Element("user").Element("location").Value,
//    //                 Description =
//    //                   status.Element("user").Element("description").Value,
//    //                 ProfileImgUrl =
//    //                   status.Element("user").Element("profile_image_url").Value,
//    //                 Url = status.Element("user").Element("url").Value,
//    //                 //Protected = bool.Parse(status.Element("user").Element("protected").Value),
//    //                 FollowersCount =
//    //                   int.Parse(
//    //                     status.Element("user").Element("followers_count").Value),
//    //                 StatusUseCount =
//    //                   status.Element("statuses_count") != null
//    //                     ? int.Parse(status.Element("statuses_count").Value)
//    //                     : 0,
//    //                 Geolocation = GetLocation(status)
//    //               };
//    //    }
//    //  }
//    //  catch (Exception e)
//    //  {
//    //    Console.WriteLine(e);
//    //  }
//    //  return null;
//    //}

//    //public static TwitterUser GetUser(XElement status)
//    //{
//    //  try
//    //  {
//    //    if (status.Element("retweeted_status") != null)
//    //    {
//    //      status = status.Element("retweeted_status");
//    //      return new TwitterUser
//    //               {
//    //                 Id = status.Element("user").Element("id").Value,
//    //                 Name = status.Element("user").Element("name").Value,
//    //                 NickName = status.Element("user").Element("screen_name").Value,
//    //                 ProfileUrl =
//    //                   TwitterResources.home +
//    //                   status.Element("user").Element("screen_name").Value,
//    //                 Location = status.Element("user").Element("location").Value,
//    //                 Description =
//    //                   status.Element("user").Element("description").Value,
//    //                 ProfileImgUrl =
//    //                   status.Element("user").Element("profile_image_url").Value,
//    //                 Url = status.Element("user").Element("url").Value,
//    //                 FollowersCount =
//    //                   int.Parse(
//    //                     status.Element("user").Element("followers_count").Value),
//    //                 StatusUseCount =
//    //                   status.Element("statuses_count") != null ? int.Parse(status.Element("statuses_count").Value) : 0,
//    //                 Geolocation = GetLocation(status)
//    //               };
//    //    }
//    //    return new TwitterUser
//    //             {
//    //               Id = status.Element("user").Element("id").Value,
//    //               Name = status.Element("user").Element("name").Value,
//    //               NickName = status.Element("user").Element("screen_name").Value,
//    //               ProfileUrl =
//    //                 TwitterResources.home +
//    //                 status.Element("user").Element("screen_name").Value,
//    //               Location = status.Element("user").Element("location").Value,
//    //               Description =
//    //                 status.Element("user").Element("description").Value,
//    //               ProfileImgUrl =
//    //                 status.Element("user").Element("profile_image_url").Value,
//    //               Url = status.Element("user").Element("url").Value,
//    //               //Protected = bool.Parse(status.Element("user").Element("protected").Value),
//    //               FollowersCount =
//    //                 int.Parse(
//    //                   status.Element("user").Element("followers_count").Value),
//    //               StatusUseCount =
//    //                 status.Element("statuses_count") != null ? int.Parse(status.Element("statuses_count").Value) : 0,
//    //               Geolocation = GetLocation(status)
//    //             };
//    //  }
//    //  catch (Exception e)
//    //  {
//    //    Console.WriteLine(e);
//    //  }
//    //  return null;
//    //}

//    //private static Geoloc GetLocation(XContainer status)
//    //{
//    //  if (status.Element("geo") != null && (status.Element("geo").HasAttributes || status.Element("geo").HasElements) &&
//    //      status.Element("geo").Element("georss_point") != null)
//    //  {
//    //    var value = status.Element("geo").Element("georss_point").Value;
//    //    var values = value.Split(' ');
//    //    return new Geoloc {Latitude = double.Parse(values[0]), Longitude = double.Parse(values[1])};
//    //  }
//    //  if (status.Element("geo") != null && status.Element("geo").HasElements)
//    //  {
//    //    XNamespace georss = "http://www.georss.org/georss";
//    //    if (status.Element("geo").Element(georss + "point") != null)
//    //    {
//    //      var s = status.Element("geo").Element(georss + "point").Value;
//    //      var values = s.Split(' ');
//    //      return new Geoloc {Latitude = double.Parse(values[0]), Longitude = double.Parse(values[1])};
//    //    }
//    //  }

//    //  XNamespace atom = "http://www.w3.org/2005/Atom";
//    //  if (status.Element(atom + "twitter_geo") != null &&
//    //      (status.Element(atom + "twitter_geo").HasAttributes || status.Element(atom + "twitter_geo").HasElements) &&
//    //      status.Element(atom + "twitter_geo").Element(atom + "georss_point") != null)
//    //  {
//    //    var value = status.Element(atom + "twitter_geo").Element(atom + "georss_point").Value;
//    //    var values = value.Split(' ');
//    //    return new Geoloc {Latitude = double.Parse(values[0]), Longitude = double.Parse(values[1])};
//    //  }
//    //  return null;
//    //}

//    //private static string GetTitle(string decode, XContainer element)
//    //{
//    //  return element.Element("retweeted_status") != null
//    //           ? HttpUtility.HtmlDecode(element.Element("retweeted_status").Element("text").Value)
//    //           : decode;
//    //}

//    public static bool AddToList(string consumerKey, string consumerSecret, string sessionKey, string secret,
//                                 string userList, string listSlug,
//                                 string userID, WebProxy proxy)
//    {
//      // Create the web request  
//      var url = string.Format("http://api.twitter.com/1.1/{0}/{1}/members.xml", userList, listSlug);

//      // Set values for the request back
//      //request.ContentType = "application/x-www-form-urlencoded";
//      var sourceParam = string.Format("id={0}&list_id={1}", HttpUtility.UrlEncode(userID), listSlug);


//      var feedXML =
//        XDocument.Parse(OAuthWebRequest(Method.POST, url, sourceParam, consumerKey, consumerSecret, sessionKey, secret,
//                                        proxy));

//      // Get the response stream  
//      if (feedXML != null)
//      {
//        return true;
//      }
//      return false;
//    }


//    public static List<TwitterUser> GetListMembers(string consumerKey, string consumerSecret, string sessionKey,
//                                                   string secret,
//                                                   string userList,
//                                                   string listSlug, out string errorMsg, WebProxy proxy)
//    {
//      var lists = new List<TwitterUser>();

//      try
//      {
//        var feedXML =
//          XDocument.Parse(OAuthWebRequest(Method.GET,
//                                          string.Format("http://api.twitter.com/1.1/{0}/{1}/members.xml", userList,
//                                                        listSlug),
//                                          string.Empty, consumerKey, consumerSecret, sessionKey, secret, proxy));

//        var f = TwitterLib.LoadUser(feedXML);

//        if (f.Count > 0)
//        {
//          lists.AddRange(f);
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("TwitterLib::GetTwitterFriends()",
//                          ex);
//      }
//      errorMsg = "";
//      return lists;
//    }


//    public static bool DeleteListMembers(string consumerKey, string consumerSecret, string sessionKey, string secret,
//                                         string userList, string listSlug,
//                                         string userId, WebProxy proxy)
//    {
//      // Create the web request  
//      var url = string.Format("http://api.twitter.com/1.1/{0}/{1}/members.xml", userList, listSlug);

//      // Set values for the request back
//      //request.ContentType = "application/x-www-form-urlencoded";
//      var sourceParam = string.Format("id={0}&list_id={1}&_method=DELETE", HttpUtility.UrlEncode(userId), listSlug);


//      var feedXML =
//        XDocument.Parse(OAuthWebRequest(Method.POST, url, sourceParam, consumerKey, consumerSecret, sessionKey, secret,
//                                        proxy));

//      // Get the response stream  
//      if (feedXML != null)
//      {
//        return true;
//      }
//      return false;
//    }

//    #endregion

//    public static TwitterContext _twitterContext;

//    public static TwitterContext GetTwitterContext(TwitterCredentials twitterCredentials)
//    {
//      if (_twitterContext == null)
//      {
//        // configure the OAuth object
//        var auth = new SingleUserAuthorizer
//                     {
//                       CredentialStore = new InMemoryCredentialStore()
//                                       {
//                                         ConsumerKey = twitterCredentials.ConsumerKey,
//                                         ConsumerSecret = twitterCredentials.ConsumerSecret,
//                                         OAuthToken = twitterCredentials.OAuthToken,
//                                         AccessToken = twitterCredentials.AccessToken
//                                       }
//                     };
//        _twitterContext = new TwitterContext(auth, "https://api.twitter.com/1.1/", "https://search.twitter.com/");
//      }
//      return _twitterContext;
//    }

//    /// <summary>
//    ///   PostTweetWithMedia
//    /// </summary>
//    /// <param name="twitterContext"></param>
//    /// <param name="status"></param>
//    /// <param name="imageFilePath"></param>
//    /// <param name="fileName"></param>
//    /// <returns></returns>
//    public static TwitterEntry PostTweetWithMedia(TwitterContext twitterContext, string status, string imageFilePath,
//                                                  string fileName)
//    {
//      const bool PossiblySensitive = false;
//      const decimal Latitude = StatusExtensions.NoCoordinate; //37.78215m;
//      const decimal Longitude = StatusExtensions.NoCoordinate; // -122.40060m;
//      const bool DisplayCoordinates = false;

//      try
//      {
//        var mediaItems =
//          new List<Media>
//            {
//              new Media
//                {
//                  Data = Utilities.GetFileBytes(imageFilePath),
//                  FileName = fileName,
//                  ContentType = MediaContentType.Png
//                }
//            };

//        var tweet = twitterContext.TweetWithMedia(status, PossiblySensitive, Latitude, Longitude, null,
//                                                  DisplayCoordinates, mediaItems, null);
//        var twitterEntry = LinqToTwitterHelper.ConvertStatusToTwitterEntry(tweet);
//        return twitterEntry;
//      }
//      catch (Exception ex)
//      {
//        BLogManager.LogEntry(APPNAME, "PostTweetWithMedia", ex);
//      }
//      return null;
//    }
//  }
//}
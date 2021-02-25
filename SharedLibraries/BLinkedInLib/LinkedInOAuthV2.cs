#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using BUtility;
using DotNetOpenAuth.OAuth.ChannelElements;
using LinkedIn;
using LinkedIn.ServiceEntities;
using Sobees.Library.BGenericLib;
using Sobees.Library.BLinkedInLib.Cls;
using Sobees.Library.BLinkedInLib.Helpers;
using Sobees.Tools.Logging;
using Visibility = System.Windows.Visibility;

#endregion

namespace Sobees.Library.BLinkedInLib
{
  public class OAuthLinkedInV2 : OAuthBaseV2
  {
    #region Method enum

    #region EnumLinkedInSearchNetwork enum

    public enum EnumLinkedInSearchNetwork
    {
      IN,
      OUT
    }

    public const string ACCESS_TOKEN = "https://api.linkedin.com/uas/oauth/accessToken";

    public const string AUTHORIZE = "https://api.linkedin.com/uas/oauth/authorize";

    public const string CALLBACK = "liconnect://success";
    public const string REQUEST_TOKEN = "https://api.linkedin.com/uas/oauth/requestToken";

    //public const string USER_AGENT = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)";
    public const string USER_AGENT = "YourAgent";
    private const string APPNAME = "OAuthLinkedInV2";

    private string _consumerKey = "";
    private string _consumerSecret = "";
    private InMemoryTokenManager _tokenManager;

    public OAuthLinkedInV2(string consumerKey, string consumerSecret) :
      this(consumerKey, consumerSecret, string.Empty, string.Empty)
    {
    }

    public OAuthLinkedInV2(string linkedinWpfKey, string linkedinWpfSecret, string token, string secret)
    {
      ConsumerKey = linkedinWpfKey;
      ConsumerSecret = linkedinWpfSecret;
      Token = token;
      TokenSecret = secret;
      Friends = new List<LinkedInUser>();
      Authorization = new WebOAuthAuthorization(TokenManager, Token);
    }

    private IConsumerTokenManager TokenManager => _tokenManager ?? (_tokenManager = new InMemoryTokenManager(ConsumerKey, ConsumerSecret));

    #endregion

    #region EnumLinkedInSearchSort enum

    public enum EnumLinkedInSearchSort
    {
      ctx,
      endorsers,
      distance,
      relevance
    }

    #endregion

    #region EnumLinkedInUpdateType enum

    public enum EnumLinkedInUpdateType
    {
      APPS,
      APPM,
      CONN,
      JOBS,
      JGRP,
      RECU,
      PRFU,
      SHAR, 
      SVPR, 
      PICU, 
    }

    #endregion

    public enum Method
    {
      GET,
      POST,
      PUT,
      DELETE
    };

    #endregion

    #region Properties

    public string CallBackUrl { get; set; }

    public string ConsumerKey
    {
      get
      {
        if (_consumerKey.Length == 0)
        {
          _consumerKey = LinkedInGlobals.LINKEDIN_WPF_KEY;
        }
        return _consumerKey;
      }
      set => _consumerKey = value;
    }

    public string ConsumerSecret
    {
      get
      {
        if (_consumerSecret.Length == 0)
        {
          _consumerSecret = LinkedInGlobals.LINKEDIN_WPF_SECRET;
        }
        return _consumerSecret;
      }
      set => _consumerSecret = value;
    }

    public List<LinkedInUser> Friends { get; set; }

    public string OAuthVerifier { get; set; }

    #endregion

    #region Properties

    public string Token { get; set; }

    public string TokenSecret { get; set; }

    #endregion

    protected WebOAuthAuthorization Authorization { get; private set; }

    public static List<LinkedInUser> ParseConnectionsDataSt(string result, bool isFriends)
    {
      if (string.IsNullOrEmpty(result))
      {
        return null;
      }

      try
      {
        var tr = new StringReader(result);
        var xdoc = XDocument.Load(tr);
        var query = (from user in xdoc.Descendants("person")
                     select
                       new LinkedInUser
                       {
                         Id =
                           user.Element("api-standard-profile-request") != null
                             ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
                             : user.Element("id") != null ? user.Element("id").Value : null,
                         FirstName = user.Element("first-name").Value,
                         Name = user.Element("last-name").Value,
                         NickName = $"{user.Element("first-name").Value} {user.Element("last-name").Value}",
                         Description = user.Element("headline") != null ? user.Element("headline").Value : string.Empty,
                         Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
                         ProfileImgUrl =
                           user.Element("picture-url") != null
                             ? user.Element("picture-url").Value
                             : "http://static03.linkedin.com/img/icon/icon_no_photo_40x40.png",
                         Url = user.Element("api-standard-profile-request") != null
                           ? user.Element("api-standard-profile-request").Element("url").Value
                           : string.Empty,
                       });
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseConnectionsData", ex);
        return null;
      }
    }

    private static DateTime _unixStartDate = new DateTime(1970, 1, 1);
    internal static DateTime GetRealDateTime(double milliseconds)
    {
      return _unixStartDate.AddMilliseconds(milliseconds).ToLocalTime();
    }
      
    public static List<LinkedInEntry> ParseNetworkUpdateDataSt(string result)
    {
      if (string.IsNullOrEmpty(result))
      {
        return null;
      }

      try
      {
        var tr = new StringReader(result);
        var xdoc = XDocument.Load(tr);
        var query = (xdoc.Descendants("update").Select(update => new LinkedInEntry
        {
          Id =
            update.Element("update-key") != null
              ? update.Element("update-key").Value
              : update.Element("timestamp").Value,
          Type = EnumType.LinkedIn,
          UpdateType = update.Element("update-type").Value,
          PubDate =
            update.Element("timestamp") != null
              ? GetRealDateTime(Convert.ToDouble(update.Element("timestamp").Value.Trim()))
              : new DateTime(),
          User = FindCorrectUserSt(ParseUserData(update.ToString())),
          Users = ParseUpdateLinkedInUsersSt(update.Element("update-type").Value, update),
          Educations = ParseUpdateLinkedInEducations(update.Element("update-type").Value, update),
          Job = ParseUpdateLinkedInJob(update.Element("update-type").Value, update),
          Groups = ParseUpdateLinkedInGroups(update.Element("update-type").Value, update),
          Activities = ParseUpdateLinkedInActivities(update.Element("update-type").Value, update),
          Recommendations = ParseUpdateLinkedInRecommendations(update.Element("update-type").Value, update),
          CanPost = bool.Parse(update.Element("is-commentable").Value) ? 1 : 0,
          Title =
            update.Element("update-content").Element("person") != null
              ? update.Element("update-content").Element("person").Element("current-status") != null
                ? update.Element("update-content").Element("person").Element("current-status").Value
                : null
              : null,
          Comments = (ObservableCollection<Comment>) update.Elements("update-comment").Select(LinkedInHelper.BuildComment)
        }));
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParsingNetworkRUpdateData", ex);
        return null;
      }
    }

 
    public static List<LinkedInUser> ParseSearchResult(string result)
    {
      if (string.IsNullOrEmpty(result))
      {
        return null;
      }

      try
      {
        var tr = new StringReader(result);
        var xdoc = XDocument.Load(tr);
        var query = (from user in xdoc.Descendants("person")
                     select
                       new LinkedInUser
                       {
                         Id = user.Element("id").Value,
                         FirstName = user.Element("first-name").Value,
                         Name = user.Element("last-name").Value,
                         NickName = $"{user.Element("first-name").Value} {user.Element("last-name").Value}",
                         Description = user.Element("headline").Value,
                         Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
                         //IndustryCode = user.Element("industry") != null
                         //                 ?
                         //                   IndustryCode.GetIndustry(
                         //                     user.Element("industry").Value)
                         //                 : null,
                         NbConnections = int.Parse(user.Element("connections").Attribute("total").Value),
                         NbRecommendations = int.Parse(user.Element("num-recommenders").Value),
                         Distance = int.Parse(user.Element("distance").Value),
                         Positions = ParsePositions(user),
                         Url = user.Element("site-standard-profile-request").Element("url").Value
                       });
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseSearch", ex);
        return null;
      }
    }

    public static LinkedInUser ParseStandardUserData(string result)
    {
      if (string.IsNullOrEmpty(result))
      {
        return null;
      }

      try
      {
        var tr = new StringReader(result);
        var xdoc = XDocument.Load(tr);
        var query = (from user in xdoc.Descendants("person")
                     select
                       new LinkedInUser
                       {
                         Id = user.Element("id") != null ? user.Element("id").Value : string.Empty,
                         FirstName = user.Element("first-name").Value,
                         Name = user.Element("last-name").Value,
                         NickName = $"{user.Element("first-name").Value} {user.Element("last-name").Value}",
                         Description = user.Element("headline") != null ? user.Element("headline").Value : string.Empty,
                         Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
                         //IndustryCode =
                         //  user.Element("industry") != null
                         //    ? IndustryCode.GetIndustry(user.Element("industry").Value)
                         //    : null,
                         NbConnections =
                           user.Element("connections") != null
                             ? int.Parse(user.Element("connections").Attribute("total").Value)
                             : -1,
                         NbRecommendations =
                           user.Element("num-recommenders") != null ? int.Parse(user.Element("num-recommenders").Value) : -1,
                         Distance = user.Element("distance") != null ? int.Parse(user.Element("distance").Value) : 0,
                         Positions = ParsePositions(user),
                         Url =
                           user.Element("site-standard-profile-request") != null
                             ? user.Element("site-standard-profile-request").Element("url").Value
                             : string.Empty,
                         LastStatusString = user.Element("current-status") != null ? user.Element("current-status").Value : null,
                         LastStatusDate =
                           user.Element("current-status-timestamp") != null
                             ? new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(user.Element("current-status-timestamp").Value))
                             : new DateTime(),
                         Summary = user.Element("summary") != null ? user.Element("summary").Value : null,
                         Specialties = user.Element("specialties") != null ? user.Element("specialties").Value : null,
                         Associations = user.Element("associations") != null ? user.Element("associations").Value : null,
                         Urls = ParseUrl(user.Element("member-url-resources")),
                         ProfileImgUrl =
                           user.Element("picture-url") != null
                             ? user.Element("picture-url").Value
                             : "http://static03.linkedin.com/img/icon/icon_no_photo_40x40.png"
                       });
        if (!query.Any())
        {
          query = (from user in xdoc.Descendants()
                   select
                     new LinkedInUser
                     {
                       Id = user.Element("id") != null ? user.Element("id").Value : string.Empty,
                       FirstName = user.Element("first-name").Value,
                       Name = user.Element("last-name").Value,
                       NickName = $"{user.Element("first-name").Value} {user.Element("last-name").Value}",
                       Description = user.Element("headline") != null ? user.Element("headline").Value : string.Empty,
                       Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
                       //IndustryCode =
                       //  user.Element("industry") != null
                       //    ? IndustryCode.GetIndustry(user.Element("industry").Value)
                       //    : null,
                       NbConnections =
                         user.Element("connections") != null
                           ? int.Parse(user.Element("connections").Attribute("total").Value)
                           : -1,
                       NbRecommendations =
                         user.Element("num-recommenders") != null ? int.Parse(user.Element("num-recommenders").Value) : -1,
                       Distance = user.Element("distance") != null ? int.Parse(user.Element("distance").Value) : 0,
                       Positions = ParsePositions(user),
                       Url =
                         user.Element("site-standard-profile-request") != null
                           ? user.Element("site-standard-profile-request").Element("url").Value
                           : string.Empty,
                       LastStatusString = user.Element("current-status") != null ? user.Element("current-status").Value : null,
                       LastStatusDate =
                         user.Element("current-status-timestamp") != null
                           ? new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(user.Element("current-status-timestamp").Value))
                           : new DateTime(),
                       Summary = user.Element("summary") != null ? user.Element("summary").Value : null,
                       Specialties = user.Element("specialties") != null ? user.Element("specialties").Value : null,
                       Associations = user.Element("associations") != null ? user.Element("associations").Value : null,
                       Urls = ParseUrl(user.Element("member-url-resources")),
                       ProfileImgUrl =
                         user.Element("picture-url") != null
                           ? user.Element("picture-url").Value
                           : "http://static03.linkedin.com/img/icon/icon_no_photo_40x40.png"
                     });
        }
        return query.ToList().First();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseStandardUserData", ex);
        return null;
      }
    }

    private const string PRIVATE_ID_VALUE = "private";

    public static LinkedInUser ParseUserData(string data)
    {
      if (string.IsNullOrEmpty(data))
        return null;

      try
      {
        var tr = new StringReader(data);
        var xdoc = XDocument.Load(tr);
        var query = (from user in xdoc.Descendants("person")
                     select new LinkedInUser
                     {
                       //Id = user.Element("id").Value,
                       Id =
                         user.Element("api-standard-profile-request") != null
                           ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
                           : string.Empty,
                       FirstName = user.Element("first-name").Value,
                       Name = user.Element("last-name").Value,
                       NickName = $"{user.Element("first-name").Value} {user.Element("last-name").Value}",
                       Description = user.Element("headline") != null ? user.Element("headline").Value : string.Empty,
                       Url = user.Element("api-standard-profile-request") != null
                         ? user.Element("api-standard-profile-request").Element("url").Value
                         : string.Empty,
                     });
        var lst = query.ToList();
        if (lst.Any())
        {
          return lst.First(f => !f.Id.Contains(PRIVATE_ID_VALUE));
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseConnectionsData", ex);
      }
      return null;
    }

    /// <summary>
    ///   Exchange the request token for an access token.
    /// </summary>
    /// <param name="authToken">The oauth_token is supplied by Twitter's authorization page following the callback.</param>
    public string AccessTokenGet(string authToken)
    {
      Token = authToken;
      var response = string.Empty;
      try
      {
        response = OAuthWebRequest(Method.POST, ACCESS_TOKEN, string.Empty);
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("" + "::oAuthWebRequest:", ex);
      }

      if (response.Length > 0)
      {
        //Store the Token and Token Secret
        var qs = HttpUtility.ParseQueryString(response);
        if (qs["oauth_token"] != null)
        {
          Token = qs["oauth_token"];
        }
        if (qs["oauth_token_secret"] != null)
        {
          TokenSecret = qs["oauth_token_secret"];
        }
      }
      return Token;
    }

    /// <summary>
    ///   WebRequestWithPut
    /// </summary>
    /// <param name="method">WebRequestWithPut</param>
    /// <param name="url"></param>
    /// <param name="postData"></param>
    /// <returns></returns>
    public string ApiWebRequest(string method, string url, string postData)
    {
      var uri = new Uri(url);
      var nonce = GenerateNonce();
      var timeStamp = GenerateTimeStamp();

      string outUrl, querystring;

      //Generate Signature
      var sig = GenerateSignature(uri,
        ConsumerKey,
        ConsumerSecret,
        Token,
        TokenSecret,
        method,
        timeStamp,
        nonce,
        null,
        out outUrl,
        out querystring);

      HttpWebRequest webRequest = null;

      webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
      webRequest.Method = method;
      webRequest.Credentials = CredentialCache.DefaultCredentials;
      webRequest.AllowWriteStreamBuffering = true;

      webRequest.PreAuthenticate = true;
      webRequest.ServicePoint.Expect100Continue = false;
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

      webRequest.Headers.Add("Authorization",
                             $"OAuth realm=\"http://api.linkedin.com/\",oauth_consumer_key=\"{ConsumerKey}\",oauth_token=\"{Token}\",oauth_signature_method=\"HMAC-SHA1\",oauth_signature=\"{HttpUtility.UrlEncode(sig)}\",oauth_timestamp=\"{timeStamp}\",oauth_nonce=\"{nonce}\",oauth_verifier=\"{Verifier}\", oauth_version=\"1.0\"");

      if (postData != null)
      {
        var fileToSend = Encoding.UTF8.GetBytes(postData);
        webRequest.ContentLength = fileToSend.Length;

        var reqStream = webRequest.GetRequestStream();
        reqStream.Write(fileToSend, 0, fileToSend.Length);
        reqStream.Close();
      }

      var returned = WebResponseGet(webRequest);

      return returned;
    }

    /// <summary>
    ///   Get the link to Twitter's authorization page for this application.
    /// </summary>
    /// <returns>The url with a valid request token, or a null string.</returns>
    public string AuthorizationLinkGet()
    {
      string ret = null;

      var response = OAuthWebRequest(Method.POST, REQUEST_TOKEN, String.Empty);
      if (response.Length > 0)
      {
        //response contains token and token secret.  We only need the token.
        //oauth_token=36d1871d-5315-499f-a256-7231fdb6a1e0&oauth_token_secret=34a6cb8e-4279-4a0b-b840-085234678ab4&oauth_callback_confirmed=true
        var qs = HttpUtility.ParseQueryString(response);
        if (qs["oauth_token"] != null)
        {
          Token = qs["oauth_token"];
          TokenSecret = qs["oauth_token_secret"];
          ret = $"{AUTHORIZE}?oauth_token={Token}";
        }
      }
      return ret;
    }

    public String AuthorizeToken()
    {
      if (string.IsNullOrEmpty(Token))
      {
        var e = new Exception("The request token is not set");
        throw e;
      }

      var aw = new AuthorizeWindow(this) { Visibility = Visibility.Visible };
      Token = aw.Token;
      Verifier = aw.Verifier;
      return !string.IsNullOrEmpty(Verifier) ? Token : null;
    }

    public List<LinkedInUser> GetConnections()
    {
      return ParseConnectionsData(OAuthWebRequest(Method.GET, LinkedInResources.CONNECTIONS_CURRENT_URL, String.Empty),
        true);
    }

    public LinkedInUser GetCurrentProfile()
    {
      return
        ParseStandardUserData(
          OAuthWebRequest(
            Method.GET,
            $"{LinkedInResources.PROFIL_CURRENT_URL}:(id,first-name,last-name,headline,location,industry,distance,relation-to-viewer,current-status,current-status-timestamp,summary,specialties,proposal-comments,associations,positions,educations,picture-url,site-standard-profile-request,member-url-resources,num-recommenders)",
            String.Empty));
    }

    public LinkedInUser GetInfoUser(string id)
    {
      return
        ParseStandardUserData(
          OAuthWebRequest(
            Method.GET,
            $"{LinkedInResources.PROFIL_URL}id={id}:(id,first-name,last-name,headline,location,industry,distance,relation-to-viewer,current-status,current-status-timestamp,summary,specialties,proposal-comments,associations,positions,educations,picture-url,site-standard-profile-request,member-url-resources,num-recommenders)",
            string.Empty));
    }

    public List<LinkedInEntry> GetNetworkUpdate(EnumLinkedInUpdateType type, int nbToGet, int offset, long after,
      long before)
    {
      var url = LinkedInResources.NETWORK_UPDATES_URL;
      url = $"{url}type={type}&count={nbToGet}";
      
      if (offset > 0)
        url = $"{url}&start={offset}";
      
      if (after > 0)
        url = $"{url}&after={after}";
      
      if (before > 0)
        url = $"{url}&before={before}";

      return ParseNetworkUpdateData(OAuthWebRequest(Method.GET, url, String.Empty));
    }

    public List<LinkedInEntry> GetNetworkUpdate(List<EnumLinkedInUpdateType> types, int nbToGet, int offset, long after,long before)
    {
      var url = LinkedInResources.NETWORK_UPDATES_URL;
      if (types != null)
      {
        url = types.Aggregate(url, (current, type) => $"{current}type={type}&");
      }
      url = $"{url}count={nbToGet}";
      if (offset > 0)
        url = $"{url}&start={offset}";
      if (after > 0)
        url = $"{url}&after={after}";
      if (before > 0)
        url = $"{url}&before={before}";
      
      return ParseNetworkUpdateData(OAuthWebRequest(Method.GET, url, String.Empty));
    }

    /// <summary>
    ///   Submit a web request using oAuth.
    /// </summary>
    /// <param name="method">GET or POST</param>
    /// <param name="url">The full url, including the querystring.</param>
    /// <param name="postData">Data to post (querystring format)</param>
    /// <returns>The web server response.</returns>
    public string OAuthWebRequest(Method method, string url, string postData)
    {
      var outUrl = "";
      var querystring = "";
      var ret = "";

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
            {
              postData += "&";
            }
            qs[key] = HttpUtility.UrlDecode(qs[key]);
            qs[key] = UrlEncode(qs[key]);
            postData += $"{key}={qs[key]}";
          }
          if (url.IndexOf("?") > 0)
          {
            url += "&";
          }
          else
          {
            url += "?";
          }
          url += postData;
        }
      }

      var uri = new Uri(url);

      var nonce = GenerateNonce();
      var timeStamp = GenerateTimeStamp();

      var callback = "";
      //if (url.Contains(REQUEST_TOKEN))
      //    callback = CALLBACK;

      //Generate Signature
      var sig = GenerateSignature(uri,
        ConsumerKey,
        ConsumerSecret,
        Token,
        TokenSecret,
        method.ToString(),
        timeStamp,
        nonce,
        callback,
        out outUrl,
        out querystring);

      querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

      //Convert the querystring to postData
      if (method == Method.POST)
      {
        postData = querystring;
        querystring = "";
      }

      if (querystring.Length > 0)
      {
        outUrl += "?";
      }

      if (method == Method.POST || method == Method.GET)
        ret = WebRequest(method, outUrl + querystring, postData);

      return ret;
    }

    public List<LinkedInUser> ParseConnectionsData(string result, bool isFriends)
    {
      if (string.IsNullOrEmpty(result))
      {
        return null;
      }
      try
      {
        var tr = new StringReader(result);
        var xdoc = XDocument.Load(tr);
        var query = (from user in xdoc.Descendants("person")
                     select
                       new LinkedInUser
                       {
                         Id =
                           user.Element("api-standard-profile-request") != null
                             ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
                             : user.Element("id") != null ? user.Element("id").Value : null,
                         FirstName = user.Element("first-name").Value,
                         Name = user.Element("last-name").Value,
                         NickName = $"{user.Element("first-name").Value} {user.Element("last-name").Value}",
                         Description = user.Element("headline") != null ? user.Element("headline").Value : "",
                         Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
                         ProfileImgUrl =
                           user.Element("picture-url") != null
                             ? user.Element("picture-url").Value
                             : "http://static03.linkedin.com/img/icon/icon_no_photo_40x40.png",
                         Url =
                           user.Element("site-standard-profile-request") != null
                             ? user.Element("site-standard-profile-request").Element("url").Value
                             : ""
                       });
        if (!isFriends) return query.ToList();
        foreach (var user in query.Where(user => !Friends.Contains(user)))
        {
          Friends.Add(user);
        }
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        return null;
      }
    }

    public List<LinkedInEntry> ParseNetworkUpdateData(string result)
    {
      if (string.IsNullOrEmpty(result))
      {
        return null;
      }

      try
      {
        var tr = new StringReader(result);
        var xdoc = XDocument.Load(tr);
        var query = (from update in xdoc.Descendants("update")
                     select
                       new LinkedInEntry
                       {
                         Id =
                           update.Element("update-key") != null
                             ? update.Element("update-key").Value
                             : update.Element("timestamp").Value,
                         Type = EnumType.LinkedIn,
                         UpdateType = update.Element("update-type").Value,
                         PubDate =
                           update.Element("timestamp") != null
                             ? new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(update.Element("timestamp").Value)).ToLocalTime()
                             : new DateTime().ToLocalTime(),
                         User = FindCorrectUser(ParseUserData(update.ToString())),
                         Users = ParseUpdateLinkedInUsers(update.Element("update-type").Value, update),
                         Educations = ParseUpdateLinkedInEducations(update.Element("update-type").Value, update),
                         Job = ParseUpdateLinkedInJob(update.Element("update-type").Value, update),
                         Groups = ParseUpdateLinkedInGroups(update.Element("update-type").Value, update),
                         Activities = ParseUpdateLinkedInActivities(update.Element("update-type").Value, update),
                         Recommendations = ParseUpdateLinkedInRecommendations(update.Element("update-type").Value, update),
                         CanPost = bool.Parse(update.Element("is-commentable").Value) ? 1 : 0,
                       
                         Title =
                           update.Element("update-content").Element("person") != null
                             ? update.Element("update-content").Element("person").Element("current-share") != null
                               ? update.Element("update-content").Element("person").Element("current-share").Element("content").Element("title").Value
                               : null
                             : null,
                         DisplayLink = 
                      update.Element("update-content").Element("person") != null
                      ? update.Element("update-content").Element("person").Element("current-share") != null
                        ? update.Element("update-content").Element("person").Element("current-share").Element("content").Element("submitted-url").Value
                        : null
                      : null,
                        Link =
                        update.Element("update-content").Element("person") != null
                        ? update.Element("update-content").Element("person").Element("current-share") != null
                        ? update.Element("update-content").Element("person").Element("current-share").Element("content").Element("submitted-url").Value
                        : null
                      : null,
                         Comments = ParseComments(update.Element("update-comments"))
                       });
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParsingNetworkRUpdateData", ex);
        return null;
      }
    }

    public List<LinkedInUser> Search(string keywords,
      string name,
      string company,
      bool currentCompany,
      string title,
      bool currentTitle,
      LinkedInIndustryCode industryCode,
      EnumLinkedInSearchNetwork network,
      int start,
      int count,
      EnumLinkedInSearchSort sort)
    {
      var url = LinkedInResources.SEARCH_URL;
      if (!string.IsNullOrEmpty(keywords))
      {
        url += $"keywords={keywords}";
      }
      if (!string.IsNullOrEmpty(name))
      {
        url += $"&name={name}";
      }
      if (!string.IsNullOrEmpty(company))
      {
        url += $"&company={company}";
        url += $"&current-company={currentCompany}";
      }
      if (!string.IsNullOrEmpty(title))
      {
        url += $"&title={title}";
        url += $"&current-title={currentTitle}";
      }
      if (industryCode != null)
      {
        url += $"&industry-code={industryCode.Code}";
      }
      url += $"&network={network.ToString().ToLower()}";
      url += $"&start={start}";
      url += $"&count={count}";
      url += $"&sort-criteria={sort}";
      return ParseSearchResult(OAuthWebRequest(Method.GET, url, string.Empty));
    }

    public bool SetComments(string status, string id)
    {
      var txt =
        $"<?xml version='1.0' encoding='UTF-8'?><update-comment><comment>{status}</comment></update-comment>";

      return
        OAuthWebRequest(Method.POST, LinkedInResources.COMMENT_URL_START + id + LinkedInResources.COMMENT_URL_END, txt) ==
        "Created";
    }

    public bool SetStatus(string status)
    {
      var txt = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><current-status>{status}</current-status>";

      //string result = OAuthWebRequest(Method.PUT, Resources.STATUS_URL, txt, true);
      return OAuthWebRequest(Method.PUT, LinkedInResources.STATUS_URL, txt) == "NoContent";
    }

    public bool SetUpdate(string body)
    {
      return
        OAuthWebRequest(
          Method.POST, LinkedInResources.NETWORKUPDATE_URL,
                        $"{LinkedInResources.NETWORKUPDATE_URL_START}{body}{LinkedInResources.NETWORKUPDATE_URL_END}") == "Created";
    }

    public bool PostShare(string body)
    {
      try
      {
        Authorization = new WebOAuthAuthorization(TokenManager, Token);
        _tokenManager.StoreNewRequestToken(Token, TokenSecret);
        var service = new LinkedInService(Authorization);
        var res = service.CreateShare(body, VisibilityCode.ConnectionsOnly);
        return res;
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "PostShare", ex);
      }
      return false;
    }


    /// <summary>
    ///   Web Request Wrapper
    /// </summary>
    /// <param name="method">Http Method</param>
    /// <param name="url">Full url to the web resource</param>
    /// <param name="postData">Data to post in querystring format</param>
    /// <returns>The web server response.</returns>
    public string WebRequest(Method method, string url, string postData)
    {
      HttpWebRequest webRequest = null;
      var responseData = "";

      webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
      webRequest.Method = method.ToString();
      webRequest.ServicePoint.Expect100Continue = false;
      webRequest.UserAgent = USER_AGENT;
      webRequest.Timeout = 20000;

      if (method == Method.POST)
      {
        webRequest.ContentType = url.Contains("person-activities") ? "linkedin-html" : "application/x-www-form-urlencoded";

        var requestWriter = new StreamWriter(webRequest.GetRequestStream());
        try
        {
          requestWriter.Write(postData);
        }
        finally
        {
          requestWriter.Close();
          requestWriter = null;
        }
      }

      responseData = WebResponseGet(webRequest);
      return responseData;
    }

    /// <summary>
    ///   Process the web response.
    /// </summary>
    /// <param name="webRequest">The request object.</param>
    /// <returns>The response data.</returns>
    public string WebResponseGet(HttpWebRequest webRequest)
    {
      StreamReader responseReader = null;
      var responseData = "";

      try
      {
        responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
        responseData = responseReader.ReadToEnd();
      }
      catch (Exception e)
      {
        throw e;
      }
      finally
      {
        webRequest.GetResponse().GetResponseStream().Close();
        responseReader.Close();
        responseReader = null;
      }

      return responseData;
    }

    #region NetworkUpdate

    #endregion

    #region Status Update

    #endregion

    #region Comments

    #endregion

    #region Update

    #endregion

    #region Search

    private static LinkedInUser FindCorrectUserSt(LinkedInUser user)
    {
      return user ?? null;
    }

    private static List<LinkedInActivity> ParseActivitiesData(XContainer element)
    {
      if (element == null)
      {
        return null;
      }
      try
      {
        var query =
          (from job in element.Descendants("activity")
           select new LinkedInActivity { Body = job.Element("body").Value, AppId = job.Element("app-id").Value, });
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseActivitiesData", ex);
        return null;
      }
    }

    private static ObservableCollection<Comment> ParseComments(XElement element)
    {
      if (element == null )
        return null;

      try
      {
        var numberOfComments = Convert.ToInt32(element.Attribute("total").Value.Trim());
        if (numberOfComments == 0)
          return null;

        var query = (from comment in element.Descendants("update-comment") select LinkedInHelper.BuildComment(comment));
        return new ObservableCollection<Comment>(query.ToList());
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseComments", ex);
        return null;
      }
    }

    private static List<LinkedInEducation> ParseEducations(XContainer element)
    {
      if (element == null)
      {
        return null;
      }
      try
      {
        var query = (from user in element.Descendants("education")
                     select
                       new LinkedInEducation
                       {
                         Id = user.Element("id").Value,
                         FieldOfStudy = user.Element("field-of-study").Value,
                         SchoolName = user.Element("school-name").Value,
                       });
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseEducations", ex);
        return null;
      }
    }

    private static List<LinkedInGroup> ParseGroupsData(string data)
    {
      if (string.IsNullOrEmpty(data))
      {
        return null;
      }

      try
      {
        var tr = new StringReader(data);
        var xdoc = XDocument.Load(tr);
        var query = (from user in xdoc.Descendants("member-group")
                     select
                       new LinkedInGroup
                       {
                         Id = user.Element("id").Value,
                         Name = user.Element("name").Value,
                         Url = user.Element("site-group-request").Element("url").Value
                       });
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseGroupsData", ex);
      }

      return null;
    }

    private static LinkedInJob ParseJobData(XContainer element)
    {
      if (element == null)
      {
        return null;
      }
      try
      {
        var query = (from job in element.Descendants("job")
                     select
                       new LinkedInJob
                       {
                         Id = job.Element("id").Value,
                         Title = job.Element("position").Element("title").Value,
                         Compagny = job.Element("company").Element("name").Value,
                         JobPoster = ParseUserDataJob(job.ToString()),
                         Url = job.Element("site-job-request").Element("url").Value
                       });
        return query.First();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseJobData", ex);
        return null;
      }
    }

    private static List<LinkedInPosition> ParsePositions(XContainer element)
    {
      if (element == null)
      {
        return null;
      }
      try
      {
        var query = (from user in element.Descendants("position")
                     select
                       new LinkedInPosition
                       {
                         Id = int.Parse(user.Element("id").Value),
                         Title = user.Element("title").Value,
                         Summary = user.Element("summary") != null ? user.Element("summary").Value : string.Empty,
                         StartY =
                           user.Element("start-date") != null
                             ? user.Element("start-date").Element("year") != null
                               ? int.Parse(user.Element("start-date").Element("year").Value)
                               : -1
                             : -1,
                         StartM =
                           user.Element("start-date") != null
                             ? user.Element("start-date").Element("month") != null
                               ? int.Parse(user.Element("start-date").Element("month").Value)
                               : -1
                             : -1,
                         EndY =
                           user.Element("end-date") != null
                             ? user.Element("end-date").Element("year") != null
                               ? int.Parse(user.Element("end-date").Element("year").Value)
                               : -1
                             : -1,
                         EndM =
                           user.Element("end-date") != null
                             ? user.Element("end-date").Element("month") != null
                               ? int.Parse(user.Element("end-date").Element("month").Value)
                               : -1
                             : -1,
                         IsCurrent = user.Element("is-current").Value == "0" ? false : true,
                         Compagny = user.Element("company") != null ? user.Element("company").Element("name").Value : "",
                       });
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParsePositions", ex);
        return null;
      }
    }

    private static List<LinkedInRecommendation> ParseRecommendationsData(XContainer element)
    {
      if (element == null)
      {
        return null;
      }
      try
      {
        var query = (from job in element.Descendants("recommendation")
                     select
                       new LinkedInRecommendation
                       {
                         Id = job.Element("id") != null ? job.Element("id").Value : string.Empty,
                         Type =
                           job.Element("recommendation-type") != null ? job.Element("recommendation-type").Value : string.Empty,
                         Snippet =
                           job.Element("recommendation-snippet") != null
                             ? job.Element("recommendation-snippet").Value
                             : string.Empty,
                         Recommendee = ParseUserDataRecommendation(job.ToString()),
                         Url = job.Element("web-url") != null ? job.Element("web-url").Value : string.Empty
                       });
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseRecommendationsData", ex);
        return null;
      }
    }

    private static List<LinkedInActivity> ParseUpdateLinkedInActivities(string element, XContainer update)
    {
      try
      {
        switch (element)
        {
        case "APPM":
          return ParseActivitiesData(update);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseUpdateLinkedInActivities", ex);
        return null;
      }
      return null;
    }

    private static List<LinkedInEducation> ParseUpdateLinkedInEducations(string element, XContainer update)
    {
      try
      {
        switch (element)
        {
        case "PROF":
          return ParseEducations(update.Element("update-content").Element("person").Element("educations"));
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseUpdateLinkedInEducations", ex);
        return null;
      }
      return null;
    }

    private static List<LinkedInGroup> ParseUpdateLinkedInGroups(string element, XContainer update)
    {
      try
      {
        switch (element)
        {
        case "JGRP":
          return ParseGroupsData(update.ToString());
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseUpdateLinkedInGroups", ex);
        return null;
      }
      return null;
    }

    private static LinkedInJob ParseUpdateLinkedInJob(string element, XContainer update)
    {
      try
      {
        switch (element)
        {
        case "JOBP":
          return ParseJobData(update);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseUpdateLinkedInJob", ex);
        return null;
      }
      return null;
    }

    private static List<LinkedInRecommendation> ParseUpdateLinkedInRecommendations(string element, XContainer update)
    {
      try
      {
        switch (element)
        {
        case "PREC":
          return ParseRecommendationsData(update);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseUpdateLinkedInRecommendations", ex);
        return null;
      }
      return null;
    }

    private static List<LinkedInUser> ParseUpdateLinkedInUsersSt(string element, XContainer update)
    {
      try
      {
        switch (element)
        {
        case "CONN":
          return
            ParseConnectionsDataSt(
              update.Element("update-content").Element("person").Element("connections").ToString(), false);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseUpdateLinkedInUsers", ex);
        return null;
      }
      return null;
    }

    private static List<LinkedInUrlType> ParseUrl(XContainer element)
    {
      if (element == null)
      {
        return null;
      }
      try
      {
        var query =
          (from user in element.Descendants("member-url")
           select new LinkedInUrlType { UrlSrc = user.Element("url").Value, Name = user.Element("name").Value, });
        return query.ToList();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseUrl", ex);
        return null;
      }
    }

    private static LinkedInUser ParseUserDataJob(string data)
    {
      if (string.IsNullOrEmpty(data))
      {
        return null;
      }

      try
      {
        var tr = new StringReader(data);
        var xdoc = XDocument.Load(tr);
        var query = (from user in xdoc.Descendants("job-poster")
                     select new LinkedInUser
                     {
                       //Id = user.Element("id").Value,
                       Id =
                         user.Element("api-standard-profile-request") != null
                           ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
                           : string.Empty,
                       FirstName = user.Element("first-name").Value,
                       Name = user.Element("last-name").Value,
                       NickName = $"{user.Element("first-name").Value} {user.Element("last-name").Value}",
                       Description = user.Element("headline").Value,
                       Url = user.Element("site-standard-profile-request").Element("url").Value
                     });
        var lst = query.ToList();
        if (lst.Count > 0)
        {
          return lst.First();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseConnectionsData", ex);
      }
      return null;
    }

    private static User ParseUserDataRecommendation(string data)
    {
      if (string.IsNullOrEmpty(data))
      {
        return null;
      }

      var tr = new StringReader(data);
      var xdoc = XDocument.Load(tr);
      var query = (from user in xdoc.Descendants("recommendee")
                   select new LinkedInUser
                   {
                     //Id = user.Element("id").Value,
                     Id =
                       user.Element("api-standard-profile-request") != null
                         ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
                         : string.Empty,
                     FirstName = user.Element("first-name").Value,
                     Name = user.Element("last-name").Value,
                     NickName = $"{user.Element("first-name").Value} {user.Element("last-name").Value}",
                     Description = user.Element("headline").Value,
                     Url = user.Element("site-standard-profile-request").Element("url").Value
                   });
      var lst = query.ToList();
      if (lst.Count > 0)
      {
        return lst.First();
      }
      query = (from user in xdoc.Descendants("recommender")
               select new LinkedInUser
               {
                 //Id = user.Element("id").Value,
                 Id =
                   user.Element("api-standard-profile-request") != null
                     ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
                     : string.Empty,
                 FirstName = user.Element("first-name").Value,
                 Name = user.Element("last-name").Value,
                 NickName = $"{user.Element("first-name").Value} {user.Element("last-name").Value}",
                 Description = user.Element("headline").Value,
                 Url = user.Element("site-standard-profile-request").Element("url").Value
               });
      lst = query.ToList();
      if (lst.Count > 0)
      {
        return lst.First();
      }
      return null;
    }

    private LinkedInUser FindCorrectUser(LinkedInUser user)
    {
      if (user == null)
        return null;

      foreach (var friend in Friends.Where(friend => user.Id == friend.Id))
        return friend;

      return user;
    }

    private List<LinkedInUser> ParseUpdateLinkedInUsers(string element, XContainer update)
    {
      try
      {
        switch (element)
        {
        case "CONN":
          return
            ParseConnectionsData(
              update.Element("update-content").Element("person").Element("connections").ToString(), false);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("ParseUpdateLinkedInUsers", ex);
        return null;
      }
      return null;
    }

    #endregion

   
  }
}
#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Library.BLinkedInLib
{
  public class OAuthLinkedInV2Static : OAuthStatic
  {
    #region Utility Parsing

    #region EnumLinkedInSearchNetwork enum

    public enum EnumLinkedInSearchNetwork
    {
      IN,
      OUT
    }

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

    

    #region Method enum

    public enum Method
    {
      GET,
      POST,
      PUT,
      DELETE
    }

    #endregion

    public static readonly string AccessToken = string.Format("{0}{1}", LinkedInResources.BASE_URL,
      LinkedInResources.ACCESS_TOKEN_PATH);

    public static readonly string Authorize = string.Format("{0}{1}", LinkedInResources.BASE_URL,
      LinkedInResources.AUTHORIZE_PATH);

    public static readonly string RequestToken = string.Format("{0}{1}", LinkedInResources.BASE_URL,
      LinkedInResources.REQUEST_TOKEN_PATH);

    private static string _callBackUrl = "oob";
    private static string _consumerKey = "";
    private static string _consumerSecret = "";
    private static string _oauthVerifier = "";
    private static string _token = "";
    private static string _tokenSecret = "";

    #region Properties

    public static string ConsumerKey
    {
      get { return _consumerKey; }
      set { _consumerKey = value; }
    }

    public static string ConsumerSecret
    {
      get { return _consumerSecret; }
      set { _consumerSecret = value; }
    }

    public static string Token
    {
      get { return _token; }
      set { _token = value; }
    }

    public static string TokenSecret
    {
      get { return _tokenSecret; }
      set { _tokenSecret = value; }
    }

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

    public OAuthLinkedInV2Static(string consumerKey, string consumerSecret, string token, string tokenSecret)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      //Friends = new List<User>();
    }

    /// <summary>
    ///   Get the link to Twitter's authorization page for this application.
    /// </summary>
    /// <returns>The url with a valid request token, or a null string.</returns>
    public static string AuthorizationLinkGet(string consumerKey, string consumerSecret, string callbackUrl,
      out string token, out string tokenSecret)
    {
      TraceHelper.Trace("TwitterLib AuthorizationLinkGet", "Started");
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      CallBackUrl = callbackUrl;
      Token = "";
      TokenSecret = "";
      OAuthVerifier = "";
      token = "";
      tokenSecret = "";
      string ret = null;
      var response = OAuthWebRequest(Method.GET, RequestToken, String.Empty, false);
      TraceHelper.Trace("TwitterLib AuthorizationLinkGet", response);
      if (response.Length > 0)
      {
        //response contains token and token secret.  We only need the token.
        var qs = HttpUtility.ParseQueryString(response);

        if (qs["oauth_callback_confirmed"] != null)
        {
          if (qs["oauth_callback_confirmed"] != "true")
          {
            throw new Exception("OAuth callback not confirmed.");
          }
        }

        if (qs["oauth_token"] != null)
        {
          ret = Authorize + "?oauth_token=" + qs["oauth_token"];
          Token = qs["oauth_token"];
          token = qs["oauth_token"];
          TokenSecret = qs["oauth_token_secret"];
          tokenSecret = qs["oauth_token_secret"];
        }
      }
      TraceHelper.Trace("TwitterLib AuthorizationLinkGet", ret);

      return ret;
    }

    /// <summary>
    ///   Exchange the request token for an access token.
    /// </summary>
    /// <param name="oauthVerifier"></param>
    public static string AccessTokenGet(string oauthVerifier, string consumerKey, string consumerSecret, string token,
      string tokenSecret, out string tokenSecretFinal)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      OAuthVerifier = oauthVerifier;
      tokenSecretFinal = "";
      var response = OAuthWebRequest(Method.GET, AccessToken, String.Empty, false);
      TraceHelper.Trace("Twitterlib", response);
      if (response.Length > 0)
      {
        //Store the Token and Token Secret
        var qs = HttpUtility.ParseQueryString(response);
        if (qs["oauth_token"] != null)
        {
          TraceHelper.Trace("Token", qs["oauth_token"]);
          Token = qs["oauth_token"];
          token = qs["oauth_token"];
        }
        if (qs["oauth_token_secret"] != null)
        {
          TraceHelper.Trace("TokenSecret", qs["oauth_token_secret"]);
          TokenSecret = qs["oauth_token_secret"];
          tokenSecretFinal = qs["oauth_token_secret"];
        }
      }
      return Token;
    }

    /// <summary>
    ///   Exchange the request token for an access token.
    /// </summary>
    /// <param name="oauthVerifier"></param>
    public string AccessTokenGet(string oauthVerifier)
    {
      OAuthVerifier = oauthVerifier;
      var response = OAuthWebRequest(Method.GET, AccessToken, String.Empty, false);
      TraceHelper.Trace("Twitterlib", response);
      if (response.Length > 0)
      {
        //Store the Token and Token Secret
        var qs = HttpUtility.ParseQueryString(response);
        if (qs["oauth_token"] != null)
        {
          TraceHelper.Trace("Token", qs["oauth_token"]);
          Token = qs["oauth_token"];
        }
        if (qs["oauth_token_secret"] != null)
        {
          TraceHelper.Trace("TokenSecret", qs["oauth_token_secret"]);
          TokenSecret = qs["oauth_token_secret"];
        }
      }
      return Token;
    }

    /// <summary>
    ///   Submit a web request using oAuth.
    /// </summary>
    /// <param name="method">GET or POST</param>
    /// <param name="url">The full url, including the querystring.</param>
    /// <param name="postData">Data to post (querystring format)</param>
    /// <param name="useHeader"></param>
    /// <returns>The web server response.</returns>
    public static string OAuthWebRequest(Method method, string url, string postData, bool useHeader)
    {
      string outUrl;
      string querystring;


      //Setup postData for signing.
      //Add the postData to the querystring.
      //if (method == Method.POST)
      //{
      //  if (postData.Length > 0)
      //  {
      //    //Decode the parameters and re-encode using the oAuth UrlEncode method.
      //    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
      //    postData = "";
      //    foreach (string key in qs.AllKeys)
      //    {
      //      if (postData.Length > 0)
      //      {
      //        postData += "&";
      //      }
      //      qs[key] = HttpUtility.UrlDecode(qs[key]);
      //      qs[key] = UrlEncode(qs[key]);
      //      postData += key + "=" + qs[key];
      //    }
      //    if (url.IndexOf("?") > 0)
      //    {
      //      url += "&";
      //    }
      //    else
      //    {
      //      url += "?";
      //    }
      //    url += postData;
      //  }
      //}

      var uri = new Uri(url);

      var nonce = GenerateNonce();
      var timeStamp = GenerateTimeStamp();

      //Generate Signature
      var sig = GenerateSignature(uri,
        ConsumerKey,
        ConsumerSecret,
        Token,
        TokenSecret,
        CallBackUrl,
        OAuthVerifier,
        method.ToString(),
        timeStamp,
        nonce,
        out outUrl,
        out querystring);

      querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

      //Convert the querystring to postData
      //if (method == Method.POST)
      //{
      //  postData = querystring;
      //  querystring = "";
      //}

      if (querystring.Length > 0)
      {
        outUrl += "?";
      }
      return useHeader
        ? WebRequestHeader(method, outUrl + querystring, postData, sig, timeStamp, nonce)
        : WebRequest(method, outUrl + querystring, postData);
    }

    /// <summary>
    ///   Web Request Wrapper
    /// </summary>
    /// <param name="method">Http Method</param>
    /// <param name="url">Full url to the web resource</param>
    /// <param name="postData">Data to post in querystring format</param>
    /// <returns>The web server response.</returns>
    public static string WebRequest(Method method, string url, string postData)
    {
      StreamWriter requestWriter;

      var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
      if (webRequest != null)
      {
        webRequest.Method = method.ToString();
        webRequest.ServicePoint.Expect100Continue = false;
        if (method == Method.POST)
        {
          webRequest.ContentType = "application/x-www-form-urlencoded";

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
        if (method == Method.PUT)
        {
          webRequest.ContentType = "application/xml";

          try
          {
            var encoding = new ASCIIEncoding();
            var arr = encoding.GetBytes(postData);
            webRequest.Method = "PUT";
            webRequest.ContentLength = arr.Length;
            webRequest.KeepAlive = true;
            var dataStream = webRequest.GetRequestStream();
            dataStream.Write(arr, 0, arr.Length);
            dataStream.Close();
          }
          catch (Exception ex)
          {
            TraceHelper.Trace("LinkedIn WebRequest", ex);
          }
        }
      }
      return WebResponseGet(webRequest);
    }

    private static string BuildOAuthHeader(string consumerKey, string nonce, string signature, string signatureMethod,
      string timestamp, string token)
    {
      var sb = new StringBuilder();
      sb.Append("OAuth oauth_consumer_key=").Append(HttpUtility.UrlEncode(consumerKey)).Append(",");
      sb.Append("oauth_nonce=").Append(nonce).Append(",");
      sb.Append("oauth_signature=").Append(HttpUtility.UrlEncode(signature)).Append(",");
      sb.Append("oauth_signature_method=").Append(signatureMethod).Append(",");
      sb.Append("oauth_timestamp=").Append(timestamp).Append(",");
      sb.Append("oauth_token=").Append(token).Append(",");
      sb.Append("oauth_version=").Append("1.0");
      return sb.ToString();
    }

    public static string WebRequestHeader(Method method, string url, string postData, string sig, string stamp,
      string nonce)
    {
      var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
      if (webRequest != null)
      {
        webRequest.Method = method.ToString();
        var authHeader = BuildOAuthHeader(ConsumerKey, nonce, sig, "HMAC-SHA1", stamp, Token);
        webRequest.Headers.Add("Authorization", authHeader);
        webRequest.ServicePoint.Expect100Continue = false;
        if (method == Method.POST)
        {
          try
          {
            var encoding = new ASCIIEncoding();
            var arr = encoding.GetBytes(postData);
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml";
            webRequest.ContentLength = arr.Length;
            webRequest.KeepAlive = true;
            var dataStream = webRequest.GetRequestStream();
            dataStream.Write(arr, 0, arr.Length);
            dataStream.Close();
            var response = (HttpWebResponse) webRequest.GetResponse();
            return response.StatusCode.ToString();
          }
          catch (Exception ex)
          {
            TraceHelper.Trace("LinkedIn WebRequestHeader", ex);
          }
        }
        if (method == Method.PUT)
        {
          try
          {
            var encoding = new ASCIIEncoding();
            var arr = encoding.GetBytes(postData);
            webRequest.Method = "PUT";
            webRequest.ContentType = "text/plain";
            webRequest.ContentLength = arr.Length;
            webRequest.KeepAlive = true;
            var dataStream = webRequest.GetRequestStream();
            dataStream.Write(arr, 0, arr.Length);
            dataStream.Close();
            var response = (HttpWebResponse) webRequest.GetResponse();
            return response.StatusCode.ToString();
          }
          catch (Exception ex)
          {
            TraceHelper.Trace("LinkedIn WebRequestHeader", ex);
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
      StreamReader responseReader = null;
      var responseData = "";

      try
      {
        responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
        responseData = responseReader.ReadToEnd();
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

    /*private List<Entry> ParseNetworkUpdateData(string result)
    {
      if (string.IsNullOrEmpty(result))
      {
        return null;
      }

      var tr = new StringReader(result);
      var xdoc = XDocument.Load(tr);
      var query = (from update in xdoc.Descendants("update")
                   select new Entry()
                            {
                              Id = update.Element("timestamp").Value,
                              Type = EnumType.LinkedIn,
                              AppId = update.Element("update-type").Value,
                              PubDate = new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(update.Element("timestamp").Value)),
                              User = FindCorrectUser(ParseUserData(update.ToString()))
                              //LinkedIndata = 
                   });
      return query.ToList();
    }*/

    /*private static User FindCorrectUser(User user)
    {
      if (user == null)
      {
        return null;
      }
      foreach (User friend in Friends)
      {
        if (user.Id == friend.Id)
        {
          return friend;
        }
      }
      return user;
    }*/

    #endregion

    #region ProfileMethods

    public static User GetCurrentProfile(string consumerKey, string consumerSecret, string token, string tokenSecret)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      return GetCurrentProfile();
    }

    public static User GetCurrentProfile()
    {
      return
        OAuthLinkedInV2.ParseUserData(OAuthWebRequest(Method.GET, LinkedInResources.PROFIL_CURRENT_URL, String.Empty,
          true));
    }

    #endregion

    #region Connections

    public static List<LinkedInUser> GetConnections(string consumerKey, string consumerSecret, string token,
      string tokenSecret)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      return GetConnections();
    }

    public static List<LinkedInUser> GetConnections()
    {
      return
        OAuthLinkedInV2.ParseConnectionsDataSt(
          OAuthWebRequest(Method.GET, LinkedInResources.CONNECTIONS_CURRENT_URL, String.Empty, true), true);
    }

    #endregion

    #region NetworkUpdate

    public static List<LinkedInEntry> GetNetworkUpdate(string consumerKey, string consumerSecret, string token,
      string tokenSecret, EnumLinkedInUpdateType type, int nbToGet,
      int offset, long after, long before)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      return GetNetworkUpdate(type, nbToGet, offset, after, before);
    }

    public static List<LinkedInEntry> GetNetworkUpdate(EnumLinkedInUpdateType type, int nbToGet, int offset, long after,
      long before)
    {
      var url = LinkedInResources.NETWORK_UPDATES_URL;
      url = string.Format("{0}type={1}&", url, type);
      url = string.Format("{0}count={1}", url, nbToGet);
      if (offset > 0)
      {
        url = string.Format("{0}&start={1}", url, offset);
      }
      if (after > 0)
      {
        url = string.Format("{0}&after={1}", url, after);
      }
      if (before > 0)
      {
        url = string.Format("{0}&before={1}", url, before);
      }
      return OAuthLinkedInV2.ParseNetworkUpdateDataSt(OAuthWebRequest(Method.GET, url, String.Empty, true));
    }

    public static List<LinkedInEntry> GetNetworkUpdate(string consumerKey, string consumerSecret, string token,
      string tokenSecret, List<EnumLinkedInUpdateType> types,
      int nbToGet, int offset, long after, long before)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      return GetNetworkUpdate(types, nbToGet, offset, after, before);
    }

    public static List<LinkedInEntry> GetNetworkUpdate(List<EnumLinkedInUpdateType> types, int nbToGet, int offset,
      long after,
      long before)
    {
      var url = LinkedInResources.NETWORK_UPDATES_URL;
      if (types != null)
      {
        foreach (var type in types)
        {
          url = string.Format("{0}type={1}&", url, type);
        }
      }
      url = string.Format("{0}count={1}", url, nbToGet);
      if (offset > 0)
      {
        url = string.Format("{0}&start={1}", url, offset);
      }
      if (after > 0)
      {
        url = string.Format("{0}&after={1}", url, after);
      }
      if (before > 0)
      {
        url = string.Format("{0}&before={1}", url, before);
      }
      return OAuthLinkedInV2.ParseNetworkUpdateDataSt(OAuthWebRequest(Method.GET, url, String.Empty, true));
    }

    #endregion

    #region Status Update

    public static bool SetStatus(string consumerKey, string consumerSecret, string token, string tokenSecret,
      string status)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      return SetStatus(status);
    }

    public static bool SetStatus(string status)
    {
      var txt = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><current-status>{0}</current-status>",
        status);

      //string result = OAuthWebRequest(Method.PUT, Resources.STATUS_URL, txt, true);
      return OAuthWebRequest(Method.PUT, LinkedInResources.STATUS_URL, txt, true) == "NoContent";
    }

    #endregion

    #region Comments

    public static bool SetComments(string consumerKey, string consumerSecret, string token, string tokenSecret,
      string status, string id)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      return SetComments(status, id);
    }

    public static bool SetComments(string status, string id)
    {
      var txt =
        string.Format("<?xml version='1.0' encoding='UTF-8'?><update-comment><comment>{0}</comment></update-comment>",
          status);

      //string result = OAuthWebRequest(Method.PUT, Resources.STATUS_URL, txt, true);
      return
        OAuthWebRequest(Method.POST, LinkedInResources.COMMENT_URL_START + id + LinkedInResources.COMMENT_URL_END, txt,
          true) == "Created";
    }

    #endregion

    #region Search

    public static List<LinkedInUser> Search(string consumerKey, string consumerSecret, string token, string tokenSecret,
      string keywords, string name, string company, bool currentCompany,
      string title,
      bool currentTitle,
      LinkedInIndustryCode industryCode, EnumLinkedInSearchNetwork network,
      int start, int count, EnumLinkedInSearchSort sort)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      return Search(keywords, name, company, currentCompany, title, currentTitle, industryCode, network, start, count,
        sort);
    }

    public static List<LinkedInUser> Search(string keywords, string name, string company, bool currentCompany,
      string title,
      bool currentTitle,
      LinkedInIndustryCode industryCode, EnumLinkedInSearchNetwork network,
      int start, int count, EnumLinkedInSearchSort sort)
    {
      var url = LinkedInResources.SEARCH_URL;
      if (!string.IsNullOrEmpty(keywords))
      {
        url += string.Format("keywords={0}", keywords);
      }
      if (!string.IsNullOrEmpty(name))
      {
        url += string.Format("&name={0}", name);
      }
      if (!string.IsNullOrEmpty(company))
      {
        url += string.Format("&company={0}", company);
        url += string.Format("&current-company={0}", currentCompany);
      }
      if (!string.IsNullOrEmpty(title))
      {
        url += string.Format("&title={0}", title);
        url += string.Format("&current-title={0}", currentTitle);
      }
      if (industryCode != null)
      {
        url += string.Format("&industry-code={0}", industryCode.Code);
      }
      url += string.Format("&network={0}", network.ToString().ToLower());
      url += string.Format("&start={0}", start);
      url += string.Format("&count={0}", count);
      url += string.Format("&sort-criteria={0}", sort);
      return OAuthLinkedInV2.ParseSearchResult(OAuthWebRequest(Method.GET, url, string.Empty, true));
    }

    #endregion

    public static LinkedInUser GetInfoUser(string consumerKey, string consumerSecret, string token, string tokenSecret,
      string id)
    {
      ConsumerKey = consumerKey;
      ConsumerSecret = consumerSecret;
      Token = token;
      TokenSecret = tokenSecret;
      return GetInfoUser(id);
    }

    public static LinkedInUser GetInfoUser(string id)
    {
      var user =
        OAuthLinkedInV2.ParseStandardUserData(OAuthWebRequest(Method.GET,
          string.Format("{0}id={1}:full", LinkedInResources.PROFIL_URL,
            id),
          string.Empty, true));
      return user;
    }
  }
}
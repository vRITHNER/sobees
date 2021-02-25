//#region

//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Web;
//using System.Xml.Linq;
//using Sobees.Library.BGenericLib;
//using Sobees.Tools.Logging;

//#endregion

//namespace Sobees.Library.BLinkedInLib
//{
//  public class LinkedInOAuth_OLD : OAuthBaseV2
//  {
//    #region Utility Parsing

//    #region EnumLinkedInSearchNetwork enum

//    public enum EnumLinkedInSearchNetwork
//    {
//      IN,
//      OUT
//    }

//    #endregion

//    #region EnumLinkedInSearchSort enum

//    public enum EnumLinkedInSearchSort
//    {
//      ctx,
//      endorsers,
//      distance,
//      relevance
//    }

//    #endregion

//    #region EnumLinkedInUpdateType enum

//    public enum EnumLinkedInUpdateType
//    {
//      APPS,
//      CONN,
//      JOBS,
//      JGRP,
//      PICT,
//      RECU,
//      PRFU,
//      QSTN,
//      STAT
//    }

//    #endregion

//    #region Method enum

//    public enum Method
//    {
//      GET,
//      POST,
//      PUT,
//      DELETE
//    }

//    #endregion

//    public readonly string ACCESS_TOKEN = string.Format("{0}{1}", LinkedInResources.BASE_URL, LinkedInResources.ACCESS_TOKEN_PATH);
//    public readonly string AUTHORIZE = string.Format("{0}{1}", LinkedInResources.BASE_URL, LinkedInResources.AUTHORIZE_PATH);
//    public readonly string REQUESTTOKEN = string.Format("{0}{1}", LinkedInResources.BASE_URL, LinkedInResources.REQUEST_TOKEN_PATH);
//    private string _callBackUrl = "oob";

//    private string _consumerKey = "";
//    private string _consumerSecret = "";
//    private string _oauthVerifier = "";
//    private string _token = "";
//    private string _tokenSecret = "";

//    #region Properties

//    public string ConsumerKey
//    {
//      get { return _consumerKey; }
//      set { _consumerKey = value; }
//    }

//    public string ConsumerSecret
//    {
//      get { return _consumerSecret; }
//      set { _consumerSecret = value; }
//    }

//    public string Token
//    {
//      get { return _token; }
//      set { _token = value; }
//    }

//    public string TokenSecret
//    {
//      get { return _tokenSecret; }
//      set { _tokenSecret = value; }
//    }

//    public string OAuthVerifier
//    {
//      get { return _oauthVerifier; }
//      set { _oauthVerifier = value; }
//    }

//    public string CallBackUrl
//    {
//      get { return _callBackUrl; }
//      set { _callBackUrl = value; }
//    }

//    public List<LinkedInUser> Friends { get; set; }

//    #endregion

//    public LinkedInOAuth_OLD(string consumerKey, string consumerSecret, string token, string tokenSecret)
//    {
//      ConsumerKey = consumerKey;
//      ConsumerSecret = consumerSecret;
//      Token = token;
//      TokenSecret = tokenSecret;
//      Friends = new List<LinkedInUser>();
//    }

//    /// <summary>
//    ///   Get the link to LinkedIn's authorization page for this application.
//    /// </summary>
//    /// <returns>The url with a valid request token, or a null string.</returns>
//    public string AuthorizationLinkGet(string consumerKey, string consumerSecret, string callbackUrl)
//    {
//      TraceHelper.Trace("TwitterLib AuthorizationLinkGet", "Started");
//      ConsumerKey = consumerKey;
//      ConsumerSecret = consumerSecret;
//      CallBackUrl = callbackUrl;
//      Token = "";
//      TokenSecret = "";
//      OAuthVerifier = "";
//      string ret = null;
//      var response = OAuthWebRequest(Method.GET, REQUESTTOKEN, String.Empty);
//      TraceHelper.Trace("TwitterLib AuthorizationLinkGet", response);
//      if (response.Length > 0)
//      {
//        //response contains token and token secret.  We only need the token.
//        var qs = HttpUtility.ParseQueryString(response);

//        if (qs["oauth_callback_confirmed"] != null)
//        {
//          if (qs["oauth_callback_confirmed"] != "true")
//          {
//            throw new Exception("OAuth callback not confirmed.");
//          }
//        }

//        if (qs["oauth_token"] != null)
//        {
//          ret = AUTHORIZE + "?oauth_token=" + qs["oauth_token"];
//          Token = qs["oauth_token"];
//          TokenSecret = qs["oauth_token_secret"];
//        }
//      }
//      TraceHelper.Trace("TwitterLib AuthorizationLinkGet", ret);
//      return ret;
//    }

//    /// <summary>
//    ///   Exchange the request token for an access token.
//    /// </summary>
//    /// <param name = "oauthVerifier"></param>
//    public string AccessTokenGet(string oauthVerifier)
//    {
//      OAuthVerifier = oauthVerifier;
//      var response = OAuthWebRequest(Method.GET, ACCESS_TOKEN, String.Empty);
//      TraceHelper.Trace("Twitterlib", response);
//      if (response.Length > 0)
//      {
//        //Store the Token and Token Secret
//        var qs = HttpUtility.ParseQueryString(response);
//        if (qs["oauth_token"] != null)
//        {
//          TraceHelper.Trace("Token", qs["oauth_token"]);
//          Token = qs["oauth_token"];
//        }
//        if (qs["oauth_token_secret"] != null)
//        {
//          TraceHelper.Trace("TokenSecret", qs["oauth_token_secret"]);
//          TokenSecret = qs["oauth_token_secret"];
//        }
//      }
//      return Token;
//    }

//    /// <summary>
//    ///   Submit a web request using oAuth.
//    /// </summary>
//    /// <param name = "method">GET or POST</param>
//    /// <param name = "url">The full url, including the querystring.</param>
//    /// <param name = "postData">Data to post (querystring format)</param>
//    /// <returns>The web server response.</returns>
//    public string OAuthWebRequest(Method method, string url, string postData)
//    {
//      var outUrl = "";
//      var querystring = "";
//      var ret = "";


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
//            {
//              postData += "&";
//            }
//            qs[key] = HttpUtility.UrlDecode(qs[key]);
//            qs[key] = UrlEncode(qs[key]);
//            postData += key + "=" + qs[key];
//          }
//          if (url.IndexOf("?") > 0)
//          {
//            url += "&";
//          }
//          else
//          {
//            url += "?";
//          }
//          url += postData;
//        }
//      }

//      var uri = new Uri(url);

//      var nonce = GenerateNonce();
//      var timeStamp = GenerateTimeStamp();

//      //Generate Signature
//      var sig = GenerateSignature(uri, ConsumerKey, ConsumerSecret, Token, TokenSecret, method.ToString(), timeStamp, nonce, out outUrl, out querystring);


//      querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

//      //Convert the querystring to postData
//      if (method == Method.POST)
//      {
//        postData = querystring;
//        querystring = "";
//      }

//      if (querystring.Length > 0)
//      {
//        outUrl += "?";
//      }

//      if (method == Method.POST || method == Method.GET)
//        ret = WebRequest(method, outUrl + querystring, postData);
//      //else if (method == Method.PUT)
//      //ret = WebRequestWithPut(outUrl + querystring, postData);
//      return ret;
//    }


//    ///// <summary>
//    ///// Web Request Wrapper
//    ///// </summary>
//    ///// <param name="method">Http Method</param>
//    ///// <param name="url">Full url to the web resource</param>
//    ///// <param name="postData">Data to post in querystring format</param>
//    ///// <returns>The web server response.</returns>
//    //public string WebRequest(Method method, string url, string postData)
//    //{
//    //  StreamWriter requestWriter;

//    //  var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
//    //  if (webRequest != null)
//    //  {
//    //    webRequest.Method = method.ToString();
//    //    webRequest.ServicePoint.Expect100Continue = false;
//    //    if (method == Method.POST)
//    //    {
//    //      webRequest.ContentType = "application/x-www-form-urlencoded";

//    //      //POST the data.
//    //      requestWriter = new StreamWriter(webRequest.GetRequestStream(), Encoding.UTF8);
//    //      try
//    //      {
//    //        requestWriter.Write(postData);
//    //      }
//    //      finally
//    //      {
//    //        requestWriter.Close();
//    //      }
//    //    }
//    //    if (method == Method.PUT)
//    //    {
//    //      webRequest.ContentType = "application/xml";

//    //      try
//    //      {
//    //        var encoding = new ASCIIEncoding();
//    //        byte[] arr = encoding.GetBytes(postData);
//    //        webRequest.Method = "PUT";
//    //        webRequest.ContentLength = arr.Length;
//    //        webRequest.KeepAlive = true;
//    //        Stream dataStream = webRequest.GetRequestStream();
//    //        dataStream.Write(arr, 0, arr.Length);
//    //        dataStream.Close();
//    //      }
//    //      catch (Exception ex)
//    //      {
//    //        TraceHelper.Trace(this, ex);
//    //      }
//    //    }
//    //  }
//    //  return WebResponseGet(webRequest);
//    //}

//    ///// <summary>
//    ///// Submit a web request using oAuth.
//    ///// </summary>
//    ///// <param name="method">GET or POST</param>
//    ///// <param name="url">The full url, including the querystring.</param>
//    ///// <param name="postData">Data to post (querystring format)</param>
//    ///// <returns>The web server response.</returns>
//    //public string oAuthWebRequest(Method method, string url, string postData)
//    //{
//    //  string outUrl = "";
//    //  string querystring = "";
//    //  string ret = "";


//    //  //Setup postData for signing.
//    //  //Add the postData to the querystring.
//    //  if (method == Method.POST)
//    //  {
//    //    if (postData.Length > 0)
//    //    {
//    //      //Decode the parameters and re-encode using the oAuth UrlEncode method.
//    //      NameValueCollection qs = HttpUtility.ParseQueryString(postData);
//    //      postData = "";
//    //      foreach (string key in qs.AllKeys)
//    //      {
//    //        if (postData.Length > 0)
//    //        {
//    //          postData += "&";
//    //        }
//    //        qs[key] = HttpUtility.UrlDecode(qs[key]);
//    //        qs[key] = this.UrlEncode(qs[key]);
//    //        postData += key + "=" + qs[key];

//    //      }
//    //      if (url.IndexOf("?") > 0)
//    //      {
//    //        url += "&";
//    //      }
//    //      else
//    //      {
//    //        url += "?";
//    //      }
//    //      url += postData;
//    //    }
//    //  }

//    //  Uri uri = new Uri(url);

//    //  string nonce = this.GenerateNonce();
//    //  string timeStamp = this.GenerateTimeStamp();

//    //  //Generate Signature
//    //  string sig = this.GenerateSignature(uri,
//    //      this.ConsumerKey,
//    //      this.ConsumerSecret,
//    //      this.Token,
//    //      this.TokenSecret,
//    //      method.ToString(),
//    //      timeStamp,
//    //      nonce,
//    //      out outUrl,
//    //      out querystring);


//    //  querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

//    //  //Convert the querystring to postData
//    //  if (method == Method.POST)
//    //  {
//    //    postData = querystring;
//    //    querystring = "";
//    //  }

//    //  if (querystring.Length > 0)
//    //  {
//    //    outUrl += "?";
//    //  }

//    //  if (method == Method.POST || method == Method.GET)
//    //    ret = WebRequest(method, outUrl + querystring, postData);
//    //  //else if (method == Method.PUT)
//    //  //ret = WebRequestWithPut(outUrl + querystring, postData);
//    //  return ret;
//    //}

//    private static string BuildOAuthHeader(string consumerKey, string nonce, string signature, string signatureMethod, string timestamp, string token)
//    {
//      var sb = new StringBuilder();
//      sb.Append("OAuth oauth_consumer_key=").Append(HttpUtility.UrlEncode(consumerKey)).Append(",");
//      sb.Append("oauth_nonce=").Append(nonce).Append(",");
//      sb.Append("oauth_signature=").Append(HttpUtility.UrlEncode(signature)).Append(",");
//      sb.Append("oauth_signature_method=").Append(signatureMethod).Append(",");
//      sb.Append("oauth_timestamp=").Append(timestamp).Append(",");
//      sb.Append("oauth_token=").Append(token).Append(",");
//      sb.Append("oauth_version=").Append("1.0");
//      return sb.ToString();
//    }


//    /// <summary>
//    ///   WebRequestWithPut
//    /// </summary>
//    /// <param name = "method">WebRequestWithPut</param>
//    /// <param name = "url"></param>
//    /// <param name = "postData"></param>
//    /// <returns></returns>
//    public string APIWebRequest(string method, string url, string postData)
//    {
//      var uri = new Uri(url);
//      var nonce = GenerateNonce();
//      var timeStamp = GenerateTimeStamp();

//      string outUrl, querystring;

//      //Generate Signature
//      var sig = GenerateSignature(uri, ConsumerKey, ConsumerSecret, Token, TokenSecret, method, timeStamp, nonce, out outUrl, out querystring);

//      //querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);
//      //NameValueCollection qs = HttpUtility.ParseQueryString(querystring);

//      //string finalGetUrl = outUrl + "?" + querystring;

//      HttpWebRequest webRequest = null;

//      //webRequest = System.Net.WebRequest.Create(finalGetUrl) as HttpWebRequest;
//      webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
//      //webRequest.ContentType = "text/xml";
//      webRequest.Method = method;
//      webRequest.Credentials = CredentialCache.DefaultCredentials;
//      webRequest.AllowWriteStreamBuffering = true;

//      webRequest.PreAuthenticate = true;
//      webRequest.ServicePoint.Expect100Continue = false;
//      ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

//      webRequest.Headers.Add(
//        "Authorization",
//        "OAuth realm=\"http://api.linkedin.com/\",oauth_consumer_key=\"" + ConsumerKey + "\",oauth_token=\"" + Token +
//        "\",oauth_signature_method=\"HMAC-SHA1\",oauth_signature=\"" + HttpUtility.UrlEncode(sig) + "\",oauth_timestamp=\"" + timeStamp + "\",oauth_nonce=\"" +
//        nonce + "\",oauth_verifier=\"" + Verifier + "\", oauth_version=\"1.0\"");
//      //webRequest.Headers.Add("Authorization", "OAuth oauth_nonce=\"" + nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + timeStamp + "\", oauth_consumer_key=\"" + this.ConsumerKey + "\", oauth_token=\"" + this.Token + "\", oauth_signature=\"" + HttpUtility.UrlEncode(sig) + "\", oauth_version=\"1.0\"");
//      if (postData != null)
//      {
//        var fileToSend = Encoding.UTF8.GetBytes(postData);
//        webRequest.ContentLength = fileToSend.Length;

//        var reqStream = webRequest.GetRequestStream();

//        reqStream.Write(fileToSend, 0, fileToSend.Length);
//        reqStream.Close();
//      }

//      var returned = WebResponseGet(webRequest);

//      return returned;
//    }

//    /// <summary>
//    ///   Web Request Wrapper
//    /// </summary>
//    /// <param name = "method">Http Method</param>
//    /// <param name = "url">Full url to the web resource</param>
//    /// <param name = "postData">Data to post in querystring format</param>
//    /// <returns>The web server response.</returns>
//    public string WebRequest(Method method, string url, string postData)
//    {
//      HttpWebRequest webRequest = null;
//      StreamWriter requestWriter = null;
//      var responseData = "";

//      webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
//      webRequest.Method = method.ToString();
//      webRequest.ServicePoint.Expect100Continue = false;
//      //webRequest.UserAgent  = "Identify your application please.";
//      //webRequest.Timeout = 20000;

//      if (method == Method.POST)
//      {
//        webRequest.ContentType = "application/x-www-form-urlencoded";

//        //POST the data.
//        requestWriter = new StreamWriter(webRequest.GetRequestStream());
//        try
//        {
//          requestWriter.Write(postData);
//        }
//        catch
//        {
//          throw;
//        }
//        finally
//        {
//          requestWriter.Close();
//          requestWriter = null;
//        }
//      }

//      responseData = WebResponseGet(webRequest);

//      webRequest = null;

//      return responseData;
//    }

//    public string WebRequestHeader(Method method, string url, string postData, string sig, string stamp, string nonce)
//    {
//      var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
//      if (webRequest != null)
//      {
//        webRequest.Method = method.ToString();
//        var authHeader = BuildOAuthHeader(ConsumerKey, nonce, sig, "HMAC-SHA1", stamp, Token);
//        webRequest.Headers.Add("Authorization", authHeader);
//        webRequest.ServicePoint.Expect100Continue = false;
//        if (method == Method.POST)
//        {
//          try
//          {
//            var encoding = new ASCIIEncoding();
//            var arr = encoding.GetBytes(postData);
//            webRequest.Method = "POST";
//            webRequest.ContentType = "text/xml";
//            webRequest.ContentLength = arr.Length;
//            webRequest.KeepAlive = true;
//            var dataStream = webRequest.GetRequestStream();
//            dataStream.Write(arr, 0, arr.Length);
//            dataStream.Close();
//            var response = (HttpWebResponse) webRequest.GetResponse();
//            return response.StatusCode.ToString();
//          }
//          catch (Exception ex)
//          {
//            TraceHelper.Trace(this, ex);
//          }
//        }
//        if (method == Method.PUT)
//        {
//          try
//          {
//            var encoding = new ASCIIEncoding();
//            var arr = encoding.GetBytes(postData);
//            webRequest.Method = "PUT";
//            webRequest.ContentType = "text/plain";
//            webRequest.ContentLength = arr.Length;
//            webRequest.KeepAlive = true;
//            var dataStream = webRequest.GetRequestStream();
//            dataStream.Write(arr, 0, arr.Length);
//            dataStream.Close();
//            var response = (HttpWebResponse) webRequest.GetResponse();
//            return response.StatusCode.ToString();
//          }
//          catch (Exception ex)
//          {
//            TraceHelper.Trace(this, ex);
//          }
//        }
//      }
//      return WebResponseGet(webRequest);
//    }

//    /// <summary>
//    ///   Process the web response.
//    /// </summary>
//    /// <param name = "webRequest">The request object.</param>
//    /// <returns>The response data.</returns>
//    public string WebResponseGet(HttpWebRequest webRequest)
//    {
//      StreamReader responseReader = null;
//      var responseData = "";

//      try
//      {
//        responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
//        responseData = responseReader.ReadToEnd();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("TwitterLib", ex.Message);
//      }
//      finally
//      {
//        webRequest.GetResponse().GetResponseStream().Close();
//        if (responseReader != null)
//          responseReader.Close();
//      }

//      return responseData;
//    }

//    public static LinkedInUser ParseUserData(string data)
//    {
//      if (string.IsNullOrEmpty(data))
//        return null;


//      try
//      {
//        var tr = new StringReader(data);
//        var xdoc = XDocument.Load(tr);
//        var query = (from user in xdoc.Descendants("person")
//                     select new LinkedInUser
//                              {
////Id = user.Element("id").Value,
//                                Id =
//                                  user.Element("api-standard-profile-request") != null
//                                    ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
//                                    : string.Empty,
//                                FirstName = user.Element("first-name").Value,
//                                Name = user.Element("last-name").Value,
//                                NickName = string.Format("{0} {1}", user.Element("first-name").Value, user.Element("last-name").Value),
//                                Description = user.Element("headline").Value,
//                                Url = user.Element("site-standard-profile-request").Element("url").Value
//                              });
//        var lst = query.ToList();
//        if (lst.Count > 0)
//        {
//          return lst.First();
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseConnectionsData", ex);
//      }
//      return null;
//    }

//    private static LinkedInUser ParseUserDataJob(string data)
//    {
//      if (string.IsNullOrEmpty(data))
//      {
//        return null;
//      }

//      try
//      {
//        var tr = new StringReader(data);
//        var xdoc = XDocument.Load(tr);
//        var query = (from user in xdoc.Descendants("job-poster")
//                     select new LinkedInUser
//                              {
////Id = user.Element("id").Value,
//                                Id =
//                                  user.Element("api-standard-profile-request") != null
//                                    ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
//                                    : string.Empty,
//                                FirstName = user.Element("first-name").Value,
//                                Name = user.Element("last-name").Value,
//                                NickName = string.Format("{0} {1}", user.Element("first-name").Value, user.Element("last-name").Value),
//                                Description = user.Element("headline").Value,
//                                Url = user.Element("site-standard-profile-request").Element("url").Value
//                              });
//        var lst = query.ToList();
//        if (lst.Count > 0)
//        {
//          return lst.First();
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseConnectionsData", ex);
//      }
//      return null;
//    }

//    public static List<LinkedInUser> ParseConnectionsDataSt(string result, bool isFriends)
//    {
//      if (string.IsNullOrEmpty(result))
//      {
//        return null;
//      }

//      try
//      {
//        var tr = new StringReader(result);
//        var xdoc = XDocument.Load(tr);
//        var query = (from user in xdoc.Descendants("person")
//                     select
//                       new LinkedInUser
//                         {
//                           Id =
//                             user.Element("api-standard-profile-request") != null
//                               ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
//                               : user.Element("id") != null ? user.Element("id").Value : null,
//                           FirstName = user.Element("first-name").Value,
//                           Name = user.Element("last-name").Value,
//                           NickName = string.Format("{0} {1}", user.Element("first-name").Value, user.Element("last-name").Value),
//                           Description = user.Element("headline").Value,
//                           Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
//                           //                     FacebookCountry = user.Element("location").Element("country").Element("code").Value,
//                           ProfileImgUrl =
//                             user.Element("picture-url") != null
//                               ? user.Element("picture-url").Value
//                               : "http://static03.linkedin.com/img/icon/icon_no_photo_40x40.png",
//                           Url = user.Element("site-standard-profile-request").Element("url").Value
//                         });
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseConnectionsData", ex);
//        return null;
//      }
//    }

//    public List<LinkedInUser> ParseConnectionsData(string result, bool isFriends)
//    {
//      if (string.IsNullOrEmpty(result))
//      {
//        return null;
//      }
//      try
//      {
//        var tr = new StringReader(result);
//        var xdoc = XDocument.Load(tr);
//        var query = (from user in xdoc.Descendants("person")
//                     select
//                       new LinkedInUser
//                         {
//                           Id =
//                             user.Element("api-standard-profile-request") != null
//                               ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
//                               : user.Element("id") != null ? user.Element("id").Value : null,
//                           FirstName = user.Element("first-name").Value,
//                           Name = user.Element("last-name").Value,
//                           NickName = string.Format("{0} {1}", user.Element("first-name").Value, user.Element("last-name").Value),
//                           Description = user.Element("headline").Value,
//                           Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
//                           //                     FacebookCountry = user.Element("location").Element("country").Element("code").Value,
//                           ProfileImgUrl =
//                             user.Element("picture-url") != null
//                               ? user.Element("picture-url").Value
//                               : "http://static03.linkedin.com/img/icon/icon_no_photo_40x40.png",
//                           Url = user.Element("site-standard-profile-request").Element("url").Value
//                         });
//        if (isFriends)
//        {
//          foreach (var user in query)
//          {
//            if (!Friends.Contains(user))
//            {
//              Friends.Add(user);
//            }
//          }
//        }
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace(this, ex);
//        return null;
//      }
//    }

//    public static List<LinkedInEntry> ParseNetworkUpdateDataSt(string result)
//    {
//      if (string.IsNullOrEmpty(result))
//      {
//        return null;
//      }

//      try
//      {
//        var tr = new StringReader(result);
//        var xdoc = XDocument.Load(tr);
//        var query = (from update in xdoc.Descendants("update")
//                     select
//                       new LinkedInEntry
//                         {
//                           Id = update.Element("update-key") != null ? update.Element("update-key").Value : update.Element("timestamp").Value,
//                           Type = EnumType.LinkedIn,
//                           UpdateType = update.Element("update-type").Value,
//                           PubDate =
//                             update.Element("timestamp") != null
//                               ? new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(update.Element("timestamp").Value))
//                               : new DateTime(),
//                           User = FindCorrectUserSt(ParseUserData(update.ToString())),
//                           Users = ParseUpdateLinkedInUsersSt(update.Element("update-type").Value, update),
//                           Educations = ParseUpdateLinkedInEducations(update.Element("update-type").Value, update),
//                           Job = ParseUpdateLinkedInJob(update.Element("update-type").Value, update),
//                           Groups = ParseUpdateLinkedInGroups(update.Element("update-type").Value, update),
//                           Activities = ParseUpdateLinkedInActivities(update.Element("update-type").Value, update),
//                           Recommendations = ParseUpdateLinkedInRecommendations(update.Element("update-type").Value, update),
//                           CanPost = bool.Parse(update.Element("is-commentable").Value) ? 1 : 0,
//                           Title =
//                             update.Element("update-content").Element("person") != null
//                               ? update.Element("update-content").Element("person").Element("current-status") != null
//                                   ? update.Element("update-content").Element("person").Element("current-status").Value
//                                   : null
//                               : null,
//                           Comments = ParseComments(update.Element("update-comments"))
//                         });
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParsingNetworkRUpdateData", ex);
//        return null;
//      }
//    }

//    public List<LinkedInEntry> ParseNetworkUpdateData(string result)
//    {
//      if (string.IsNullOrEmpty(result))
//      {
//        return null;
//      }

//      try
//      {
//        var tr = new StringReader(result);
//        var xdoc = XDocument.Load(tr);
//        var query = (from update in xdoc.Descendants("update")
//                     select
//                       new LinkedInEntry
//                         {
//                           Id = update.Element("update-key") != null ? update.Element("update-key").Value : update.Element("timestamp").Value,
//                           Type = EnumType.LinkedIn,
//                           UpdateType = update.Element("update-type").Value,
//                           PubDate =
//                             update.Element("timestamp") != null
//                               ? new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(update.Element("timestamp").Value))
//                               : new DateTime(),
//                           User = FindCorrectUser(ParseUserData(update.ToString())),
//                           Users = ParseUpdateLinkedInUsers(update.Element("update-type").Value, update),
//                           Educations = ParseUpdateLinkedInEducations(update.Element("update-type").Value, update),
//                           Job = ParseUpdateLinkedInJob(update.Element("update-type").Value, update),
//                           Groups = ParseUpdateLinkedInGroups(update.Element("update-type").Value, update),
//                           Activities = ParseUpdateLinkedInActivities(update.Element("update-type").Value, update),
//                           Recommendations = ParseUpdateLinkedInRecommendations(update.Element("update-type").Value, update),
//                           CanPost = bool.Parse(update.Element("is-commentable").Value) ? 1 : 0,
//                           Title =
//                             update.Element("update-content").Element("person") != null
//                               ? update.Element("update-content").Element("person").Element("current-status") != null
//                                   ? update.Element("update-content").Element("person").Element("current-status").Value
//                                   : null
//                               : null,
//                           Comments = ParseComments(update.Element("update-comments"))
//                         });
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParsingNetworkRUpdateData", ex);
//        return null;
//      }
//    }

//    public static List<LinkedInUser> ParseSearchResult(string result)
//    {
//      if (string.IsNullOrEmpty(result))
//      {
//        return null;
//      }

//      try
//      {
//        var tr = new StringReader(result);
//        var xdoc = XDocument.Load(tr);
//        var query = (from user in xdoc.Descendants("person")
//                     select
//                       new LinkedInUser
//                         {
//                           Id = user.Element("id").Value,
//                           FirstName = user.Element("first-name").Value,
//                           Name = user.Element("last-name").Value,
//                           NickName = string.Format("{0} {1}", user.Element("first-name").Value, user.Element("last-name").Value),
//                           Description = user.Element("headline").Value,
//                           Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
//                           //IndustryCode = user.Element("industry") != null
//                           //                 ?
//                           //                   IndustryCode.GetIndustry(
//                           //                     user.Element("industry").Value)
//                           //                 : null,
//                           NbConnections = int.Parse(user.Element("connections").Attribute("total").Value),
//                           NbRecommendations = int.Parse(user.Element("num-recommenders").Value),
//                           Distance = int.Parse(user.Element("distance").Value),
//                           Positions = ParsePositions(user),
//                           Url = user.Element("site-standard-profile-request").Element("url").Value
//                         });
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseSearch", ex);
//        return null;
//      }
//    }

//    private static List<LinkedInPosition> ParsePositions(XContainer element)
//    {
//      if (element == null)
//      {
//        return null;
//      }
//      try
//      {
//        var query = (from user in element.Descendants("position")
//                     select
//                       new LinkedInPosition
//                         {
//                           Id = int.Parse(user.Element("id").Value),
//                           Title = user.Element("title").Value,
//                           Summary = user.Element("summary") != null ? user.Element("summary").Value : string.Empty,
//                           StartY =
//                             user.Element("start-date") != null
//                               ? user.Element("start-date").Element("year") != null ? int.Parse(user.Element("start-date").Element("year").Value) : -1
//                               : -1,
//                           StartM =
//                             user.Element("start-date") != null
//                               ? user.Element("start-date").Element("month") != null ? int.Parse(user.Element("start-date").Element("month").Value) : -1
//                               : -1,
//                           EndY =
//                             user.Element("end-date") != null
//                               ? user.Element("end-date").Element("year") != null ? int.Parse(user.Element("end-date").Element("year").Value) : -1
//                               : -1,
//                           EndM =
//                             user.Element("end-date") != null
//                               ? user.Element("end-date").Element("month") != null ? int.Parse(user.Element("end-date").Element("month").Value) : -1
//                               : -1,
//                           IsCurrent = user.Element("is-current").Value == "0" ? false : true,
//                           Compagny = user.Element("compagny") != null ? user.Element("location").Element("name").Value : "",
//                         });
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParsePositions", ex);
//        return null;
//      }
//    }

//    private static List<LinkedInUser> ParseUpdateLinkedInUsersSt(string element, XContainer update)
//    {
//      try
//      {
//        switch (element)
//        {
//          case "CONN":
//            return ParseConnectionsDataSt(update.Element("update-content").Element("person").Element("connections").ToString(), false);
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseUpdateLinkedInUsers", ex);
//        return null;
//      }
//      return null;
//    }

//    private List<LinkedInUser> ParseUpdateLinkedInUsers(string element, XContainer update)
//    {
//      try
//      {
//        switch (element)
//        {
//          case "CONN":
//            return ParseConnectionsData(update.Element("update-content").Element("person").Element("connections").ToString(), false);
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseUpdateLinkedInUsers", ex);
//        return null;
//      }
//      return null;
//    }

//    private static List<LinkedInEducation> ParseUpdateLinkedInEducations(string element, XContainer update)
//    {
//      try
//      {
//        switch (element)
//        {
//          case "PROF":
//            return ParseEducations(update.Element("update-content").Element("person").Element("educations"));
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseUpdateLinkedInEducations", ex);
//        return null;
//      }
//      return null;
//    }

//    private static LinkedInJob ParseUpdateLinkedInJob(string element, XContainer update)
//    {
//      try
//      {
//        switch (element)
//        {
//          case "JOBP":
//            return ParseJobData(update);
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseUpdateLinkedInJob", ex);
//        return null;
//      }
//      return null;
//    }

//    private static List<LinkedInGroup> ParseUpdateLinkedInGroups(string element, XContainer update)
//    {
//      try
//      {
//        switch (element)
//        {
//          case "JGRP":
//            return ParseGroupsData(update.ToString());
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseUpdateLinkedInGroups", ex);
//        return null;
//      }
//      return null;
//    }

//    private static List<LinkedInActivity> ParseUpdateLinkedInActivities(string element, XContainer update)
//    {
//      try
//      {
//        switch (element)
//        {
//          case "APPM":
//            return ParseActivitiesData(update);
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseUpdateLinkedInActivities", ex);
//        return null;
//      }
//      return null;
//    }

//    private static List<LinkedInRecommendation> ParseUpdateLinkedInRecommendations(string element, XContainer update)
//    {
//      try
//      {
//        switch (element)
//        {
//          case "PREC":
//            return ParseRecommendationsData(update);
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseUpdateLinkedInRecommendations", ex);
//        return null;
//      }
//      return null;
//    }

//    /*private LinkedIndata ParseUpdateLinkedIn(string element, XContainer update)
//    {
//      try
//      {
//        LinkedIndata linkedInData;
//        switch (element)
//        {
//          case "CONN":
//            linkedInData = new LinkedIndata
//                             {
//                               Users =
//                                 ParseConnectionsData(
//                                 update.Element("update-content").Element("person").Element("connections").ToString(), false)
//                             };
//            return linkedInData;
//          case "PROF":
//            linkedInData = new LinkedIndata
//                             {
//                               Educations =
//                                 ParseEducations(update.Element("update-content").Element("person").Element("educations"))
//                             };
//            return linkedInData;
//          case "JOBP":
//            linkedInData = new LinkedIndata { Job = ParseJobData(update) };
//            return linkedInData;
//          case "NCOM":
//            return null;
//          case "PICU":
//            return null;
//          case "STAT":
//            return null;
//          case "JGRP":
//            linkedInData = new LinkedIndata { Groups = ParseGroupsData(update.ToString()) };
//            return linkedInData;
//          case "APPM":
//            linkedInData = new LinkedIndata { Activities = ParseActivitiesData(update) };
//            return linkedInData;
//          case "PREC":
//            linkedInData = new LinkedIndata { Recommendations = ParseRecommendationsData(update) };
//            return linkedInData;
//          default:
//            return null;
//        }
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace(this, ex);
//        return null;
//      }
//    }*/

//    private static List<LinkedInGroup> ParseGroupsData(string data)
//    {
//      if (string.IsNullOrEmpty(data))
//      {
//        return null;
//      }

//      try
//      {
//        var tr = new StringReader(data);
//        var xdoc = XDocument.Load(tr);
//        var query = (from user in xdoc.Descendants("member-group")
//                     select
//                       new LinkedInGroup
//                         {Id = user.Element("id").Value, Name = user.Element("name").Value, Url = user.Element("site-group-request").Element("url").Value});
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseGroupsData", ex);
//      }

//      return null;
//    }

//    private static User ParseUserDataRecommendation(string data)
//    {
//      if (string.IsNullOrEmpty(data))
//      {
//        return null;
//      }

//      var tr = new StringReader(data);
//      var xdoc = XDocument.Load(tr);
//      var query = (from user in xdoc.Descendants("recommendee")
//                   select new LinkedInUser
//                            {
////Id = user.Element("id").Value,
//                              Id =
//                                user.Element("api-standard-profile-request") != null
//                                  ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
//                                  : string.Empty,
//                              FirstName = user.Element("first-name").Value,
//                              Name = user.Element("last-name").Value,
//                              NickName = string.Format("{0} {1}", user.Element("first-name").Value, user.Element("last-name").Value),
//                              Description = user.Element("headline").Value,
//                              Url = user.Element("site-standard-profile-request").Element("url").Value
//                            });
//      var lst = query.ToList();
//      if (lst.Count > 0)
//      {
//        return lst.First();
//      }
//      query = (from user in xdoc.Descendants("recommender")
//               select new LinkedInUser
//                        {
////Id = user.Element("id").Value,
//                          Id =
//                            user.Element("api-standard-profile-request") != null
//                              ? user.Element("api-standard-profile-request").Element("url").Value.Split('/')[5].Split(':')[0]
//                              : string.Empty,
//                          FirstName = user.Element("first-name").Value,
//                          Name = user.Element("last-name").Value,
//                          NickName = string.Format("{0} {1}", user.Element("first-name").Value, user.Element("last-name").Value),
//                          Description = user.Element("headline").Value,
//                          Url = user.Element("site-standard-profile-request").Element("url").Value
//                        });
//      lst = query.ToList();
//      if (lst.Count > 0)
//      {
//        return lst.First();
//      }
//      return null;
//    }

//    private static List<LinkedInActivity> ParseActivitiesData(XContainer element)
//    {
//      if (element == null)
//      {
//        return null;
//      }
//      try
//      {
//        var query =
//          (from job in element.Descendants("activity") select new LinkedInActivity {Body = job.Element("body").Value, AppId = job.Element("app-id").Value,});
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseActivitiesData", ex);
//        return null;
//      }
//    }

//    private static List<LinkedInRecommendation> ParseRecommendationsData(XContainer element)
//    {
//      if (element == null)
//      {
//        return null;
//      }
//      try
//      {
//        var query = (from job in element.Descendants("recommendation")
//                     select
//                       new LinkedInRecommendation
//                         {
//                           Id = job.Element("id") != null ? job.Element("id").Value : string.Empty,
//                           Type = job.Element("recommendation-type") != null ? job.Element("recommendation-type").Value : string.Empty,
//                           Snippet = job.Element("recommendation-snippet") != null ? job.Element("recommendation-snippet").Value : string.Empty,
//                           Recommendee = ParseUserDataRecommendation(job.ToString()),
//                           Url = job.Element("web-url") != null ? job.Element("web-url").Value : string.Empty
//                         });
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseRecommendationsData", ex);
//        return null;
//      }
//    }


//    private static LinkedInJob ParseJobData(XContainer element)
//    {
//      if (element == null)
//      {
//        return null;
//      }
//      try
//      {
//        var query = (from job in element.Descendants("job")
//                     select
//                       new LinkedInJob
//                         {
//                           Id = job.Element("id").Value,
//                           Title = job.Element("position").Element("title").Value,
//                           Compagny = job.Element("company").Element("name").Value,
//                           JobPoster = ParseUserDataJob(job.ToString()),
//                           Url = job.Element("site-job-request").Element("url").Value
//                         });
//        return query.First();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseJobData", ex);
//        return null;
//      }
//    }

//    private static List<LinkedInEducation> ParseEducations(XContainer element)
//    {
//      if (element == null)
//      {
//        return null;
//      }
//      try
//      {
//        var query = (from user in element.Descendants("education")
//                     select
//                       new LinkedInEducation
//                         {Id = user.Element("id").Value, FieldOfStudy = user.Element("field-of-study").Value, SchoolName = user.Element("school-name").Value,});
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseEducations", ex);
//        return null;
//      }
//    }

//    /*private List<Entry> ParseNetworkUpdateData(string result)
//    {
//      if (string.IsNullOrEmpty(result))
//      {
//        return null;
//      }

//      var tr = new StringReader(result);
//      var xdoc = XDocument.Load(tr);
//      var query = (from update in xdoc.Descendants("update")
//                   select new Entry()
//                            {
//                              Id = update.Element("timestamp").Value,
//                              Type = EnumType.LinkedIn,
//                              AppId = update.Element("update-type").Value,
//                              PubDate = new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(update.Element("timestamp").Value)),
//                              User = FindCorrectUser(ParseUserData(update.ToString()))
//                              //LinkedIndata = 
//                   });
//      return query.ToList();
//    }*/

//    private static LinkedInUser FindCorrectUserSt(LinkedInUser user)
//    {
//      if (user == null)
//      {
//        return null;
//      }
//      //foreach (LinkedInUser friend in Friends)
//      //{
//      //  if (user.Id == friend.Id)
//      //  {
//      //    return friend;
//      //  }
//      //}
//      return user;
//    }

//    private LinkedInUser FindCorrectUser(LinkedInUser user)
//    {
//      if (user == null)
//      {
//        return null;
//      }
//      foreach (var friend in Friends)
//      {
//        if (user.Id == friend.Id)
//        {
//          return friend;
//        }
//      }
//      return user;
//    }

//    #endregion

//    #region ProfileMethods

//    public LinkedInUser GetCurrentProfile()
//    {
//      return
//        ParseStandardUserData(
//          OAuthWebRequest(
//            Method.GET,
//            string.Format(
//              "{0}:(id,first-name,last-name,headline,location,industry,distance,relation-to-viewer,current-status,current-status-timestamp,summary,specialties,proposal-comments,associations,positions,educations,picture-url,site-standard-profile-request,member-url-resources,num-recommenders)",
//              LinkedInResources.PROFIL_CURRENT_URL),
//            String.Empty));
//    }

//    #endregion

//    #region Connections

//    public List<LinkedInUser> GetConnections()
//    {
//      return ParseConnectionsData(OAuthWebRequest(Method.GET, LinkedInResources.CONNECTIONS_CURRENT_URL, String.Empty), true);
//    }

//    #endregion

//    #region NetworkUpdate

//    public List<LinkedInEntry> GetNetworkUpdate(EnumLinkedInUpdateType type, int nbToGet, int offset, long after, long before)
//    {
//      var url = LinkedInResources.NETWORK_UPDATES_URL;
//      url = string.Format("{0}type={1}&", url, type);
//      url = string.Format("{0}count={1}", url, nbToGet);
//      if (offset > 0)
//      {
//        url = string.Format("{0}&start={1}", url, offset);
//      }
//      if (after > 0)
//      {
//        url = string.Format("{0}&after={1}", url, after);
//      }
//      if (before > 0)
//      {
//        url = string.Format("{0}&before={1}", url, before);
//      }
//      return ParseNetworkUpdateData(OAuthWebRequest(Method.GET, url, String.Empty));
//    }

//    public List<LinkedInEntry> GetNetworkUpdate(List<EnumLinkedInUpdateType> types, int nbToGet, int offset, long after, long before)
//    {
//      var url = LinkedInResources.NETWORK_UPDATES_URL;
//      if (types != null)
//      {
//        foreach (var type in types)
//        {
//          url = string.Format("{0}type={1}&", url, type);
//        }
//      }
//      url = string.Format("{0}count={1}", url, nbToGet);
//      if (offset > 0)
//      {
//        url = string.Format("{0}&start={1}", url, offset);
//      }
//      if (after > 0)
//      {
//        url = string.Format("{0}&after={1}", url, after);
//      }
//      if (before > 0)
//      {
//        url = string.Format("{0}&before={1}", url, before);
//      }
//      return ParseNetworkUpdateData(OAuthWebRequest(Method.GET, url, String.Empty));
//    }

//    #endregion

//    #region Status Update

//    public bool SetStatus(string status)
//    {
//      var txt = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><current-status>{0}</current-status>", status);

//      //string result = OAuthWebRequest(Method.PUT, Resources.STATUS_URL, txt, true);
//      return OAuthWebRequest(Method.PUT, LinkedInResources.STATUS_URL, txt) == "NoContent";
//    }

//    #endregion

//    #region Comments

//    public bool SetComments(string status, string id)
//    {
//      var txt = string.Format("<?xml version='1.0' encoding='UTF-8'?><update-comment><comment>{0}</comment></update-comment>", status);

//      //string result = OAuthWebRequest(Method.PUT, Resources.STATUS_URL, txt, true);
//      return OAuthWebRequest(Method.POST, LinkedInResources.COMMENT_URL_START + id + LinkedInResources.COMMENT_URL_END, txt) == "Created";
//    }

//    #endregion

//    #region Update

//    public bool SetUpdate(string body)
//    {
//      return
//        OAuthWebRequest(
//          Method.POST, LinkedInResources.NETWORKUPDATE_URL, LinkedInResources.NETWORKUPDATE_URL_START + body + LinkedInResources.NETWORKUPDATE_URL_END) ==
//        "Created";
//    }

//    #endregion

//    #region Search

//    public List<LinkedInUser> Search(string keywords,
//                                     string name,
//                                     string company,
//                                     bool currentCompany,
//                                     string title,
//                                     bool currentTitle,
//                                     LinkedInIndustryCode industryCode,
//                                     EnumLinkedInSearchNetwork network,
//                                     int start,
//                                     int count,
//                                     EnumLinkedInSearchSort sort)
//    {
//      var url = LinkedInResources.SEARCH_URL;
//      if (!string.IsNullOrEmpty(keywords))
//      {
//        url += string.Format("keywords={0}", keywords);
//      }
//      if (!string.IsNullOrEmpty(name))
//      {
//        url += string.Format("&name={0}", name);
//      }
//      if (!string.IsNullOrEmpty(company))
//      {
//        url += string.Format("&company={0}", company);
//        url += string.Format("&current-company={0}", currentCompany);
//      }
//      if (!string.IsNullOrEmpty(title))
//      {
//        url += string.Format("&title={0}", title);
//        url += string.Format("&current-title={0}", currentTitle);
//      }
//      if (industryCode != null)
//      {
//        url += string.Format("&industry-code={0}", industryCode.Code);
//      }
//      url += string.Format("&network={0}", network.ToString().ToLower());
//      url += string.Format("&start={0}", start);
//      url += string.Format("&count={0}", count);
//      url += string.Format("&sort-criteria={0}", sort);
//      return ParseSearchResult(OAuthWebRequest(Method.GET, url, string.Empty));
//    }

//    #endregion

//    public LinkedInUser GetInfoUser(string id)
//    {
//      return
//        ParseStandardUserData(
//          OAuthWebRequest(
//            Method.GET,
//            string.Format(
//              "{0}id={1}:(id,first-name,last-name,headline,location,industry,distance,relation-to-viewer,current-status,current-status-timestamp,summary,specialties,proposal-comments,associations,positions,educations,picture-url,site-standard-profile-request,member-url-resources,num-recommenders)",
//              LinkedInResources.PROFIL_URL,
//              id),
//            string.Empty));
//    }

//    public static LinkedInUser ParseStandardUserData(string result)
//    {
//      if (string.IsNullOrEmpty(result))
//      {
//        return null;
//      }

//      try
//      {
//        var tr = new StringReader(result);
//        var xdoc = XDocument.Load(tr);
//        var query = (from user in xdoc.Descendants("person")
//                     select
//                       new LinkedInUser
//                         {
//                           Id = user.Element("id") != null ? user.Element("id").Value : string.Empty,
//                           FirstName = user.Element("first-name").Value,
//                           Name = user.Element("last-name").Value,
//                           NickName = string.Format("{0} {1}", user.Element("first-name").Value, user.Element("last-name").Value),
//                           Description = user.Element("headline") != null ? user.Element("headline").Value : string.Empty,
//                           Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
//                           //IndustryCode =
//                           //  user.Element("industry") != null
//                           //    ? IndustryCode.GetIndustry(user.Element("industry").Value)
//                           //    : null,
//                           NbConnections = user.Element("connections") != null ? int.Parse(user.Element("connections").Attribute("total").Value) : -1,
//                           NbRecommendations = user.Element("num-recommenders") != null ? int.Parse(user.Element("num-recommenders").Value) : -1,
//                           Distance = user.Element("distance") != null ? int.Parse(user.Element("distance").Value) : 0,
//                           Positions = ParsePositions(user),
//                           Url =
//                             user.Element("site-standard-profile-request") != null
//                               ? user.Element("site-standard-profile-request").Element("url").Value
//                               : string.Empty,
//                           LastStatusString = user.Element("current-status") != null ? user.Element("current-status").Value : null,
//                           LastStatusDate =
//                             user.Element("current-status-timestamp") != null
//                               ? new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(user.Element("current-status-timestamp").Value))
//                               : new DateTime(),
//                           Summary = user.Element("summary") != null ? user.Element("summary").Value : null,
//                           Specialties = user.Element("specialties") != null ? user.Element("specialties").Value : null,
//                           Associations = user.Element("associations") != null ? user.Element("associations").Value : null,
//                           Urls = ParseUrl(user.Element("member-url-resources")),
//                           ProfileImgUrl =
//                             user.Element("picture-url") != null
//                               ? user.Element("picture-url").Value
//                               : "http://static03.linkedin.com/img/icon/icon_no_photo_40x40.png"
//                         });
//        if (query.Count() == 0)
//        {
//          query = (from user in xdoc.Descendants()
//                   select
//                     new LinkedInUser
//                       {
//                         Id = user.Element("id") != null ? user.Element("id").Value : string.Empty,
//                         FirstName = user.Element("first-name").Value,
//                         Name = user.Element("last-name").Value,
//                         NickName = string.Format("{0} {1}", user.Element("first-name").Value, user.Element("last-name").Value),
//                         Description = user.Element("headline") != null ? user.Element("headline").Value : string.Empty,
//                         Location = user.Element("location") != null ? user.Element("location").Element("name").Value : "",
//                         //IndustryCode =
//                         //  user.Element("industry") != null
//                         //    ? IndustryCode.GetIndustry(user.Element("industry").Value)
//                         //    : null,
//                         NbConnections = user.Element("connections") != null ? int.Parse(user.Element("connections").Attribute("total").Value) : -1,
//                         NbRecommendations = user.Element("num-recommenders") != null ? int.Parse(user.Element("num-recommenders").Value) : -1,
//                         Distance = user.Element("distance") != null ? int.Parse(user.Element("distance").Value) : 0,
//                         Positions = ParsePositions(user),
//                         Url =
//                           user.Element("site-standard-profile-request") != null
//                             ? user.Element("site-standard-profile-request").Element("url").Value
//                             : string.Empty,
//                         LastStatusString = user.Element("current-status") != null ? user.Element("current-status").Value : null,
//                         LastStatusDate =
//                           user.Element("current-status-timestamp") != null
//                             ? new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(user.Element("current-status-timestamp").Value))
//                             : new DateTime(),
//                         Summary = user.Element("summary") != null ? user.Element("summary").Value : null,
//                         Specialties = user.Element("specialties") != null ? user.Element("specialties").Value : null,
//                         Associations = user.Element("associations") != null ? user.Element("associations").Value : null,
//                         Urls = ParseUrl(user.Element("member-url-resources")),
//                         ProfileImgUrl =
//                           user.Element("picture-url") != null
//                             ? user.Element("picture-url").Value
//                             : "http://static03.linkedin.com/img/icon/icon_no_photo_40x40.png"
//                       });
//        }
//        return query.ToList().First();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseStandardUserData", ex);
//        return null;
//      }
//    }

//    private static ObservableCollection<Comment> ParseComments(XContainer element)
//    {
//      if (element == null)
//      {
//        return null;
//      }
//      try
//      {
//        var query = (from user in element.Descendants("update-comment")
//                     select new Comment {Body = user.Element("comment").Value, User = ParseUserData(user.Element("person").ToString()),});
//        return new ObservableCollection<Comment>(query.ToList());
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseComments", ex);
//        return null;
//      }
//    }

//    private static List<LinkedInUrlType> ParseUrl(XContainer element)
//    {
//      if (element == null)
//      {
//        return null;
//      }
//      try
//      {
//        var query =
//          (from user in element.Descendants("member-url") select new LinkedInUrlType {UrlSrc = user.Element("url").Value, Name = user.Element("name").Value,});
//        return query.ToList();
//      }
//      catch (Exception ex)
//      {
//        TraceHelper.Trace("ParseUrl", ex);
//        return null;
//      }
//    }
//  }
//}
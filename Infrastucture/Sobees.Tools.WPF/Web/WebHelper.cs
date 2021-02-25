#region

using BUtility;
using Sobees.Tools.Crypto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

#if SILVERLIGHT
using System.Windows.Browser;
using System.Windows;

#else

#endif

#endregion

namespace Sobees.Tools.Web
{
  /// <summary>
  ///   HyperLinkHelper
  /// </summary>
  public class HyperLinkHelper
  {
    public static bool IsHyperlink(string word)
    {
      if (word == "test")
        Debug.Write("test");

      if (word == null)
      {
        return false;
      }
      if (word.Contains("flickr.com/pho"))
      {
        return true;
      }
      if (word.Contains("mailto"))
      {
        return true;
      }
      if (word.Contains("@") || word.Contains("href"))
      {
        return false;
      }
      const string regex =
        @"\b(((ftp|https?)://)?[-\w]+(\.\w[-\w]*){2,4}|[a-z0-9](?:[-a-z0-9]*[a-z0-9])?\.)+(com\b|edu\b|biz\b|gov\b|in(?:t|fo)\b|mil\b|net\b|org\b|[a-z][a-z]\b)(:\d+)?(/[-a-z0-9_:\@&?=+,.!/~*'%\$]*)*(?<![.,?!])(?!((?!(?:<a )).)*?(?:</a>))(?!((?!(?:<!--)).)*?(?:-->))";
      const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline));

      return Regex.IsMatch(word, regex, options) || word.Contains("http://localhost");
    }

    /// <summary>
    /// GetHyperlink
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string GetHyperlink(string text)
    {
      if (text == null)
      {
        return null;
      }

      const string regex =
        @"\b(((ftp|https?)://)?[-\w]+(\.\w[-\w]*){2,4}|[a-z0-9](?:[-a-z0-9]*[a-z0-9])?\.)+(com\b|edu\b|biz\b|gov\b|in(?:t|fo)\b|mil\b|net\b|org\b|[a-z][a-z]\b)(:\d+)?(/[-a-z0-9_:\@&?=+,.!/~*'%\$]*)*(?<![.,?!])(?!((?!(?:<a )).)*?(?:</a>))(?!((?!(?:<!--)).)*?(?:-->))";
      const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);

      var match = Regex.Match(text, regex, options);
      return match.Value;
    }

    /// <summary>
    /// GetUrlFromHtmlLink
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string GetUrlFromHtmlLink(string text)
    {
      if (text == null)
      {
        return null;
      }

      var split = text.Split('"');
      return split.Length > 2 ? split[1] : null;
    }

    /// <summary>
    /// GetNameLinkFromHtmlLink
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string GetNameLinkFromHtmlLink(string text)
    {
      if (text == null)
        return null;

      var split = text.Split("<>".ToCharArray());
      return split.Length > 3 ? split[2] : text;
    }
  }

  /// <summary>
  ///   WebHelper
  /// </summary>
  public class WebHelper
  {
    public static string UrlEncode(string url)
    {
      return HttpUtility.UrlEncode(url);
    }

    /// <summary>
    /// UrlEncode
    /// </summary>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static string UrlEncode(Dictionary<string, string> arguments)
    {
      var parts = new string[arguments.Count];
      var i = 0;
      foreach (var pair in arguments)
        parts[i++] = string.Format("{0}={1}", UrlEncode(pair.Key), UrlEncode(pair.Value));
      return string.Join("&", parts);
    }

    public static string RemoveHtml(string text)
    {
      return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
    }

    public static string ExtractDomainNameFromUrl(string url)
    {
      if (!url.Contains("://"))
        url = string.Format("http://{0}", url);

      return new Uri(url).Host;
    }

    public static string EnsureMinimalProtocol(string url)
    {
      // if our url doesn't have a protocol, we'll at least assume it's plain old http, otherwise good to go
      const string minimalProtocal = @"http://";
      if (url.ToLower().StartsWith("http"))
        return url;

      return minimalProtocal + url;
    }

    /// <summary>
    /// NavigateToUrl
    /// </summary>
    /// <param name="url"></param>
    /// <param name="page"></param>
    public static void NavigateToUrl(string url,
                                     string page)
    {
      try
      {
        if (!string.IsNullOrEmpty(url) && HyperLinkHelper.IsHyperlink(url))
        {
          var navigateUri = url;
          Process.Start(new ProcessStartInfo(EnsureMinimalProtocol(navigateUri)));
        }
        else
        {
          BLogManager.LogEntry("bTools -> WebHelper", " -> URL was null or not valid!");
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("WebHelper -> NavigateToUrl -> ", ex);
      }
    }

    /// <summary>
    /// NavigateToUrl
    /// </summary>
    /// <param name="url"></param>
    public static void NavigateToUrl(string url)
    {
      try
      {
        if (!string.IsNullOrEmpty(url) && HyperLinkHelper.IsHyperlink(url))
        {
          var navigateUri = url;
          Process.Start(new ProcessStartInfo(EnsureMinimalProtocol(navigateUri)));
        }
        else
        {
          BLogManager.LogEntry("bTools -> WebHelper", " -> URL was null or not valid!");
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("WebHelper -> NavigateToUrl -> ", ex);
      }
    }

    public static string HttpPost(string login,
                                  string pwd,
                                  bool isRemoteKey,
                                  string uri,
                                  string parameters)
    {
      var webRequest = WebRequest.Create(uri);
      //string ProxyString =
      //   System.Configuration.ConfigurationManager.AppSettings
      //   [GetConfigKey("proxy")];
      //webRequest.Proxy = new WebProxy (ProxyString, true);
      //Commenting out above required change to App.Config

      webRequest.Credentials = isRemoteKey ? new NetworkCredential(login, pwd) : new NetworkCredential(login, EncryptionHelper.Decrypt(pwd));

      webRequest.ContentType = "application/x-www-form-urlencoded";
      webRequest.Method = "POST";
      var bytes = Encoding.ASCII.GetBytes(parameters);
      Stream os = null;
      try
      {
        // send the Post
        webRequest.ContentLength = bytes.Length; //Count bytes to send
        os = webRequest.GetRequestStream();
        os.Write(bytes, 0, bytes.Length); //Send it
      }
      catch (WebException ex)
      {
        return "HttpPost: Response error: " + ex.Message;
        //MessageBox.Show(ex.Message, "HttpPost: Request error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      finally
      {
        if (os != null)
          os.Close();
      }
      try
      {
        // get the response
        using (var webResponse = webRequest.GetResponse())
        {
          if (webResponse == null)
          {
            return null;
          }
          var sr = new StreamReader(webResponse.GetResponseStream());
          return sr.ReadToEnd().Trim();
        }
      }
      catch (WebException ex)
      {
        return "HttpPost: Response error: " + ex.Message;
        //MessageBox.Show(ex.Message, "HttpPost: Response error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }
}
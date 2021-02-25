using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Sobees.Tools.Web;

namespace Sobees.Library.BServicesLib
{
  /// <summary>
  /// Wrapper for interacting with TinyUrl API
  /// </summary>
  public class DiggHelper
  {
    public const string DiggApikey = "http%3A%2F%2Fwww.sobees.com";

    public static string ConvertUrlsToTinyUrls(string text, WebProxy proxy)
    {
      if (text == null)
        throw new ArgumentNullException("text");

      string[] textSplitIntoWords = text.Split(' ');

      bool foundUrl = false;
      for (int i = 0; i < textSplitIntoWords.Length; i++)
      {
        if (HyperLinkHelper.IsHyperlink(textSplitIntoWords[i]))
        {
          foundUrl = true;
          // replace found url with tinyurl
          textSplitIntoWords[i] = GetNewTinyUrl(textSplitIntoWords[i], proxy);
        }
      }

      // reassemble if we found at least 1 url, otherwise return unaltered
      return foundUrl ? String.Join(" ", textSplitIntoWords) : text;
    }

    public static string GetNewTinyUrl(string sourceUrl, WebProxy proxy)
    {
      if (sourceUrl == null)
        throw new ArgumentNullException("sourceUrl");

      // fallback will be source url
      string result = sourceUrl;
      //Added 11/3/2007 scottckoon
      //17 is the shortest a tinyURl can be (http://digg.com/a)
      //so if the sourceUrl is shorter than that, don't make a request to TinyURL
      if (sourceUrl.Length > 17 && !sourceUrl.Contains("http://digg.com"))
      {
        // tinyurl doesn't like urls w/o protocols so we'll ensure we have at least http
        string requestUrl = BuildRequestUrl(EnsureMinimalProtocol(sourceUrl));
        try
        {
          var webRequest = (HttpWebRequest) WebRequest.Create(requestUrl);
          webRequest.UserAgent = "sobees (www.sobees.com)";
          webRequest.Credentials = CredentialCache.DefaultCredentials;
          webRequest.Accept = "text/xml";
          if (proxy != null)
          {
            webRequest.Proxy = proxy;
          }
          var webResponse = (HttpWebResponse) webRequest.GetResponse();

          Stream responseStream = webResponse.GetResponseStream();
          using (var reader = new XmlTextReader(responseStream))
          {
            XDocument xdoc = XDocument.Load(reader);
            result =
              (xdoc.Descendants("shorturls").Select(url => url.Element("shorturl").Attribute("short_url"))).First().
                Value;
          }
        }
        catch
        {
        }
      }
      //scottckoon - It doesn't make sense to return a TinyURL that is longer than the original.
      if (result.Length > sourceUrl.Length)
      {
        result = sourceUrl;
      }
      return result;
    }

    private static string BuildRequestUrl(string sourceUrl)
    {
      const string tinyUrlFormat = "http://services.digg.com/url/short/create?url={0}&appkey=" + DiggApikey;
      return String.Format(tinyUrlFormat, sourceUrl);
    }

    // REFACTOR: DRY vs. StringUtils - didn't want this static
    private static bool IsUrl(string word)
    {
      const string urlRegex =
        @"(((ht|f)tp(s?))\:\/\/)?(www\.|[a-zA-Z]+\.)?[a-zA-Z0-9]+\.(com|edu|gov|mil|net|org|biz|info|name|museum|us|ca|uk)(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\;\?\'\\\+&%\$#\=~_\-]+))*(?=[\.\?!,;])";

      bool isUrl;
      try
      {
        isUrl = Regex.IsMatch(word, urlRegex,
                              RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
      }
      catch (ArgumentException)
      {
        // issue with the regular expression
        // TODO: not sure what overarching exhandling strategy is
        throw;
      }
      return isUrl;
    }

    private static string EnsureMinimalProtocol(string url)
    {
      // if our url doesn't have a protocol, we'll at least assume it's plain old http, otherwise good to go
      const string minimalProtocal = @"http://";
      if (url.ToLower().StartsWith("http"))
      {
        return url;
      }
      else
      {
        return minimalProtocal + url;
      }
    }
  }
}
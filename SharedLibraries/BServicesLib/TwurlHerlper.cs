#region

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Sobees.Tools.Web;

#endregion

namespace Sobees.Library.BServicesLib
{
  /// <summary>
  /// Wrapper for interacting with TinyUrl API
  /// </summary>
  public class TwurlHelper
  {
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
      return foundUrl
               ? String.Join(" ",
                             textSplitIntoWords)
               : text;
    }

    public static string GetNewTinyUrl(string sourceUrl, WebProxy proxy)
    {
      if (sourceUrl == null)
        throw new ArgumentNullException("sourceUrl");

      // fallback will be source url
      string result = sourceUrl;
      //Added 11/3/2007 scottckoon
      //20 is the shortest a tinyURl can be (http://tinyurl.com/a)
      if (sourceUrl.Length > 20 && !sourceUrl.Contains("http://twurl.nl"))
      {
        // tinyurl doesn't like urls w/o protocols so we'll ensure we have at least http
        string requestUrl = "http://tweetburner.com/links";
        var request = WebRequest.Create(requestUrl) as HttpWebRequest;

        request.Method = "POST";
        // Set values for the request back
        request.ContentType = "application/x-www-form-urlencoded";
        string urlToShorten = "link[url]=" + sourceUrl;
        request.ContentLength = urlToShorten.Length;
        request.ServicePoint.Expect100Continue = false;
        request.ProtocolVersion = HttpVersion.Version10;
        if (proxy != null)
        {
          request.Proxy = proxy;
        }
        var stOut = new StreamWriter(request.GetRequestStream(),
                                     Encoding.ASCII);
        stOut.Write(urlToShorten);
        stOut.Close();
        using (Stream responseStream = request.GetResponse().GetResponseStream())
        {
          var reader = new StreamReader(responseStream,
                                        Encoding.ASCII);
          result = reader.ReadToEnd();
          return result;
        }
      }
      return "";
    }


    //// REFACTOR: DRY vs. StringUtils - didn't want this static
    //private static bool IsUrl(string word)
    //{
    //  const string urlRegex =
    //    @"(((ht|f)tp(s?))\:\/\/)?(www\.|[a-zA-Z]+\.)?[a-zA-Z0-9]+\.(com|edu|gov|mil|net|org|biz|info|name|museum|us|ca|uk)(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\;\?\'\\\+&%\$#\=~_\-]+))*(?=[\.\?!,;])";

    //  bool isUrl;
    //  try
    //  {
    //    isUrl = Regex.IsMatch(word,
    //                          urlRegex,
    //                          RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
    //  }
    //  catch (ArgumentException)
    //  {
    //    // issue with the regular expression
    //    // TODO: not sure what overarching exhandling strategy is
    //    throw;
    //  }
    //  return isUrl;
    //}

    //private static string EnsureMinimalProtocol(string url)
    //{
    //  // if our url doesn't have a protocol, we'll at least assume it's plain old http, otherwise good to go
    //  const string minimalProtocal = @"http://";
    //  if (url.ToLower().StartsWith("http"))
    //  {
    //    return url;
    //  }
    //  else
    //  {
    //    return minimalProtocal + url;
    //  }
    //}
  }
}
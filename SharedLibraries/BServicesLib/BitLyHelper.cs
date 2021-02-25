#region

using System;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Sobees.Tools.Web;

#endregion

namespace Sobees.Library.BServicesLib
{
  /// <summary>
  ///   Wrapper for interacting with TinyUrl API
  /// </summary>
  public class BitLyHelper
  {
    private const string BitLyApikeyConst = "R_a290d7a46a6b5673e647232120d3e4a3";
    private const string BitLyApiuserConst = "sobees";

    private static string BitLyApikey = "";
    private static string BitLyApiuser = "";

    public static string ConvertUrlsToTinyUrls(string text, WebProxy proxy, string bitLyApiuser, string bitLyApikey)
    {
      if (!string.IsNullOrEmpty(bitLyApiuser) && !string.IsNullOrEmpty(bitLyApikey))
      {
        BitLyApikey = bitLyApikey;
        BitLyApiuser = bitLyApiuser;
      }
      else
      {
        BitLyApikey = BitLyApikeyConst;
        BitLyApiuser = BitLyApiuserConst;
      }

      if (text == null)
        throw new ArgumentNullException("text");

      var textSplitIntoWords = text.Split(' ');

      var foundUrl = false;
      for (var i = 0; i < textSplitIntoWords.Length; i++)
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
      var result = sourceUrl;
      //Added 11/3/2007 scottckoon
      //15 is the shortest a tinyURl can be (http://bit.ly/a)
      //so if the sourceUrl is shorter than that, don't make a request to TinyURL
      if (sourceUrl.Length > 15 && !sourceUrl.Contains("http://bit.ly"))
      {
        // tinyurl doesn't like urls w/o protocols so we'll ensure we have at least http
        var requestUrl = BuildRequestUrl(WebHelper.EnsureMinimalProtocol(sourceUrl));
        var request = WebRequest.Create(requestUrl);
        if (proxy != null)
        {
          request.Proxy = proxy;
        }
        try
        {
          using (var reader = new XmlTextReader(request.GetResponse().GetResponseStream()))
          {
            var xdoc = XDocument.Load(reader);
            result =
              (xdoc.Descendants("bitly").Select(
                url => url.Element("results").Element("nodeKeyVal").Element("shortUrl").Value)).First();
          }
        }
        catch
        {
          // eat it and return original url
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
      var tinyUrlFormat = "http://api.bit.ly/shorten?longUrl={0}&version=2.0.1&format=xml&login=" + BitLyApiuser +
                          "&apiKey=" + BitLyApikey;
      return String.Format(tinyUrlFormat,
                           sourceUrl);
    }

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
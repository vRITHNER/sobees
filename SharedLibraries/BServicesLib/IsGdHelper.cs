﻿#region

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
  public class IsGdHelper
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
      //14 is the shortest a tinyURl can be (http://is.gd/a)
      //so if the sourceUrl is shorter than that, don't make a request to TinyURL
      if (sourceUrl.Length > 14 && !sourceUrl.Contains("http://is.gd"))
      {
        string requestUrl = BuildRequestUrl(sourceUrl);
        WebRequest request = WebRequest.Create(requestUrl);
        if (proxy != null)
        {
          request.Proxy = proxy;
        }
        try
        {
          using (Stream responseStream = request.GetResponse().GetResponseStream())
          {
            var reader = new StreamReader(responseStream,
                                          Encoding.ASCII);
            result = reader.ReadToEnd();
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
      const string tinyUrlFormat = "http://is.gd/api.php?longurl={0}";
      return String.Format(tinyUrlFormat,
                           sourceUrl);
    }

    // REFACTOR: DRY vs. StringUtils - didn't want this static
    private static bool IsUrl(string word)
    {
      const string urlRegex =
        @"(((ht|f)tp(s?))\:\/\/)?(www\.|[a-zA-Z]+\.)?[a-zA-Z0-9]+\.(com|edu|gov|mil|net|org|biz|info|name|museum|us|ca|uk)(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\;\?\'\\\+&%\$#\=~_\-]+))*(?=[\.\?!,;])";

      bool isUrl;
      try
      {
        isUrl = Regex.IsMatch(word,
                              urlRegex,
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
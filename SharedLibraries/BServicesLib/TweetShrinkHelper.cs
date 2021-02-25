#region

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Library.BServicesLib
{
  /// <summary>
  /// Wrapper for interacting with TinyUrl API
  /// </summary>
  public class TweetShrinkHelper
  {
    public static string ConvertUrlsToTweetShrink(string text)
    {
      return ConvertUrlsToTweetShrink(text, null);
    }

    public static string ConvertUrlsToTweetShrink(string text, WebProxy proxy)
    {
      if (text == null)
        throw new ArgumentNullException("text");

      return GetNewTweetShrink(text, proxy);
    }

    public static string GetNewTweetShrink(string text, WebProxy proxy)
    {
      if (text == null)
        throw new ArgumentNullException("text");

      string result = HttpUtility.UrlEncode(text);
      //Added 11/3/2007 scottckoon
      //20 is the shortest a tinyURl can be (http://tinyurl.com/a)
      //so if the sourceUrl is shorter than that, don't make a request to TinyURL
      // tinyurl doesn't like urls w/o protocols so we'll ensure we have at least http
      string requestUrl = BuildRequestUrl(result);
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
      catch (Exception ex)
      {
        TraceHelper.Trace("TweetShrink", ex);
      }
      //scottckoon - It doesn't make sense to return a TinyURL that is longer than the original.
      return result;
    }

    private static string BuildRequestUrl(string text)
    {
      const string tinyUrlFormat = "http://tweetshrink.com/shrink?format=string&text={0}";
      return String.Format(tinyUrlFormat,
                           text);
    }
  }
}
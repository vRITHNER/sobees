#region

using System;
using System.Net;
using BUtility;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Tools.Extensions
{
  public static class UrlExtension
  {
    private const string APPNAME = "UrlExtension";

    public static string ToBaseUrl(this string url)
    {
      try
      {
        url = url.Replace("http://", string.Empty).Replace(":81", string.Empty);
        url = url.Substring(0, url.IndexOf('/'));
        return string.Format("http://{0}/", url);
      }
      catch (Exception)
      {
        return "http://newsmix.me/";
      }
    }

    public static string ToDomainHost(this string url)
    {
      try
      {
        var host = new Uri(url).Host;
        return host;
      }
      catch (Exception)
      {
        return url;
      }
    }

    public static string ToRealShortUrl(this string shortUrl)
    {
      try
      {
        if (shortUrl.Length > 25 || shortUrl.Contains("tiny") || shortUrl.Contains("bit.ly"))
          return shortUrl;

        var webReq = WebRequest.Create(shortUrl) as HttpWebRequest;
        webReq.AllowAutoRedirect = false;
        var webResponse = webReq.GetResponse() as HttpWebResponse;
        if (webResponse.StatusCode == HttpStatusCode.MovedPermanently)
        {
          return webResponse.Headers["Location"];
        }
        return shortUrl;
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME + "::DecodeShortUrl:", ex);
        return shortUrl;
      }
    }
  }
}
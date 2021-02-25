#define DOTNET40
#undef SILVERLIGHT
#if SILVERLIGHT

using System;
using System.Net;
using System.Web;
using System.Windows.Browser;

#else

#region

using System;
using System.Net;
using System.Threading;
#if DOTNET40
#endif

using System.Web;
using Sobees.Tools.Extensions;
#endregion

#endif

namespace Sobees.Tools.Web
{
  public class HttpHelper
  {
    public static string EncodeUrl(string url)
    {
      return HttpUtility.UrlEncode(url);
    }

    //public async static Task<HttpWebResponse> ExecuteRequestWithTimeOut(Uri uri, TimeSpan timeSpan)
    //{
    //  var request = (HttpWebRequest)WebRequest.Create(uri);
    //  request.Method = "GET";
    //  request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)";
    //  request.AllowAutoRedirect = false;
    //  var task = request.GetResponseAsync();
    //  var response = await task.WithTimeout(timeSpan);

    //  return response as HttpWebResponse;
    //}

#if DOTNET40
    public static HttpWebResponse ExecuteRequestForImage(Uri uri)
    {
      var request = (HttpWebRequest)WebRequest.Create(uri);
      request.Method = "GET";
      request.AllowWriteStreamBuffering = true;
      request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)";
      request.Referer = "http://www.google.com/";
      request.Timeout = 20000;
      return request.GetResponse() as HttpWebResponse;
      
    }

    public static HttpWebResponse ExecuteRequest(Uri uri)
    {
      var request = (HttpWebRequest)WebRequest.Create(uri);
      request.Method = "GET";
      request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)";
      request.Timeout = 15000;
      request.AllowAutoRedirect = false;
      return request.GetResponse() as HttpWebResponse;
    }
#endif
  }
}
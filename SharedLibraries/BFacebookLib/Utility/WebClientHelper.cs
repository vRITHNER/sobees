#region

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security;
using System.Text;

#endregion

namespace Sobees.Library.BFacebookLibV1.Utility
{
  internal class WebClientHelper
  {
    private readonly Object _userState;
    private HttpWebRequest _webRequest;

    public WebClientHelper(Object userState)
    {
      _userState = userState;
      Method = "GET";
    }

    public string Method { get; set; }
    public string ContentType { get; set; }

    public Uri RequestUri
    {
      get
      {
        if (_webRequest != null)
        {
          return _webRequest.RequestUri;
        }
        return null;
      }
    }

    public event EventHandler<RequestCompletedEventArgs> RequestCompleted;

    public void SendRequest(Uri uri, string data)
    {
      byte[] postData = null;

      if (data != null)
      {
        postData = Encoding.UTF8.GetBytes(data);
      }

      SendRequest(uri, postData, "application/x-www-form-urlencoded");
    }


    /// <summary>
    /// </summary>
    /// <param name="uri"> </param>
    /// <param name="postData"> </param>
    /// <param name="contentType"> </param>
    public void SendRequest(Uri uri, byte[] postData, string contentType)
    {
      _webRequest = (HttpWebRequest) WebRequest.Create(uri);
#if !SILVERLIGHT
      _webRequest.KeepAlive = true;
#endif
      _webRequest.Method = Method;

      if (postData != null)
      {
        _webRequest.ContentType = contentType;
        _webRequest.BeginGetRequestStream(BeginRequest, postData);
      }
      else
      {
        _webRequest.BeginGetResponse(BeginResponse, null);
      }
    }

    /// <summary>
    ///   Writes post data to stream and begin to retrieve response from server
    /// </summary>
    /// <param name="ar"> </param>
    private void BeginRequest(IAsyncResult ar)
    {
      using (var stm = _webRequest.EndGetRequestStream(ar))
      {
        var postData = (byte[]) ar.AsyncState;
        stm.Write(postData, 0, postData.Length);
        stm.Close();
      }

      _webRequest.BeginGetResponse(BeginResponse, null);
    }

    /// <summary>
    ///   Parses response from server and signals the async operation
    /// </summary>
    /// <param name="ar"> </param>
    private void BeginResponse(IAsyncResult ar)
    {
      Stream response = null;
      FacebookException exception = null;

      try
      {
        using (var webResponse = _webRequest.EndGetResponse(ar))
        {
          var stm = webResponse.GetResponseStream();
          ContentType = webResponse.ContentType;

          using (var reader = new BinaryReader(stm))
          {
            var buffer = new byte[2048];
            response = new MemoryStream();
            int bytesRead;

            do
            {
              bytesRead = reader.Read(buffer, 0, buffer.Length);
              response.Write(buffer, 0, bytesRead);
            } while (bytesRead > 0);

            response.Position = 0;
          }
          stm.Close();
          webResponse.Close();
        }
      }
      catch (WebException e)
      {
        exception = new FacebookException("An unknown exception occured. Look at innerexception for details", e);
      }
      catch (SecurityException e)
      {
        exception =
          new FacebookException("An exception occured while downloading data. Look at innerexception for details", e);
      }


      if (RequestCompleted != null)
      {
        RequestCompleted(this, new RequestCompletedEventArgs(response, exception, _userState));
      }
    }
  }

  internal class RequestCompletedEventArgs : AsyncCompletedEventArgs
  {
    public RequestCompletedEventArgs(Stream response, Exception ex, Object userState)
      : base(ex, false, userState)
    {
      Response = response;
    }

    public Stream Response { get; private set; }
  }
}
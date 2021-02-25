﻿#if NETFX_CORE
using System.Threading.Tasks;
#endif

#if SILVERLIGHT && !WINDOWS_PHONE
    using System.Windows.Browser;
#elif !SILVERLIGHT && !WINDOWS_PHONE && !NETFX_CORE && !L2T_PCL
#endif

using System.Threading;
using System.Net.Http;
using LinqToTwitter.Net;
using StreamContent = LinqToTwitter.StreamContent;

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;
using Sobees.Library.BTwitterLib.Extensions;

#endregion

#if !SILVERLIGHT && !L2T_PCL

#else
using Ionic.Zlib;
#endif

namespace LinqToTwitter
{
  /// <summary>
  ///   Logic that performs actual communication with Twitter
  /// </summary>
  internal class TwitterExecute : ITwitterExecute, IDisposable
  {
    internal const string DefaultUserAgent = "LINQ-To-Twitter/3.0";
    internal const int DefaultReadWriteTimeout = 300000;
    internal const int DefaultTimeout = 100000;
    private readonly object asyncCallbackLock = new object();
    private readonly object streamingCallbackLock = new object();

    /// <summary>
    ///   supports testing
    /// </summary>
    public TwitterExecute(IAuthorizer authorizer)
    {
      if (authorizer == null)
      {
        throw new ArgumentNullException("authorizedClient");
      }

      Authorizer = authorizer;
      Authorizer.UserAgent = Authorizer.UserAgent ?? DefaultUserAgent;
    }

    /// <summary>
    ///   Assign your TextWriter instance to receive LINQ to Twitter output
    /// </summary>
    public static TextWriter Log { get; set; }

    /// <summary>
    ///   HttpClient instance being used in a streaming operation
    /// </summary>
    internal HttpClient StreamingClient { get; set; }

    /// <summary>
    ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Gets or sets the object that can send authorized requests to Twitter.
    /// </summary>
    public IAuthorizer Authorizer { get; set; }

    /// <summary>
    ///   Timeout (milliseconds) for writing to request
    ///   stream or reading from response stream
    /// </summary>
    public int ReadWriteTimeout { get; set; }

    /// <summary>
    ///   Timeout (milliseconds) to wait for a server response
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    ///   Gets the most recent URL executed
    /// </summary>
    /// <remarks>
    ///   This is very useful for debugging
    /// </remarks>
    public Uri LastUrl { get; private set; }

    /// <summary>
    ///   list of response headers from query
    /// </summary>
    public IDictionary<string, string> ResponseHeaders { get; set; }

    /// <summary>
    ///   Gets and sets HTTP UserAgent header
    /// </summary>
    public string UserAgent
    {
      get { return Authorizer.UserAgent; }
      set
      {
        Authorizer.UserAgent =
          string.IsNullOrWhiteSpace(value)
            ? Authorizer.UserAgent
            : value + ", " + Authorizer.UserAgent;
      }
    }

    /// <summary>
    ///   Allows users to process content returned from stream
    /// </summary>
    public Func<StreamContent, Task> StreamingCallbackAsync { get; set; }

    /// <summary>
    ///   Set to true to close stream, false means stream is still open
    /// </summary>
    public bool IsStreamClosed { get; internal set; }

    public CancellationToken CancellationToken
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public Task<string> PostToTwitterAsync<T>(string url, IDictionary<string, string> postData, CancellationToken cancelToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> PostMediaAsync(string url, IDictionary<string, string> postData, byte[] image, string name, string fileName, string contentType,
      CancellationToken cancelToken)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Used in queries to read information from Twitter API endpoints.
    /// </summary>
    /// <param name="request">Request with url endpoint and all query parameters</param>
    /// <param name="reqProc">Request Processor for Async Results</param>
    /// <returns>XML Respose from Twitter</returns>
    public async Task<string> QueryTwitterAsync<T>(Request request, IRequestProcessor<T> reqProc)
    {
      WriteLog(request.FullUrl, "QueryTwitterAsync");

      var req = new HttpRequestMessage(HttpMethod.Get, request.FullUrl);

      var parms = request.RequestParameters
        .ToDictionary(
          key => key.Name,
          val => val.Value);
      var handler = new GetMessageHandler(this, parms, request.FullUrl);

      using (var client = new HttpClient(handler))
      {
        if (Timeout != 0)
          client.Timeout = new TimeSpan(0, 0, 0, Timeout);

        var msg = await client.SendAsync(req);

        return await HandleResponseAsync(msg);
      }
    }

    /// <summary>
    ///   Performs a query on the Twitter Stream.
    /// </summary>
    /// <param name="request">Request with url endpoint and all query parameters.</param>
    /// <returns>
    ///   Caller expects an JSON formatted string response, but
    ///   real response(s) with streams is fed to the callback.
    /// </returns>
    public async Task<string> QueryTwitterStreamAsync(Request request)
    {
      WriteLog(request.FullUrl, "QueryTwitterAsync");

      var handler = new HttpClientHandler();
      if (Authorizer.Proxy != null && handler.SupportsProxy)
        handler.Proxy = Authorizer.Proxy;

      using (StreamingClient = new HttpClient(handler))
      {
        StreamingClient.Timeout = TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.Endpoint);

        var parameters =
          string.Join("&",
            (from parm in request.RequestParameters
              select parm.Name + "=" + Url.PercentEncode(parm.Value))
              .ToList());
        var content = new StringContent(parameters, Encoding.UTF8, "application/x-www-form-urlencoded");
        httpRequest.Content = content;

        var parms = request.RequestParameters
          .ToDictionary(
            key => key.Name,
            val => val.Value);
        SetAuthorizationHeader(HttpMethod.Post, request.FullUrl, parms, httpRequest);
        httpRequest.Headers.Add("User-Agent", UserAgent);
        httpRequest.Headers.ExpectContinue = false;

        //if (Authorizer.SupportsCompression)
        //  httpRequest.Headers.AcceptEncoding.TryParseAdd("gzip");

        var response = await StreamingClient.SendAsync(
          httpRequest, HttpCompletionOption.ResponseHeadersRead);

        await TwitterErrorHandler.ThrowIfErrorAsync(response);

        using (var stream = await response.Content.ReadAsStreamAsync())
          //if (Authorizer.SupportsCompression)
          //{
          //  using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
          //  {
          //    var compressedBuffer = new byte[8192];

          //    while (stream.CanRead && !IsStreamClosed)
          //    {
          //      var readCount = await gzip.ReadAsync(compressedBuffer, 0, compressedBuffer.Length);

          //      // When Twitter breaks the connection, we need to exit the
          //      // entire loop and start over. Otherwise, the reads
          //      // keep returning blank lines that are incorrectly interpreted
          //      // as keep-alive messages in a tight loop.
          //      if (readCount == 0)
          //      {
          //        if (!IsStreamClosed)
          //        {
          //          IsStreamClosed = true;
          //          throw new WebException("Twitter closed the stream.", WebExceptionStatus.ConnectFailure);
          //        }

          //        break;
          //      }

          //      if (!compressedBuffer.Contains((byte) 0x0D)) continue;

          //      var outputString = Encoding.UTF8.GetString(compressedBuffer, 0, readCount);

          //      var lines = outputString.Split(new[] {"\r\n"}, new StringSplitOptions());
          //      for (var i = 0; i < (lines.Length - 1); i++)
          //      {
          //        var strmContent = new StreamContent(this, lines[i]);

          //        await StreamingCallbackAsync(strmContent);
          //      }

          //      compressedBuffer = new byte[8192];
          //    }
          //  }
          //}
          //else
          {
            using (var reader = new StreamReader(stream))
            {
              while (stream.CanRead && !IsStreamClosed)
              {
                var line = await reader.ReadLineAsync();

                var strmContent = new StreamContent(this, line);

                await StreamingCallbackAsync(strmContent);
              }
            }
          }
      }

      IsStreamClosed = false;

      return "<streaming></streaming>";
    }

    /// <summary>
    ///   Closes the stream
    /// </summary>
    public void CloseStream()
    {
      IsStreamClosed = true;

      if (StreamingClient != null)
        StreamingClient.CancelPendingRequests();
    }

    /// <summary>
    ///   Performs HTTP POST media byte array upload to Twitter.
    /// </summary>
    /// <param name="url">Url to upload to.</param>
    /// <param name="postData">Request parameters.</param>
    /// <param name="data">Image to upload.</param>
    /// <param name="name">Image parameter name.</param>
    /// <param name="fileName">Image file name.</param>
    /// <param name="contentType">Type of image: must be one of jpg, gif, or png.</param>
    /// <param name="reqProc">Request processor for handling results.</param>
    /// <returns>JSON response From Twitter.</returns>
    public async Task<string> PostMediaAsync(string url, IDictionary<string, string> postData, byte[] data, string name,
      string fileName, string contentType)
    {
      WriteLog(url, "QueryTwitterAsync");

      var multiPartContent = new MultipartFormDataContent();
      var byteArrayContent = new ByteArrayContent(data);
      byteArrayContent.Headers.Add("Content-Type", contentType);
      multiPartContent.Add(byteArrayContent, name, fileName);

      var cleanPostData = new Dictionary<string, string>();

      foreach (var pair in postData)
      {
        if (pair.Value != null)
        {
          cleanPostData.Add(pair.Key, pair.Value);
          multiPartContent.Add(new StringContent(pair.Value), pair.Key);
        }
      }

      var handler = new PostMessageHandler(this, new Dictionary<string, string>(), url);
      using (var client = new HttpClient(handler))
      {
        if (Timeout != 0)
          client.Timeout = new TimeSpan(0, 0, 0, Timeout);

        HttpResponseMessage msg = await client.PostAsync(url, multiPartContent);

        return await HandleResponseAsync(msg);
      }
    }

    /// <summary>
    ///   performs HTTP POST to Twitter
    /// </summary>
    /// <param name="url">URL of request</param>
    /// <param name="postData">parameters to post</param>
    /// <param name="getResult">callback for handling async Json response - null if synchronous</param>
    /// <returns>Json Response from Twitter - empty string if async</returns>
    public async Task<string> PostToTwitterAsync<T>(string url, IDictionary<string, string> postData)
    {
      WriteLog(url, "PostToTwitterAsync");

      var cleanPostData = new Dictionary<string, string>();

      var dataString = new StringBuilder();

      foreach (var pair in postData)
      {
        if (pair.Value != null)
        {
          dataString.AppendFormat("{0}={1}&", pair.Key, Url.PercentEncode(pair.Value));
          cleanPostData.Add(pair.Key, pair.Value);
        }
      }

      var content = new StringContent(dataString.ToString().TrimEnd('&'), Encoding.UTF8,
        "application/x-www-form-urlencoded");
      var handler = new PostMessageHandler(this, cleanPostData, url);
      using (var client = new HttpClient(handler))
      {
        if (Timeout != 0)
          client.Timeout = new TimeSpan(0, 0, 0, Timeout);

        HttpResponseMessage msg = await client.PostAsync(url, content);

        return await HandleResponseAsync(msg);
      }
    }

    internal void SetAuthorizationHeader(HttpMethod method, string url, IDictionary<string, string> parms,
      HttpRequestMessage req)
    {
      var authStringParms = parms.ToDictionary(parm => parm.Key, elm => elm.Value);
      authStringParms.Add("oauth_consumer_key", Authorizer.CredentialStore.ConsumerKey);
      authStringParms.Add("oauth_token", Authorizer.CredentialStore.OAuthToken);

      var authorizationString = Authorizer.GetAuthorizationString(method.Method, url, authStringParms);

      req.Headers.Add("Authorization", authorizationString);
    }

    private async Task<string> HandleResponseAsync(HttpResponseMessage msg)
    {
      LastUrl = msg.RequestMessage.RequestUri;

      ResponseHeaders =
        (from header in msg.Headers
          select new
          {
            header.Key,
            Value = string.Join(", ", header.Value)
          })
          .ToDictionary(
            pair => pair.Key,
            pair => pair.Value);

      await TwitterErrorHandler.ThrowIfErrorAsync(msg);

      return await msg.Content.ReadAsStringAsync();
    }

    private void WriteLog(string content, string currentMethod)
    {
      if (Log != null)
      {
        Log.WriteLine("--Log Starts Here--");
        Log.WriteLine("Query:" + content);
        Log.WriteLine("Method:" + currentMethod);
        Log.WriteLine("--Log Ends Here--");
        Log.Flush();
      }
    }

    /// <summary>
    ///   Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    ///   unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        StreamingCallbackAsync = null;

        if (Log != null)
        {
          Log.Dispose();
        }
      }
    }
  }
}






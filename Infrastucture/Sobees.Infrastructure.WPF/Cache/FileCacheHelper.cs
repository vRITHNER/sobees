#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Sobees.Configuration.BGlobals;

#endregion

namespace Sobees.Infrastructure.Cache
{
  public sealed class FileCacheHelper : IDisposable
  {
    // The list of photos that are being requested.
    // It's a stack, assuming that the most recent request is the one that the UI
    // really wants to show first.
    private static readonly Object SaveFile = new object();
    private readonly Stack<_DataRequest> _activePhotoRequests = new Stack<_DataRequest>();
    private readonly Stack<_DataRequest> _activeWebRequests = new Stack<_DataRequest>();
    private readonly string _cachePathBig;
    private readonly string _cachePathNormal;
    private readonly string _cachePathSmall;
    private readonly Dispatcher _callbackDispatcher;
    // The list of photos that the UI may want to show at some point.
    // If we don't have any active requests happening we can preemptively pull photos
    // from the web so we'll have them cached when they are requested.
    // TODO: We should also clean up old photos that are no longer part of the app.
    //    We can do this with rotating folders or just keeping track of which photos
    //    are being requested and after a certain amount of time start deleting the rest.
    private readonly object _localLock = new object();
    private readonly Stack<SmallUri> _passiveWebRequests = new Stack<SmallUri>();
    private readonly object _webLock = new object();
    private DispatcherPool _asyncPhotoPool;
    private DispatcherPool _asyncWebRequestPool;
    private bool _disposed;

    public FileCacheHelper(Dispatcher dispatcher, string settingsPath)
    {
      //Verify.IsNeitherNullNorEmpty(settingsPath, "settingsPath");
      //Verify.IsNotNull(dispatcher, "dispatcher");

      _callbackDispatcher = dispatcher;
      _cachePathSmall = Path.Combine(settingsPath, "ImageCache\\Small") + "\\";
      _cachePathNormal = Path.Combine(settingsPath, "ImageCache\\Normal") + "\\";
      _cachePathBig = Path.Combine(settingsPath, "ImageCache\\Big") + "\\";
      EnsureDirectory(_cachePathBig);
      EnsureDirectory(_cachePathNormal);
      EnsureDirectory(_cachePathSmall);

      _asyncPhotoPool = new DispatcherPool("Photo Fetching Thread", 1);
      _asyncWebRequestPool = new DispatcherPool("Web Photo Fetching Thread", 2,
                                                () => new WebClient {CachePolicy = HttpWebRequest.DefaultCachePolicy});
    }

    public static void EnsureDirectory(string path)
    {
      if (!Directory.Exists(Path.GetDirectoryName(path)))
      {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
      }
    }

    public static void CopyStream(Stream destination, Stream source)
    {
      //Assert.IsNotNull(source);
      //Assert.IsNotNull(destination);

      destination.Position = 0;

      // If we're copying from, say, a web stream, don't fail because of this.
      if (source.CanSeek)
      {
        source.Position = 0;

        // Consider that this could throw because 
        // the source stream doesn't know it's size...
        destination.SetLength(source.Length);
      }

      var buffer = new byte[4096];
      int cbRead;

      do
      {
        cbRead = source.Read(buffer, 0, buffer.Length);
        if (0 != cbRead)
        {
          destination.Write(buffer, 0, cbRead);
        }
      } while (buffer.Length == cbRead);

      // Reset the Seek pointer before returning.
      destination.Position = 0;
    }

    private static BitmapSource _GetBitmapSourceFromStream(Stream stream)
    {
      //Verify.IsNotNull(stream, "stream");

      var imgstm = new MemoryStream((int) stream.Length);
      CopyStream(imgstm, stream);
      try
      {
        var bi = new BitmapImage();
        {
          bi.BeginInit();
          bi.StreamSource = imgstm;
          bi.CacheOption = BitmapCacheOption.OnLoad;
          bi.EndInit();
          bi.Freeze();
        }

        return bi;
      }
      catch (NotSupportedException e)
      {
        throw new InvalidOperationException("The stream doesn't represent an image.", e);
      }
    }

    private void _Verify()
    {
      if (_disposed)
      {
        throw new ObjectDisposedException("this");
      }
    }

    public static string GetHashString(string value)
    {
      var signatureHash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value));
      var signature = signatureHash.Aggregate(
        new StringBuilder(),
        (sb, b) => sb.Append(b.ToString("x2"))).ToString();
      return signature;
    }

    private string _GenerateCachePath(SmallUri ssUri, ImageKind kind)
    {
      var key = ssUri.GetString();
      if (key.Length > 100)
      {
        key = GetHashString(key) + key.Substring(key.Length - 100);
      }
      var filePart = key
        .Replace('\\', '_')
        .Replace('/', '_')
        .Replace(':', '_')
        .Replace('*', '_')
        .Replace('?', '_')
        .Replace('\"', '_')
        .Replace('<', '_')
        .Replace('>', '_')
        .Replace('|', '_');
      switch (kind)
      {
        case ImageKind.Small:
          return Path.Combine(_cachePathSmall, filePart);
        case ImageKind.Normal:
          return Path.Combine(_cachePathNormal, filePart);
        case ImageKind.Big:
          return Path.Combine(_cachePathBig, filePart);
        default:
          return Path.Combine(_cachePathSmall, filePart);
      }
    }

    public void QueueImageRequest(SmallUri ssUri, ImageKind kind)
    {
      ssUri.Kind = kind;
      if (_disposed)
        return;

      if (ssUri == default(SmallUri))
        return;

      if (IsImageCached(ssUri, kind))
        return;
      
      var needToQueue = false;
      lock (_localLock)
      {
        needToQueue = !_asyncWebRequestPool.HasPendingRequests;
        _passiveWebRequests.Push(ssUri);
      }

      if (!needToQueue) return;
      // When we queue web requests, do it twice to effectively use the pool.
      // Since _ProcessNextWebRequest operates in a tight loop flushing our list it's not aware
      // that the work can be shared.
      _asyncWebRequestPool.QueueRequest(DispatcherPriority.Background, _ProcessNextWebRequest, null);
      _asyncWebRequestPool.QueueRequest(DispatcherPriority.Background, _ProcessNextWebRequest, null);
    }

    public void GetImageSourceAsync(object sender, object userState, SmallUri ssUri, ImageKind kind,
                                    GetImageSourceAsyncCallback callback)
    {
      //Verify.UriIsAbsolute(uri, "uri");
      //Verify.IsNotNull(sender, "sender");
      // UserState may be null.
      // Verify.IsNotNull(userState, "userState");
      //Verify.IsNotNull(callback, "callback");

      if (_disposed)
      {
        callback(this, new GetImageSourceCompletedEventArgs(new ObjectDisposedException("this"), false, userState));
        return;
      }

      if (default(SmallUri) == ssUri)
      {
        callback(this,
                 new GetImageSourceCompletedEventArgs(
                   new ArgumentException("The requested image doesn't exist.", "ssUri"), false, userState));
        return;
      }

      // Make asynchronous request to get ImageSource object from the data feed.
      var imageRequest = new _DataRequest
                           {
                             Sender = sender,
                             UserState = userState,
                             SmallUri = ssUri,
                             Callback = callback,
                             Kind = kind
                           };

      var needToCache = _GetCacheLocation(ssUri, kind) == null;
      if (needToCache)
      {
        var needToQueue = false;
        lock (_webLock)
        {
          needToQueue = !_asyncWebRequestPool.HasPendingRequests;
          _activeWebRequests.Push(imageRequest);
        }

        if (needToQueue)
        {
          _asyncWebRequestPool.QueueRequest(_ProcessNextWebRequest, null);
          _asyncWebRequestPool.QueueRequest(_ProcessNextWebRequest, null);
        }
      }
      else
      {
        var needToQueue = false;
        lock (_localLock)
        {
          needToQueue = !_asyncPhotoPool.HasPendingRequests;
          _activePhotoRequests.Push(imageRequest);
        }

        if (needToQueue)
        {
          _asyncPhotoPool.QueueRequest(_ProcessNextLocalRequest, null);
        }
      }
    }

    private string _GetCacheLocation(SmallUri ssUri, ImageKind kind)
    {
      var path = _GenerateCachePath(ssUri, kind);
      if (File.Exists(path))
      {
        return path;
      }
      return null;
    }

    public bool IsImageCached(SmallUri ssUri, ImageKind kind)
    {
      _Verify();
      return _GetCacheLocation(ssUri, kind) != null;
    }

    public string GetImageFile(SmallUri ssUri, ImageKind kind)
    {
      _Verify();
      return _GetCacheLocation(ssUri, kind);
    }

    private void _ProcessNextLocalRequest(object unused)
    {
      while (_asyncPhotoPool != null)
      {
        // Retrieve the next data request for processing.
        GetImageSourceAsyncCallback callback = null;
        object userState = null;
        object sender = null;
        var kind = ImageKind.Small;
        var ssUri = default(SmallUri);

        lock (_localLock)
        {
          while (_activePhotoRequests.Count > 0)
          {
            var nextDataRequest = _activePhotoRequests.Pop();
            if (!nextDataRequest.Canceled)
            {
              sender = nextDataRequest.Sender;
              callback = nextDataRequest.Callback;
              userState = nextDataRequest.UserState;
              ssUri = nextDataRequest.SmallUri;
              kind = nextDataRequest.Kind;
              break;
            }
          }

          if (ssUri == default(SmallUri))
          {
            //Assert.AreEqual(0, _activePhotoRequests.Count);
            return;
          }
        }

        //Assert.IsNotNull(callback);
        GetImageSourceCompletedEventArgs callbackArgs = null;
        string localCachePath = null;
        try
        {
          localCachePath = _GetCacheLocation(ssUri, kind);
          if (localCachePath == null)
          {
            callbackArgs = new GetImageSourceCompletedEventArgs(new FileNotFoundException(), false, userState);
          }
          else
          {
            using (Stream imageStream = File.Open(localCachePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
              callbackArgs = new GetImageSourceCompletedEventArgs(_GetBitmapSourceFromStream(imageStream), userState);
            }
          }
        }
        catch (Exception e)
        {
          if (!string.IsNullOrEmpty(localCachePath))
          {
            // If the cache is bad try removing the file so we can fetch it correctly next time.
            try
            {
              File.Delete(localCachePath);
            }
            catch
            {
            }
          }
          callbackArgs = new GetImageSourceCompletedEventArgs(e, false, userState);
        }

        _callbackDispatcher.BeginInvoke(callback, sender, callbackArgs);
      }
    }

    private void _ProcessNextWebRequest(object unused)
    {
      if (_asyncWebRequestPool == null)
      {
        return;
      }

      var webClient = _asyncWebRequestPool.Tag as WebClient;
      //Assert.IsNotNull(webClient);

      while (_asyncWebRequestPool != null)
      {
        // Retrieve the next data request for processing.
        _DataRequest activeRequest = null;
        var ssUri = default(SmallUri);
        var isPassive = false;

        var kind = ImageKind.Small;
        lock (_webLock)
        {
          while (_activeWebRequests.Count > 0)
          {
            activeRequest = _activeWebRequests.Pop();
            if (!activeRequest.Canceled)
            {
              ssUri = activeRequest.SmallUri;
              kind = activeRequest.Kind;
              break;
            }
          }

          if (ssUri == default(SmallUri) && _passiveWebRequests.Count > 0)
          {
            ssUri = _passiveWebRequests.Pop();
            kind = ssUri.Kind;
            isPassive = true;
          }

          if (ssUri == default(SmallUri))
          {
            //Assert.AreEqual(0, _activeWebRequests.Count);
            //Assert.AreEqual(0, _passiveWebRequests.Count);
            return;
          }
        }

        try
        {
          var localCachePath = _GenerateCachePath(ssUri, kind);
          if (!File.Exists(localCachePath))
          {
            // There's a potential race here with other attempts to write the same file.
            // We don't really care because there's not much we can do about it when
            // it happens from multiple processes.
            var tempFile = Path.GetTempFileName();
            var address = ssUri.GetUri();
            try
            {
              var url = ssUri.GetUri().OriginalString.Replace("_normal", "_bigger");
              webClient.DownloadFile(new Uri(url), tempFile);
            }
            catch (WebException)
            {
              // Fail once, just try again.  Servers are flakey.
              // Fails again let it throw.  Caller is expected to catch.
              webClient.DownloadFile(address, tempFile);
            }
            CreateResizedImage(tempFile, kind);
            // Should really block multiple web requests for the same file, which causes this...
            lock (SaveFile)
            {
              if (!File.Exists(localCachePath))
              {
                File.Move(tempFile, localCachePath);
              }
            }
          }

          if (!isPassive)
          {
            var needToQueue = false;
            lock (_localLock)
            {
              needToQueue = !_asyncPhotoPool.HasPendingRequests;
              _activePhotoRequests.Push(activeRequest);
            }

            if (needToQueue)
            {
              _asyncPhotoPool.QueueRequest(_ProcessNextLocalRequest, null);
            }
          }
        }
        catch
        {
        }
      }
    }

    private static void CreateResizedImage(string s, ImageKind kind)
    {
      var bi = new BitmapImage();
      bi.BeginInit();
      switch (kind)
      {
        case ImageKind.Small:
          bi.DecodePixelHeight = 47;
          bi.DecodePixelWidth = 47;
          break;
        case ImageKind.Normal:
          bi.DecodePixelHeight = 100;
          bi.DecodePixelWidth = 100;
          break;
        case ImageKind.Big:
          break;
        default:
          bi.DecodePixelHeight = 10;
          bi.DecodePixelWidth = 10;
          break;
      }
      bi.CacheOption = BitmapCacheOption.OnLoad;
      bi.UriSource = new Uri(s);
      bi.EndInit();
      var encoder = new PngBitmapEncoder {};
      encoder.Interlace = PngInterlaceOption.Off;
      var stream = new FileStream(s, FileMode.Create);
      encoder.Frames.Add(BitmapFrame.Create(bi));
      encoder.Save(stream);
      stream.Close();
    }

    public static void SafeDispose<T>(ref T disposable) where T : IDisposable
    {
      // Dispose can safely be called on an object multiple times.
      IDisposable t = disposable;
      disposable = default(T);
      if (null != t)
      {
        t.Dispose();
      }
    }

    internal void Shutdown(Action<string> deleteCallback)
    {
      _disposed = true;
      SafeDispose(ref _asyncPhotoPool);
      SafeDispose(ref _asyncWebRequestPool);

      if (deleteCallback != null)
      {
        deleteCallback(_cachePathBig);
        deleteCallback(_cachePathSmall);
        deleteCallback(_cachePathNormal);
      }
    }

    public void CleanCache()
    {
      try
      {
        var tmp = DateTime.Now;
        foreach (var file in Directory.GetFiles(_cachePathBig))
        {
          if (DateTime.Now.Subtract(File.GetLastAccessTime(file)).TotalDays > BGlobals.CACHE_KEEP_TIME)
          {
            File.Delete(file);
          }
        }
        foreach (var file in Directory.GetFiles(_cachePathNormal))
        {
          if (DateTime.Now.Subtract(File.GetLastAccessTime(file)).TotalDays > BGlobals.CACHE_KEEP_TIME)
          {
            File.Delete(file);
          }
        }
        foreach (var file in Directory.GetFiles(_cachePathSmall))
        {
          if (DateTime.Now.Subtract(File.GetLastAccessTime(file)).TotalDays > BGlobals.CACHE_KEEP_TIME)
          {
            File.Delete(file);
          }
        }
        var i = DateTime.Now.Subtract(tmp).TotalMilliseconds;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    public void CleanCacheAll()
    {
      try
      {
        var tmp = DateTime.Now;
        foreach (var file in Directory.GetFiles(_cachePathBig))
        {
          File.Delete(file);
        }
        foreach (var file in Directory.GetFiles(_cachePathNormal))
        {
          File.Delete(file);
        }
        foreach (var file in Directory.GetFiles(_cachePathSmall))
        {
          File.Delete(file);
        }
        var i = DateTime.Now.Subtract(tmp).TotalMilliseconds;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    #region Nested type: _DataRequest

    /// <summary>
    ///   An AsyncDataRequest that saves the context of the request.
    /// </summary>
    private sealed class _DataRequest
    {
      public object Sender { get; set; }
      public bool Canceled { get; set; }
      public SmallUri SmallUri { get; set; }
      public ImageKind Kind { get; set; }
      public object UserState { get; set; }
      public GetImageSourceAsyncCallback Callback { get; set; }
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      Shutdown(null);
    }

    #endregion
  }
}
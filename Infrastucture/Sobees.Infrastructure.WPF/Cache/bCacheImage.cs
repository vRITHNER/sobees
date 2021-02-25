using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Sobees.Configuration.BGlobals;

namespace Sobees.Infrastructure.Cache
{
  public class BCacheImage
  {
    private static readonly string Folder = BGlobals.FOLDER;

    public static FileCacheHelper WebGetter = new FileCacheHelper(System.Windows.Application.Current.Dispatcher, Folder);

    private Thickness? _margin;
    private ImageKind _imageKind;
    private SmallUri? _normal;
    private WeakReference _normalWR;


    public BCacheImage()
    {
    }
    public BCacheImage(Uri uri,ImageKind imageKind)
    {
      if (uri != null)
      {
        _normal = new SmallUri(uri.OriginalString, _imageKind);
        _imageKind = imageKind;
        WebGetter.QueueImageRequest(_normal.Value,_imageKind);
      }
    }
    public BCacheImage(Uri uri)
    {
      if (uri != null)
      {
        _normal = new SmallUri(uri.OriginalString, _imageKind);
        WebGetter.QueueImageRequest(_normal.Value, ImageKind.Normal);
      }
    }

    public void GetImageAsync(GetImageSourceAsyncCallback callback)
    {
      callback(this,
               new GetImageSourceCompletedEventArgs(
                 new InvalidOperationException("The requested image doesn't exist"), false, this));

      BitmapSource img;
      if (_TryGetFromWeakCache(out img))
      {
        callback(this, new GetImageSourceCompletedEventArgs(img, this));
      }

      var userState = new _ImageCallbackState {Callback = callback};
      WebGetter.GetImageSourceAsync(this, userState, (SmallUri) _normal,_imageKind, _AddWeakCacheCallback);
    }

    private bool _TryGetFromWeakCache(out BitmapSource img)
    {
      img = null;
      if (_normalWR != null)
      {
        img = _normalWR.Target as BitmapSource;
      }
      return img != null;
    }

    private void _AddWeakCacheCallback(object sender, GetImageSourceCompletedEventArgs e)
    {
      if (e.Error != null || e.Cancelled)
      {
        return;
      }

      var bs = (BitmapSource) e.ImageSource;
      if (_margin.HasValue)
      {
        Thickness margin = _margin.Value;
        bs = new CroppedBitmap(bs,
                               new Int32Rect((int) (margin.Left*bs.Width), (int) (margin.Top*bs.Height),
                                             (int) (bs.Width - (margin.Left + margin.Right)*bs.Width),
                                             (int) (bs.Height - (margin.Top + margin.Bottom)*bs.Height)));
      }

      var userState = (_ImageCallbackState) e.UserState;

      _normalWR = new WeakReference(bs);
      if (userState.Callback != null)
      {
        userState.Callback(this, new GetImageSourceCompletedEventArgs(bs, this));
      }
    }

    /// <remarks>
    /// This is a synchronous operation that actively fetches this image.
    /// </remarks>
    public string GetCachePath()
    {
      SmallUri sizedString = _GetSmallUriFromRequestedSize();
      //Assert.IsNotDefault(sizedString);
      return WebGetter.GetImageFile(sizedString,_imageKind);
    }

    /// <remarks>
    /// This is a synchronous operation.
    /// </remarks>
    public void SaveToFile(string path)
    {
      string cachePath = GetCachePath();
      if (cachePath == null)
      {
        return;
      }

      File.Copy(cachePath, path);
    }

    public bool IsCached()
    {
      SmallUri sizedString = _GetSmallUriFromRequestedSize();
      //Assert.IsNotDefault(sizedString);
      return WebGetter.IsImageCached(sizedString, _imageKind);
    }

    private SmallUri _GetSmallUriFromRequestedSize()
    {
      // If one url type isn't available, try to fallback on other provided values.

      SmallUri? str = null;

      str = _normal;


      return str ?? default(SmallUri);
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as BCacheImage);
    }

    public override int GetHashCode()
    {
      return _normal.GetHashCode();
    }

    public bool Equals(BCacheImage other)
    {
      if (other == null)
      {
        return false;
      }

      return other._normal == _normal;
    }

    internal bool SafeMerge(BCacheImage other)
    {
      bool modified = false;

      if (other == null)
      {
        return false;
      }

      if (_normal == null && other._normal != null)
      {
        _normal = other._normal;
        modified = true;
      }

      return modified;
    }

    public BCacheImage Clone()
    {
      var img = new BCacheImage
                  {
                    _margin = _margin,
                    _normal = _normal,
                    _normalWR = _normalWR,
                  };

      return img;
    }

    #region Nested type: _ImageCallbackState

    private class _ImageCallbackState
    {
      public GetImageSourceAsyncCallback Callback { get; set; }
    }

    #endregion
  }
}
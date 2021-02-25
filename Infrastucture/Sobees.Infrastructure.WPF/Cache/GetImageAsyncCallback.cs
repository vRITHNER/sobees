using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sobees.Infrastructure.Cache
{
  public delegate void GetImageSourceAsyncCallback(object sender, GetImageSourceCompletedEventArgs e);

  /// <summary>
  /// Event arguments accompanying delegate for GetImageSource event.
  /// </summary>
  public class GetImageSourceCompletedEventArgs : AsyncCompletedEventArgs
  {
    /// <summary>The ImageSource representing data from the Internet resource.</summary>
    private readonly ImageSource _imageSource;

    private readonly Size? _naturalSize;

    /// <summary>
    /// Initializes a new instance of the GetImageSourceCompletedEventArgs class for successful completion.
    /// </summary>
    /// <param name="bitmapSource">The ImageSource representing data from the Internet resource.</param>
    /// <param name="userState">The user-supplied state object.</param>
    public GetImageSourceCompletedEventArgs(BitmapSource bitmapSource, object userState)
      : base(null, false, userState)
    {
      if (bitmapSource == null)
        return;
      
      _imageSource = bitmapSource;
      _naturalSize = new Size(bitmapSource.Width, bitmapSource.Height);
    }

    /// <summary>
    /// Initializes a new instance of the GetImageSourceCompletedEventArgs class for an error or a cancellation.
    /// </summary>
    /// <param name="error">Any error that occurred during the asynchronous operation.</param>
    /// <param name="cancelled">A value indicating whether the asynchronous operation was canceled.</param>
    /// <param name="userState">The user-supplied state object.</param>
    public GetImageSourceCompletedEventArgs(Exception error, bool cancelled, object userState)
      : base(error, cancelled, userState)
    {
    }

    /// <summary>
    /// Gets the ImageSource representing data from the Internet resource.
    /// </summary>
    public ImageSource ImageSource
    {
      get
      {
        RaiseExceptionIfNecessary();
        return _imageSource;
      }
    }

    public Size? NaturalSize
    {
      get
      {
        RaiseExceptionIfNecessary();
        return _naturalSize;
      }
    }
  }
}
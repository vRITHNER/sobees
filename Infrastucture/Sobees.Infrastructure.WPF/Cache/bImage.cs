using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sobees.Tools.Logging;

namespace Sobees.Infrastructure.Cache
{
  public enum ImageType
  {
    Person,
    Other,
  }

  public enum ImageKind
  {
    Small,
    Normal,
    Big,
  }

  /// <summary>
  /// Control used to display a friend image thumbnail.
  /// </summary>
  public class BImage : Control
  {
    /// <summary>Dependency Property backing store for ImageSource.</summary>
    private static readonly DependencyPropertyKey _ImageSourcePropertyKey = DependencyProperty.RegisterReadOnly(
      "ImageSource",
      typeof(ImageSource),
      typeof(BImage),
      new UIPropertyMetadata(null));

    public static readonly DependencyProperty ActivityColorProperty =
      DependencyProperty.Register(
        "ActivityColor",
        typeof(Color),
        typeof(BImage),
        new UIPropertyMetadata(Colors.White, OnActivityColorChanged));

    /// <summary>Dependency Property backing store for BCacheImage.</summary>
    public static readonly DependencyProperty BCacheImageProperty = DependencyProperty.Register(
      "BCacheImage",
      typeof(string),
      typeof(BImage),
      new UIPropertyMetadata((d, e) => ((BImage)d)._UpdateImage()));

    public static readonly DependencyProperty ImagePaddingProperty =
      DependencyProperty.Register(
        "ImagePadding",
        typeof(double),
        typeof(BImage),
        new UIPropertyMetadata(0.0));

    public static readonly DependencyProperty ImageSourceProperty = _ImageSourcePropertyKey.DependencyProperty;

    public static readonly DependencyProperty ImageTypeProperty = DependencyProperty.Register(
      "ImageType",
      typeof(ImageType),
      typeof(BImage),
      new PropertyMetadata(ImageType.Person));


    public static readonly DependencyProperty ImageKindProperty = DependencyProperty.Register(
      "ImageKind",
      typeof(ImageKind),
      typeof(BImage),
      new UIPropertyMetadata((d, e) => ((BImage)d)._UpdateImage()));

    /// <summary>
    /// SizeBasedOnContent Dependency Property
    /// </summary>
    public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.Register(
      "SizeToContent",
      typeof(SizeToContent),
      typeof(BImage),
      new UIPropertyMetadata(
        SizeToContent.Manual,
        (d, e) => ((BImage)d)._UpdateImage()));

    private static ImageBrush _anonymousLandscapeBrush;

    /// <summary>
    /// The default photo for an anonymous user.
    /// </summary>
    private static ImageBrush _anonymousPersonBrush;

    /// <summary>
    /// The pen used to draw the image border.
    /// </summary>
    private static Pen borderPen;

    /// <summary>
    /// A Dictionary cache of previously used linear gradient brushes.
    /// </summary>
    private static Dictionary<Color, LinearGradientBrush> gradientBrushes = new Dictionary<Color, LinearGradientBrush>();

    /// <summary>
    /// The rect used to draw the ImageThumbnailControl.
    /// </summary>
    private Rect brushRect = Rect.Empty;

    /// <summary>
    /// The gradient (from the cache) for this photo.
    /// </summary>
    private LinearGradientBrush gradient;

    /// <summary>
    /// The brush used to draw the user's image.
    /// </summary>
    private ImageBrush userImage;

    /// <summary>
    /// Initializes static members of the ImageThumbnailControl class.
    /// </summary>
    static BImage()
    {
      try
      {
        Type myType = typeof(BImage);

        // Override some property defaults.
        VerticalAlignmentProperty.OverrideMetadata(myType, new FrameworkPropertyMetadata(VerticalAlignment.Stretch));
        HorizontalAlignmentProperty.OverrideMetadata(myType, new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
        RenderOptions.BitmapScalingModeProperty.OverrideMetadata(myType,
                                                                 new FrameworkPropertyMetadata(BitmapScalingMode.Linear));

        borderPen = new Pen(Brushes.Black, 1.0);
        borderPen.Freeze();

        var bi = new BitmapImage();
        bi.BeginInit();
        bi.CacheOption = BitmapCacheOption.OnLoad;
        bi.DecodePixelWidth = 47;
        bi.UriSource = new Uri(@"pack://application:,,,/Sobees.Templates;Component/Images/Icon/icon.png");
        bi.EndInit();

        _anonymousPersonBrush = new ImageBrush(bi);
        _anonymousPersonBrush.Freeze();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("Constructor BImage",ex);
      }

    }

    /// <summary>
    /// Initializes a new instance of the PhotoThumbnailControl class.
    /// Adds a render transform for animating via XAML styles.
    /// </summary>
    public BImage()
    {
      try
      {
        var thumbnailScaleTransform = new ScaleTransform(1.0, 1.0);
        var thumbnailTransformGroup = new TransformGroup();
        thumbnailTransformGroup.Children.Add(thumbnailScaleTransform);
        RenderTransform = thumbnailTransformGroup;

        gradient = null;
        gradientBrushes.TryGetValue(ActivityColor, out gradient);
        ActivityColor = new Color();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,ex);
      }
      /*if (gradient == null)
      {
        gradient = AddKnownGradient(ActivityColor);
      }*/
    }

    /// <summary>Gets or sets the photo to display.</summary>
    public string BCacheImage
    {
      get { return GetValue(BCacheImageProperty) as string; }
      set { SetValue(BCacheImageProperty, value); }
    }

    /// <summary>
    /// Gets the actual image content to display.
    /// </summary>
    public ImageSource ImageSource
    {
      get { return (ImageSource)GetValue(ImageSourceProperty); }
      protected set { SetValue(_ImageSourcePropertyKey, value); }
    }

    public ImageType ImageType
    {
      get { return (ImageType)GetValue(ImageTypeProperty); }
      set { SetValue(ImageTypeProperty, value); }
    }
    public ImageKind ImageKind
    {
      get { return (ImageKind)GetValue(ImageKindProperty); }
      set { SetValue(ImageKindProperty, value); }
    }


    public SizeToContent SizeToContent
    {
      get { return (SizeToContent)GetValue(SizeToContentProperty); }
      set { SetValue(SizeToContentProperty, value); }
    }

    /// <summary>
    /// Corner radius to use for rounded rects.
    /// </summary>
    public double CornerRadius { get; set; }

    /// <summary>
    /// The amount of padding to use when framing the Image.
    /// </summary>
    public double ImagePadding
    {
      get { return (double)GetValue(ImagePaddingProperty); }
      set { SetValue(ImagePaddingProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ImagePadding.  This enables animation, styling, binding, etc...

    /// <summary>
    /// Color to use in the gradient behind the user's photo.
    /// </summary>
    public Color ActivityColor
    {
      get { return (Color)GetValue(ActivityColorProperty); }
      set { SetValue(ActivityColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ActivityColor.  This enables animation, styling, binding, etc...

    /// <summary>
    /// Updates the activity gradient behind the user photo.
    /// </summary>
    private static void OnActivityColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {/*
      var itc = sender as BImage;

      var newColor = (Color)e.NewValue;

      itc.gradient = null;
      gradientBrushes.TryGetValue(newColor, out itc.gradient);
      * */
    }

    /// <summary>
    /// Invoked to render the ImageThumbnailControl.
    /// </summary>
    protected override void OnRender(DrawingContext drawingContext)
    {
      //if (CornerRadius > 0)
      //{
      //  drawingContext.DrawRoundedRectangle(gradient, borderPen, new Rect(0, 0, ActualWidth, ActualHeight),
      //                                      CornerRadius + 2, CornerRadius + 2);
      //}
      //else
      //{
      //  drawingContext.DrawRectangle(gradient, borderPen, new Rect(0, 0, ActualWidth, ActualHeight));
      //}

      double pad = ImagePadding;
      double width = ActualWidth - 2.0 * pad > 0.0 ? ActualWidth - 2.0 * pad : 0.0;
      double height = ActualHeight - 2.0 * pad > 0.0 ? ActualHeight - 2.0 * pad : 0.0;

      if (brushRect.Width != width || brushRect.Height != height)
      {
        brushRect = new Rect(pad, pad, width, height);
      }

      if (ImageSource != null && userImage == null)
      {
        userImage = new ImageBrush(ImageSource);

        if (ImageSource.Height > ImageSource.Width)
        {
          userImage.Viewport = new Rect(0, 0, 1.0, ImageSource.Height / ImageSource.Width);
        }
        else if (ImageSource.Width > ImageSource.Height)
        {
          userImage.Viewport = new Rect(0, 0, ImageSource.Width / ImageSource.Height, 1.0);
        }
      }

      drawingContext.DrawRoundedRectangle(
        userImage ?? (ImageType == ImageType.Person ? _anonymousPersonBrush : _anonymousLandscapeBrush),
        null,
        brushRect,
        CornerRadius,
        CornerRadius);
    }

    /// <summary>
    /// Adds a new known gradient to the linear gradient cache.
    /// </summary>
    private static LinearGradientBrush AddKnownGradient(Color color)
    {
      LinearGradientBrush lgb = null;
      lgb = new LinearGradientBrush {StartPoint = new Point(0, 0), EndPoint = new Point(0, 1)};
      lgb.GradientStops.Add(new GradientStop(Colors.White, 0.0));
      lgb.GradientStops.Add(new GradientStop(color, 1.0));
      lgb.Freeze();

      gradientBrushes.Add(color, lgb);
      return lgb;
    }

    /// <summary>
    /// Updates the content of the control to contain the image at Photo.ThumbnailUri.
    /// </summary>
    private void _UpdateImage()
    {
      if (!string.IsNullOrEmpty(BCacheImage))
      {
        new BCacheImage(new Uri(BCacheImage), ImageKind).GetImageAsync(OnGetImageSourceCompleted);
      }
      else
      {
        ImageSource = null;
      }
    }

    /// <summary>
    /// Sets the ImageSource of the control as soon as the asynchronous get is completed.
    /// </summary>
    /// <param name="e">Arguments describing the event.</param>
    protected virtual void OnGetImageSourceCompleted(object sender, GetImageSourceCompletedEventArgs e)
    {
      try
      {
        if (e.Error == null && !e.Cancelled)
        {
          ImageSource = e.ImageSource;
          if (SizeToContent != SizeToContent.Manual)
          {
            // Not bothering to detect more granular values for SizeToContent.
            SetValue(WidthProperty, e.NaturalSize.Value.Width);
            SetValue(HeightProperty, e.NaturalSize.Value.Height);
          }
          InvalidateVisual();
        }
        else
        {
          ImageSource = null;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,ex);
      }
    }
  }
}

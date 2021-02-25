#region

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

#endregion

namespace Sobees.Library.BTransitionEffects
{
  /// <summary>
  ///   Cloudy transition effect.
  /// </summary>
  public abstract class CloudyTransitionEffect : RandomizedTransitionEffect
  {
    #region Fields

    /// <summary>
    ///   DependencyProperty for
    ///   <see cref="CloudImage" />
    ///   property.
    /// </summary>
    protected static readonly DependencyProperty CloudImageProperty =
      RegisterPixelShaderSamplerProperty("CloudImage", typeof (CloudyTransitionEffect), 2,
                                                      SamplingMode.Bilinear);

    #endregion

    #region Methods

    /// <summary>
    ///   Initializes a new instance of the CloudyTransitionEffect class.
    /// </summary>
    protected CloudyTransitionEffect()
    {
      CloudImage = new ImageBrush(new BitmapImage(TransitionUtilities.MakePackUri("Images/clouds.png")));
      UpdateShaderValue(CloudImageProperty);
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets the Brush to be used as cloud image for this effect.
    /// </summary>
    protected Brush CloudImage
    {
      get { return (Brush) GetValue(CloudImageProperty); }
      set { SetValue(CloudImageProperty, value); }
    }

    #endregion
  }
}
#region

using System.Windows.Media.Effects;

#endregion

namespace Sobees.Library.BTransitionEffects
{
  /// <summary>
  ///   Drop fade transition effect.
  /// </summary>
  public class DropFadeTransitionEffect : CloudyTransitionEffect
  {
    /// <summary>
    ///   Initializes a new instance of the DropFadeTransitionEffect class.
    /// </summary>
    public DropFadeTransitionEffect()
    {
      var shader = new PixelShader();
      shader.UriSource = TransitionUtilities.MakePackUri("Shaders/DropFade.fx.ps");
      PixelShader = shader;
    }
  }
}
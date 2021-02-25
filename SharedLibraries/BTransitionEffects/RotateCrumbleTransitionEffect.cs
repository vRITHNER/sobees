#region

using System.Windows.Media.Effects;

#endregion

namespace Sobees.Library.BTransitionEffects
{
  /// <summary>
  ///   Rotate crumble transition effect.
  /// </summary>
  public class RotateCrumbleTransitionEffect : CloudyTransitionEffect
  {
    /// <summary>
    ///   Initializes a new instance of the RotateCrumbleTransitionEffect class.
    /// </summary>
    public RotateCrumbleTransitionEffect()
    {
      var shader = new PixelShader();
      shader.UriSource = TransitionUtilities.MakePackUri("Shaders/RotateCrumble.fx.ps");
      PixelShader = shader;
    }
  }
}
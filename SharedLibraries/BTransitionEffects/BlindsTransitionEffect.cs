#region

using System.Windows.Media.Effects;

#endregion

namespace Sobees.Library.BTransitionEffects
{
  /// <summary>
  ///   Blinds transition effect
  /// </summary>
  public class BlindsTransitionEffect : TransitionEffect
  {
    /// <summary>
    ///   Initializes a new instance of the BlindsTransitionEffect class.
    /// </summary>
    public BlindsTransitionEffect()
    {
      var shader = new PixelShader();
      shader.UriSource = TransitionUtilities.MakePackUri("Shaders/Blinds.fx.ps");
      PixelShader = shader;
    }
  }
}
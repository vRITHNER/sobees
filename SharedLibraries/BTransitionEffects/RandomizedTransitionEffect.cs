﻿#region

using System.Windows;

#endregion

namespace Sobees.Library.BTransitionEffects
{
  /// <summary>
  ///   Randomized transition effect.
  /// </summary>
  public abstract class RandomizedTransitionEffect : TransitionEffect
  {
    #region Fields

    /// <summary>
    ///   DependencyProperty for
    ///   <see cref="RandomSeed" />
    ///   property
    /// </summary>
    public static readonly DependencyProperty RandomSeedProperty = DependencyProperty.Register("RandomSeed",
                                                                                               typeof (double),
                                                                                               typeof (
                                                                                                 RandomizedTransitionEffect
                                                                                                 ),
                                                                                               new UIPropertyMetadata(
                                                                                                 0.0,
                                                                                                 PixelShaderConstantCallback
                                                                                                   (1)));

    #endregion

    #region Methods

    /// <summary>
    ///   Initializes a new instance of the RandomizedTransitionEffect class.
    /// </summary>
    protected RandomizedTransitionEffect()
    {
      UpdateShaderValue(RandomSeedProperty);
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets the random seed value for this effect.
    /// </summary>
    public double RandomSeed
    {
      get { return (double) GetValue(RandomSeedProperty); }
      set { SetValue(RandomSeedProperty, value); }
    }

    #endregion
  }
}
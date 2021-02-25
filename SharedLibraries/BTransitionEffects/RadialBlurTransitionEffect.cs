//-----------------------------------------------------------------------
// <copyright file="RadialBlurTransitionEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Code for radial blur transition effect
// </summary>
//-----------------------------------------------------------------------

namespace Sobees.Library.BTransitionEffects
{
    using System.Windows.Media.Effects;

    /// <summary>
    /// Radial blur transition effect.
    /// </summary>
    public class RadialBlurTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Initializes a new instance of the RadialBlurTransitionEffect class.
        /// </summary>
        public RadialBlurTransitionEffect()
        {
            var shader = new PixelShader();
            shader.UriSource = TransitionUtilities.MakePackUri("Shaders/RadialBlur.fx.ps");
            PixelShader = shader;
        }
    }
}

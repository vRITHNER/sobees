//-----------------------------------------------------------------------
// <copyright file="PixelateOutTransitionEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Code for pixelate out transition effect
// </summary>
//-----------------------------------------------------------------------

namespace Sobees.Library.BTransitionEffects
{
    using System.Windows.Media.Effects;

    /// <summary>
    /// Pixelate out transition effect.
    /// </summary>
    public class PixelateOutTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Initializes a new instance of the PixelateOutTransitionEffect class.
        /// </summary>
        public PixelateOutTransitionEffect()
        {
            var shader = new PixelShader();
            shader.UriSource = TransitionUtilities.MakePackUri("Shaders/PixelateOut.fx.ps");
            PixelShader = shader;
        }
    }
}

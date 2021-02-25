// <copyright file="SaturateTransitionEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Code for saturate transition effect
// </summary>
//-----------------------------------------------------------------------

namespace Sobees.Library.BTransitionEffects
{
    using System.Windows.Media.Effects;

    /// <summary>
    /// Saturate transition effect.
    /// </summary>
    public class SaturateTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Initializes a new instance of the SaturateTransitionEffect class.
        /// </summary>
        public SaturateTransitionEffect()
        {
            var shader = new PixelShader();
            shader.UriSource = TransitionUtilities.MakePackUri("Shaders/Saturate.fx.ps");
            PixelShader = shader;
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="FadeTransitionEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Code for fade transition effect
// </summary>
//-----------------------------------------------------------------------

namespace Sobees.Library.BTransitionEffects
{
    using System.Windows.Media.Effects;

    /// <summary>
    /// Fade transition effect.
    /// </summary>
    public class FadeTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Initializes a new instance of the FadeTransitionEffect class.
        /// </summary>
        public FadeTransitionEffect()
        {
            var shader = new PixelShader();
            shader.UriSource = TransitionUtilities.MakePackUri("Shaders/Fade.fx.ps");
            PixelShader = shader;
        }
    }
}

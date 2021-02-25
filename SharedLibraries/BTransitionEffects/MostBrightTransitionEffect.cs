//-----------------------------------------------------------------------
// <copyright file="MostBrightTransitionEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Code for most bright transition effect
// </summary>
//-----------------------------------------------------------------------

namespace Sobees.Library.BTransitionEffects
{
    using System.Windows.Media.Effects;

    /// <summary>
    /// Most bright transition effect.
    /// </summary>
    public class MostBrightTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Initializes a new instance of the MostBrightTransitionEffect class.
        /// </summary>
        public MostBrightTransitionEffect()
        {
            var shader = new PixelShader();
            shader.UriSource = TransitionUtilities.MakePackUri("Shaders/MostBright.fx.ps");
            PixelShader = shader;
        }
    }
}

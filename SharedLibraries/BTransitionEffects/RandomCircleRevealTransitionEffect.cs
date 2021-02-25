//-----------------------------------------------------------------------
// <copyright file="RandomCircleRevealTransitionEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Code for random circle reveal transition effect
// </summary>
//-----------------------------------------------------------------------

namespace Sobees.Library.BTransitionEffects
{
    using System.Windows.Media.Effects;

    /// <summary>
    /// Random circle reveal transition effect.
    /// </summary>
    public class RandomCircleRevealTransitionEffect : CloudyTransitionEffect
    {
        /// <summary>
        /// Initializes a new instance of the RandomCircleRevealTransitionEffect class.
        /// </summary>
        public RandomCircleRevealTransitionEffect()
        {
            var shader = new PixelShader();
            shader.UriSource = TransitionUtilities.MakePackUri("Shaders/RandomCircleReveal.fx.ps");
            PixelShader = shader;
        }
    }
}

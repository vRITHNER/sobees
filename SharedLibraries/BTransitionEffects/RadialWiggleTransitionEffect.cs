//-----------------------------------------------------------------------
// <copyright file="RadialWiggleTransitionEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Code for radial wiggle transition effect
// </summary>
//-----------------------------------------------------------------------

namespace Sobees.Library.BTransitionEffects
{
    using System.Windows.Media.Effects;

    /// <summary>
    /// Radial wiggle transition effect.
    /// </summary>
    public class RadialWiggleTransitionEffect : CloudyTransitionEffect
    {
        /// <summary>
        /// Initializes a new instance of the RadialWiggleTransitionEffect class.
        /// </summary>
        public RadialWiggleTransitionEffect()
        {
            var shader = new PixelShader();
            shader.UriSource = TransitionUtilities.MakePackUri("Shaders/RadialWiggle.fx.ps");
            PixelShader = shader; 
        }
    }
}

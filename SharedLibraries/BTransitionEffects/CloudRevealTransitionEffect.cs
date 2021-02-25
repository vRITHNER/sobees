//-----------------------------------------------------------------------
// <copyright file="CloudRevealTransitionEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Code for cloud reveal effect
// </summary>
//-----------------------------------------------------------------------

namespace Sobees.Library.BTransitionEffects
{
    using System.Windows.Media.Effects;

    /// <summary>
    /// Cloud reveal transition effect.
    /// </summary>
    public class CloudRevealTransitionEffect : CloudyTransitionEffect
    {
        /// <summary>
        /// Initializes a new instance of the CloudRevealTransitionEffect class.
        /// </summary>
        public CloudRevealTransitionEffect()
        {
            var shader = new PixelShader();
            shader.UriSource = TransitionUtilities.MakePackUri("Shaders/CloudReveal.fx.ps");
            PixelShader = shader;
        }
    }
}

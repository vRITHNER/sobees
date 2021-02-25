//-----------------------------------------------------------------------
// <copyright file="CircleStretchTransitionEffect.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Code for circle stretch transition effect
// </summary>
//-----------------------------------------------------------------------

namespace Sobees.Library.BTransitionEffects
{
    using System.Windows.Media.Effects;

    /// <summary>
    /// Circle Stretch transition effect.
    /// </summary>
    public class CircleStretchTransitionEffect : TransitionEffect
    {
        /// <summary>
        /// Initializes a new instance of the CircleStretchTransitionEffect class.
        /// </summary>
        public CircleStretchTransitionEffect()
        {
            var shader = new PixelShader();
            shader.UriSource = TransitionUtilities.MakePackUri("Shaders/CircleStretch.fx.ps");
            PixelShader = shader;
        }
    }
}

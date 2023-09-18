// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;

namespace Microsoft.Test.Graphics
{

// Build break fix. This really should build into the Part1 DLL, but for now 
// since the project includes *.cs, just don't compile this class if building for a 2.0-CLR

#if !TESTBUILD_CLR20
    /// <summary>
    /// Test ContainerVisual property getters and setters
    /// </summary>`
    [Test(1, @"Arrowhead\ApiTests", "ContainerVisualApiTest",Description="Testing property getter and setter on ContainerVisual.")]
    public class ContainerVisualApiTest : StepsTest
    {

        #region Methods

        [Variation()]
        public ContainerVisualApiTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        /// <summary>
        /// Start testing. 
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            ContainerVisual visual = new ContainerVisual();
            if (!TestOpacityProperty(visual)
                || !TestEffectProperty(visual)
            // BItmapEffect is obsolete, so we need disable warning 0618. 
#pragma warning disable 0618
                || !TestBitmapEffectProperty(visual)
                || !TestBitmapEffectInputProperty(visual)
#pragma warning restore 0618
                || !TestOpacityMaskProperty(visual)
                || !TestTransformProperty(visual)
               )
            {
                return TestResult.Fail;
            }
                
            return TestResult.Pass;
        }

        private bool TestOpacityProperty(ContainerVisual visual)
        {
            visual.Opacity = 0.3;
            double valueGot = visual.Opacity;
            return valueGot == 0.3;
        }

        private bool TestEffectProperty(ContainerVisual visual)
        {
            Effect effect = new BlurEffect();
            visual.Effect = effect;
            Effect valueGot = visual.Effect;
            visual.Effect = null;
            return valueGot == effect;
        }

        private bool TestBitmapEffectProperty(ContainerVisual visual)
        {
            BitmapEffect effect = new BlurBitmapEffect();
            visual.BitmapEffect = effect;
            BitmapEffect valueGot = visual.BitmapEffect;
            return valueGot == effect;
        }

        private bool TestBitmapEffectInputProperty(ContainerVisual visual)
        {
            BitmapEffectInput effectInput = new BitmapEffectInput();
            visual.BitmapEffectInput = effectInput;
            BitmapEffectInput valueGot = visual.BitmapEffectInput;
            return valueGot == effectInput;
        }

        private bool TestOpacityMaskProperty(ContainerVisual visual)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            visual.OpacityMask = brush;
            LinearGradientBrush valueGot = visual.OpacityMask as LinearGradientBrush;
            return valueGot == brush;
        }
        
        private bool TestTransformProperty(ContainerVisual visual)
        {
            ScaleTransform transform = new ScaleTransform();
            visual.Transform = transform;
            ScaleTransform valueGot = visual.Transform as ScaleTransform;
            return valueGot == transform;
        }

        
        #endregion
    }

#endif
}
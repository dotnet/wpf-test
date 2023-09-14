// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test animation on ShaderEffect constants. 
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.IO;
using System.Windows;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Test animate DP corresponding to shader constants. 
    /// Requirment for the xaml: Has a Storyboard with name "storyboard" which should 
    /// be finished in 5 seconds. 
    /// </summary>`
   //commenting ignored cases
    // [Test(2, "Arrowhead\\ParameterAnimation", "ShaderConstantAnimation",
    //     SupportFiles = @"FeatureTests\Effects\Xamls\ShaderConstantAnimationTest.xaml,
    //     FeatureTests\Effects\Xamls\AnimateBlurEffectProperties.xaml,
    //     FeatureTests\Effects\Xamls\AnimateDropShadowEffectProperties.xaml,
    //     FeatureTests\Effects\Masters\StartShaderConstantAnimationTest.xaml.png,
    //     FeatureTests\Effects\Masters\EndShaderConstantAnimationTest.xaml.png,
    //     FeatureTests\Effects\Masters\StartAnimateBlurEffectProperties.xaml.png,
    //     FeatureTests\Effects\Masters\EndAnimateBlurEffectProperties.xaml.png,
    //     FeatureTests\Effects\Masters\StartAnimateDropShadowEffectProperties.xaml.png,
    //     FeatureTests\Effects\Masters\EndAnimateDropShadowEffectProperties.xaml.png,
    //     common\InvariantTheme.xaml,
    //     FeatureTests\Effects\Model\testprofilenovisualvalidation.xml")]
    public class EffectAnimationTest : XamlBasedChangingTest 
    {
        #region Methods

        /// <summary>
        /// Constructor, pass in xaml file name, master file name, and shader file name. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        [Variation("ShaderConstantAnimationTest.xaml", "StartShaderConstantAnimationTest.xaml.png", "EndShaderConstantAnimationTest.xaml.png")]
        [Variation("AnimateBlurEffectProperties.xaml", "StartAnimateBlurEffectProperties.xaml.png", "EndAnimateBlurEffectProperties.xaml.png")]
        [Variation("AnimateDropShadowEffectProperties.xaml", "StartAnimateDropShadowEffectProperties.xaml.png", "EndAnimateDropShadowEffectProperties.xaml.png")]
        public EffectAnimationTest(string xamlFileName, string startMasterImageName, string endMasterImageName)
            : base(xamlFileName, startMasterImageName, endMasterImageName)
        {
            ToleranceFilePath = "testprofilenovisualvalidation.xml";
        }

    
        //active animation
        protected override void Change(FrameworkElement content)
        {
            try
            {
                Storyboard storyboard = content.FindResource("storyboard") as Storyboard;
                storyboard.Begin(content);
            }
            catch (ResourceReferenceKeyNotFoundException e)
            {
                Log.LogEvidence(e.Message);
            }
        }

        #endregion

    }
}
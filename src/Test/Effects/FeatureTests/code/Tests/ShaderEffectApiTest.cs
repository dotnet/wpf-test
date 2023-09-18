// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: API test for ShaderEffect class
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Xml;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;
using Microsoft.Test.Globalization;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Api Test for class ShaderEffect
    /// </summary>`
   //commenting ignored cases
    // [Test(1, "Arrowhead\\ApiTests", "ShaderEffectApiTest", SupportFiles = @"Common\Effects\Shaders\reddish.ps")]
    public class ShaderEffectApiTest : WindowTest
    {
        #region Private Data

        private Button _button;

        #endregion 

        #region Methods

        /// <summary>
        /// Constructor, set up test step. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        [Variation()]
        public ShaderEffectApiTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        /// <summary>
        /// Start testing. 
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            //Create a button with ShaderEffect and add it as content of current window.
            _button = new Button();
            PixelShader shader = new PixelShader();
            shader.UriSource = new Uri("pack://siteoforigin:,,,/reddish.ps");
            BasicShaderEffect effect = new BasicShaderEffect();
            effect.PixelShader = shader;

            _button.Effect = effect;
            Window.Content = _button;

            //Test case fails if any test failed. 
            if (!EffectsTestHelper.TestClone<BasicShaderEffect>(_button.Effect)
                || !EffectsTestHelper.TestCloneCurrentValue<ShaderEffect>(_button.Effect))
            {
                return TestResult.Fail;
            }
            
            //No failure, test case passes. 
            return TestResult.Pass;
        }
        #endregion
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: API test for DropShadowEffect class
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Xml;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Loaders;
using Microsoft.Test.Globalization;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Api Test for class DropShadowEffect
    /// </summary>`
    [Test(1, "Arrowhead\\ApiTests", "DropShadowEffectApiTest")]
    public class DropShadowEffectApiTest : WindowTest
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
        public DropShadowEffectApiTest()
        {
            RunSteps += new TestStep(RunTest);
        }


        /// <summary>
        /// Entry point.
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            //Create a button with DropShadowEffect and add it as content of current window.
            _button = new Button();
            _button.Effect = new DropShadowEffect();
            Window.Content = _button;

            //Test case fails if any test failed. 
            if (!TestDirectionProperty()
                || !TestOpacityProperty()
                || !TestBlurRadiusProperty()
                || !TestShadowDepthProperty()
                || !TestColorProperty()
                || !EffectsTestHelper.TestClone<DropShadowEffect>(_button.Effect)
                || !EffectsTestHelper.TestCloneCurrentValue<DropShadowEffect>(_button.Effect)
            )
            {
                return TestResult.Fail;
            }

            //No failure, test case passes. 
            return TestResult.Pass;
        }

        /// <summary>
        /// Test getter and setter of Directory property. 
        /// </summary>
        /// <returns></returns>
        private bool TestDirectionProperty()
        {
            ((DropShadowEffect)_button.Effect).Direction = 20.0;
            double direction = ((DropShadowEffect)_button.Effect).Direction;
            if (direction != 20.0)
            {
                Log.LogEvidence("Direction got is not the value set.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Test getter and setter of Opacity property.
        /// </summary>
        /// <returns></returns>
        private bool TestOpacityProperty()
        {
            ((DropShadowEffect)_button.Effect).Opacity = 0.6;
            double opacity = ((DropShadowEffect)_button.Effect).Opacity;
            if (opacity != 0.6)
            {
                Log.LogEvidence("Opacity got is not the value set.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Test getter and setter of BlurRadius property.
        /// </summary>
        /// <returns></returns>
        private bool TestBlurRadiusProperty()
        {
            ((DropShadowEffect)_button.Effect).BlurRadius = 0.6;
            double blurRadius = ((DropShadowEffect)_button.Effect).BlurRadius;
            if (blurRadius != 0.6)
            {
                Log.LogEvidence("BlurRadius got is not the value set.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Test getter and setter of Depth property.
        /// </summary>
        /// <returns></returns>
        private bool TestShadowDepthProperty()
        {

            ((DropShadowEffect)_button.Effect).ShadowDepth = 20.0;
            double shadowDepth = ((DropShadowEffect)_button.Effect).ShadowDepth;
            if (shadowDepth != 20.0)
            {
                Log.LogEvidence("ShadowDepth got is not the value set.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Test getter and setter of Color property.
        /// </summary>
        /// <returns></returns>
        private bool TestColorProperty()
        {
            ((DropShadowEffect)_button.Effect).Color = Colors.Blue;
            Color color = ((DropShadowEffect)_button.Effect).Color;
            if (color != Colors.Blue)
            {
                Log.LogEvidence("Color got is not the value set.");
                return false;
            }
            return true;
        }
        
        #endregion
    }
}
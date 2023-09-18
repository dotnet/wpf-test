// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Invalid shader tests for Effect 
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;
using Microsoft.Test.Globalization;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Test public property ImplicitImput. 
    /// </summary>`
   //commenting ignored cases
    // [Test(1, "Arrowhead\\NegativeTest", "InvalidShader", 
    //   SupportFiles = @"Common\InvariantTheme.xaml,
    //                    Common\Effects\Shaders\*.ps,
    //                    FeatureTests\Effects\Masters\MasterImageForInvalidShaderTest.png,
    //                    FeatureTests\Effects\Model\testprofilebad.xml")]
    public class InvalidShaderTest : EffectsWindowTest
    {
        #region Private Data

        private string _shaderFileName;
        private bool _invalidPixelShaderHandled = false;
        private bool _expectedNotified = false;
        private ShaderRenderMode _shaderRenderMode = ShaderRenderMode.Auto;
        private PixelShader _pixelShader;
        private const string goodShader = "reddish.ps";
        private const string masterImage = "MasterImageForInvalidShaderTest.png";
        private EventHandler _invalidPixelShaderEncounteredHandler;


        #endregion 

        #region Methods

        /// <summary>
        /// Conatrutor
        /// </summary>
        /// <param name="shaderFile">Compiled shader</param>
        /// <param name="renderMode">ShaderRenderMode</param>
        /// <param name="expectToBeNotified">Expecting notification for bad shader</param>
        [Variation("Reddish.1_1.ps", ShaderRenderMode.Auto, true)]
        [Variation("Reddish.1_2.ps", ShaderRenderMode.Auto, true)]
        [Variation("Reddish.1_3.ps", ShaderRenderMode.Auto, true)]
        [Variation("Reddish.1_4.ps", ShaderRenderMode.Auto, true)]
        [Variation("Reddish.2_a.ps", ShaderRenderMode.Auto, true)]
        [Variation("Reddish.2_b.ps", ShaderRenderMode.Auto, true)]
        [Variation("Reddish.3_0.ps", ShaderRenderMode.Auto, false)]
        // Part1 Regression_Bug3
        [Variation("Reddish.3_sw.ps", ShaderRenderMode.Auto, false)]
        [Variation("DividedBy0.ps", ShaderRenderMode.Auto, false)]
        [Variation("NotCompiledShader.ps", ShaderRenderMode.Auto, true)]
        [Variation("Reddish.3_0.ps", ShaderRenderMode.SoftwareOnly, false)]
        [Variation("Reddish.1_1.ps", ShaderRenderMode.SoftwareOnly, true)]
        public InvalidShaderTest(string shaderFile, ShaderRenderMode renderMode, bool expectToBeNotified)
        {
            InitializeSteps += new TestStep(AddInvalidPixelShaderEncounteredHander);
            RunSteps += new TestStep(RunTest);
            CleanUpSteps += new TestStep(RemoveInvalidPixelShaderEncounteredHander);
            _shaderFileName = shaderFile;
            _expectedNotified = expectToBeNotified;
            _shaderRenderMode = renderMode;
        }

        /// <summary>
        /// Add a handler on PixelShader.InvalidPixelShaderEncountered.
        /// </summary>
        /// <returns></returns>
        TestResult AddInvalidPixelShaderEncounteredHander()
        {
            _invalidPixelShaderEncounteredHandler = new EventHandler(PixelShaderEncountered);
            PixelShader.InvalidPixelShaderEncountered += _invalidPixelShaderEncounteredHandler;
            return TestResult.Pass;
        }

        /// <summary>
        /// Remove the handler on PixelShader.InvalidPixelShaderEncountered.
        /// </summary>
        /// <returns></returns>
        TestResult RemoveInvalidPixelShaderEncounteredHander()
        {
            PixelShader.InvalidPixelShaderEncountered -= _invalidPixelShaderEncounteredHandler;
            return TestResult.Pass;
        }
        /// <summary>
        /// Event handler on PixelShader.InvalidPixelShaderEncountered. Verify that
        /// After refresh the shader with a good one, we got correct rendering. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void PixelShaderEncountered(object sender, EventArgs args)
        {
            Status("Handler on PixelShader.InvalidPixelShaderEncountered invoked.");
            _pixelShader.UriSource = new Uri("pack://application:,,,/ShaderEffects;component/" + goodShader);
            WaitFor(200);
            if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, masterImage, "TestprofileBad.xml"))
            {
                Log.LogEvidence("Rendereing after restoring is different with expected.");
            }
            else
            {
                _invalidPixelShaderHandled = true;
            }
        }
            
        /// <summary>
        /// RunTest apply effect with negative shader on a button, verify that . 
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            TestResult result = TestResult.Fail;
            try
            {
                Rectangle rect = new Rectangle();
                rect.Width = 150;
                rect.Height = 150;
                rect.Fill = Brushes.Gray;
                ContentRoot.Content = rect;

                Log.LogStatus("Created an effect.");
                BasicShaderEffect effect = new BasicShaderEffect();
                _pixelShader = new PixelShader();
                _pixelShader.UriSource = new Uri("pack://application:,,,/ShaderEffects;component/" + _shaderFileName);

                _pixelShader.ShaderRenderMode = _shaderRenderMode;

                effect.PixelShader = _pixelShader;

                Log.LogStatus("Created a Rectangle.");

                rect.Effect = effect;

                //wait for rendering. 
                Log.LogStatus("Rendering.");
                WaitFor(200);

                //handler on PixelShader.InvalidPixelShaderEncountered get executed as expected. 
                if (_invalidPixelShaderHandled)
                {
                    result = TestResult.Pass;
                }

                //not expecting handler to get executed, and didn't get it. 
                if (!_expectedNotified)
                {
                    Log.LogEvidence("Test case passed.");
                    result = TestResult.Pass;
                }
            }
            catch (Exception e)
            {
                Log.LogEvidence("Got un expected exception: Message : \n{0}, \nStackTrace:\n{1}", e.Message, e.StackTrace);
                result = TestResult.Fail;
            }
            Window.Close();
            return result;
        }

        #endregion
    }
}
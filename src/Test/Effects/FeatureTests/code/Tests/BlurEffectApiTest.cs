// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: API test for BlurEffect class
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
    /// Api Test for class BlurEffect
    /// </summary>`
    [Test(1, "Arrowhead\\ApiTests", "BlurEffectApiTest")]
    public class BlurEffectApiTest : WindowTest
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
        public BlurEffectApiTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        /// <summary>
        /// Start testing. 
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            //Create a button with BlurEffect and add it as content of current window.
            _button = new Button();
            _button.Effect = new BlurEffect();
            Window.Content = _button;

            //Test case fails if any test failed. 
            if (!TestRadiusProperty()
                || !TestKernelTypeProperty()
                || !EffectsTestHelper.TestClone<BlurEffect>(_button.Effect)
                || !EffectsTestHelper.TestCloneCurrentValue<BlurEffect>(_button.Effect)
                || !TestNegativePropertValue()
            )
            {
                return TestResult.Fail;
            }
            
            //No failure, test case passes. 
            return TestResult.Pass;
        }

        /// <summary>
        /// Test setter and getter of the Radius property. 
        /// </summary>
        /// <returns></returns>
        private bool TestRadiusProperty()
        {
            //Verify Radius getter and setter. 
            ((BlurEffect)_button.Effect).Radius = 20.0;
            double radius = ((BlurEffect)_button.Effect).Radius;
            if (radius != 20.0)
            {
                Log.LogStatus("Radius got is not the value set.");
                return false;
            }
            //Render the window and verify no exception. 
            WaitFor(0);

            return true;
        }

        /// <summary>
        /// Test getter and setter of KernelType property. 
        /// </summary>
        /// <returns></returns>
        private bool TestKernelTypeProperty()
        {
            //Verify KernelType getter and setter. 
            ((BlurEffect)_button.Effect).KernelType = KernelType.Box;
            KernelType kernelType = ((BlurEffect)_button.Effect).KernelType;
            if (kernelType != KernelType.Box)
            {
                Log.LogStatus("KernelType got is not the value set.");
                return false;
            }
            //Render the window and verify no exception. 
            WaitFor(0);

            return true;
        }

        /// <summary>
        /// Test setting KernelType to an invalid value caused an Augument Exception. 
        /// </summary>
        /// <returns></returns>
        private bool TestNegativePropertValue()
        {
            try
            {
                ((BlurEffect)_button.Effect).KernelType = (KernelType)10;
            }
            catch (ArgumentException exception)
            {
                //Verify Expeced a ArgumentException

                if (exception.Message.Contains("KernelType"))
                {
                    return true;
                }
                else
                {
                    Log.LogStatus("Wrong Exception Message: {0}.", exception.Message); 
                    return false;
                }
            }

            //fail if no exception. 
            Log.LogStatus("No exception caught.");
            return false;
        }

        #endregion
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test GetEffect method in VisualTreeHelper. 
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Xml;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;


namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Test public property ImplicitImput. 
    /// </summary>`
    [Test(1, "Arrowhead\\ApiTests", "VisualTreeHelperApiTest")]
    public class VisualTreeHelperApiTest : AvalonTest
    {
        #region Methods

        /// <summary/>
        [Variation()]
        public VisualTreeHelperApiTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        
        /// <summary>
        /// Runtest Verify VisualTreeHelper.GetEffect
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            TestResult result = TestResult.Pass;

            Button button = new Button();

            //No effect set
            Effect effect = VisualTreeHelper.GetEffect(button);
            if(effect != null)
            {
                Log.LogEvidence("Effect should be null for an visual without effect set.");
                result = TestResult.Fail;
            }

            //BlurEffection
            BlurEffect blurEffect = new BlurEffect();
            button.Effect = blurEffect;
            effect = VisualTreeHelper.GetEffect(button);
            if(effect != blurEffect)
            {
                Log.LogEvidence("Effect should be blurEffect.");
                result = TestResult.Fail;
            }

            //null effect. 
            button.Effect = null;
            effect = VisualTreeHelper.GetEffect(button);
            if (effect != null)
            {
                Log.LogEvidence("Effect should be null.");
                result = TestResult.Fail;
            }

            return result;
        }

        #endregion
    }
}
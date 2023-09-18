// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: API test for Effect class
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Xml;
using System.Windows.Media;
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
    [Test(1, "Arrowhead\\ApiTests", "EffectApiTest")]
    public class EffectApiTest : AvalonTest
    {
        #region Methods

        /// <summary>
        /// Constructor, set up test step. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        [Variation()]
        public EffectApiTest()
        {
            RunSteps += new TestStep(RunTest);
        }

        
        /// <summary>
        /// Runtest Verify ImplicitInpup is not SolidColorBrush any more (Regression_Bug45)
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            //Verify ImplicitImput is SolidColorBrush.
            SolidColorBrush implicitInput = Effect.ImplicitInput as SolidColorBrush;
            if (implicitInput != null)
            {             
                Log.LogStatus("Effect.ImplicitInput is still SolidColorBrush.");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        #endregion
    }
}
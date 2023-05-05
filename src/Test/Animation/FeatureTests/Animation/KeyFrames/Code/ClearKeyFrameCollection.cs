// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Regressions</area>


    [Test(2, "Animation.KeyFrames.Regressions", "ClearKeyFrameCollectionTest")]
    public class ClearKeyFrameCollectionTest : WindowTest
    {
        #region Constructor
        
        /******************************************************************************
        * Function:          ClearKeyFrameCollectionTest Constructor
        ******************************************************************************/
        public ClearKeyFrameCollectionTest()
        {
            InitializeSteps += new TestStep(StartTest);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Starts the test case. 
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult StartTest()
        {
            ColorKeyFrameCollection kfc1 = new ColorKeyFrameCollection();
            kfc1.Clear();

            GlobalLog.LogEvidence("-----RESULTS-----");
            GlobalLog.LogEvidence("-----Expected Count: 0");
            GlobalLog.LogEvidence("-----Actual Count:  " + kfc1.Count);

            if (kfc1.Count == 0)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion
    }
}

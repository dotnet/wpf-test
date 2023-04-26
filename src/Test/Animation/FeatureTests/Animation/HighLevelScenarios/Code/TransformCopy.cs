// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Documents;
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
    /// <priority>0</priority>

    [Test(0, "Animation.HighLevelScenarios.Regressions", "TransformCopyTest")]
    public class TransformCopyTest : WindowTest
    {
        #region Test case members
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          TransformCopyTest Constructor
        ******************************************************************************/
        public TransformCopyTest()
        {
            RunSteps += new TestStep(CopyTransform);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CopyTransform
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        TestResult CopyTransform()
        {
            TransformCollection tc = new TransformCollection();
            Transform t = new RotateTransform();
            tc.Add(t);

            TransformCollection tc2 = tc.Clone();

            bool b1 = ReferenceEquals(tc[0], tc2[0]);

            Brush brush1 = new SolidColorBrush();
            brush1.Transform = t;

            Brush brush2 = brush1.Clone();

            bool b2 = ReferenceEquals(brush1.Transform, brush2.Transform);
            
            
            GlobalLog.LogEvidence("-----CopyTransforming the Animation-----");
            GlobalLog.LogEvidence("ReferenceEquals PostCopy tc[0]");
            GlobalLog.LogEvidence("   Actual:   " + b1);
            GlobalLog.LogEvidence("   Expected: False");
            GlobalLog.LogEvidence("ReferenceEquals PostCopy b.Transform");
            GlobalLog.LogEvidence("   Actual:   " + b2);
            GlobalLog.LogEvidence("   Expected: False");
            
            if (!b1 && !b2)
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

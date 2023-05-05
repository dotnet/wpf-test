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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.Regressions</area>
    /// <priority>2</priority>
    /// <description>
    /// Regression Test: "MIL:Changeables: Copy method of PathAnimation class throws a NullReferenceException"
    /// </description>
    /// </summary>
    [Test(2, "Animation.PropertyMethodEvent.Regressions", "AnimationUsingPathTest")]
    public class AnimationUsingPathTest : AvalonTest
    {

        #region Constructor

        /******************************************************************************
        * Function:          AnimationUsingPathTest Constructor
        ******************************************************************************/
        public AnimationUsingPathTest()
        {
            RunSteps += new TestStep(StartTest);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        TestResult StartTest()
        {
            DoubleAnimationUsingPath animDouble = new DoubleAnimationUsingPath();
            animDouble.Clone();
            
            MatrixAnimationUsingPath animMatrix = new MatrixAnimationUsingPath();
            animMatrix.Clone();

            DoubleAnimationUsingPath copy1 = animDouble.Clone();
            MatrixAnimationUsingPath copy2 = animMatrix.Clone();
            
            Type expType1 = typeof(DoubleAnimationUsingPath);
            Type expType2 = typeof(MatrixAnimationUsingPath);

            bool b1 = ( copy1.GetType() == expType1 );
            bool b2 = ( copy2.GetType() == expType2 );

            GlobalLog.LogEvidence( "Result 1: " + b1);
            GlobalLog.LogEvidence( "Result 2: " + b2);

            if (b1 && b2)
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

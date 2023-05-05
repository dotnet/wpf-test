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
using System.Windows.Shapes;
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
    /// <area>Animation.KeyFrames.Regressions</area>


    [Test(2, "Animation.KeyFrames.Regressions", "KeyValueNullTest")]
    public class KeyValueNullTest : WindowTest
    {
        #region Constructor
        
        /******************************************************************************
        * Function:          KeyValueNullTest Constructor
        ******************************************************************************/
        public KeyValueNullTest()
        {
            InitializeSteps += new TestStep(StartTest);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Carries out the test case.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult StartTest()
        {
            KeyTime keyTime1 = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1));
            KeyTime keyTime2 = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2));
            KeyTime keyTime3 = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(3));

            LinearInt32KeyFrame keyFrame1 = new LinearInt32KeyFrame(0,  keyTime1);
            LinearInt32KeyFrame keyFrame2 = null;

            Int32AnimationUsingKeyFrames animKeyFrame = new Int32AnimationUsingKeyFrames();
            Int32KeyFrameCollection Int32KeyFrame = new Int32KeyFrameCollection();
            Int32KeyFrame.Add(keyFrame1);

            SetExpectedErrorTypeInStep(typeof(ArgumentNullException), "Outer");                
            Int32KeyFrame.Add(keyFrame2);

            return TestResult.Pass;
        }
        #endregion
    }
}

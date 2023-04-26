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
    /// <area>Animation.KeyFrames.Regressions</area>


    [Test(2, "Animation.KeyFrames.Regressions", "NullKeySplineConstructorTest")]
    public class NullKeySplineConstructorTest : WindowTest
    {

        #region Test case members
        
        private SplineColorKeyFrame _skf;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          NullKeySplineConstructorTest Constructor
        ******************************************************************************/
        public NullKeySplineConstructorTest()
        {
            InitializeSteps += new TestStep(CreateKeyFrame);
            RunSteps += new TestStep(SetKeySpline);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          CreateKeyFrame
        ******************************************************************************/
        /// <summary>
        /// Creates a SplineColorKeyFrame.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult CreateKeyFrame()
        {
            _skf = new SplineColorKeyFrame();
            _skf.Value       = Colors.Orange;
            _skf.KeyTime     = KeyTime.FromPercent(.5);
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          SetKeySpline
        ******************************************************************************/
        /// <summary>
        /// Sets KeySpline to null, which should throw a NullReferenceException.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult SetKeySpline()
        {
            SetExpectedErrorTypeInStep(typeof(System.ArgumentNullException), "Outer");                
            _skf.KeySpline   = null;
            
            return TestResult.Pass;
        }

        #endregion
    }
}

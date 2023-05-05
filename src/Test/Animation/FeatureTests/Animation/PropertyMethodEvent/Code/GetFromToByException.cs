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
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>
    /// <priority>2</priority>

    [Test(2, "Storyboards.LowLevelScenarios.Regressions", "GetFromToByExceptionTest")]
    public class GetFromToByExceptionTest : WindowTest
    {
        #region Test case members

        private string                      _inputString     = "";

        #endregion


        #region Constructor
        
        [Variation("From")]
        [Variation("To")]
        [Variation("By")]

        /******************************************************************************
        * Function:          GetFromToByExceptionTest Constructor
        ******************************************************************************/
        public GetFromToByExceptionTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(StartTest);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Creates the page content, an Animation, and a corresponding Clock.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult StartTest()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.BeginTime        = TimeSpan.FromMilliseconds(0);
            animation.Duration         = new Duration(TimeSpan.FromMilliseconds(50));

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException), "Outer");

            switch (_inputString)
            {
                case "From":
                    double result1 = (double)animation.From;
                    break;
                case "To":
                    double result2 = (double)animation.To;
                    break;
                case "By":
                    double result3 = (double)animation.By;
                    break;
            }
                       
            return TestResult.Pass;
        }

        #endregion
    }
}

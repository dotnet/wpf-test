// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//****************************************************************** 
//* Purpose: Test TS Scroll Acceleration feature
//******************************************************************
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.TS
{
    /// <summary>
    /// This class implements the simple xaml-based testing of Scroll Acceleration feature.
    /// </summary>
    public class InteractionTest : ScrollAccelerationTestBase
    {
        #region Constructor & Destructor

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="log">The test log to be used for any logging.</param>
        /// <param name="parameters">The test parameters.</param>
        public InteractionTest(TestLog log, PropertyBag parameters)
            : base(log, parameters)
        {
            _dispatcherSignalHelper = new DispatcherSignalHelper();
        }

        ~InteractionTest()
        {
            _dispatcherSignalHelper = null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Takes the snapshot of a given test window and returns it as the test result.
        /// </summary>
        /// <param name="window">A test window.</param>
        /// <returns>The snapshot of the window.</returns>
        public override object GetTestResult(Window window)
        {
            bool result = PerformClickTesting(window);
            log.LogStatus("Click-Testing performed on the window.");
            return result;
        }

        /// <summary>
        /// Compares the results of two tests and throws a TestValidationException if the results are not the same.
        /// For a InteractionTest test, this will be done by boolean comparison.
        /// </summary>
        /// <param name="referenceTestResult">The captured test result of the reference window.</param>
        /// <param name="renderedTestResult">The captured test result of the test window.</param>
        public override void CompareTestResults(object referenceTestResult, object renderedTestResult)
        {
            if ((bool)referenceTestResult == (bool)renderedTestResult)
            {
                log.LogStatus("Click-Testing Passed.");
            }
            else
            {
                throw new TestValidationException(string.Format("Click-Testing Failed: Expected={0} , Actual={1}", (bool)referenceTestResult, (bool)renderedTestResult));
            }
        }

        /// <summary>
        /// Perform click-testing on a button object in a test window.
        /// </summary>
        /// <param name="window">The test window</param>
        /// <returns>TRUE if the button was click-tested (clicked), FALSE otherwise.</returns>
        private bool PerformClickTesting(Window window)
        {
            Button btn = (Button)GetElementByName(window, btnName);
            if (btn != null)
            {
                btn.Click += new RoutedEventHandler(ClickTestingButton_MouseClick);
            }

            UserInput.MouseLeftDown(btn, 5, 5);
            UserInput.MouseLeftUp(btn, 5, 5);
            DispatcherHelper.DoEvents(intervalForRender);

            return (_dispatcherSignalHelper.WaitForSignal("MouseClick") == TestResult.Pass);
        }

        private void ClickTestingButton_MouseClick(object sender, RoutedEventArgs e)
        {
            _dispatcherSignalHelper.Signal("MouseClick", TestResult.Pass);
        }

        #endregion

        #region Data

        protected const string btnName = "Part_ClickTestingButton"; // The name of the Button object which will be click-tested
        private DispatcherSignalHelper _dispatcherSignalHelper;

        #endregion
    }
}


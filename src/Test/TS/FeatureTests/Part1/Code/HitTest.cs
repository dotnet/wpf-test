// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//****************************************************************** 
//* Purpose: Test TS Scroll Acceleration feature
//******************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Microsoft.Test.Graphics;
using Microsoft.Test.RenderingVerification;

namespace Microsoft.Test.TS
{
    /// <summary>
    /// This class implements hit testing of Scroll Acceleration feature.
    /// </summary>
    public class HitTest : ScrollAccelerationTestBase
    {
        #region Constructor

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="log">The test log to be used for any logging.</param>
        /// <param name="parameters">The test parameters.</param>
        public HitTest(TestLog log, PropertyBag parameters)
            : base(log, parameters)
        {
            SetExtraTestParameters(parameters);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Does the hit-testing at the given test points and returns the result as an array of bool.
        /// </summary>
        /// <param name="window">A test window.</param>
        /// <returns>The hit-testing result as an array of bool, indicating whether the control of interest was hit at any of the test points.</returns>
        public override object GetTestResult(Window window)
        {
            bool[] actual = new bool[testPoints.Length];

            for (int i = 0; i < testPoints.Length; i++)
            {
                PointHitTestResult hitResult = VisualTreeHelper.HitTest(window, testPoints[i].Key) as PointHitTestResult;
                actual[i] = (hitResult != null && hitTestCtrl.Equals((string)hitResult.VisualHit.GetValue(Control.NameProperty)));
            }

            return actual;
        }

        /// <summary>
        /// Compares the hit-testing results that were performed on a HW and SW rendered window against
        /// an expected list of hit-testing results and throws TestValidationException if comparison fails.
        /// </summary>
        /// <param name="hwRenderedWindowResults">The results of hit-testing on the HW rendered window.</param>
        /// <param name="swRenderedWindowResults">The results of hit-testing on the SW rendered window.</param>
        public override void CompareTestResults(object hwRenderedWindowResults, object swRenderedWindowResults)
        {
            log.LogStatus("Hit-Test results of HW rendered window:");
            bool hwPassed = CheckHitTestResults((bool[])hwRenderedWindowResults);

            log.LogStatus("Hit-Test results of SW rendered window:");
            bool swPassed = CheckHitTestResults((bool[])swRenderedWindowResults);

            if (hwPassed == false || swPassed == false)
            {
                throw new TestValidationException("Hit-Testing failed.");
            }
            else
            {
                log.LogStatus("Hit-Testing Passed.");
            }
        }

        /// <summary>
        /// Helpter method for comparing the actual hit-test results against an expected set of hit-test results.
        /// </summary>
        /// <param name="actual">The array of booleans indicating the actual hit-test results at each of the test points.</param>
        /// <returns>TRUE if every 'actual' hit-test result is as expected, FALSE otherwise.</returns>
        private bool CheckHitTestResults(bool[] actual)
        {
            bool passed = true;
            string msg;

            for (int i = 0; i < testPoints.Length; i++)
            {
                if (testPoints[i].Value != actual[i])
                {
                    msg = "FAILED: ";
                    passed = false;
                }
                else
                {
                    msg = "PASSED: ";
                }
                msg += "At " + testPoints[i].Key + "   Expected: " + testPoints[i].Value + "  Actual: " + actual[i] + "\n";
                log.LogStatus(msg);
            }

            return passed;
        }

        /// <summary>
        /// Loads the particular test parameters that for a test of type HitTest.
        /// </summary>
        /// <param name="parameters">The test parameters.</param>
        private void SetExtraTestParameters(PropertyBag parameters)
        {
            hitTestCtrl = (string)testParameters["HitTestControl"];
            if (hitTestCtrl == null || hitTestCtrl.Equals(string.Empty))
            {
                throw new TestValidationException("Parameter HitTestControl not provided.");
            }
            else if (testParameters["HitTestPoints"] == null)
            {
                throw new TestValidationException("Parameter HitTestPoints not provided.");
            }
            else if (testParameters["HitTestExpectedResults"] == null)
            {
                throw new TestValidationException("Parameter HitTestExpectedResults not provided.");
            }

            char[] sep = { '|' };
            string[] points = ((string)testParameters["HitTestPoints"]).Split(sep, StringSplitOptions.RemoveEmptyEntries);
            string[] results = ((string)testParameters["HitTestExpectedResults"]).Split(sep, StringSplitOptions.RemoveEmptyEntries);

            if (points.Length != results.Length)
            {
                throw new TestValidationException("The number of test points (" + points.Length + ") doesn't match the number of expected results (" + results.Length + ")");
            }
            else
            {
                testPoints = new KeyValuePair<Point, bool>[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    Point point = Point.Parse(points[i]);
                    bool expected = bool.Parse(results[i]);
                    testPoints[i] = new KeyValuePair<Point, bool>(point, expected);
                }
            }
        }

        #endregion

        #region Data

        protected string hitTestCtrl; // The name of the control to be hit-tested
        protected KeyValuePair<Point, bool>[] testPoints; // A set of test points and whether the control of interest is expected to be hit at each point.

        #endregion
    }
}


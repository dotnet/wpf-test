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
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Microsoft.Test.Graphics;
using Microsoft.Test.RenderingVerification;

namespace Microsoft.Test.TS
{
    /// <summary>
    /// This class implements the simple xaml-based testing of Scroll Acceleration feature.
    /// </summary>
    public class SimpleXamlBasedScrollTest : ScrollAccelerationTestBase
    {
        #region Constructor

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="log">The test log to be used for any logging.</param>
        /// <param name="parameters">The test parameters.</param>
        public SimpleXamlBasedScrollTest(TestLog log, PropertyBag parameters)
            : base(log, parameters)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Takes the snapshot of a given test window and returns it as the test result.
        /// </summary>
        /// <param name="window">A test window.</param>
        /// <returns>The snapshot of the window.</returns>
        public override object GetTestResult(Window window)
        {
            ImageAdapter snapShot = TakeSnapshot(window);
            log.LogStatus("Took snapshot of the window.");
            return snapShot;
        }

        /// <summary>
        /// Compares the results of two tests and throws a TestValidationException if the results are not the same.
        /// For a SimpleXamlBasedScrollTest test, this will be done by image comparison.
        /// </summary>
        /// <param name="referenceTestResult">The captured test result of the reference window.</param>
        /// <param name="renderedTestResult">The captured test result of the test window.</param>
        public override void CompareTestResults(object referenceTestResult, object renderedTestResult)
        {
            CompareSnapshots((ImageAdapter)referenceTestResult, "reference_content.png", (ImageAdapter)renderedTestResult, "rendered_content.png", "diff_content.png");
            log.LogStatus("Visually validated reference and rendered windows.");
        }

        #endregion
    }
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Regression_Bug42 
*/
using System.Windows.Media;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Regression test for Regression_Bug42
    /// </summary>
    // commented ignored tests
    // [Test(1, "Regression", "Regression_Bug42",
    //     Area = "2D",
    //     Description = @"Regression test for Regression_Bug42 : 
    //            NullReference when calling ToString on an empty pathfigure")]
    public class Regression_Bug42 : StepsTest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        [Variation()]
        public Regression_Bug42()
        {
            RunSteps += new TestStep(VerifyNoNullRef);
        }

        /// <summary>
        /// Attempt our repro steps.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyNoNullRef()
        {
            PathFigure pf = new PathFigure();
            PolyBezierSegment seg = new PolyBezierSegment();
            seg.Points = null;
            pf.Segments.Add(seg);
            pf.ToString();

            return TestResult.Pass;
        }
    }
}
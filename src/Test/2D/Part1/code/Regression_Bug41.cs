// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test
*/
using System.ComponentModel.Design.Serialization;
using System.Windows.Media;
using System.Windows.Media.Converters;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Regression test for Regression_Bug41
    /// </summary>
    // commented ignored tests
    // [Test(1, "Regression", "Regression_Bug41",
    //     Area = "2D",        
    //     Description = @"Regression test for Regression_Bug41 : PointCollectionConverter")]
    public class Regression_Bug41 : StepsTest
    {

        /// <summary>
        /// Constructor
        /// </summary>
        [Variation()]
        public Regression_Bug41()
        {
            RunSteps += new TestStep(Verify);
        }

        /// <summary>
        /// Attempt our repro steps.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Verify()
        {
            PointCollectionConverter converter = new PointCollectionConverter();
            if (converter.CanConvertFrom(typeof(InstanceDescriptor)))
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
    }
}